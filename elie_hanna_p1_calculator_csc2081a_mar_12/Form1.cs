using System;                            // Provides basic system types
using System.Drawing;                    // Provides drawing and color types
using System.Globalization;              // Provides culture info for parsing/formatting
using System.Windows.Forms;              // Provides WinForms UI types

namespace elie_hanna_p1_calculator_csc2081a_mar_12
{
    public partial class Form1 : Form     // Form1 inherits from WinForms Form
    {
        #region Initial variables and constants

        // =========================
        // Initial variables / fields
        // =========================

        private string currentThemeName = "Light";      // Stores the name of the current theme
        private Color baseDigitBackColor;               // Base background color for digit buttons (per theme)
        private Color baseNonDigitBackColor;            // Base background color for non-digit buttons (per theme)

        private const int MAX_TOKENS = 256;             // Maximum number of tokens (numbers/operators/parentheses)
        private readonly string[] tokens = new string[MAX_TOKENS]; // Array holding expression tokens
        private int tokenCount = 0;                     // Current count of used tokens in the tokens array

        private double memoryValue = 0.0;               // Single memory value for MC/MR/MS/M+/M-

        private string lastBinaryOp = "";               // Last binary operator for repeated equals
        private double lastBinaryRight = 0;             // Last right operand for repeated equals
        private bool canRepeatEquals = false;           // Flag indicating if we can repeat equals

        private bool isEnteringNumber = false;          // True when user is typing a number in lbl_result

        private bool isSecondMode = false;              // True when "2nd" mode is active
        private Color secondDefaultBackColor = SystemColors.ActiveBorder; // Default background for 2nd button
        private bool fxIsEnteringAmount = false;        // True when user is typing amount in FX display

        private const int MAX_RESULT_LEN = 14;          // Max characters before switching to scientific format
        private const string SCI_FORMAT = "0.###E+0";   // Scientific notation format string
        private const double INTEGER_TOLERANCE = 1e-12; // Tolerance to treat double as integer
        private const int MAX_FACTORIAL_INPUT = 170;    // Max n for n! to avoid overflow

        // FX hard-coded rates relative to 1 USD
        private const double EUR_PER_USD = 0.92;        // 1 USD ≈ 0.92 EUR (example fixed rate)
        private const double LBP_PER_USD = 89500.0;     // 1 USD = 89,500 LBP (example fixed rate)

        #endregion

        public Form1()
        {
            InitializeComponent();                                      // Initialize designer-created controls

            rad_light.CheckedChanged += ThemeRadio_CheckedChanged;      // Handle theme change for Light radio
            rad_dark.CheckedChanged += ThemeRadio_CheckedChanged;       // Handle theme change for Dark radio
            rad_midnight.CheckedChanged += ThemeRadio_CheckedChanged;   // Handle theme change for Midnight radio

            // Attach hover handlers to all buttons in both tabs
            WireAllButtonsHover(tabPage1);                              // Setup hover events on calculator tab
            WireAllButtonsHover(tabPage2);                              // Setup hover events on FX tab

            ApplyTheme("Light");                                        // Apply default Light theme on startup

            WireAllButtonsToOneHandlerIterative(tabPage1, Button_Click);      // Wire calculator buttons to Button_Click
            WireAllButtonsToOneHandlerIterative(tabPage2, Button_Click_FX);   // Wire FX buttons to Button_Click_FX

            ClearAll();                                                 // Reset calculator internal state and display
            SetSecondMode(false);                                       // Ensure 2nd mode is off initially

            FxInitCombos();                                             // Initialize FX combo boxes
            FxClearAll();                                               // Reset FX display and status
        }

        #region Common helper methods

        // =========================
        // Common helpers
        // =========================

        // ------------- Wire all buttons under a root control to a single Click handler -------------
        private void WireAllButtonsToOneHandlerIterative(Control root, EventHandler handler)
        {
            // If root itself is a Button, attach handler
            Button b = root as Button;                  // Try to cast root to Button
            if (b != null)
            {
                b.Click -= handler;                     // Remove handler (avoid duplicates)
                b.Click += handler;                     // Add handler
            }

            // Recurse into child controls
            for (int i = 0; i < root.Controls.Count; i++)   // Loop over all child controls
            {
                WireAllButtonsToOneHandlerIterative(root.Controls[i], handler); // Recurse on each child
            }
        }

        // ------------- Copy text from a label into clipboard, update status label -------------
        private void CopyFromLabel(Label sourceLabel, Label statusLabel)
        {
            if (sourceLabel == null) return;            // Safety check: nothing to copy from

            try
            {
                Clipboard.SetText(sourceLabel.Text);    // Put label text into clipboard
                if (statusLabel != null)               // If there is a status label
                    statusLabel.Text = "Copied";        // Show success message
            }
            catch
            {
                if (statusLabel != null)               // On error, if status label exists
                    statusLabel.Text = "Copy failed";  // Show failure message
            }
        }

        // ------------- Paste numeric value from clipboard into a label and set entering-flag -------------
        private void PasteToLabel(Label targetLabel, Label statusLabel, ref bool isEnteringFlag)
        {
            if (targetLabel == null) return;            // No target: nothing to do

            try
            {
                string t = Clipboard.GetText();         // Read text from clipboard
                if (t == null) t = "";                  // Guard against null
                t = t.Trim();                           // Trim whitespace
                if (t.Length == 0)                      // If clipboard is empty string
                {
                    if (statusLabel != null)
                        statusLabel.Text = "Clipboard empty";   // Show message
                    return;
                }

                double v;                               // Parsed numeric value
                // Try parse with invariant and current culture
                if (!double.TryParse(t, NumberStyles.Float, CultureInfo.InvariantCulture, out v) &&
                    !double.TryParse(t, NumberStyles.Float, CultureInfo.CurrentCulture, out v))
                {
                    if (statusLabel != null)
                        statusLabel.Text = "Invalid paste";     // Not numeric
                    return;
                }

                // Set label text using calculator formatting
                targetLabel.Text = FormatForDisplay(v);         // Show formatted number in target label

                isEnteringFlag = true;                          // We are now "entering" this value
                if (statusLabel != null)
                    statusLabel.Text = "";                      // Clear status
            }
            catch
            {
                if (statusLabel != null)
                    statusLabel.Text = "Paste failed";          // Exception: paste failed
            }
        }

