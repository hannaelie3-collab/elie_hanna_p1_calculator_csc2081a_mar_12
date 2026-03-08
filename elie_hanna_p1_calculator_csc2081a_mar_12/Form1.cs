using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace elie_hanna_p1_calculator_csc2081a_mar_12
{
    public partial class Form1 : Form
    {
        #region global var

        private double leftOperand = 0;                 // Stores the left side number for binary operations
        private string pendingOperator = "";            // Stores the operator waiting to be applied (ex: +, -, X, ÷, mod, xʸ)
        private bool isEnteringNumber = false;          // True when the user is actively typing a number into lbl_result

        private double lastRightOperand = 0;            // Stores the last right operand to support repeated "=" presses
        private string lastOperator = "";               // Stores the last operator to support repeated "=" presses

        private bool isSecondMode = false;              // True when 2nd mode is enabled (changes some button texts)
        private Color secondDefaultBackColor = SystemColors.ActiveBorder; // Default color for the 2nd button when not active

        #endregion

        public Form1()
        {
            InitializeComponent();                      // Builds all UI controls from the Designer file
            WireAllButtonsToOneHandlerIterative();       // Attaches Button_Click to all buttons in tabPage1
            ClearAll();                                  // Resets calculator state and display on startup
            SetSecondMode(false);                        // Ensures 2nd mode starts off and button texts/colors are correct
        }

        #region event handlers

        private void Button_Click(object sender, EventArgs e)
        {
            if (sender is not Button btn) return;        // Ensures the sender is a Button before continuing
            string v = btn.Text.Trim();                  // Reads the visible button text and removes extra spaces

            if (v == "2nd")                              // Checks if the user clicked the 2nd toggle button
            {
                ToggleSecondMode();                      // Switches 2nd mode on/off and updates UI
                return;                                  // Stops further processing for this click
            }

            if (v.Length == 1 && char.IsDigit(v[0]))     // Checks if the button is a single digit (0..9)
            {
                if (IsRedundantLeadingZero(v)) return;   // Prevents typing extra leading zeros like "000"
                AppendDigit(v);                          // Adds the digit to the number currently being entered
                return;                                  // Stops further processing for this click
            }

            if (v == ".")                                // Checks if the decimal point button was clicked
            {
                AppendDecimal();                         // Adds a decimal point if it is valid to do so
                return;                                  // Stops further processing for this click
            }

            if (v == "+/-" || v == "±")                  // Checks if the sign toggle button was clicked
            {
                ToggleSign();                            // Changes the sign of the current number
                return;                                  // Stops further processing for this click
            }

            if (v == "C")                                // Checks if the clear-all button was clicked
            {
                ClearAll();                              // Clears everything (state + display)
                return;                                  // Stops further processing for this click
            }

            if (v == "CE")                               // Checks if the clear-entry button was clicked
            {
                ClearEntry();                            // Clears only the current entry (display only)
                return;                                  // Stops further processing for this click
            }

            if (v == "⌫" || v == "←" || v == "Back" || v == "⟵") // Checks if a backspace-style key was clicked
            {
                Backspace();                             // Removes the last character of the current entry
                return;                                  // Stops further processing for this click
            }

            if (v == "%")                                // Checks if percent was clicked
            {
                Percent();                               // Applies percent behavior (standard calculator style)
                return;                                  // Stops further processing for this click
            }

            if (v == "1/x")                              // Checks if reciprocal was clicked
            {
                Reciprocal();                            // Calculates 1 divided by the current value
                return;                                  // Stops further processing for this click
            }

            if (v == "x²" || v == "x^2")                 // Checks if square was clicked
            {
                Square();                                // Squares the current value
                return;                                  // Stops further processing for this click
            }

            if (v == "²√x" || v == "√x" || v == "2√x")    // Checks if square root was clicked
            {
                Sqrt();                                  // Calculates the square root of the current value
                return;                                  // Stops further processing for this click
            }

            if (v == "|x|")                              // Checks if absolute value was clicked
            {
                Abs();                                   // Converts the current value to its absolute value
                return;                                  // Stops further processing for this click
            }

            if (v == "π")                                // Checks if pi constant was clicked
            {
                InsertConstant(Math.PI);                 // Inserts pi into the display as the current value
                return;                                  // Stops further processing for this click
            }

            if (v == "e")                                // Checks if e constant was clicked
            {
                InsertConstant(Math.E);                  // Inserts Euler's number into the display as the current value
                return;                                  // Stops further processing for this click
            }

            if (v == "exp")                              // Checks if exp was clicked
            {
                Exp();                                   // Calculates e^x for the current value x
                return;                                  // Stops further processing for this click
            }

            if (v == "ln")                               // Checks if natural log was clicked
            {
                Ln();                                    // Calculates ln(x) for the current value x
                return;                                  // Stops further processing for this click
            }

            if (v == "log")                              // Checks if base-10 log was clicked
            {
                Log10();                                 // Calculates log10(x) for the current value x
                return;                                  // Stops further processing for this click
            }

            if (v == "10ˣ" || v == "10^x")               // Checks if 10-to-the-x was clicked
            {
                TenPowerX();                             // Calculates 10^x for the current value x
                return;                                  // Stops further processing for this click
            }

            if (v == "sin")                              // Checks if sin was clicked (2nd-mode replacement)
            {
                Sin();                                   // Calculates sin(x) for the current value x
                return;                                  // Stops further processing for this click
            }

            if (v == "cos")                              // Checks if cos was clicked (2nd-mode replacement)
            {
                Cos();                                   // Calculates cos(x) for the current value x
                return;                                  // Stops further processing for this click
            }

            if (v == "tan")                              // Checks if tan was clicked (2nd-mode replacement)
            {
                Tan();                                   // Calculates tan(x) for the current value x
                return;                                  // Stops further processing for this click
            }

            if (v == "n!")                               // Checks if factorial was clicked
            {
                FactorialUnary();                        // Calculates factorial (only for non-negative integers)
                return;                                  // Stops further processing for this click
            }

            if (v == "(")                                // Checks if open parenthesis was clicked
            {
                AppendParen("(");                        // Displays it in the expression line (full parsing comes later)
                return;                                  // Stops further processing for this click
            }

            if (v == ")")                                // Checks if close parenthesis was clicked
            {
                AppendParen(")");                        // Displays it in the expression line (full parsing comes later)
                return;                                  // Stops further processing for this click
            }

            if (v == "+" || v == "-" || v == "X" || v == "x" || v == "÷" || v == "/") // Checks if a basic binary operator was clicked
            {
                if (v == "x") v = "X";                   // Normalizes x to X so we have one multiplication symbol internally
                PressOperator(v);                        // Stores operator and prepares for the next number
                return;                                  // Stops further processing for this click
            }

            if (v == "mod")                              // Checks if modulo was clicked
            {
                PressOperator("mod");                    // Sets the pending operator to modulo
                return;                                  // Stops further processing for this click
            }

            if (v == "xʸ" || v == "x^y")                 // Checks if x-to-the-y was clicked
            {
                PressOperator("xʸ");                     // Sets the pending operator to power
                return;                                  // Stops further processing for this click
            }

            if (v == "=")                                // Checks if equals was clicked
            {
                PressEquals();                           // Performs the pending operation and shows the result
                return;                                  // Stops further processing for this click
            }
        }

        #endregion

        #region private methods

        #region helper methods

        // -----------CurrentValue------------
        // Reads the current display text and converts it into a double value.
        private double CurrentValue()
        {
            if (double.TryParse(lbl_result.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double v)) // Tries parsing with invariant culture
                return v;                                         // Returns the parsed number if successful

            if (double.TryParse(lbl_result.Text, out v))           // Falls back to current culture parsing
                return v;                                         // Returns the parsed number if successful

            return 0;                                             // Returns 0 if parsing fails
        }

        // -----------SetResult------------
        // Writes a double to the display using invariant culture formatting.
        private void SetResult(double v)
        {
            if (double.IsNaN(v) || double.IsInfinity(v))          // Checks if the number is invalid for display
            {
                ShowError("Invalid input");                       // Displays an error and resets state
                return;                                           // Stops the method
            }

            lbl_result.Text = v.ToString(CultureInfo.InvariantCulture); // Updates the result label with the formatted number
        }

        // -----------ShowError------------
        // Displays an error message and resets the calculator state.
        private void ShowError(string message)
        {
            lbl_expression.Text = "";                              // Clears the expression line
            lbl_result.Text = message;                             // Shows the error text in the result label

            leftOperand = 0;                                      // Resets stored left operand
            pendingOperator = "";                                 // Clears any pending operator
            lastRightOperand = 0;                                 // Clears repeated "=" right operand tracking
            lastOperator = "";                                    // Clears repeated "=" operator tracking
            isEnteringNumber = false;                              // Forces the next digit to start a new number
        }

        // -----------IsRedundantLeadingZero------------
        // Prevents entering a second leading zero when the display is already "0".
        private bool IsRedundantLeadingZero(string digit)
        {
            return digit == "0" && lbl_result.Text == "0" && !isEnteringNumber; // Returns true if adding another leading zero is unnecessary
        }

        // -----------Evaluate------------
        // Applies a binary operation (a op b) and returns the result.
        private double Evaluate(double a, double b, string op)
        {
            return op switch                                      // Uses the operator string to decide which operation to apply
            {
                "+" => a + b,                                     // Addition
                "-" => a - b,                                     // Subtraction
                "X" => a * b,                                     // Multiplication (normalized symbol)
                "x" => a * b,                                     // Multiplication (fallback)
                "÷" => b == 0 ? throw new DivideByZeroException() : a / b, // Division with divide-by-zero protection
                "/" => b == 0 ? throw new DivideByZeroException() : a / b, // Division fallback

                "mod" => b == 0 ? throw new DivideByZeroException() : a % b, // Modulo with divide-by-zero protection
                "xʸ" => Math.Pow(a, b),                           // Power: a raised to b

                _ => b                                            // Default behavior (should not happen in normal use)
            };
        }

        // -----------IsInteger------------
        // Checks if a double is very close to an integer value.
        private static bool IsInteger(double x)
        {
            return Math.Abs(x - Math.Round(x)) < 1e-12;           // True if x is essentially an integer
        }

        // -----------Factorial------------
        // Computes factorial for non-negative integers (as a double).
        private static double Factorial(double n)
        {
            if (n < 0) throw new ArgumentOutOfRangeException(nameof(n)); // Factorial is not defined for negative numbers
            if (!IsInteger(n)) throw new ArgumentException("Factorial requires integer input."); // Disallows non-integers
            if (n > 170) throw new OverflowException("Result too large."); // Limits n to avoid infinity in double

            double r = 1;                                         // Starts result at 1
            for (int i = 2; i <= (int)Math.Round(n); i++)          // Loops from 2 to n
                r *= i;                                           // Multiplies r by each integer i
            return r;                                             // Returns factorial result
        }

        // -----------WireAllButtonsToOneHandlerIterative------------
        // Attaches Button_Click to all Button controls inside tabPage1.
        private void WireAllButtonsToOneHandlerIterative()
        {
            var stack = new Stack<Control>();                      // Creates a stack for iterative traversal of child controls
            stack.Push(tabPage1);                                  // Starts traversal from tabPage1 so tabPage2 is ignored

            while (stack.Count > 0)                                // Continues until all controls are processed
            {
                Control current = stack.Pop();                     // Gets the next control to process

                if (current is Button b)                           // Checks if this control is a Button
                {
                    b.Click -= Button_Click;                       // Removes handler to prevent double subscription
                    b.Click += Button_Click;                       // Adds handler so all buttons use the same click logic
                }

                foreach (Control child in current.Controls)        // Iterates over each child control
                    stack.Push(child);                             // Pushes child controls so they will be processed too
            }
        }

        #endregion

        #region button handlers

        // -----------SetSecondMode------------
        // Enables/disables 2nd mode and updates button text and colors.
        private void SetSecondMode(bool enabled)
        {
            isSecondMode = enabled;                                // Saves the new toggle state

            btn_second.BackColor = enabled                         // Sets the 2nd button background color based on state
                ? Color.FromArgb(255, 205, 145)                    // Light orange when enabled
                : secondDefaultBackColor;                          // Default color when disabled

            btn_power_10.Text = enabled ? "sin" : "10ˣ";           // Replaces 10ˣ with sin in 2nd mode
            btn_log10.Text = enabled ? "cos" : "log";              // Replaces log with cos in 2nd mode
            btn_ln.Text = enabled ? "tan" : "ln";                  // Replaces ln with tan in 2nd mode
        }

        // -----------ToggleSecondMode------------
        // Toggles the 2nd mode state between on and off.
        private void ToggleSecondMode()
        {
            SetSecondMode(!isSecondMode);                          // Inverts current state and applies it
        }

        // -----------AppendDigit------------
        // Adds a digit to the display.
        private void AppendDigit(string digit)
        {
            if (!isEnteringNumber || lbl_result.Text == "0")       // Checks if we are starting a new number
            {
                lbl_result.Text = digit;                           // Replaces the display with the new digit
                isEnteringNumber = true;                           // Marks that we are now entering a number
            }
            else
            {
                lbl_result.Text += digit;                          // Appends the digit to the display text
            }
        }

        // -----------AppendDecimal------------
        // Adds a decimal point to the current number if allowed.
        private void AppendDecimal()
        {
            if (!isEnteringNumber)                                 // If not entering a number, start with "0."
            {
                lbl_result.Text = "0.";                            // Sets the display to start a decimal number
                isEnteringNumber = true;                           // Marks that we are entering a number
                return;                                            // Stops the method
            }

            if (!lbl_result.Text.Contains("."))                    // Only allow one decimal point
                lbl_result.Text += ".";                            // Appends the decimal point
        }

        // -----------Backspace------------
        // Deletes the last typed character from the current entry.
        private void Backspace()
        {
            if (!isEnteringNumber)                                 // If not typing, treat as clear entry
            {
                ClearEntry();                                      // Clears just the entry
                return;                                            // Stops the method
            }

            if (lbl_result.Text.Length <= 1 || (lbl_result.Text.Length == 2 && lbl_result.Text.StartsWith("-"))) // Handles deleting the last digit safely
            {
                lbl_result.Text = "0";                             // Resets to zero when nothing meaningful remains
                isEnteringNumber = false;                          // Marks we are no longer entering a number
                return;                                            // Stops the method
            }

            lbl_result.Text = lbl_result.Text.Substring(0, lbl_result.Text.Length - 1); // Removes last character from the display

            if (lbl_result.Text == "-" || lbl_result.Text.Length == 0) // Prevents leaving only a minus sign
            {
                lbl_result.Text = "0";                             // Resets to zero
                isEnteringNumber = false;                          // Marks we are no longer entering a number
            }
        }

        // -----------ToggleSign------------
        // Toggles the sign of the number in the display.
        private void ToggleSign()
        {
            if (lbl_result.Text.StartsWith("-"))                   // Checks if it is already negative
                lbl_result.Text = lbl_result.Text.Substring(1);    // Removes the minus sign
            else if (lbl_result.Text != "0")                       // Only add negative sign if not zero
                lbl_result.Text = "-" + lbl_result.Text;           // Adds the minus sign

            isEnteringNumber = true;                               // Treats this as editing the current number
        }

        // -----------ClearAll------------
        // Clears everything including stored operands and pending operator.
        private void ClearAll()
        {
            leftOperand = 0;                                       // Resets the left operand
            pendingOperator = "";                                  // Clears the pending operator
            lbl_expression.Text = "";                              // Clears the expression line
            lbl_result.Text = "0";                                 // Resets display to zero
            isEnteringNumber = false;                              // Next digit starts fresh

            lastRightOperand = 0;                                  // Clears repeat "=" right operand
            lastOperator = "";                                     // Clears repeat "=" operator
        }

        // -----------ClearEntry------------
        // Clears only the current entry (display) without clearing the full operation state.
        private void ClearEntry()
        {
            lbl_result.Text = "0";                                 // Resets display to zero
            isEnteringNumber = false;                              // Next digit starts fresh
        }

        // -----------PressOperator------------
        // Stores the operator and prepares to accept the next number, computing partial results when chaining.
        private void PressOperator(string op)
        {
            double current = CurrentValue();                       // Reads the current display value

            if (pendingOperator == "")                             // If no operator is pending, store the current as left operand
            {
                leftOperand = current;                             // Saves current value as left operand
            }
            else
            {
                if (isEnteringNumber)                              // Only compute if the user entered a right operand
                {
                    try
                    {
                        leftOperand = Evaluate(leftOperand, current, pendingOperator); // Applies previous pending operation
                        SetResult(leftOperand);                    // Shows the intermediate result
                    }
                    catch (DivideByZeroException)
                    {
                        ShowError("Cannot divide by 0");           // Displays divide-by-zero error
                        return;                                    // Stops the method
                    }
                    catch
                    {
                        ShowError("Invalid input");                // Displays a generic error
                        return;                                    // Stops the method
                    }
                }
            }

            pendingOperator = op;                                  // Stores the new operator as pending
            lbl_expression.Text = $"{leftOperand} {pendingOperator}"; // Updates the expression label to show the current operation
            isEnteringNumber = false;                              // Next digit starts a new number

            lastRightOperand = 0;                                  // Clears repeat "=" right operand
            lastOperator = "";                                     // Clears repeat "=" operator
        }

        // -----------PressEquals------------
        // Evaluates the pending operation and supports repeated "=" behavior.
        private void PressEquals()
        {
            if (pendingOperator == "")                             // If no pending operator, handle repeated "="
            {
                if (lastOperator != "")                            // Only repeat if there was a previous stored operator
                {
                    try
                    {
                        double current = CurrentValue();           // Gets current display value
                        double r2 = Evaluate(current, lastRightOperand, lastOperator); // Re-applies last operation
                        lbl_expression.Text = $"{current} {lastOperator} {lastRightOperand} ="; // Shows the repeated expression
                        SetResult(r2);                             // Shows the result

                        leftOperand = r2;                          // Stores result as left operand for further chaining
                        isEnteringNumber = false;                  // Next digit starts a new number
                    }
                    catch (DivideByZeroException)
                    {
                        ShowError("Cannot divide by 0");           // Displays divide-by-zero error
                    }
                    catch
                    {
                        ShowError("Invalid input");                // Displays a generic error
                    }
                }
                return;                                            // Stops because there is nothing else to compute
            }

            double right = CurrentValue();                         // Reads the right operand from the display

            try
            {
                double r = Evaluate(leftOperand, right, pendingOperator); // Evaluates left op right
                lbl_expression.Text = $"{leftOperand} {pendingOperator} {right} ="; // Shows the completed expression
                SetResult(r);                                      // Displays the result

                lastRightOperand = right;                          // Stores right operand for repeated "="
                lastOperator = pendingOperator;                    // Stores operator for repeated "="

                leftOperand = r;                                   // Stores result for further chaining
                pendingOperator = "";                              // Clears pending operator
                isEnteringNumber = false;                          // Next digit starts a new number
            }
            catch (DivideByZeroException)
            {
                ShowError("Cannot divide by 0");                   // Displays divide-by-zero error
            }
            catch
            {
                ShowError("Invalid input");                        // Displays a generic error
            }
        }

        // -----------Percent------------
        // Applies percent behavior similar to standard calculators.
        private void Percent()
        {
            double current = CurrentValue();                       // Reads current value from the display

            if (pendingOperator != "")                             // If an operator exists, percent is based on left operand
            {
                double percentValue = leftOperand * (current / 100.0); // Converts right operand into a percentage of left operand
                SetResult(percentValue);                           // Displays the percent-based value
            }
            else
            {
                SetResult(current / 100.0);                        // If no operator, treat it as simple percentage
            }

            isEnteringNumber = true;                               // Treat result as current entry
        }

        // -----------Reciprocal------------
        // Calculates 1/x.
        private void Reciprocal()
        {
            double x = CurrentValue();                             // Reads current value

            if (x == 0)                                            // Checks divide-by-zero case
            {
                ShowError("Cannot divide by 0");                   // Displays an error
                return;                                            // Stops the method
            }

            SetResult(1.0 / x);                                    // Displays reciprocal
            isEnteringNumber = true;                               // Treat result as current entry
        }

        // -----------Square------------
        // Calculates x².
        private void Square()
        {
            double x = CurrentValue();                             // Reads current value
            SetResult(x * x);                                      // Squares the value
            isEnteringNumber = true;                               // Treat result as current entry
        }

        // -----------Sqrt------------
        // Calculates √x.
        private void Sqrt()
        {
            double x = CurrentValue();                             // Reads current value

            if (x < 0)                                             // Square root not valid for negatives in real numbers
            {
                ShowError("Invalid input");                        // Displays error
                return;                                            // Stops the method
            }

            SetResult(Math.Sqrt(x));                               // Displays square root
            isEnteringNumber = true;                               // Treat result as current entry
        }

        // -----------Abs------------
        // Calculates |x|.
        private void Abs()
        {
            double x = CurrentValue();                             // Reads current value
            SetResult(Math.Abs(x));                                // Displays absolute value
            isEnteringNumber = true;                               // Treat result as current entry
        }

        // -----------InsertConstant------------
        // Inserts a constant value into the display.
        private void InsertConstant(double value)
        {
            SetResult(value);                                      // Writes the constant to the display
            isEnteringNumber = true;                               // Treats constant as the current entry
        }

        // -----------Exp------------
        // Calculates e^x.
        private void Exp()
        {
            double x = CurrentValue();                             // Reads current value
            SetResult(Math.Exp(x));                                // Displays e^x
            isEnteringNumber = true;                               // Treat result as current entry
        }

        // -----------Ln------------
        // Calculates natural log ln(x).
        private void Ln()
        {
            double x = CurrentValue();                             // Reads current value

            if (x <= 0)                                            // ln(x) requires x > 0
            {
                ShowError("Invalid input");                        // Displays error
                return;                                            // Stops the method
            }

            SetResult(Math.Log(x));                                // Displays natural log
            isEnteringNumber = true;                               // Treat result as current entry
        }

        // -----------Log10------------
        // Calculates base-10 log log10(x).
        private void Log10()
        {
            double x = CurrentValue();                             // Reads current value

            if (x <= 0)                                            // log10(x) requires x > 0
            {
                ShowError("Invalid input");                        // Displays error
                return;                                            // Stops the method
            }

            SetResult(Math.Log10(x));                              // Displays base-10 log
            isEnteringNumber = true;                               // Treat result as current entry
        }

        // -----------TenPowerX------------
        // Calculates 10^x.
        private void TenPowerX()
        {
            double x = CurrentValue();                             // Reads current value
            SetResult(Math.Pow(10, x));                             // Displays 10^x
            isEnteringNumber = true;                               // Treat result as current entry
        }

        // -----------FactorialUnary------------
        // Calculates n! for non-negative integers.
        private void FactorialUnary()
        {
            double x = CurrentValue();                             // Reads current value

            try
            {
                SetResult(Factorial(x));                           // Attempts factorial calculation
                isEnteringNumber = true;                           // Treat result as current entry
            }
            catch
            {
                ShowError("Invalid input");                        // Displays error for invalid factorial cases
            }
        }

        // -----------Sin------------
        // Calculates sin(x). (Currently uses radians.)
        private void Sin()
        {
            double x = CurrentValue();                             // Reads current value
            SetResult(Math.Sin(x));                                // Displays sin(x)
            isEnteringNumber = true;                               // Treat result as current entry
        }

        // -----------Cos------------
        // Calculates cos(x). (Currently uses radians.)
        private void Cos()
        {
            double x = CurrentValue();                             // Reads current value
            SetResult(Math.Cos(x));                                // Displays cos(x)
            isEnteringNumber = true;                               // Treat result as current entry
        }

        // -----------Tan------------
        // Calculates tan(x). (Currently uses radians.)
        private void Tan()
        {
            double x = CurrentValue();                             // Reads current value
            SetResult(Math.Tan(x));                                // Displays tan(x)
            isEnteringNumber = true;                               // Treat result as current entry
        }

        // -----------AppendParen------------
        // Displays parentheses in the expression label (full parsing will be added later).
        private void AppendParen(string paren)
        {
            lbl_expression.Text = $"{lbl_expression.Text} {paren}".Trim(); // Appends the parenthesis to the expression label
        }

        #endregion

        #endregion


       
    }
}