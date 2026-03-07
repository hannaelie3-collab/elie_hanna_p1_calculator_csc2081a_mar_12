using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace elie_hanna_p1_calculator_csc2081a_mar_12
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            WireAllButtonsToOneHandlerIterative();
            ClearAll(); // start clean
        }

        #region state variables

        // Calculator state (simple + predictable)
        private double leftOperand = 0;
        private string pendingOperator = "";     // "+", "-", "x", "÷"
        private bool isEnteringNumber = false;   // are we currently typing digits into lbl_result?

        #endregion

        #region helpers

        private double CurrentValue()
        {
            if (double.TryParse(lbl_result.Text, out double v))
                return v;

            // If something unexpected is in the label, treat as 0
            return 0;
        }

        private void SetResult(double v)
        {
            lbl_result.Text = v.ToString();
        }

        private bool IsRedundantLeadingZero(string digit, string currentDisplay)
        {
            // Keep your original idea, but safer (no double.Parse crash)
            if (digit != "0") return false;
            return (currentDisplay == "0" && !isEnteringNumber);
        }

        private double Evaluate(double a, double b, string op)
        {
            return op switch
            {
                "+" => a + b,
                "-" => a - b,
                "x" => a * b,
                "÷" => b == 0 ? throw new DivideByZeroException() : a / b,
                _ => b
            };
        }

        #endregion

        #region button actions

        private void AppendDigit(string digit)
        {
            // If we are not currently typing a number, start fresh
            if (!isEnteringNumber || lbl_result.Text == "0")
            {
                lbl_result.Text = digit;
                isEnteringNumber = true;
            }
            else
            {
                lbl_result.Text += digit;
            }
        }

        private void AppendDecimal()
        {
            // If user presses "." before typing digits, start "0."
            if (!isEnteringNumber)
            {
                lbl_result.Text = "0.";
                isEnteringNumber = true;
                return;
            }

            if (!lbl_result.Text.Contains("."))
                lbl_result.Text += ".";
        }

        private void ToggleSign()
        {
            if (lbl_result.Text.StartsWith("-"))
                lbl_result.Text = lbl_result.Text.Substring(1);
            else if (lbl_result.Text != "0")
                lbl_result.Text = "-" + lbl_result.Text;

            isEnteringNumber = true;
        }

        private void PressOperator(string op)
        {
            double current = CurrentValue();

            if (pendingOperator == "")
            {
                // First operator in a chain: store left operand
                leftOperand = current;
            }
            else
            {
                // If user has typed the right operand, evaluate now (so 2 + 3 + 4 works)
                if (isEnteringNumber)
                {
                    leftOperand = Evaluate(leftOperand, current, pendingOperator);
                    SetResult(leftOperand);
                }
                // else: user pressed operator twice; we just replace operator
            }

            pendingOperator = op;
            lbl_expression.Text = $"{leftOperand} {pendingOperator}";
            isEnteringNumber = false;
        }

        private void PressEquals()
        {
            if (pendingOperator == "")
                return;

            double rightOperand = CurrentValue();

            try
            {
                double r = Evaluate(leftOperand, rightOperand, pendingOperator);
                lbl_expression.Text = $"{leftOperand} {pendingOperator} {rightOperand} =";
                SetResult(r);

                // After equals: keep result as the new leftOperand, clear pending operator
                leftOperand = r;
                pendingOperator = "";
                isEnteringNumber = false;
            }
            catch (DivideByZeroException)
            {
                // Simple error handling for class project
                lbl_expression.Text = "";
                lbl_result.Text = "Cannot divide by 0";
                leftOperand = 0;
                pendingOperator = "";
                isEnteringNumber = false;
            }
        }

        private void ClearAll()
        {
            leftOperand = 0;
            pendingOperator = "";
            lbl_expression.Text = "";
            lbl_result.Text = "0";
            isEnteringNumber = false;
        }

        private void ClearEntry()
        {
            lbl_result.Text = "0";
            isEnteringNumber = false;
        }

        #endregion

        #region event handlers

        private void Button_Click(object sender, EventArgs e)
        {
            if (sender is not Button btn) return;

            string v = btn.Text.Trim();

            // DIGITS
            if (v.Length == 1 && char.IsDigit(v[0]))
            {
                if (IsRedundantLeadingZero(v, lbl_result.Text)) return;
                AppendDigit(v);
                return;
            }

            // DECIMAL
            if (v == ".")
            {
                AppendDecimal();
                return;
            }

            // OPERATORS
            if (v == "+" || v == "-" || v == "x" || v == "÷")
            {
                PressOperator(v);
                return;
            }

            // EQUALS
            if (v == "=")
            {
                PressEquals();
                return;
            }

            // CLEAR
            if (v == "C")
            {
                ClearAll();
                return;
            }

            if (v == "CE")
            {
                ClearEntry();
                return;
            }

            // TOGGLE SIGN (depends on what your button text is)
            if (v == "±" || v == "+/-")
            {
                ToggleSign();
                return;
            }

            // If your assignment has other buttons, add them above.
        }

        private void WireAllButtonsToOneHandlerIterative()
        {
            var stack = new Stack<Control>();
            stack.Push(this);

            while (stack.Count > 0)
            {
                Control current = stack.Pop();

                if (current is Button b)
                {
                    b.Click -= Button_Click;   // avoid double-subscribe
                    b.Click += Button_Click;   // SAME handler for every button
                }

                foreach (Control child in current.Controls)
                    stack.Push(child);
            }
        }

        #endregion
    }
}