        // ------------- Append a decimal point to a display label -------------
        private void AppendDecimalCommon(Label displayLabel, ref bool isEnteringFlag)
        {
            if (!isEnteringFlag)                                // If not currently typing a number
            {
                displayLabel.Text = "0.";                       // Start with "0."
                isEnteringFlag = true;                          // Mark as entering
                return;
            }

            if (displayLabel.Text.IndexOf('.') < 0)             // Only add "." if not already present
                displayLabel.Text += ".";                       // Append "."
        }

        // ------------- Append a digit character to a display label -------------
        private void AppendDigitCommon(Label displayLabel, string digit, ref bool isEnteringFlag)
        {
            if (!isEnteringFlag || displayLabel.Text == "0")    // If not typing or display is "0"
            {
                displayLabel.Text = digit;                      // Replace with this digit
                isEnteringFlag = true;                          // Mark as entering a number
            }
            else
            {
                displayLabel.Text += digit;                     // Append digit to existing text
            }
        }

        // ------------- Handle backspace for a numeric display label -------------
        private void BackspaceCommon(Label displayLabel, ref bool isEnteringFlag, Action clearEntryAction)
        {
            if (!isEnteringFlag)                                // If user is not currently entering
            {
                clearEntryAction();                             // Clear entire entry
                return;
            }

            // If length is 1, or "-x" with length 2, revert to "0"
            if (displayLabel.Text.Length <= 1 ||
                (displayLabel.Text.Length == 2 && displayLabel.Text.StartsWith("-", StringComparison.Ordinal)))
            {
                displayLabel.Text = "0";                        // Reset to 0
                isEnteringFlag = false;                         // No longer entering
                return;
            }

            // Remove last character
            displayLabel.Text = displayLabel.Text.Substring(0, displayLabel.Text.Length - 1);

            // If became "-" or empty, reset to "0"
            if (displayLabel.Text == "-" || displayLabel.Text.Length == 0)
            {
                displayLabel.Text = "0";                        // Reset to 0
                isEnteringFlag = false;                         // Not entering anymore
            }
        }

        // ------------- Parse a label text into double (using invariant), default 0 -------------
        private double ParseDisplayValue(Label displayLabel)
        {
            double v;                                           // Parsed value
            if (double.TryParse(displayLabel.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out v))
                return v;                                       // Return parsed value

            return 0;                                           // If parse fails, return 0
        }

        // =========================
        // Token array helpers
        // =========================

        // ------------- Clear token list -------------
        private void TokensClear()
        {
            tokenCount = 0;                                     // Reset token count to zero
        }

        // ------------- Add a token to the tokens array -------------
        private void TokensAdd(string value)
        {
            if (tokenCount < MAX_TOKENS)                        // Only add if space available
            {
                tokens[tokenCount] = value;                     // Store token in array
                tokenCount++;                                   // Increment token count
            }
        }

        // ------------- Check if tokens contain a specific value -------------
        private bool TokensContains(string value)
        {
            for (int i = 0; i < tokenCount; i++)                // Loop over used tokens
            {
                if (tokens[i] == value) return true;            // Return true if match found
            }
            return false;                                       // No match found
        }

        // ------------- Get last token or null if no tokens -------------
        private string TokensLastOrDefault()
        {
            if (tokenCount == 0) return null;                   // No tokens: return null
            return tokens[tokenCount - 1];                      // Return last token
        }

        // ------------- Copy tokens into another array, return count -------------
        private int CopyTokensTo(string[] destination)
        {
            int n = tokenCount;                                 // Number of tokens to copy
            for (int i = 0; i < n; i++)                         // Loop over tokens
                destination[i] = tokens[i];                     // Copy each token
            return n;                                           // Return count copied
        }

        // ------------- Build expression string from current tokens -------------
        private string BuildExpressionString()
        {
            return BuildExpressionStringFromArray(tokens, tokenCount); // Use helper with full tokens
        }

        // ------------- Build expression string from any token array and count -------------
        private string BuildExpressionStringFromArray(string[] arr, int count)
        {
            if (count == 0) return "";                          // No tokens => empty string

            string result = arr[0];                             // Start with first token
            for (int i = 1; i < count; i++)                     // Append the rest with spaces
            {
                result += " " + arr[i];
            }
            return result;                                      // Return final expression string
        }

        // ------------- Check if a double is close to an integer -------------
        private static bool IsInteger(double x)
        {
            return Math.Abs(x - Math.Round(x)) < INTEGER_TOLERANCE; // Compare with rounding and tolerance
        }

        // ------------- Compute factorial of n if valid, else NaN -------------
        private static double Factorial(double n)
        {
            if (n < 0) return double.NaN;                       // Factorial undefined for negative
            if (!IsInteger(n)) return double.NaN;               // Only allow integer values
            if (n > MAX_FACTORIAL_INPUT) return double.NaN;     // Avoid overflow for very large n

            double r = 1;                                       // Accumulator
            int limit = (int)Math.Round(n);                     // Final integer
            for (int i = 2; i <= limit; i++)                    // Multiply from 2 to n
                r *= i;
            return r;                                           // Return factorial result
        }

        // ------------- Format double into display string with length limit and sci-notation -------------
        private string FormatForDisplay(double v)
        {
            if (Math.Abs(v) < 1e-15) v = 0;                     // Treat tiny values as 0

            string normal = v.ToString(CultureInfo.InvariantCulture); // Convert normally
            if (normal.Length <= MAX_RESULT_LEN) return normal; // If short enough, use normal
            return v.ToString(SCI_FORMAT, CultureInfo.InvariantCulture); // Else scientific format
        }

        #endregion

        #region Calculator logic

        // =========================
        // Calculator: button click
        // =========================

        // ------------- Handle all calculator button clicks (main tab) -------------
        private void Button_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;                      // Cast sender to Button
            if (btn == null) return;                            // Safety: if not a button, do nothing
            string v = btn.Text.Trim();                         // Get button text

