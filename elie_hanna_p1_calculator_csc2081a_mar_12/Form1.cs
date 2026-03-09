using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace elie_hanna_p1_calculator_csc2081a_mar_12
{
    public partial class Form1 : Form
    {
        #region Variables

        #region Calculator Variables

        // Stores the left side value of a binary operation (e.g., in "5 + 3", this would be 5)
        private double leftOperand = 0;

        // Holds the current pending operator (+, -, X, ÷, etc.) waiting for the right operand
        private string pendingOperator = "";

        // Flag indicating whether the user is currently typing a number
        private bool isEnteringNumber = false;

        // Stores the right operand from the last calculation for repeat operations when pressing equals multiple times
        private double lastRightOperand = 0;

        // Stores the operator from the last calculation for repeat operations
        private string lastOperator = "";

        // Flag indicating whether the calculator is in "2nd" mode (shows alternate functions like sin, cos, tan)
        private bool isSecondMode = false;

        // Stores the default background color of the "2nd" button when not active
        private Color secondDefaultBackColor = SystemColors.ActiveBorder;

        #endregion

        #region FX Converter Variables

        // Flag indicating whether the user is currently entering an amount in the FX converter
        private bool fxIsEnteringAmount = false;

        // Shared HttpClient for making API requests to the currency exchange rate service
        private static readonly HttpClient fxHttp = new HttpClient();

        #endregion

        #endregion

        #region Constants

        // Tolerance for checking if a double is effectively an integer
        private const double INTEGER_TOLERANCE = 1e-12;

        // Maximum factorial value that can be calculated without overflow
        private const int MAX_FACTORIAL_INPUT = 170;

        #endregion

        // ------ Form1 Constructor ------
        // Initializes the calculator form, wires up all button event handlers,
        // and sets initial state for both calculator and FX converter tabs
        public Form1()
        {
            // Initialize all form components (auto-generated code)
            InitializeComponent();

            // Wire all buttons in the calculator tab to the main click handler
            WireAllButtonsToOneHandlerIterative(tabPage1, Button_Click);

            // Wire all buttons in the FX converter tab to the FX click handler
            WireAllButtonsToOneHandlerIterative(tabPage2, Button_Click_FX);

            // Reset calculator to initial state
            ClearAll();

            // Ensure calculator starts in normal mode (not "2nd" mode)
            SetSecondMode(false);

            // Initialize the currency dropdown combo boxes
            FxInitCombos();

            // Reset FX converter to initial state
            FxClearAll();
        }

        #region Event Handlers

        #region Calculator Event Handlers

        // ------ Button_Click ------
        // Main event handler for all calculator buttons (tabPage1)
        // Routes button clicks to appropriate operations based on button text
        private void Button_Click(object sender, EventArgs e)
        {
            if (sender is not Button btn) return;
            string v = btn.Text.Trim();

            switch (v)
            {
                case "2nd":
                    ToggleSecondMode();
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

                case "10ˣ" or "10^x":
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

                case "+" or "-" or "X" or "÷" or "mod" or "xʸ":
                    PressOperator(v);
                    break;

                case "=":
                    PressEquals();
                    break;

                default:
                    if (v.Length == 1 && char.IsDigit(v[0]))
                        AppendDigitCommon(lbl_result, v, ref isEnteringNumber);
                    break;
            }
        }

        #endregion

        #region FX Converter Event Handlers

        // ------ Button_Click_FX ------
        // Event handler for all FX converter buttons (tabPage2)
        // Routes button clicks to appropriate currency converter operations
        private async void Button_Click_FX(object sender, EventArgs e)
        {
            if (sender is not Button btn) return;
            string v = btn.Text.Trim();

            // Handle convert button first (needs btn.Name check)
            if (btn.Name == "btn_fx_convert" || v.Equals("Convert", StringComparison.OrdinalIgnoreCase))
            {
                await FxConvertAsync();
                return;
            }

            switch (v)
            {
                case ".":
                    AppendDecimalCommon(lbl_fx_display, ref fxIsEnteringAmount);
                    break;

                case "C":
                    FxClearAll();
                    break;

                case "⌫" or "←" or "Back" or "⟵":
                    BackspaceCommon(lbl_fx_display, ref fxIsEnteringAmount, FxClearEntry);
                    break;

                case var _ when v.Equals("Copy", StringComparison.OrdinalIgnoreCase):
                    FxCopy();
                    break;

                case var _ when v.Equals("Paste", StringComparison.OrdinalIgnoreCase):
                    FxPaste();
                    break;

                default:
                    if (v.Length == 1 && char.IsDigit(v[0]))
                        AppendDigitCommon(lbl_fx_display, v, ref fxIsEnteringAmount);
                    break;
            }
        }

        #endregion



        #region Private Methods

        #region Common Helper Methods

        // ------ WireAllButtonsToOneHandlerIterative ------
        // Recursively finds all Button controls within a container and attaches the same event handler
        // Uses iterative approach with a stack to avoid deep recursion
        private void WireAllButtonsToOneHandlerIterative(Control root, EventHandler handler)
        {
            // Create a stack to traverse the control tree
            var stack = new Stack<Control>();
            stack.Push(root);  // Start with the root control

            // Process controls until the stack is empty
            while (stack.Count > 0)
            {
                // Pop the next control to process
                Control current = stack.Pop();

                // If the current control is a Button, wire up the event handler
                if (current is Button b)
                {
                    b.Click -= handler;  // Remove handler first to avoid duplicates
                    b.Click += handler;   // Add the handler
                }

                // Push all child controls onto the stack for processing
                foreach (Control child in current.Controls)
                    stack.Push(child);
            }
        }

        // ------ AppendDecimalCommon ------
        // Common method to append a decimal point to a display label
        // Used by both calculator and FX converter to avoid code duplication
        private void AppendDecimalCommon(Label displayLabel, ref bool isEnteringFlag)
        {
            // If not currently entering a value, start with "0."
            if (!isEnteringFlag)
            {
                displayLabel.Text = "0.";
                isEnteringFlag = true;
                return;
            }

            // Only add decimal point if one doesn't already exist
            if (!displayLabel.Text.Contains("."))
                displayLabel.Text += ".";
        }

        // ------ AppendDigitCommon ------
        // Common method to append a digit to a display label
        // Used by both calculator and FX converter to avoid code duplication
        private void AppendDigitCommon(Label displayLabel, string digit, ref bool isEnteringFlag)
        {
            // If not currently entering a value or display shows "0", start a new number
            if (!isEnteringFlag || displayLabel.Text == "0")
            {
                displayLabel.Text = digit;      // Replace display with the new digit
                isEnteringFlag = true;           // Mark that user is now entering a value
            }
            else
            {
                displayLabel.Text += digit;      // Append digit to existing value
            }
        }

        // ------ BackspaceCommon ------
        // Common method to remove the last character from a display label
        // Used by both calculator and FX converter to avoid code duplication
        private void BackspaceCommon(Label displayLabel, ref bool isEnteringFlag, Action clearEntryAction)
        {
            // If not entering a value, call the appropriate clear entry method
            if (!isEnteringFlag)
            {
                clearEntryAction();
                return;
            }

            // If only one character left (or "-X" for negative single digit), reset to 0
            if (displayLabel.Text.Length <= 1 || (displayLabel.Text.Length == 2 && displayLabel.Text.StartsWith("-")))
            {
                displayLabel.Text = "0";
                isEnteringFlag = false;
                return;
            }

            // Remove the last character
            displayLabel.Text = displayLabel.Text.Substring(0, displayLabel.Text.Length - 1);

            // If only "-" or empty string remains, reset to 0
            if (displayLabel.Text == "-" || displayLabel.Text.Length == 0)
            {
                displayLabel.Text = "0";
                isEnteringFlag = false;
            }
        }

        // ------ ParseDisplayValue ------
        // Common method to parse a numeric value from a display label
        // Returns 0 if parsing fails
        private double ParseDisplayValue(Label displayLabel)
        {
            // Try to parse using invariant culture (e.g., "123.45")
            if (double.TryParse(displayLabel.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double v))
                return v;

            // Try to parse using current culture as fallback (e.g., "123,45" in some locales)
            if (double.TryParse(displayLabel.Text, out v))
                return v;

            // Return 0 if both parsing attempts fail
            return 0;
        }

        #endregion

        #region Calculator Methods

        #region Calculator Helper Methods

        // ------ CurrentValue ------
        // Retrieves the current numeric value displayed in the calculator result label
        // Returns 0 if parsing fails
        private double CurrentValue()
        {
            // Call the common parsing method with calculator display label
            return ParseDisplayValue(lbl_result);
        }

        // ------ SetResult ------
        // Sets the calculator display to show the given numeric value
        // Shows error message if value is NaN or Infinity
        private void SetResult(double v)
        {
            // Check if the value is invalid (Not a Number or Infinity)
            if (double.IsNaN(v) || double.IsInfinity(v))
            {
                ShowError("Invalid input");
                return;
            }

            // Convert the value to string using invariant culture for consistency
            lbl_result.Text = v.ToString(CultureInfo.InvariantCulture);
        }

        // ------ ShowError ------
        // Displays an error message and resets the calculator state
        // Used when invalid operations occur (e.g., division by zero)
        private void ShowError(string message)
        {
            // Clear the expression display
            lbl_expression.Text = "";

            // Show the error message in the result display
            lbl_result.Text = message;

            // Reset all calculator state variables
            leftOperand = 0;
            pendingOperator = "";
            lastRightOperand = 0;
            lastOperator = "";
            isEnteringNumber = false;
        }

        // ------ Evaluate ------
        // Performs a binary operation on two operands based on the given operator
        // Returns the result or throws an exception for invalid operations (e.g., division by zero)
        private double Evaluate(double a, double b, string op)
        {
            // Use switch expression to determine which operation to perform
            return op switch
            {
                "+" => a + b,                    // Addition
                "-" => a - b,                    // Subtraction
                "X" => a * b,                    // Multiplication
                "÷" => b == 0 ? throw new DivideByZeroException() : a / b,  // Division (check for zero)
                "mod" => b == 0 ? throw new DivideByZeroException() : a % b, // Modulo (check for zero)
                "xʸ" => Math.Pow(a, b),            // Power (x^y)
                _ => throw new InvalidOperationException($"Unknown operator: {op}")  // Throw error for unknown operators
            };
        }

        // ------ IsInteger ------
        // Checks if a double value is effectively an integer (within floating-point tolerance)
        // Used to validate factorial input
        private static bool IsInteger(double x)
        {
            // Compare the difference between x and its rounded value to a small epsilon
            return Math.Abs(x - Math.Round(x)) < INTEGER_TOLERANCE;
        }

        // ------ Factorial ------
        // Calculates the factorial of a number (n!)
        // Throws exceptions for negative numbers, non-integers, or values too large (>170)
        private static double Factorial(double n)
        {
            // Factorial is not defined for negative numbers
            if (n < 0) throw new ArgumentOutOfRangeException(nameof(n));

            // Factorial is only defined for integers
            if (!IsInteger(n)) throw new ArgumentException("Factorial requires integer input.");

            // Results larger than 170! cause overflow in double precision
            if (n > MAX_FACTORIAL_INPUT) throw new OverflowException("Result too large.");

            // Calculate factorial iteratively
            double r = 1;
            for (int i = 2; i <= (int)Math.Round(n); i++)
                r *= i;  // Multiply result by each integer from 2 to n
            return r;
        }

        #endregion

        #region Calculator Button Handlers

        // ------ ToggleSign ------
        // Toggles the sign of the current number between positive and negative
        // Adds or removes a minus sign from the display
        private void ToggleSign()
        {
            // If number is negative, remove the minus sign
            if (lbl_result.Text.StartsWith("-"))
                lbl_result.Text = lbl_result.Text.Substring(1);
            // If number is positive and not zero, add the minus sign
            else if (lbl_result.Text != "0")
                lbl_result.Text = "-" + lbl_result.Text;

            // Mark that user is entering a number
            isEnteringNumber = true;
        }

        // ------ ClearAll ------
        // Resets the calculator to its initial state
        // Clears all operands, operators, and display values
        private void ClearAll()
        {
            // Reset the left operand (first number in operation)
            leftOperand = 0;

            // Clear any pending operator
            pendingOperator = "";

            // Clear the expression display (top label)
            lbl_expression.Text = "";

            // Reset the result display to "0"
            lbl_result.Text = "0";

            // Mark that user is not entering a number
            isEnteringNumber = false;

            // Clear the last operation memory (for equals repeat functionality)
            lastRightOperand = 0;
            lastOperator = "";
        }

        // ------ ClearEntry ------
        // Clears only the current entry without affecting stored operands or operators
        // Resets display to "0" and stops number entry mode
        private void ClearEntry()
        {
            // Reset the result display to "0"
            lbl_result.Text = "0";

            // Mark that user is not entering a number
            isEnteringNumber = false;
        }

        // ------ PressOperator ------
        // Handles pressing an operator button (+, -, X, ÷, mod, x^y)
        // Performs pending operation if one exists, then prepares for the next operation
        private void PressOperator(string op)
        {
            // If we are waiting for the right operand (user is NOT entering a number yet)
            // and they press "-" or "+", treat it as a sign for the next number, not an operator replacement.
            if (!isEnteringNumber && pendingOperator != "" && (op == "-" || op == "+"))
            {
                // Start entering the right operand with a sign.
                // If you want "+" to do nothing, you can ignore it.
                if (op == "-")
                {
                    // Begin a negative right operand
                    SetResult(-0.0); // or SetResult(0); then ToggleSign(); depending on your formatting
                    isEnteringNumber = true;
                }
                else
                {
                    // "+" sign: just start entering the number (optional)
                    SetResult(0.0);
                    isEnteringNumber = true;
                }

                // Keep pendingOperator as-is (so xʸ is not removed)
                return;
            }

            // Get the current display value
            double current = CurrentValue();

            if (pendingOperator == "")
            {
                leftOperand = current;
            }
            else
            {
                if (isEnteringNumber)
                {
                    try
                    {
                        leftOperand = Evaluate(leftOperand, current, pendingOperator);
                        SetResult(leftOperand);
                    }
                    catch (DivideByZeroException)
                    {
                        ShowError("Cannot divide by 0");
                        return;
                    }
                    catch
                    {
                        ShowError("Invalid input");
                        return;
                    }
                }
            }

            pendingOperator = op;
            lbl_expression.Text = $"{leftOperand} {pendingOperator}";
            isEnteringNumber = false;

            lastRightOperand = 0;
            lastOperator = "";
        }

        // ------ PressEquals ------
        // Handles pressing the equals button
        // Completes the current operation or repeats the last operation if pressed multiple times
        private void PressEquals()
        {
            // If no pending operator, check if we can repeat the last operation
            if (pendingOperator == "")
            {
                // If there's a last operation saved, repeat it with the current value
                if (lastOperator != "")
                {
                    try
                    {
                        double current = CurrentValue();
                        // Perform the last operation again (e.g., "5 = = =" repeats "+5")
                        double r2 = Evaluate(current, lastRightOperand, lastOperator);
                        // Update expression display
                        lbl_expression.Text = $"{current} {lastOperator} {lastRightOperand} =";
                        SetResult(r2);  // Display the result

                        leftOperand = r2;        // Store result for potential next operation
                        isEnteringNumber = false; // Not entering a number anymore
                    }
                    catch (DivideByZeroException)
                    {
                        ShowError("Cannot divide by 0");
                    }
                    catch
                    {
                        ShowError("Invalid input");
                    }
                }
                return;
            }

            // Get the right operand from the display
            double right = CurrentValue();

            try
            {
                // Perform the pending operation
                double r = Evaluate(leftOperand, right, pendingOperator);
                // Update expression display to show the complete calculation
                lbl_expression.Text = $"{leftOperand} {pendingOperator} {right} =";
                SetResult(r);  // Display the result

                // Save this operation for potential repeat when equals is pressed again
                lastRightOperand = right;
                lastOperator = pendingOperator;

                // Store result and clear pending operator
                leftOperand = r;
                pendingOperator = "";
                isEnteringNumber = false;
            }
            catch (DivideByZeroException)
            {
                ShowError("Cannot divide by 0");
            }
            catch
            {
                ShowError("Invalid input");
            }
        }

        // ------ Percent ------
        // Calculates the percentage based on context
        // If there's a pending operation, calculates percentage of left operand; otherwise divides by 100
        private void Percent()
        {
            // Get the current display value
            double current = CurrentValue();

            // If there's a pending operator, calculate percentage relative to left operand
            // Example: "200 + 10%" becomes "200 + 20" (10% of 200)
            if (pendingOperator != "")
            {
                double percentValue = leftOperand * (current / 100.0);
                SetResult(percentValue);
            }
            else
            {
                // No pending operator: just divide by 100
                // Example: "50%" becomes "0.5"
                SetResult(current / 100.0);
            }

            // Mark that user is entering a number (can continue editing)
            isEnteringNumber = true;
        }

        // ------ Reciprocal ------
        // Calculates the reciprocal (1/x) of the current value
        // Shows error if current value is zero (division by zero)
        private void Reciprocal()
        {
            // Get the current display value
            double x = CurrentValue();

            // Check for division by zero
            if (x == 0)
            {
                ShowError("Cannot divide by 0");
                return;
            }

            // Calculate and display 1/x
            SetResult(1.0 / x);

            // Mark that user is entering a number (can continue editing)
            isEnteringNumber = true;
        }

        // ------ Square ------
        // Calculates the square (x²) of the current value
        private void Square()
        {
            // Get the current display value
            double x = CurrentValue();

            // Calculate and display x * x
            SetResult(x * x);

            // Mark that user is entering a number (can continue editing)
            isEnteringNumber = true;
        }

        // ------ Sqrt ------
        // Calculates the square root (√x) of the current value
        // Shows error if current value is negative (no complex numbers)
        private void Sqrt()
        {
            // Get the current display value
            double x = CurrentValue();

            // Check if value is negative (square root of negative is complex)
            if (x < 0)
            {
                ShowError("Invalid input");
                return;
            }

            // Calculate and display square root
            SetResult(Math.Sqrt(x));

            // Mark that user is entering a number (can continue editing)
            isEnteringNumber = true;
        }

        // ------ Abs ------
        // Calculates the absolute value (|x|) of the current value
        // Converts negative numbers to positive, positive numbers remain unchanged
        private void Abs()
        {
            // Get the current display value
            double x = CurrentValue();

            // Calculate and display absolute value
            SetResult(Math.Abs(x));

            // Mark that user is entering a number (can continue editing)
            isEnteringNumber = true;
        }

        // ------ InsertConstant ------
        // Inserts a mathematical constant (π or e) into the display
        // Replaces current value with the constant
        private void InsertConstant(double value)
        {
            // Display the constant value
            SetResult(value);

            // Mark that user is entering a number (can continue editing)
            isEnteringNumber = true;
        }

        // ------ Exp ------
        // Calculates e raised to the power of x (e^x)
        // Uses Euler's number (approximately 2.71828) as the base
        private void Exp()
        {
            // Get the current display value
            double x = CurrentValue();

            // Calculate and display e^x
            SetResult(Math.Exp(x));

            // Mark that user is entering a number (can continue editing)
            isEnteringNumber = true;
        }

        // ------ Ln ------
        // Calculates the natural logarithm (ln(x)) of the current value
        // Shows error if value is zero or negative (ln only defined for positive numbers)
        private void Ln()
        {
            // Get the current display value
            double x = CurrentValue();

            // Check if value is non-positive (ln only defined for x > 0)
            if (x <= 0)
            {
                ShowError("Invalid input");
                return;
            }

            // Calculate and display natural logarithm
            SetResult(Math.Log(x));

            // Mark that user is entering a number (can continue editing)
            isEnteringNumber = true;
        }

        // ------ Log10 ------
        // Calculates the base-10 logarithm (log10(x)) of the current value
        // Shows error if value is zero or negative (log only defined for positive numbers)
        private void Log10()
        {
            // Get the current display value
            double x = CurrentValue();

            // Check if value is non-positive (log only defined for x > 0)
            if (x <= 0)
            {
                ShowError("Invalid input");
                return;
            }

            // Calculate and display base-10 logarithm
            SetResult(Math.Log10(x));

            // Mark that user is entering a number (can continue editing)
            isEnteringNumber = true;
        }

        // ------ TenPowerX ------
        // Calculates 10 raised to the power of x (10^x)
        // Inverse operation of Log10
        private void TenPowerX()
        {
            // Get the current display value
            double x = CurrentValue();

            // Calculate and display 10^x
            SetResult(Math.Pow(10, x));

            // Mark that user is entering a number (can continue editing)
            isEnteringNumber = true;
        }

        // ------ FactorialUnary ------
        // Calculates the factorial (n!) of the current value
        // Shows error if value is negative, non-integer, or too large (>170)
        private void FactorialUnary()
        {
            // Get the current display value
            double x = CurrentValue();

            try
            {
                // Calculate and display factorial
                SetResult(Factorial(x));

                // Mark that user is entering a number (can continue editing)
                isEnteringNumber = true;
            }
            catch
            {
                // Show error for invalid factorial input
                ShowError("Invalid input");
            }
        }

        // ------ Sin ------
        // Calculates the sine of the current value (in radians)
        // Part of the trigonometric functions available in "2nd" mode
        private void Sin()
        {
            // Get the current display value
            double x = CurrentValue();

            // Calculate and display sine (input assumed to be in radians)
            SetResult(Math.Sin(x));

            // Mark that user is entering a number (can continue editing)
            isEnteringNumber = true;
        }

        // ------ Cos ------
        // Calculates the cosine of the current value (in radians)
        // Part of the trigonometric functions available in "2nd" mode
        private void Cos()
        {
            // Get the current display value
            double x = CurrentValue();

            // Calculate and display cosine (input assumed to be in radians)
            SetResult(Math.Cos(x));

            // Mark that user is entering a number (can continue editing)
            isEnteringNumber = true;
        }

        // ------ Tan ------
        // Calculates the tangent of the current value (in radians)
        // Part of the trigonometric functions available in "2nd" mode
        private void Tan()
        {
            // Get the current display value
            double x = CurrentValue();

            // Calculate and display tangent (input assumed to be in radians)
            SetResult(Math.Tan(x));

            // Mark that user is entering a number (can continue editing)
            isEnteringNumber = true;
        }

        // ------ SetSecondMode ------
        // Sets the calculator to "2nd" mode or normal mode
        // Changes button labels to show alternate functions (sin/cos/tan vs 10^x/log/ln)
        private void SetSecondMode(bool enabled)
        {
            // Update the mode flag
            isSecondMode = enabled;

            // Change the "2nd" button background color to indicate active mode
            btn_second.BackColor = enabled
                ? Color.FromArgb(255, 205, 145)  // Orange color when active
                : secondDefaultBackColor;         // Default gray when inactive

            // Update button labels based on mode
            // In 2nd mode: show trig functions; in normal mode: show exponential/log functions
            btn_power_10.Text = enabled ? "sin" : "10ˣ";
            btn_log10.Text = enabled ? "cos" : "log";
            btn_ln.Text = enabled ? "tan" : "ln";
        }

        // ------ ToggleSecondMode ------
        // Toggles between normal mode and "2nd" mode
        // Called when the "2nd" button is pressed
        private void ToggleSecondMode()
        {
            // Flip the mode state
            SetSecondMode(!isSecondMode);
        }

        #endregion

        #endregion

        #region FX Converter Methods

        #region FX Helper Methods

        // ------ FxCurrentValue ------
        // Retrieves the current numeric value displayed in the FX converter display label
        // Returns 0 if parsing fails
        private double FxCurrentValue()
        {
            // Call the common parsing method with FX display label
            return ParseDisplayValue(lbl_fx_display);
        }

        // ------ FxSetDisplay ------
        // Sets the FX converter display to show the given numeric value
        // Shows error in status label if value is NaN or Infinity
        private void FxSetDisplay(double v)
        {
            // Check if the value is invalid (Not a Number or Infinity)
            if (double.IsNaN(v) || double.IsInfinity(v))
            {
                lbl_fx_status.Text = "Invalid input";
                return;
            }

            // Convert the value to string using invariant culture for consistency
            lbl_fx_display.Text = v.ToString(CultureInfo.InvariantCulture);
        }

        // ------ NormalizeCurrency ------
        // Normalizes a currency code to uppercase and trims whitespace
        // Ensures consistent currency code format for API calls and caching
        private static string NormalizeCurrency(string code)
        {
            // Handle null input, trim whitespace, and convert to uppercase (e.g., "usd" -> "USD")
            return (code ?? "").Trim().ToUpperInvariant();
        }

        #endregion

        #region FX Button Handlers

        // ------ FxInitCombos ------
        // Initializes the currency combo boxes with default values
        // Sets up USD, EUR, and LBP as available currencies
        private void FxInitCombos()
        {
            // Check if combo boxes exist
            if (cmb_fx_from == null || cmb_fx_to == null) return;

            // Populate the "FROM" currency dropdown if empty
            if (cmb_fx_from.Items.Count == 0)
            {
                cmb_fx_from.Items.Add("USD");  // US Dollar
                cmb_fx_from.Items.Add("EUR");  // Euro
                cmb_fx_from.Items.Add("LBP");  // Lebanese Pound
            }

            // Populate the "TO" currency dropdown if empty
            if (cmb_fx_to.Items.Count == 0)
            {
                cmb_fx_to.Items.Add("USD");  // US Dollar
                cmb_fx_to.Items.Add("EUR");  // Euro
                cmb_fx_to.Items.Add("LBP");  // Lebanese Pound
            }

            // Set combo boxes to dropdown list style (no manual text entry)
            cmb_fx_from.DropDownStyle = ComboBoxStyle.DropDownList;
            cmb_fx_to.DropDownStyle = ComboBoxStyle.DropDownList;

            // Set default selections (USD -> EUR)
            cmb_fx_from.SelectedIndex = 0;  // USD
            cmb_fx_to.SelectedIndex = 1;    // EUR
        }

        // ------ FxClearAll ------
        // Resets the FX converter to its initial state
        // Clears display, status, and resets currency selections
        private void FxClearAll()
        {
            // Mark that user is not entering an amount
            fxIsEnteringAmount = false;

            // Reset the FX display to "0"
            lbl_fx_display.Text = "0";

            // Clear the status message
            lbl_fx_status.Text = "";

            // Reset currency selections to defaults (USD -> EUR)
            if (cmb_fx_from != null) cmb_fx_from.SelectedIndex = 0;
            if (cmb_fx_to != null) cmb_fx_to.SelectedIndex = 1;
        }

        // ------ FxClearEntry ------
        // Clears only the current entry without affecting other FX state
        // Resets display to "0" and stops number entry mode
        private void FxClearEntry()
        {
            // Reset the FX display to "0"
            lbl_fx_display.Text = "0";

            // Mark that user is not entering an amount
            fxIsEnteringAmount = false;
        }

        // ------ FxCopy ------
        // Copies the current FX display value to the system clipboard
        // Shows status message indicating success or failure
        private void FxCopy()
        {
            try
            {
                // Copy the display text to clipboard
                Clipboard.SetText(lbl_fx_display.Text);

                // Show success message
                lbl_fx_status.Text = "Copied";
            }
            catch
            {
                // Show error message if copy fails
                lbl_fx_status.Text = "Copy failed";
            }
        }

        // ------ FxPaste ------
        // Pastes a numeric value from the system clipboard into the FX display
        // Validates the pasted text is a valid number
        private void FxPaste()
        {
            try
            {
                // Get text from clipboard and trim whitespace
                string t = Clipboard.GetText()?.Trim() ?? "";

                // Check if clipboard is empty
                if (t.Length == 0)
                {
                    lbl_fx_status.Text = "Clipboard empty";
                    return;
                }

                // Try to parse the text as a number (invariant culture first, then current culture)
                if (!double.TryParse(t, NumberStyles.Float, CultureInfo.InvariantCulture, out double v) &&
                    !double.TryParse(t, NumberStyles.Float, CultureInfo.CurrentCulture, out v))
                {
                    // Show error if text is not a valid number
                    lbl_fx_status.Text = "Invalid paste";
                    return;
                }

                // Set the parsed value to the display
                FxSetDisplay(v);

                // Mark that user is now entering an amount
                fxIsEnteringAmount = true;

                // Clear the status message
                lbl_fx_status.Text = "";
            }
            catch
            {
                // Show error message if paste operation fails
                lbl_fx_status.Text = "Paste failed";
            }
        }

        // ------ FxConvertAsync ------
        // Performs currency conversion by fetching exchange rate from API
        // Validates currency selections and displays converted amount
        private async Task FxConvertAsync()
        {
            // Get the selected FROM currency code
            string from = cmb_fx_from?.SelectedItem?.ToString() ?? "";

            // Get the selected TO currency code
            string to = cmb_fx_to?.SelectedItem?.ToString() ?? "";

            // Normalize currency codes (trim and uppercase)
            from = NormalizeCurrency(from);
            to = NormalizeCurrency(to);

            // Validate that both currencies are selected
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
            {
                lbl_fx_status.Text = "Select FROM and TO currencies";
                return;
            }

            // Check that FROM and TO currencies are different
            if (from == to)
            {
                lbl_fx_status.Text = "FROM and TO cannot be the same";
                return;
            }

            // Get the amount to convert from the display
            double amount = FxCurrentValue();

            // Show loading message
            lbl_fx_status.Text = "Loading rate...";

            try
            {
                // Fetch the exchange rate from the API
                double rate = await FxGetRateAsync(from, to);

                // Calculate the converted amount
                double converted = amount * rate;

                // Display the converted amount
                FxSetDisplay(converted);

                // Mark that user is not entering an amount (result is displayed)
                fxIsEnteringAmount = false;

                // Show the exchange rate in the status label
                lbl_fx_status.Text = $"{from}->{to} rate={rate.ToString(CultureInfo.InvariantCulture)}";
            }
            catch (Exception ex)
            {
                // Show error message if conversion fails
                lbl_fx_status.Text = "Conversion failed: " + ex.Message;
            }
        }

        // ------ FxGetRateAsync ------
        // Fetches exchange rate from API
        // Note: Expects already normalized currency codes (uppercase, trimmed)
        private async Task<double> FxGetRateAsync(string from, string to)
        {
            // Build the API URL for fetching the exchange rate
            string url = $"https://hexarate.paikama.co/api/rates/{from}/{to}/latest";

            // Make HTTP GET request to the API
            using HttpResponseMessage resp = await fxHttp.GetAsync(url);

            // Ensure the request was successful (throw exception if not)
            resp.EnsureSuccessStatusCode();

            // Read the JSON response as a string
            string json = await resp.Content.ReadAsStringAsync();

            // Parse the JSON document
            using JsonDocument doc = JsonDocument.Parse(json);

            // Extract the "data" property from the response
            if (!doc.RootElement.TryGetProperty("data", out JsonElement dataEl))
                throw new Exception("Missing data");

            // Extract the "mid" property (exchange rate value)
            if (!dataEl.TryGetProperty("mid", out JsonElement midEl))
                throw new Exception("Missing mid");

            // Get the mid value as a double
            double mid = midEl.GetDouble();

            // Some currencies have a "unit" property (e.g., 1000 LBP = X USD)
            double unit = 1;
            if (dataEl.TryGetProperty("unit", out JsonElement unitEl) && unitEl.ValueKind == JsonValueKind.Number)
                unit = unitEl.GetDouble();

            // Check if unit is valid (non-zero)
            if (unit == 0)
                throw new Exception("Invalid unit");

            // Calculate the actual rate (mid / unit)
            double rate = mid / unit;

            // Return the calculated rate
            return rate;
        }

        #endregion

        #endregion

        #endregion

        #endregion

    }
}