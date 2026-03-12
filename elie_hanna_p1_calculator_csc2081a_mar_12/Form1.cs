using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace elie_hanna_p1_calculator_csc2081a_mar_12
{
    public partial class Form1 : Form
    {
        #region Initial variables and constants

        // =========================
        // Initial variables / fields
        // =========================

        private string currentThemeName = "Light";
        private Color baseDigitBackColor;
        private Color baseNonDigitBackColor;

        private const int MAX_TOKENS = 256;
        private readonly string[] tokens = new string[MAX_TOKENS];
        private int tokenCount = 0;

        private double memoryValue = 0.0;

        private string lastBinaryOp = "";
        private double lastBinaryRight = 0;
        private bool canRepeatEquals = false;
        private bool isEnteringNumber = false;

        private bool isSecondMode = false;
        private Color secondDefaultBackColor = SystemColors.ActiveBorder;
        private bool fxIsEnteringAmount = false;

        private const int MAX_RESULT_LEN = 14;
        private const string SCI_FORMAT = "0.###E+0";
        private const double INTEGER_TOLERANCE = 1e-12;
        private const int MAX_FACTORIAL_INPUT = 170;

        // FX hard-coded rates relative to 1 USD
        private const double EUR_PER_USD = 0.92;       // 1 USD ≈ 0.92 EUR (example)
        private const double LBP_PER_USD = 89500.0;    // 1 USD = 89,500 LBP

        #endregion

        public Form1()
        {
            InitializeComponent();

            rad_light.CheckedChanged += ThemeRadio_CheckedChanged;
            rad_dark.CheckedChanged += ThemeRadio_CheckedChanged;
            rad_midnight.CheckedChanged += ThemeRadio_CheckedChanged;

            // Attach hover handlers to all buttons
            WireAllButtonsHover(tabPage1);
            WireAllButtonsHover(tabPage2);

            ApplyTheme("Light");

            WireAllButtonsToOneHandlerIterative(tabPage1, Button_Click);
            WireAllButtonsToOneHandlerIterative(tabPage2, Button_Click_FX);

            ClearAll();
            SetSecondMode(false);

            FxInitCombos();
            FxClearAll();
        }

        #region Common helper methods

        // =========================
        // Common helpers
        // =========================

        private void WireAllButtonsToOneHandlerIterative(Control root, EventHandler handler)
        {
            // Handle this control
            Button b = root as Button;
            if (b != null)
            {
                b.Click -= handler;
                b.Click += handler;
            }

            // Recurse into children
            for (int i = 0; i < root.Controls.Count; i++)
            {
                WireAllButtonsToOneHandlerIterative(root.Controls[i], handler);
            }
        }

        private void CopyFromLabel(Label sourceLabel, Label statusLabel)
        {
            if (sourceLabel == null) return;

            try
            {
                Clipboard.SetText(sourceLabel.Text);
                if (statusLabel != null)
                    statusLabel.Text = "Copied";
            }
            catch
            {
                if (statusLabel != null)
                    statusLabel.Text = "Copy failed";
            }
        }

        private void PasteToLabel(Label targetLabel, Label statusLabel, ref bool isEnteringFlag)
        {
            if (targetLabel == null) return;

            try
            {
                string t = Clipboard.GetText();
                if (t == null) t = "";
                t = t.Trim();
                if (t.Length == 0)
                {
                    if (statusLabel != null)
                        statusLabel.Text = "Clipboard empty";
                    return;
                }

                double v;
                if (!double.TryParse(t, NumberStyles.Float, CultureInfo.InvariantCulture, out v) &&
                    !double.TryParse(t, NumberStyles.Float, CultureInfo.CurrentCulture, out v))
                {
                    if (statusLabel != null)
                        statusLabel.Text = "Invalid paste";
                    return;
                }

                // For calculator, you'll call this with lbl_result;
                // for FX, with lbl_fx_display.
                targetLabel.Text = FormatForDisplay(v);

                isEnteringFlag = true;
                if (statusLabel != null)
                    statusLabel.Text = "";
            }
            catch
            {
                if (statusLabel != null)
                    statusLabel.Text = "Paste failed";
            }
        }

        private void AppendDecimalCommon(Label displayLabel, ref bool isEnteringFlag)
        {
            if (!isEnteringFlag)
            {
                displayLabel.Text = "0.";
                isEnteringFlag = true;
                return;
            }

            if (displayLabel.Text.IndexOf('.') < 0)
                displayLabel.Text += ".";
        }

        private void AppendDigitCommon(Label displayLabel, string digit, ref bool isEnteringFlag)
        {
            if (!isEnteringFlag || displayLabel.Text == "0")
            {
                displayLabel.Text = digit;
                isEnteringFlag = true;
            }
            else
            {
                displayLabel.Text += digit;
            }
        }

        private void BackspaceCommon(Label displayLabel, ref bool isEnteringFlag, Action clearEntryAction)
        {
            if (!isEnteringFlag)
            {
                clearEntryAction();
                return;
            }

            if (displayLabel.Text.Length <= 1 ||
                (displayLabel.Text.Length == 2 && displayLabel.Text.StartsWith("-", StringComparison.Ordinal)))
            {
                displayLabel.Text = "0";
                isEnteringFlag = false;
                return;
            }

            displayLabel.Text = displayLabel.Text.Substring(0, displayLabel.Text.Length - 1);

            if (displayLabel.Text == "-" || displayLabel.Text.Length == 0)
            {
                displayLabel.Text = "0";
                isEnteringFlag = false;
            }
        }

        private double ParseDisplayValue(Label displayLabel)
        {
            double v;
            if (double.TryParse(displayLabel.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out v))
                return v;

            return 0;
        }

        // =========================
        // Token array helpers
        // =========================

        private void TokensClear()
        {
            tokenCount = 0;
        }

        private void TokensAdd(string value)
        {
            if (tokenCount < MAX_TOKENS)
            {
                tokens[tokenCount] = value;
                tokenCount++;
            }
        }

        private bool TokensContains(string value)
        {
            for (int i = 0; i < tokenCount; i++)
            {
                if (tokens[i] == value) return true;
            }
            return false;
        }

        private string TokensLastOrDefault()
        {
            if (tokenCount == 0) return null;
            return tokens[tokenCount - 1];
        }

        private int CopyTokensTo(string[] destination)
        {
            int n = tokenCount;
            for (int i = 0; i < n; i++)
                destination[i] = tokens[i];
            return n;
        }

        private string BuildExpressionString()
        {
            return BuildExpressionStringFromArray(tokens, tokenCount);
        }

        private string BuildExpressionStringFromArray(string[] arr, int count)
        {
            if (count == 0) return "";

            string result = arr[0];
            for (int i = 1; i < count; i++)
            {
                result += " " + arr[i];
            }
            return result;
        }

        private static bool IsInteger(double x)
        {
            return Math.Abs(x - Math.Round(x)) < INTEGER_TOLERANCE;
        }

        private static double Factorial(double n)
        {
            if (n < 0) return double.NaN;
            if (!IsInteger(n)) return double.NaN;
            if (n > MAX_FACTORIAL_INPUT) return double.NaN;

            double r = 1;
            int limit = (int)Math.Round(n);
            for (int i = 2; i <= limit; i++)
                r *= i;
            return r;
        }

        private string FormatForDisplay(double v)
        {
            if (Math.Abs(v) < 1e-15) v = 0;
            string normal = v.ToString(CultureInfo.InvariantCulture);
            if (normal.Length <= MAX_RESULT_LEN) return normal;
            return v.ToString(SCI_FORMAT, CultureInfo.InvariantCulture);
        }

        #endregion

        #region Calculator logic

        // =========================
        // Calculator: button click
        // =========================

        private void Button_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;
            string v = btn.Text.Trim();

            switch (v)
            {
                case "2nd":
                    ToggleSecondMode();
                    break;

                case "(":
                    PressLeftParen();
                    break;

                case ")":
                    PressRightParen();
                    break;

                case ".":
                    AppendDecimalCommon(lbl_result, ref isEnteringNumber);
                    break;

                case "+/-":
                    ToggleSign();
                    break;

                case "C":
                    ClearAll();
                    break;

                case "CE":
                    ClearEntry();
                    break;

                case "⌫":
                    BackspaceCommon(lbl_result, ref isEnteringNumber, ClearEntry);
                    break;

                case "%":
                    Percent();
                    break;

                case "1/x":
                    Reciprocal();
                    break;

                case "x²":
                    Square();
                    break;

                case "²√x":
                    Sqrt();
                    break;

                case "|x|":
                    Abs();
                    break;

                case "π":
                    InsertConstant(Math.PI);
                    break;

                case "e":
                    InsertConstant(Math.E);
                    break;

                case "exp":
                    Exp();
                    break;

                case "ln":
                    Ln();
                    break;

                case "log":
                    Log10();
                    break;

                case "10ˣ":
                case "10^x":
                    TenPowerX();
                    break;

                case "sin":
                    Sin();
                    break;

                case "cos":
                    Cos();
                    break;

                case "tan":
                    Tan();
                    break;

                case "n!":
                    FactorialUnary();
                    break;

                case "+":
                case "-":
                case "X":
                case "÷":
                case "mod":
                case "xʸ":
                    PressOperator(v);
                    break;

                case "=":
                    PressEquals();
                    break;

                case "Copy":
                    CopyFromLabel(lbl_result, lbl_expression);
                    break;

                case "Paste":
                    PasteToLabel(lbl_result, lbl_expression, ref isEnteringNumber);
                    break;

                case "MC":
                    MemoryClear();
                    break;

                case "MR":
                    MemoryRecall();
                    break;

                case "MS":
                    MemoryStore();
                    break;

                case "M+":
                    MemoryAdd();
                    break;

                case "M-":
                    MemorySubtract();
                    break;
                default:
                    if (v.Length == 1 && char.IsDigit(v[0]))
                        AppendDigitCommon(lbl_result, v, ref isEnteringNumber);
                    break;
            }
        }

        // =========================
        // Calculator core logic
        // =========================

        private static int Precedence(string op)
        {
            if (op == "xʸ") return 3;
            if (op == "X" || op == "÷" || op == "mod") return 2;
            if (op == "+" || op == "-") return 1;
            return 0;
        }

        private static double Apply(double a, double b, string op)
        {
            if (op == "+") return a + b;
            if (op == "-") return a - b;
            if (op == "X") return a * b;
            if (op == "÷")
            {
                if (b == 0) return double.NaN;
                return a / b;
            }
            if (op == "mod")
            {
                if (b == 0) return double.NaN;
                return a % b;
            }
            if (op == "xʸ") return Math.Pow(a, b);
            return double.NaN;
        }

        private void CaptureRepeatEqualsMemory(string[] working, int workingCount)
        {
            canRepeatEquals = false;
            if (workingCount < 3) return;

            int i = workingCount - 1;
            double right;
            if (!double.TryParse(working[i], NumberStyles.Float, CultureInfo.InvariantCulture, out right))
                return;

            string op = working[i - 1];
            bool opIsBinary =
                op == "+" || op == "-" || op == "X" || op == "÷" || op == "mod" || op == "xʸ";
            if (!opIsBinary) return;

            string leftTok = working[i - 2];
            double dummy;
            bool leftOk =
                double.TryParse(leftTok, NumberStyles.Float, CultureInfo.InvariantCulture, out dummy) ||
                leftTok == ")";

            if (!leftOk) return;

            lastBinaryOp = op;
            lastBinaryRight = right;
            canRepeatEquals = true;
        }

        private double EvaluateTokens(string[] tks, int count)
        {
            double[] valStack = new double[MAX_TOKENS];
            int valTop = 0;

            string[] opStack = new string[MAX_TOKENS];
            int opTop = 0;

            for (int i = 0; i < count; i++)
            {
                string tk = tks[i];

                double num;
                if (double.TryParse(tk, NumberStyles.Float, CultureInfo.InvariantCulture, out num))
                {
                    if (valTop < MAX_TOKENS)
                    {
                        valStack[valTop] = num;
                        valTop++;
                    }
                }
                else if (tk == "(")
                {
                    if (opTop < MAX_TOKENS)
                    {
                        opStack[opTop] = tk;
                        opTop++;
                    }
                }
                else if (tk == ")")
                {
                    while (opTop > 0 && opStack[opTop - 1] != "(")
                    {
                        if (valTop < 2) return double.NaN;
                        string op = opStack[--opTop];
                        double b = valStack[--valTop];
                        double a = valStack[--valTop];
                        double res = Apply(a, b, op);
                        if (double.IsNaN(res) || double.IsInfinity(res)) return double.NaN;
                        valStack[valTop] = res;
                        valTop++;
                    }
                    if (opTop == 0) return double.NaN;
                    opTop--; // pop "("
                }
                else
                {
                    while (opTop > 0 &&
                           opStack[opTop - 1] != "(" &&
                           Precedence(opStack[opTop - 1]) >= Precedence(tk))
                    {
                        if (valTop < 2) return double.NaN;
                        string op = opStack[--opTop];
                        double b = valStack[--valTop];
                        double a = valStack[--valTop];
                        double res = Apply(a, b, op);
                        if (double.IsNaN(res) || double.IsInfinity(res)) return double.NaN;
                        valStack[valTop] = res;
                        valTop++;
                    }

                    if (opTop < MAX_TOKENS)
                    {
                        opStack[opTop] = tk;
                        opTop++;
                    }
                }
            }

            while (opTop > 0)
            {
                if (opStack[opTop - 1] == "(") return double.NaN;
                if (valTop < 2) return double.NaN;
                string op = opStack[--opTop];
                double b = valStack[--valTop];
                double a = valStack[--valTop];
                double res = Apply(a, b, op);
                if (double.IsNaN(res) || double.IsInfinity(res)) return double.NaN;
                valStack[valTop] = res;
                valTop++;
            }

            if (valTop != 1) return double.NaN;
            return valStack[0];
        }

        // Returns the index after the last "(" (innermost open parenthesis).
        // If there is no "(", it returns 0 (start of the whole expression).
        private int GetInnermostParenStart(string[] src, int srcCount)
        {
            int start = -1;
            for (int i = srcCount - 1; i >= 0; i--)
            {
                if (src[i] == "(")
                {
                    start = i;
                    break;
                }
            }

            if (start < 0)
                return 0;          // no "(", evaluate whole expression

            return start + 1;       // slice starts just after "("
        }

        private void RefreshDisplay()
        {
            // Copy current tokens to working array
            string[] working = new string[MAX_TOKENS];
            int workingCount = CopyTokensTo(working);

            // If user is entering a number, append it as the last token
            if (isEnteringNumber)
            {
                if (workingCount < MAX_TOKENS)
                {
                    working[workingCount] = lbl_result.Text;
                    workingCount++;
                }
            }

            if (workingCount == 0)
                return;

            // Find start index of innermost parenthesis slice
            int sliceStart = GetInnermostParenStart(working, workingCount);

            // Compute initial slice length
            int sliceLen = workingCount - sliceStart;
            if (sliceLen <= 0)
                return;

            // If last token in slice is an operator, drop it
            int lastIndex = sliceStart + sliceLen - 1;
            string last = working[lastIndex];
            if (IsOperator(last))
            {
                sliceLen--;
            }

            if (sliceLen <= 0)
                return;

            // If the slice starts with "(", it's incomplete -> do nothing
            if (working[sliceStart] == "(")
                return;

            // Build a temporary array containing only the slice
            string[] slice = new string[sliceLen];
            for (int i = 0; i < sliceLen; i++)
            {
                slice[i] = working[sliceStart + i];
            }

            // Evaluate only that slice
            double v = EvaluateTokens(slice, sliceLen);
            if (!double.IsNaN(v) && !double.IsInfinity(v))
            {
                lbl_result.Text = FormatForDisplay(v);
            }
        }

        private void PushCurrentEntryIfNeeded()
        {
            if (isEnteringNumber)
            {
                TokensAdd(lbl_result.Text);
                isEnteringNumber = false;
            }
        }

        private bool IsOperator(string tk)
        {
            return tk == "+" || tk == "-" || tk == "X" || tk == "÷" || tk == "mod" || tk == "xʸ";
        }

        private void PressLeftParen()
        {
            if (tokenCount > 0)
            {
                string last = TokensLastOrDefault();
                double dummy;
                bool lastIsNumber = double.TryParse(last, NumberStyles.Float, CultureInfo.InvariantCulture, out dummy);
                if (lastIsNumber || last == ")")
                    TokensAdd("X");
            }

            TokensAdd("(");
            lbl_expression.Text = BuildExpressionString();
            lbl_result.Text = "0";
            isEnteringNumber = false;
        }

        private void PressRightParen()
        {
            PushCurrentEntryIfNeeded();
            if (!TokensContains("(")) return;

            if (tokenCount > 0)
            {
                string last = TokensLastOrDefault();
                if (last == "(" || IsOperator(last))
                    return;
            }

            TokensAdd(")");
            lbl_expression.Text = BuildExpressionString();
            RefreshDisplay();
        }

        private void PressOperator(string op)
        {
            canRepeatEquals = false;

            PushCurrentEntryIfNeeded();

            if (tokenCount == 0)
            {
                TokensAdd(CurrentValue().ToString(CultureInfo.InvariantCulture));
                TokensAdd(op);
            }
            else
            {
                string last = TokensLastOrDefault();
                if (IsOperator(last))
                {
                    tokens[tokenCount - 1] = op;
                }
                else if (last == "(")
                {
                    if (op == "-" || op == "+")
                    {
                        lbl_result.Text = (op == "-") ? "-0" : "0";
                        isEnteringNumber = true;
                        return;
                    }
                    return;
                }
                else
                {
                    TokensAdd(op);
                }
            }

            lbl_expression.Text = BuildExpressionString();
            RefreshDisplay();
        }

        private void PressEquals()
        {
            if (tokenCount == 0 && !isEnteringNumber && canRepeatEquals && lastBinaryOp != "")
            {
                double current = CurrentValue();
                double rRepeat = Apply(current, lastBinaryRight, lastBinaryOp);
                if (double.IsNaN(rRepeat) || double.IsInfinity(rRepeat))
                {
                    ShowError("Invalid input");
                    return;
                }

                lbl_expression.Text = FormatForDisplay(current) + " " +
                                      lastBinaryOp + " " +
                                      FormatForDisplay(lastBinaryRight) + " =";
                SetResult(rRepeat);
                isEnteringNumber = false;
                return;
            }

            string[] working = new string[MAX_TOKENS];
            int workingCount = CopyTokensTo(working);

            if (isEnteringNumber)
            {
                if (workingCount < MAX_TOKENS)
                {
                    working[workingCount] = lbl_result.Text;
                    workingCount++;
                }
            }

            while (workingCount > 0)
            {
                string last = working[workingCount - 1];
                bool lastIsOp = IsOperator(last);
                if (lastIsOp || last == "(")
                {
                    workingCount--;
                    continue;
                }
                break;
            }

            if (workingCount == 0) return;

            double r = EvaluateTokens(working, workingCount);

            if (double.IsNaN(r) || double.IsInfinity(r))
            {
                ShowError("Invalid input");
                return;
            }

            lbl_expression.Text = BuildExpressionStringFromArray(working, workingCount) + " =";
            SetResult(r);
            CaptureRepeatEqualsMemory(working, workingCount);
            TokensClear();
            isEnteringNumber = false;
        }

        private double CurrentValue()
        {
            return ParseDisplayValue(lbl_result);
        }

        private void SetResult(double v)
        {
            if (double.IsNaN(v) || double.IsInfinity(v))
            {
                ShowError("Invalid input");
                return;
            }

            lbl_result.Text = FormatForDisplay(v);
        }

        private void ShowError(string message)
        {
            lbl_expression.Text = "";
            lbl_result.Text = message;

            TokensClear();
            isEnteringNumber = false;
            lastBinaryOp = "";
            lastBinaryRight = 0;
            canRepeatEquals = false;
        }

        private void ToggleSign()
        {
            if (lbl_result.Text.StartsWith("-", StringComparison.Ordinal))
                lbl_result.Text = lbl_result.Text.Substring(1);
            else if (lbl_result.Text != "0")
                lbl_result.Text = "-" + lbl_result.Text;

            isEnteringNumber = true;
        }

        private void ClearAll()
        {
            TokensClear();
            lastBinaryOp = "";
            lastBinaryRight = 0;
            canRepeatEquals = false;

            lbl_expression.Text = "";
            lbl_result.Text = "0";
            isEnteringNumber = false;
        }

        private void ClearEntry()
        {
            lbl_result.Text = "0";
            isEnteringNumber = false;
        }

        private void Percent()
        {
            double current = CurrentValue();
            SetResult(current / 100.0);
            isEnteringNumber = true;
        }

        private void Reciprocal()
        {
            double x = CurrentValue();
            if (x == 0)
            {
                ShowError("Cannot divide by 0");
                return;
            }

            SetResult(1.0 / x);
            isEnteringNumber = true;
        }

        private void Square()
        {
            double x = CurrentValue();
            SetResult(x * x);
            isEnteringNumber = true;
        }

        private void Sqrt()
        {
            double x = CurrentValue();
            if (x < 0)
            {
                ShowError("Invalid input");
                return;
            }

            SetResult(Math.Sqrt(x));
            isEnteringNumber = true;
        }

        private void Abs()
        {
            double x = CurrentValue();
            SetResult(Math.Abs(x));
            isEnteringNumber = true;
        }

        private void InsertConstant(double value)
        {
            SetResult(value);
            isEnteringNumber = true;
        }

        private void Exp()
        {
            double x = CurrentValue();
            SetResult(Math.Exp(x));
            isEnteringNumber = true;
        }

        private void Ln()
        {
            double x = CurrentValue();
            if (x <= 0)
            {
                ShowError("Invalid input");
                return;
            }

            SetResult(Math.Log(x));
            isEnteringNumber = true;
        }

        private void Log10()
        {
            double x = CurrentValue();
            if (x <= 0)
            {
                ShowError("Invalid input");
                return;
            }

            SetResult(Math.Log10(x));
            isEnteringNumber = true;
        }

        private void TenPowerX()
        {
            double x = CurrentValue();
            SetResult(Math.Pow(10, x));
            isEnteringNumber = true;
        }

        private void FactorialUnary()
        {
            double x = CurrentValue();
            double f = Factorial(x);
            if (double.IsNaN(f) || double.IsInfinity(f))
            {
                ShowError("Invalid input");
                return;
            }
            SetResult(f);
            isEnteringNumber = true;
        }

        private void Sin()
        {
            double x = CurrentValue();
            SetResult(Math.Sin(x));
            isEnteringNumber = true;
        }

        private void Cos()
        {
            double x = CurrentValue();
            SetResult(Math.Cos(x));
            isEnteringNumber = true;
        }

        private void Tan()
        {
            double x = CurrentValue();
            SetResult(Math.Tan(x));
            isEnteringNumber = true;
        }

        private void SetSecondMode(bool enabled)
        {
            isSecondMode = enabled;

            btn_second.BackColor = enabled
                ? Color.FromArgb(255, 205, 145)
                : secondDefaultBackColor;

            btn_power_10.Text = enabled ? "sin" : "10ˣ";
            btn_log10.Text = enabled ? "cos" : "log";
            btn_ln.Text = enabled ? "tan" : "ln";
        }

        private void ToggleSecondMode()
        {
            SetSecondMode(!isSecondMode);
        }

        private void MemoryClear()
        {
            memoryValue = 0.0;
        }

        private void MemoryStore()
        {
            double current = CurrentValue();
            memoryValue = current;
        }

        private void MemoryRecall()
        {
            SetResult(memoryValue);
            isEnteringNumber = true;
        }

        private void MemoryAdd()
        {
            double current = CurrentValue();
            memoryValue += current;
        }

        private void MemorySubtract()
        {
            double current = CurrentValue();
            memoryValue -= current;
        }

        #endregion

        #region FX (currency converter) logic

        // =========================
        // FX (currency) logic
        // =========================

        private void Button_Click_FX(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;
            string v = btn.Text.Trim();

            if (btn.Name == "btn_fx_convert" || v.Equals("Convert", StringComparison.OrdinalIgnoreCase))
            {
                FxConvert();
                return;
            }

            if (v == ".")
            {
                AppendDecimalCommon(lbl_fx_display, ref fxIsEnteringAmount);
            }
            else if (v == "C")
            {
                FxClearAll();
            }
            else if (v == "⌫" || v == "←" || v == "Back")
            {
                BackspaceCommon(lbl_fx_display, ref fxIsEnteringAmount, FxClearEntry);
            }
            else if (v.Equals("Copy", StringComparison.OrdinalIgnoreCase))
            {
                // Reusable copy
                CopyFromLabel(lbl_fx_display, lbl_fx_status);
            }
            else if (v.Equals("Paste", StringComparison.OrdinalIgnoreCase))
            {
                // Reusable paste
                PasteToLabel(lbl_fx_display, lbl_fx_status, ref fxIsEnteringAmount);
            }
            else
            {
                if (v.Length == 1 && char.IsDigit(v[0]))
                    AppendDigitCommon(lbl_fx_display, v, ref fxIsEnteringAmount);
            }
        }

        private double FxCurrentValue()
        {
            return ParseDisplayValue(lbl_fx_display);
        }

        private void FxSetDisplay(double v)
        {
            if (double.IsNaN(v) || double.IsInfinity(v))
            {
                lbl_fx_status.Text = "Invalid input";
                return;
            }

            lbl_fx_display.Text = FormatForDisplay(v);
        }

        private static string NormalizeCurrency(string code)
        {
            if (code == null) return "";
            return code.Trim().ToUpperInvariant();
        }

        private void FxInitCombos()
        {
            if (cmb_fx_from == null || cmb_fx_to == null) return;

            if (cmb_fx_from.Items.Count == 0)
            {
                cmb_fx_from.Items.Add("USD");
                cmb_fx_from.Items.Add("EUR");
                cmb_fx_from.Items.Add("LBP");
            }

            if (cmb_fx_to.Items.Count == 0)
            {
                cmb_fx_to.Items.Add("USD");
                cmb_fx_to.Items.Add("EUR");
                cmb_fx_to.Items.Add("LBP");
            }

            cmb_fx_from.DropDownStyle = ComboBoxStyle.DropDownList;
            cmb_fx_to.DropDownStyle = ComboBoxStyle.DropDownList;

            cmb_fx_from.SelectedIndex = 0;
            cmb_fx_to.SelectedIndex = 1;
        }

        private void FxClearAll()
        {
            fxIsEnteringAmount = false;
            lbl_fx_display.Text = "0";
            lbl_fx_status.Text = "";

            if (cmb_fx_from != null) cmb_fx_from.SelectedIndex = 0;
            if (cmb_fx_to != null) cmb_fx_to.SelectedIndex = 1;
        }

        private void FxClearEntry()
        {
            lbl_fx_display.Text = "0";
            fxIsEnteringAmount = false;
        }

        private double FxGetRateOffline(string from, string to)
        {
            double amountInUsdPerOneFrom;

            if (from == "USD")
            {
                amountInUsdPerOneFrom = 1.0;
            }
            else if (from == "EUR")
            {
                amountInUsdPerOneFrom = 1.0 / EUR_PER_USD;
            }
            else if (from == "LBP")
            {
                amountInUsdPerOneFrom = 1.0 / LBP_PER_USD;
            }
            else
            {
                return 0;
            }

            double usdToTo;

            if (to == "USD")
            {
                usdToTo = 1.0;
            }
            else if (to == "EUR")
            {
                usdToTo = EUR_PER_USD;
            }
            else if (to == "LBP")
            {
                usdToTo = LBP_PER_USD;
            }
            else
            {
                return 0;
            }

            return amountInUsdPerOneFrom * usdToTo;
        }

        private void FxConvert()
        {
            string from = cmb_fx_from == null ? "" : cmb_fx_from.SelectedItem as string;
            string to = cmb_fx_to == null ? "" : cmb_fx_to.SelectedItem as string;

            from = NormalizeCurrency(from);
            to = NormalizeCurrency(to);

            if (from.Length == 0 || to.Length == 0)
            {
                lbl_fx_status.Text = "Select FROM and TO currencies";
                return;
            }

            if (from == to)
            {
                lbl_fx_status.Text = "FROM and TO cannot be the same";
                return;
            }

            double amount = FxCurrentValue();
            double rate = FxGetRateOffline(from, to);

            if (rate == 0)
            {
                lbl_fx_status.Text = "Conversion failed (invalid rate)";
                return;
            }

            double converted = amount * rate;

            FxSetDisplay(converted);
            fxIsEnteringAmount = false;
            lbl_fx_status.Text = from + "->" + to + " rate=" +
                                 rate.ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        #region Radio theme logic

        private void ThemeRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_light.Checked)
            {
                ApplyTheme("Light");
            }
            else if (rad_dark.Checked)
            {
                ApplyTheme("Dark");
            }
            else if (rad_midnight.Checked)
            {
                ApplyTheme("Midnight");
            }
        }


        private void ApplyTheme(string themeName)
        {
            currentThemeName = themeName;

            Color formBack;
            Color tabBack;
            Color displayBack;
            Color displayFore;
            Color buttonBack;
            Color buttonDigitBack;
            Color buttonFore;
            Font displayFont;
            Font buttonFont;

            if (themeName == "Dark")
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
            else if (themeName == "Midnight")
            {
                formBack = Color.FromArgb(10, 20, 40);
                tabBack = Color.FromArgb(15, 25, 50);
                displayBack = Color.FromArgb(5, 10, 25);
                displayFore = Color.FromArgb(180, 220, 255);
                buttonBack = Color.FromArgb(25, 45, 80);
                buttonDigitBack = Color.FromArgb(40, 80, 140);
                buttonFore = Color.White;
                // Slightly different font to show “custom font” requirement
                displayFont = new Font("Consolas", 34f, FontStyle.Bold);
                buttonFont = new Font("Segoe UI", 13.5f, FontStyle.Regular);
            }
            else // Light (default)
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
            baseDigitBackColor = buttonDigitBack;
            baseNonDigitBackColor = buttonBack;

            // Form background
            this.BackColor = formBack;

            // Tab pages background
            tabPage1.BackColor = tabBack;
            tabPage2.BackColor = tabBack;

            // Displays
            lbl_result.BackColor = displayBack;
            lbl_result.ForeColor = displayFore;
            lbl_result.Font = displayFont;

            lbl_expression.BackColor = displayBack;
            lbl_expression.ForeColor = displayFore;

            lbl_fx_display.BackColor = displayBack;
            lbl_fx_display.ForeColor = displayFore;
            lbl_fx_display.Font = displayFont;

            lbl_fx_status.BackColor = displayBack;
            lbl_fx_status.ForeColor = displayFore;

            // Radio buttons (keep readable against form background)
            rad_light.BackColor = formBack;
            rad_dark.BackColor = formBack;
            rad_midnight.BackColor = formBack;
            rad_light.ForeColor = displayFore;
            rad_dark.ForeColor = displayFore;
            rad_midnight.ForeColor = displayFore;

            // Apply to all buttons on both tabs
            ApplyThemeToButtons(tabPage1, buttonBack, buttonDigitBack, buttonFore, buttonFont);
            ApplyThemeToButtons(tabPage2, buttonBack, buttonDigitBack, buttonFore, buttonFont);
        }

        private void ApplyThemeToButtons(Control parent, Color buttonBack, Color digitBack, Color buttonFore, Font buttonFont)
        {
            foreach (Control c in parent.Controls)
            {
                // Recurse into containers like TableLayoutPanel
                if (c is Panel || c is TableLayoutPanel || c is TabControl || c is TabPage || c is GroupBox)
                {
                    ApplyThemeToButtons(c, buttonBack, digitBack, buttonFore, buttonFont);
                }

                Button b = c as Button;
                if (b != null)
                {
                    // Decide if this is a digit button to use digitBack color
                    bool isDigit =
                        b.Text == "0" || b.Text == "1" || b.Text == "2" ||
                        b.Text == "3" || b.Text == "4" || b.Text == "5" ||
                        b.Text == "6" || b.Text == "7" || b.Text == "8" ||
                        b.Text == "9";

                    b.BackColor = isDigit ? digitBack : buttonBack;
                    b.ForeColor = buttonFore;
                    b.Font = buttonFont;
                }
            }
        }

        #endregion

        #region mouse hover effects for buttons

        private void WireAllButtonsHover(Control root)
        {
            foreach (Control c in root.Controls)
            {
                if (c is Button b)
                {
                    b.MouseEnter -= Button_MouseEnter;
                    b.MouseEnter += Button_MouseEnter;
                    b.MouseLeave -= Button_MouseLeave;
                    b.MouseLeave += Button_MouseLeave;
                }

                // Recurse into containers like TableLayoutPanel, TabPage, etc.
                if (c.HasChildren)
                {
                    WireAllButtonsHover(c);
                }
            }
        }

        private bool IsDigitButton(Button b)
        {
            string t = b.Text;
            return t == "0" || t == "1" || t == "2" || t == "3" || t == "4" ||
                   t == "5" || t == "6" || t == "7" || t == "8" || t == "9";
        }

        private void Button_MouseEnter(object sender, EventArgs e)
        {
            Button b = sender as Button;
            if (b == null) return;

            bool isDigit = IsDigitButton(b);

            // Swap colors on hover
            if (isDigit)
                b.BackColor = baseNonDigitBackColor;
            else
                b.BackColor = baseDigitBackColor;
        }

        private void Button_MouseLeave(object sender, EventArgs e)
        {
            Button b = sender as Button;
            if (b == null) return;

            bool isDigit = IsDigitButton(b);

            // Restore original color for this theme
            if (isDigit)
                b.BackColor = baseDigitBackColor;
            else
                b.BackColor = baseNonDigitBackColor;
        }

        #endregion

    }
}