            switch (v)                                          // Decide action based on text
            {
                case "2nd":
                    ToggleSecondMode();                         // Toggle second-mode functions
                    break;

                case "(":
                    PressLeftParen();                           // Handle "("
                    break;

                case ")":
                    PressRightParen();                          // Handle ")"
                    break;

                case ".":
                    AppendDecimalCommon(lbl_result, ref isEnteringNumber); // Append decimal to main display
                    break;

                case "+/-":
                    ToggleSign();                               // Change sign of current value
                    break;

                case "C":
                    ClearAll();                                 // Clear all expression and result
                    break;

                case "CE":
                    ClearEntry();                               // Clear only current entry
                    break;

                case "⌫":
                    BackspaceCommon(lbl_result, ref isEnteringNumber, ClearEntry); // Backspace behavior
                    break;

                case "%":
                    Percent();                                  // Convert to percent
                    break;

                case "1/x":
                    Reciprocal();                               // Reciprocal of current value
                    break;

                case "x²":
                    Square();                                   // Square of current value
                    break;

                case "²√x":
                    Sqrt();                                     // Square root
                    break;

                case "|x|":
                    Abs();                                      // Absolute value
                    break;

                case "π":
                    InsertConstant(Math.PI);                    // Insert PI constant
                    break;

                case "e":
                    InsertConstant(Math.E);                     // Insert e constant
                    break;

                case "exp":
                    Exp();                                      // e^x
                    break;

                case "ln":
                    Ln();                                       // Natural log
                    break;

                case "log":
                    Log10();                                    // Log base 10
                    break;

                case "10ˣ":
                case "10^x":
                    TenPowerX();                                // 10^x
                    break;

                case "sin":
                    Sin();                                      // Sine
                    break;

                case "cos":
                    Cos();                                      // Cosine
                    break;

                case "tan":
                    Tan();                                      // Tangent
                    break;

                case "n!":
                    FactorialUnary();                           // Factorial
                    break;

                case "+":
                case "-":
                case "X":
                case "÷":
                case "mod":
                case "xʸ":
                    PressOperator(v);                           // Binary operators
                    break;

                case "=":
                    PressEquals();                              // Evaluate expression
                    break;

                case "Copy":
                    CopyFromLabel(lbl_result, lbl_expression);  // Copy main result to clipboard, show status in expression label
                    break;

                case "Paste":
                    PasteToLabel(lbl_result, lbl_expression, ref isEnteringNumber); // Paste into main result
                    break;

                case "MC":
                    MemoryClear();                              // Clear memory
                    break;

                case "MR":
                    MemoryRecall();                             // Recall memory
                    break;

                case "MS":
                    MemoryStore();                              // Store into memory
                    break;

                case "M+":
                    MemoryAdd();                                // Add current value to memory
                    break;

                case "M-":
                    MemorySubtract();                           // Subtract current value from memory
                    break;

                default:
                    // If button text is a single digit, append it
                    if (v.Length == 1 && char.IsDigit(v[0]))
                        AppendDigitCommon(lbl_result, v, ref isEnteringNumber);
                    break;
            }
        }

        // =========================
        // Calculator core logic
        // =========================

        // ------------- Get precedence level of an operator -------------
        private static int Precedence(string op)
        {
            if (op == "xʸ") return 3;                           // Highest precedence
            if (op == "X" || op == "÷" || op == "mod") return 2;// Multiply/divide/mod
            if (op == "+" || op == "-") return 1;               // Add/subtract
            return 0;                                           // Unknown
        }

        // ------------- Apply a binary operator to two operands -------------
        private static double Apply(double a, double b, string op)
        {
            if (op == "+") return a + b;                        // Addition
            if (op == "-") return a - b;                        // Subtraction
            if (op == "X") return a * b;                        // Multiplication
            if (op == "÷")                                     // Division
            {
                if (b == 0) return double.NaN;                  // Avoid division by zero
                return a / b;
            }
            if (op == "mod")                                   // Modulo
            {
                if (b == 0) return double.NaN;                  // Avoid mod by zero
                return a % b;
            }
            if (op == "xʸ") return Math.Pow(a, b);             // Power
            return double.NaN;                                 // Unknown operator
        }

        // ------------- Capture last binary operation for repeated equals behavior -------------
        private void CaptureRepeatEqualsMemory(string[] working, int workingCount)
        {
            canRepeatEquals = false;                            // Default: cannot repeat
            if (workingCount < 3) return;                       // Need at least "a op b"

            int i = workingCount - 1;                           // Last token index
            double right;                                       // Right operand
            if (!double.TryParse(working[i], NumberStyles.Float, CultureInfo.InvariantCulture, out right))
                return;                                         // Last token not a number

            string op = working[i - 1];                         // Operator before last token
            bool opIsBinary =
                op == "+" || op == "-" || op == "X" || op == "÷" || op == "mod" || op == "xʸ";
            if (!opIsBinary) return;                            // Only track binary operators

            string leftTok = working[i - 2];                    // Left token
            double dummy;
            bool leftOk =
                double.TryParse(leftTok, NumberStyles.Float, CultureInfo.InvariantCulture, out dummy) ||
                leftTok == ")";                                 // Allow ")" as ending of subexpression

            if (!leftOk) return;                                // Left side not valid

            lastBinaryOp = op;                                  // Store operator
            lastBinaryRight = right;                            // Store right operand
            canRepeatEquals = true;                             // Enable repeat-equals
        }

        // ------------- Evaluate tokens using stacks (shunting-yard style) -------------
        private double EvaluateTokens(string[] tks, int count)
        {
            double[] valStack = new double[MAX_TOKENS];         // Value stack
            int valTop = 0;                                     // Index for top of value stack

            string[] opStack = new string[MAX_TOKENS];          // Operator stack
            int opTop = 0;                                      // Index for top of operator stack

            for (int i = 0; i < count; i++)                     // Loop over all tokens
            {
                string tk = tks[i];                             // Current token

                double num;
                if (double.TryParse(tk, NumberStyles.Float, CultureInfo.InvariantCulture, out num))
                {
                    // Token is a number: push to value stack
                    if (valTop < MAX_TOKENS)
                    {
                        valStack[valTop] = num;
                        valTop++;
                    }
                }
                else if (tk == "(")
                {
                    // Push "(" to operator stack
                    if (opTop < MAX_TOKENS)
                    {
                        opStack[opTop] = tk;
                        opTop++;
                    }
                }
                else if (tk == ")")
                {
                    // Pop and apply operators until "("
                    while (opTop > 0 && opStack[opTop - 1] != "(")
                    {
                        if (valTop < 2) return double.NaN;      // Not enough values

                        string op = opStack[--opTop];           // Pop operator
                        double b = valStack[--valTop];          // Pop operand b
                        double a = valStack[--valTop];          // Pop operand a
                        double res = Apply(a, b, op);           // Apply operator

                        if (double.IsNaN(res) || double.IsInfinity(res)) return double.NaN;

                        valStack[valTop] = res;                 // Push result
                        valTop++;
                    }
                    if (opTop == 0) return double.NaN;          // No matching "("
                    opTop--;                                    // Pop "("
                }
                else
                {
                    // Token is an operator
                    while (opTop > 0 &&
                           opStack[opTop - 1] != "(" &&
                           Precedence(opStack[opTop - 1]) >= Precedence(tk))
                    {
                        if (valTop < 2) return double.NaN;      // Not enough values

                        string op = opStack[--opTop];           // Pop operator
                        double b = valStack[--valTop];          // Pop operand b
                        double a = valStack[--valTop];          // Pop operand a
                        double res = Apply(a, b, op);           // Apply operator

                        if (double.IsNaN(res) || double.IsInfinity(res)) return double.NaN;

                        valStack[valTop] = res;                 // Push result
                        valTop++;
                    }

                    if (opTop < MAX_TOKENS)
                    {
                        opStack[opTop] = tk;                    // Push current operator
                        opTop++;
                    }
                }
            }

            // Apply remaining operators
            while (opTop > 0)
            {
                if (opStack[opTop - 1] == "(") return double.NaN;// Mismatched "("
                if (valTop < 2) return double.NaN;              // Not enough values

                string op = opStack[--opTop];                   // Pop operator
                double b = valStack[--valTop];                  // Pop operand b
                double a = valStack[--valTop];                  // Pop operand a
                double res = Apply(a, b, op);                   // Apply operator

                if (double.IsNaN(res) || double.IsInfinity(res)) return double.NaN;

                valStack[valTop] = res;                         // Push result
                valTop++;
            }

            if (valTop != 1) return double.NaN;                 // Should end with exactly one value
            return valStack[0];                                 // Final result
        }

        // ------------- Find index after innermost "(" or 0 if none -------------
        private int GetInnermostParenStart(string[] src, int srcCount)
        {
            int start = -1;                                     // Index of "("
            for (int i = srcCount - 1; i >= 0; i--)             // Scan backwards
            {
                if (src[i] == "(")
                {
                    start = i;                                  // Remember index
                    break;                                      // Stop at first from right
                }
            }

            if (start < 0)
                return 0;                                       // No "(": use full expression

            return start + 1;                                   // Return index after "("
        }

        // ------------- Recalculate live result display while user types -------------
        private void RefreshDisplay()
        {
            // Copy current tokens to a temporary working array
            string[] working = new string[MAX_TOKENS];          // Local working array
            int workingCount = CopyTokensTo(working);           // Copy tokens

            // If user is currently entering a number, append it as last token
            if (isEnteringNumber)
            {
                if (workingCount < MAX_TOKENS)
                {
                    working[workingCount] = lbl_result.Text;    // Append display text as number token
                    workingCount++;
                }
            }

            if (workingCount == 0)
                return;                                         // Nothing to evaluate

            // Find start index of innermost parenthesis slice
            int sliceStart = GetInnermostParenStart(working, workingCount);

            // Compute initial slice length (from sliceStart to end)
            int sliceLen = workingCount - sliceStart;
            if (sliceLen <= 0)
                return;                                         // No slice

            // If last token in slice is an operator, drop it (incomplete expression)
            int lastIndex = sliceStart + sliceLen - 1;          // Index of last token
            string last = working[lastIndex];                   // Last token text
            if (IsOperator(last))
            {
                sliceLen--;                                     // Ignore that operator
            }

            if (sliceLen <= 0)
                return;                                         // Nothing left to evaluate

            // If the slice starts with "(", it's incomplete, so do nothing
            if (working[sliceStart] == "(")
                return;

            // Build a temporary array containing only the slice
            string[] slice = new string[sliceLen];              // Slice array
            for (int i = 0; i < sliceLen; i++)                  // Copy relevant tokens
            {
                slice[i] = working[sliceStart + i];
            }

            // Evaluate only that slice
            double v = EvaluateTokens(slice, sliceLen);         // Evaluate slice tokens
            if (!double.IsNaN(v) && !double.IsInfinity(v))      // If valid result
            {
                lbl_result.Text = FormatForDisplay(v);          // Show formatted value
            }
        }

        // ------------- Push current typing entry into tokens if needed -------------
        private void PushCurrentEntryIfNeeded()
        {
            if (isEnteringNumber)
            {
                TokensAdd(lbl_result.Text);                     // Add current display as token
                isEnteringNumber = false;                       // Mark not entering anymore
            }
        }

        // ------------- Check if a string token is a binary operator -------------
        private bool IsOperator(string tk)
        {
            return tk == "+" || tk == "-" || tk == "X" || tk == "÷" || tk == "mod" || tk == "xʸ";
        }

        // ------------- Handle pressing "(" -------------
        private void PressLeftParen()
        {
            if (tokenCount > 0)
            {
                string last = TokensLastOrDefault();            // Last token
                double dummy;

                // If last is a number or ")", insert implicit multiply before "("
                bool lastIsNumber = double.TryParse(last, NumberStyles.Float, CultureInfo.InvariantCulture, out dummy);
                if (lastIsNumber || last == ")")
                    TokensAdd("X");
            }

            TokensAdd("(");                                     // Add "(" token
            lbl_expression.Text = BuildExpressionString();      // Update expression label
            lbl_result.Text = "0";                             // Reset current result display
            isEnteringNumber = false;                           // Not entering number now
        }

        // ------------- Handle pressing ")" -------------
        private void PressRightParen()
        {
            PushCurrentEntryIfNeeded();                         // Push current entry if needed
            if (!TokensContains("(")) return;                   // If no "(", ignore

            if (tokenCount > 0)
            {
                string last = TokensLastOrDefault();            // Last token
                if (last == "(" || IsOperator(last))
                    return;                                     // Do not close immediately after "(" or operator
            }

            TokensAdd(")");                                     // Add ")" token
            lbl_expression.Text = BuildExpressionString();      // Update expression text
            RefreshDisplay();                                   // Recalculate preview result
        }

        // ------------- Handle pressing a binary operator -------------
        private void PressOperator(string op)
        {
            canRepeatEquals = false;                            // Reset repeat-equals flag

            PushCurrentEntryIfNeeded();                         // Push current entry if needed

            if (tokenCount == 0)                                // If expression is empty
            {
                TokensAdd(CurrentValue().ToString(CultureInfo.InvariantCulture)); // Add current result as first token
                TokensAdd(op);                                  // Then add operator
            }
            else
            {
                string last = TokensLastOrDefault();            // Last token
                if (IsOperator(last))
                {
                    tokens[tokenCount - 1] = op;                // Replace last operator with new one
                }
                else if (last == "(")
                {
                    if (op == "-" || op == "+")                 // Allow unary like "(-0" or "(+0)"
                    {
                        lbl_result.Text = (op == "-") ? "-0" : "0"; // Prepare signed zero
                        isEnteringNumber = true;                // Start entering a number
                        return;
                    }
                    return;                                     // Ignore other operators just after "("
                }
                else
                {
                    TokensAdd(op);                              // Normal case: add operator
                }
            }

            lbl_expression.Text = BuildExpressionString();      // Update expression label
            RefreshDisplay();                                   // Recompute preview result
        }

        // ------------- Handle pressing "=" to evaluate expression -------------
        private void PressEquals()
        {
            // Handle repeated equals case: no tokens, not entering, previous op exists
            if (tokenCount == 0 && !isEnteringNumber && canRepeatEquals && lastBinaryOp != "")
            {
                double current = CurrentValue();                // Get current displayed value
                double rRepeat = Apply(current, lastBinaryRight, lastBinaryOp); // Apply last op again

                if (double.IsNaN(rRepeat) || double.IsInfinity(rRepeat))
                {
                    ShowError("Invalid input");                 // Show error for invalid result
                    return;
                }

                // Show expression of repeated equals
                lbl_expression.Text = FormatForDisplay(current) + " " +
                                      lastBinaryOp + " " +
                                      FormatForDisplay(lastBinaryRight) + " =";
                SetResult(rRepeat);                             // Show repeated result
                isEnteringNumber = false;                       // Not entering number
                return;
            }

            // Copy current tokens to working
            string[] working = new string[MAX_TOKENS];          // Working array
            int workingCount = CopyTokensTo(working);           // Copy tokens

            if (isEnteringNumber)
            {
                if (workingCount < MAX_TOKENS)
                {
                    working[workingCount] = lbl_result.Text;    // Append current typed number
                    workingCount++;
                }
            }

            // Trim trailing operators/"("
            while (workingCount > 0)
            {
                string last = working[workingCount - 1];        // Last token
                bool lastIsOp = IsOperator(last);               // Check if operator
                if (lastIsOp || last == "(")                    // If operator or "(" at end
                {
                    workingCount--;                             // Drop it
                    continue;
                }
                break;                                          // Stop when last is a value or ")"
            }

            if (workingCount == 0) return;                      // Nothing to evaluate

            double r = EvaluateTokens(working, workingCount);   // Evaluate expression

            if (double.IsNaN(r) || double.IsInfinity(r))
            {
                ShowError("Invalid input");                     // Show invalid input message
                return;
            }

            lbl_expression.Text = BuildExpressionStringFromArray(working, workingCount) + " ="; // Show expression with "="
            SetResult(r);                                      // Set result to display
            CaptureRepeatEqualsMemory(working, workingCount);  // Save for repeated equals
            TokensClear();                                     // Clear expression tokens
            isEnteringNumber = false;                          // Not entering number now
        }

        // ------------- Get current main display value as double -------------
        private double CurrentValue()
        {
            return ParseDisplayValue(lbl_result);               // Parse lbl_result text
        }

        // ------------- Set result on main display with validation -------------
        private void SetResult(double v)
        {
            if (double.IsNaN(v) || double.IsInfinity(v))        // If invalid numeric result
            {
                ShowError("Invalid input");                     // Show error
                return;
            }

            lbl_result.Text = FormatForDisplay(v);              // Format and show in main label
        }

        // ------------- Show global error and reset internal expression state -------------
        private void ShowError(string message)
        {
            lbl_expression.Text = "";                           // Clear expression label
            lbl_result.Text = message;                          // Show error message in result label

            TokensClear();                                      // Clear tokens
            isEnteringNumber = false;                           // Not entering
            lastBinaryOp = "";                                  // Clear last op
            lastBinaryRight = 0;                                // Clear last right operand
            canRepeatEquals = false;                            // Disable repeat equals
        }

        // ------------- Toggle sign of current value (+/-) -------------
        private void ToggleSign()
        {
            if (lbl_result.Text.StartsWith("-", StringComparison.Ordinal)) // If already negative
                lbl_result.Text = lbl_result.Text.Substring(1);            // Remove '-'
            else if (lbl_result.Text != "0")                               // If not zero
                lbl_result.Text = "-" + lbl_result.Text;                   // Prepend '-'

            isEnteringNumber = true;                                       // Still entering number
        }

        // ------------- Clear full expression and result -------------
        private void ClearAll()
        {
            TokensClear();                                     // Remove all tokens
            lastBinaryOp = "";                                 // Reset last operator
            lastBinaryRight = 0;                               // Reset last right operand
            canRepeatEquals = false;                           // Disable repeat-equals

            lbl_expression.Text = "";                          // Clear expression label
            lbl_result.Text = "0";                             // Reset result label
            isEnteringNumber = false;                          // Not entering now
        }

        // ------------- Clear only current entry in result label -------------
        private void ClearEntry()
        {
            lbl_result.Text = "0";                             // Reset to zero
            isEnteringNumber = false;                          // Not entering
        }

        // ------------- Convert current value to percent (divide by 100) -------------
        private void Percent()
        {
            double current = CurrentValue();                   // Get current value
            SetResult(current / 100.0);                        // Divide by 100 and show
            isEnteringNumber = true;                           // Keep editing new value
        }

        // ------------- Compute reciprocal (1/x) -------------
        private void Reciprocal()
        {
            double x = CurrentValue();                         // Get current value
            if (x == 0)
            {
                ShowError("Cannot divide by 0");               // Cannot divide by zero
                return;
            }

            SetResult(1.0 / x);                                // Show reciprocal
            isEnteringNumber = true;                           // Still entering number
        }

        // ------------- Square the current value -------------
        private void Square()
        {
            double x = CurrentValue();                         // Get current value
            SetResult(x * x);                                  // x^2
            isEnteringNumber = true;                           // Still entering number
        }

        // ------------- Square root of current value -------------
        private void Sqrt()
        {
            double x = CurrentValue();                         // Get current value
            if (x < 0)
            {
                ShowError("Invalid input");                    // sqrt of negative not allowed here
                return;
            }

            SetResult(Math.Sqrt(x));                           // Show sqrt(x)
            isEnteringNumber = true;                           // Still entering
        }

        // ------------- Absolute value of current value -------------
        private void Abs()
        {
            double x = CurrentValue();                         // Get current value
            SetResult(Math.Abs(x));                            // Show |x|
            isEnteringNumber = true;                           // Still entering
        }

        // ------------- Insert constant value into display -------------
        private void InsertConstant(double value)
        {
            SetResult(value);                                  // Show constant
            isEnteringNumber = true;                           // Allow editing constant
        }

        // ------------- Exponential function e^x -------------
        private void Exp()
        {
            double x = CurrentValue();                         // Get current value
            SetResult(Math.Exp(x));                            // Show e^x
            isEnteringNumber = true;                           // Still entering
        }

        // ------------- Natural logarithm ln(x) -------------
        private void Ln()
        {
            double x = CurrentValue();                         // Get current value
            if (x <= 0)
            {
                ShowError("Invalid input");                    // ln not defined for <= 0
                return;
            }

            SetResult(Math.Log(x));                            // Show ln(x)
            isEnteringNumber = true;                           // Still entering
        }

        // ------------- Logarithm base 10 log10(x) -------------
        private void Log10()
        {
            double x = CurrentValue();                         // Get current value
            if (x <= 0)
            {
                ShowError("Invalid input");                    // log10 not defined for <= 0
                return;
            }

            SetResult(Math.Log10(x));                          // Show log10(x)
            isEnteringNumber = true;                           // Still entering
        }

        // ------------- Powers of ten 10^x -------------
        private void TenPowerX()
        {
            double x = CurrentValue();                         // Get current value
            SetResult(Math.Pow(10, x));                        // Show 10^x
            isEnteringNumber = true;                           // Still entering
        }

        // ------------- Factorial of current value -------------
        private void FactorialUnary()
        {
            double x = CurrentValue();                         // Get current value
            double f = Factorial(x);                           // Compute factorial

            if (double.IsNaN(f) || double.IsInfinity(f))
            {
                ShowError("Invalid input");                    // Invalid factorial
                return;
            }

            SetResult(f);                                      // Show factorial
            isEnteringNumber = true;                           // Still entering
        }

        // ------------- Sine of current value (in radians) -------------
        private void Sin()
        {
            double x = CurrentValue();                         // Get current value
            SetResult(Math.Sin(x));                            // Show sin(x)
            isEnteringNumber = true;                           // Still entering
        }

        // ------------- Cosine of current value (in radians) -------------
        private void Cos()
        {
            double x = CurrentValue();                         // Get current value
            SetResult(Math.Cos(x));                            // Show cos(x)
            isEnteringNumber = true;                           // Still entering
        }

        // ------------- Tangent of current value (in radians) -------------
        private void Tan()
        {
            double x = CurrentValue();                         // Get current value
            SetResult(Math.Tan(x));                            // Show tan(x)
            isEnteringNumber = true;                           // Still entering
        }

        // ------------- Enable or disable 2nd mode and update button labels -------------
        private void SetSecondMode(bool enabled)
        {
            isSecondMode = enabled;                            // Store second-mode flag

            btn_second.BackColor = enabled
                ? Color.FromArgb(255, 205, 145)                // Orange-ish when on
                : secondDefaultBackColor;                      // Default when off

            btn_power_10.Text = enabled ? "sin" : "10ˣ";       // Swap 10^x / sin
            btn_log10.Text = enabled ? "cos" : "log";          // Swap log / cos
            btn_ln.Text = enabled ? "tan" : "ln";              // Swap ln / tan
        }

        // ------------- Toggle 2nd mode flag -------------
        private void ToggleSecondMode()
        {
            SetSecondMode(!isSecondMode);                      // Flip the boolean
        }

        // ------------- Clear memory value -------------
        private void MemoryClear()
        {
            memoryValue = 0.0;                                 // Reset memory to 0
        }

        // ------------- Store current value into memory -------------
        private void MemoryStore()
        {
            double current = CurrentValue();                   // Get current value
            memoryValue = current;                             // Save to memory
        }

        // ------------- Recall memory value into display -------------
        private void MemoryRecall()
        {
            SetResult(memoryValue);                            // Show memory value
            isEnteringNumber = true;                           // Allow editing it
        }

        // ------------- Add current value to memory -------------
        private void MemoryAdd()
        {
            double current = CurrentValue();                   // Get current value
            memoryValue += current;                            // Add to memory
        }

        // ------------- Subtract current value from memory -------------
        private void MemorySubtract()
        {
            double current = CurrentValue();                   // Get current value
            memoryValue -= current;                            // Subtract from memory
        }

        #endregion

        #region FX (currency converter) logic

        // =========================
        // FX (currency) logic
        // =========================

        // ------------- Handle all FX tab button clicks -------------
        private void Button_Click_FX(object sender, EventArgs e)
        {
            Button btn = sender as Button;                     // Cast to Button
            if (btn == null) return;                           // Safety check
            string v = btn.Text.Trim();                        // Get button text

            // If this is the main "Convert" button
            if (btn.Name == "btn_fx_convert" || v.Equals("Convert", StringComparison.OrdinalIgnoreCase))
            {
                FxConvert();                                   // Perform conversion
                return;
            }

            if (v == ".")
            {
                AppendDecimalCommon(lbl_fx_display, ref fxIsEnteringAmount); // Append decimal to FX display
            }
            else if (v == "C")
            {
                FxClearAll();                                  // Clear FX display and combos
            }
            else if (v == "⌫" || v == "←" || v == "Back")
            {
                BackspaceCommon(lbl_fx_display, ref fxIsEnteringAmount, FxClearEntry); // FX backspace
            }
            else if (v.Equals("Copy", StringComparison.OrdinalIgnoreCase))
            {
                // Reusable copy for FX
                CopyFromLabel(lbl_fx_display, lbl_fx_status);  // Copy FX display to clipboard
            }
            else if (v.Equals("Paste", StringComparison.OrdinalIgnoreCase))
            {
                // Reusable paste for FX
                PasteToLabel(lbl_fx_display, lbl_fx_status, ref fxIsEnteringAmount); // Paste to FX display
            }
            else
            {
                // Digits for FX
                if (v.Length == 1 && char.IsDigit(v[0]))
                    AppendDigitCommon(lbl_fx_display, v, ref fxIsEnteringAmount);
            }
        }

        // ------------- Get current FX display value as double -------------
        private double FxCurrentValue()
        {
            return ParseDisplayValue(lbl_fx_display);          // Parse FX display text
        }

        // ------------- Set FX display with validation -------------
        private void FxSetDisplay(double v)
        {
            if (double.IsNaN(v) || double.IsInfinity(v))       // Check for invalid numeric value
            {
                lbl_fx_status.Text = "Invalid input";          // Show error in FX status
                return;
            }

            lbl_fx_display.Text = FormatForDisplay(v);         // Show formatted FX value
        }

        // ------------- Normalize currency code text -------------
        private static string NormalizeCurrency(string code)
        {
            if (code == null) return "";                       // Null => empty
            return code.Trim().ToUpperInvariant();             // Trim and uppercase
        }

        // ------------- Initialize FX combo box items and defaults -------------
        private void FxInitCombos()
        {
            if (cmb_fx_from == null || cmb_fx_to == null) return;  // Safety

            if (cmb_fx_from.Items.Count == 0)                      // Only add once
            {
                cmb_fx_from.Items.Add("USD");                      // Add USD
                cmb_fx_from.Items.Add("EUR");                      // Add EUR
                cmb_fx_from.Items.Add("LBP");                      // Add LBP
            }

            if (cmb_fx_to.Items.Count == 0)                        // Only add once
            {
                cmb_fx_to.Items.Add("USD");                        // Add USD
                cmb_fx_to.Items.Add("EUR");                        // Add EUR
                cmb_fx_to.Items.Add("LBP");                        // Add LBP
            }

            cmb_fx_from.DropDownStyle = ComboBoxStyle.DropDownList; // Lock to list items
            cmb_fx_to.DropDownStyle = ComboBoxStyle.DropDownList;   // Lock to list items

            cmb_fx_from.SelectedIndex = 0;                        // Default FROM: USD
            cmb_fx_to.SelectedIndex = 1;                          // Default TO: EUR
        }

        // ------------- Clear FX state and reset combos -------------
        private void FxClearAll()
        {
            fxIsEnteringAmount = false;                           // Not entering FX amount
            lbl_fx_display.Text = "0";                            // Reset FX display
            lbl_fx_status.Text = "";                              // Clear FX status

            if (cmb_fx_from != null) cmb_fx_from.SelectedIndex = 0; // Reset FROM to first item
            if (cmb_fx_to != null) cmb_fx_to.SelectedIndex = 1;     // Reset TO to second item
        }

        // ------------- Clear only FX entry (display) -------------
        private void FxClearEntry()
        {
            lbl_fx_display.Text = "0";                           // Reset FX display to 0
            fxIsEnteringAmount = false;                          // Not entering
        }

        // ------------- Offline FX rate using fixed USD-based rates -------------
        private double FxGetRateOffline(string from, string to)
        {
            double amountInUsdPerOneFrom;                        // How many USD per 1 unit of 'from'

            if (from == "USD")
            {
                amountInUsdPerOneFrom = 1.0;                     // 1 USD = 1 USD
            }
            else if (from == "EUR")
            {
                amountInUsdPerOneFrom = 1.0 / EUR_PER_USD;       // 1 EUR => convert to USD
            }
            else if (from == "LBP")
            {
                amountInUsdPerOneFrom = 1.0 / LBP_PER_USD;       // 1 LBP => convert to USD
            }
            else
            {
                return 0;                                        // Unknown currency code
            }

            double usdToTo;                                      // How many 'to' units per 1 USD

            if (to == "USD")
            {
                usdToTo = 1.0;                                   // 1 USD stays 1 USD
            }
            else if (to == "EUR")
            {
                usdToTo = EUR_PER_USD;                           // USD => EUR
            }
            else if (to == "LBP")
            {
                usdToTo = LBP_PER_USD;                           // USD => LBP
            }
            else
            {
                return 0;                                        // Unknown currency code
            }

            return amountInUsdPerOneFrom * usdToTo;              // from -> USD -> to
        }

        // ------------- Perform currency conversion and show result -------------
        private void FxConvert()
        {
            string from = cmb_fx_from == null ? "" : cmb_fx_from.SelectedItem as string; // Selected FROM code
            string to = cmb_fx_to == null ? "" : cmb_fx_to.SelectedItem as string;       // Selected TO code

            from = NormalizeCurrency(from);                      // Normalize FROM code
            to = NormalizeCurrency(to);                          // Normalize TO code

            if (from.Length == 0 || to.Length == 0)              // Ensure both selected
            {
                lbl_fx_status.Text = "Select FROM and TO currencies"; // Show message
                return;
            }

            if (from == to)                                      // Do not allow same currency on both sides
            {
                lbl_fx_status.Text = "FROM and TO cannot be the same";
                return;
            }

            double amount = FxCurrentValue();                    // Amount to convert
            double rate = FxGetRateOffline(from, to);            // Get conversion rate

            if (rate == 0)                                       // If rate is 0, something is wrong
            {
                lbl_fx_status.Text = "Conversion failed (invalid rate)";
                return;
            }

            double converted = amount * rate;                    // Compute converted amount

            FxSetDisplay(converted);                             // Show converted value
            fxIsEnteringAmount = false;                          // Not entering now
            lbl_fx_status.Text = from + "->" + to + " rate=" +
                                 rate.ToString(CultureInfo.InvariantCulture); // Show debug info
        }

        #endregion

        #region Radio theme logic

        // ------------- Handle theme radio button changes -------------
        private void ThemeRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_light.Checked)                               // Light checked
            {
                ApplyTheme("Light");                             // Apply light theme
            }
            else if (rad_dark.Checked)                           // Dark checked
            {
                ApplyTheme("Dark");                              // Apply dark theme
            }
            else if (rad_midnight.Checked)                       // Midnight checked
            {
                ApplyTheme("Midnight");                          // Apply midnight theme
            }
        }

        // ------------- Apply theme colors/fonts to form, labels, and buttons -------------
        private void ApplyTheme(string themeName)
        {
            currentThemeName = themeName;                        // Store current theme name

            Color formBack;                                      // Form background color
            Color tabBack;                                       // Tab background color
            Color displayBack;                                   // Unused now for labels but kept for logic
            Color displayFore;                                   // Text color for displays
            Color buttonBack;                                    // Background for non-digit buttons
            Color buttonDigitBack;                               // Background for digit buttons
            Color buttonFore;                                    // Foreground color for buttons
            Font displayFont;                                    // Font for main display labels
            Font buttonFont;                                     // Font for buttons

            if (themeName == "Dark")                             // Dark theme settings
            {
                formBack = Color.FromArgb(45, 45, 48);
                tabBack = Color.FromArgb(37, 37, 38);
                displayBack = Color.FromArgb(30, 30, 30);
                displayFore = Color.White;
                buttonBack = Color.FromArgb(63, 63, 70);
                buttonDigitBack = Color.FromArgb(0, 122, 204);
                buttonFore = Color.White;
                displayFont = new Font("Segoe UI", 36f, FontStyle.Bold);
                buttonFont = new Font("Segoe UI", 14.25f, FontStyle.Regular);
            }
            else if (themeName == "Midnight")                    // Midnight theme settings
            {
                formBack = Color.FromArgb(10, 20, 40);
                tabBack = Color.FromArgb(15, 25, 50);
                displayBack = Color.FromArgb(5, 10, 25);
                displayFore = Color.FromArgb(180, 220, 255);
                buttonBack = Color.FromArgb(25, 45, 80);
                buttonDigitBack = Color.FromArgb(40, 80, 140);
                buttonFore = Color.White;
                displayFont = new Font("Consolas", 34f, FontStyle.Bold);
                buttonFont = new Font("Segoe UI", 13.5f, FontStyle.Regular);
            }
            else // Light                                           // Light theme settings
            {
                formBack = SystemColors.Control;
                tabBack = Color.White;
                displayBack = Color.White;
                displayFore = Color.Black;
                buttonBack = SystemColors.ButtonHighlight;
                buttonDigitBack = SystemColors.ActiveCaption;
                buttonFore = Color.Black;
                displayFont = new Font("Segoe UI", 36f, FontStyle.Regular);
                buttonFont = new Font("Segoe UI", 14.25f, FontStyle.Regular);
            }

            // Store base colors for hover logic
            baseDigitBackColor = buttonDigitBack;               // Save digit background
            baseNonDigitBackColor = buttonBack;                 // Save non-digit background

            // Form background
            this.BackColor = formBack;                          // Apply form background color

            // Tab pages background
            tabPage1.BackColor = tabBack;                       // Calculator tab background
            tabPage2.BackColor = tabBack;                       // FX tab background

            // Displays – BACKGROUND SAME AS TAB
            lbl_result.BackColor = tabBack;                     // Main result label background
            lbl_result.ForeColor = displayFore;                 // Main result text color
            lbl_result.Font = displayFont;                      // Main result font

            lbl_expression.BackColor = tabBack;                 // Expression label background
            lbl_expression.ForeColor = displayFore;             // Expression text color

            lbl_fx_display.BackColor = tabBack;                 // FX display label background
            lbl_fx_display.ForeColor = displayFore;             // FX display text color
            lbl_fx_display.Font = displayFont;                  // FX display font

            lbl_fx_status.BackColor = tabBack;                  // FX status label background
            lbl_fx_status.ForeColor = displayFore;              // FX status text color

            // Radio buttons (theming them to keep readable)
            rad_light.BackColor = formBack;                     // Light radio background
            rad_dark.BackColor = formBack;                      // Dark radio background
            rad_midnight.BackColor = formBack;                  // Midnight radio background
            rad_light.ForeColor = displayFore;                  // Light radio text
            rad_dark.ForeColor = displayFore;                   // Dark radio text
            rad_midnight.ForeColor = displayFore;               // Midnight radio text

            // Apply to all buttons on both tabs
            ApplyThemeToButtons(tabPage1, buttonBack, buttonDigitBack, buttonFore, buttonFont); // Calculator buttons
            ApplyThemeToButtons(tabPage2, buttonBack, buttonDigitBack, buttonFore, buttonFont); // FX buttons
        }

        // ------------- Apply button colors/fonts to all buttons inside a parent control tree -------------
        private void ApplyThemeToButtons(Control parent, Color buttonBack, Color digitBack, Color buttonFore, Font buttonFont)
        {
            foreach (Control c in parent.Controls)               // Loop over child controls
            {
                // Recurse into container controls (panels, tables, tab pages, group boxes, etc.)
                if (c is Panel || c is TableLayoutPanel || c is TabControl || c is TabPage || c is GroupBox)
                {
                    ApplyThemeToButtons(c, buttonBack, digitBack, buttonFore, buttonFont); // Recursive call
                }

                Button b = c as Button;                          // Try cast to Button
                if (b != null)
                {
                    // Decide if this is a digit button
                    bool isDigit =
                        b.Text == "0" || b.Text == "1" || b.Text == "2" ||
                        b.Text == "3" || b.Text == "4" || b.Text == "5" ||
                        b.Text == "6" || b.Text == "7" || b.Text == "8" ||
                        b.Text == "9";

                    b.BackColor = isDigit ? digitBack : buttonBack; // Set background based on digit or not
                    b.ForeColor = buttonFore;                       // Set text color
                    b.Font = buttonFont;                            // Set font
                }
            }
        }

        #endregion

        #region mouse hover effects for buttons

        // ------------- Attach MouseEnter/MouseLeave handlers to all buttons under a root control -------------
        private void WireAllButtonsHover(Control root)
        {
            foreach (Control c in root.Controls)               // Loop over child controls
            {
                if (c is Button b)
                {
                    b.MouseEnter -= Button_MouseEnter;         // Remove existing handler (avoid duplicates)
                    b.MouseEnter += Button_MouseEnter;         // Add hover enter handler
                    b.MouseLeave -= Button_MouseLeave;         // Remove existing leave handler
                    b.MouseLeave += Button_MouseLeave;         // Add hover leave handler
                }

                // Recurse into containers like TableLayoutPanel, TabPage, etc.
                if (c.HasChildren)
                {
                    WireAllButtonsHover(c);                    // Recursive call for nested controls
                }
            }
        }

        // ------------- Check if a button is a digit button by its text -------------
        private bool IsDigitButton(Button b)
        {
            string t = b.Text;                                  // Button text
            return t == "0" || t == "1" || t == "2" || t == "3" || t == "4" ||
                   t == "5" || t == "6" || t == "7" || t == "8" || t == "9";
        }

        // ------------- Mouse enter: swap digit/non-digit background colors -------------
        private void Button_MouseEnter(object sender, EventArgs e)
        {
            Button b = sender as Button;                        // Cast sender to Button
            if (b == null) return;                              // Safety

            bool isDigit = IsDigitButton(b);                    // Check if digit button

            // Swap colors on hover: digits use non-digit color, others use digit color
            if (isDigit)
                b.BackColor = baseNonDigitBackColor;            // Digit becomes non-digit color
            else
                b.BackColor = baseDigitBackColor;               // Non-digit becomes digit color
        }

        // ------------- Mouse leave: restore original background color for this theme -------------
        private void Button_MouseLeave(object sender, EventArgs e)
        {
            Button b = sender as Button;                        // Cast sender to Button
            if (b == null) return;                              // Safety

            bool isDigit = IsDigitButton(b);                    // Check if digit button

            // Restore original color based on type
            if (isDigit)
                b.BackColor = baseDigitBackColor;               // Restore digit color
            else
                b.BackColor = baseNonDigitBackColor;            // Restore non-digit color
        }

        #endregion

     
    }
}