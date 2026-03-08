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
        #region global var

        private double leftOperand = 0;
        private string pendingOperator = "";
        private bool isEnteringNumber = false;

        private double lastRightOperand = 0;
        private string lastOperator = "";

        private bool isSecondMode = false;
        private Color secondDefaultBackColor = SystemColors.ActiveBorder;

        private bool fxIsEnteringAmount = false;

        private double fxCachedRate = 0;
        private string fxCachedPair = "";
        private DateTime fxCachedAt = DateTime.MinValue;

        private static readonly HttpClient fxHttp = new HttpClient();

        #endregion

        public Form1()
        {
            InitializeComponent();

            WireAllButtonsToOneHandlerIterative(tabPage1, Button_Click);
            WireAllButtonsToOneHandlerIterative(tabPage2, Button_Click_FX);

            ClearAll();
            SetSecondMode(false);

            FxInitCombos();
            FxClearAll();
        }

        #region event handlers

        private void Button_Click(object sender, EventArgs e)
        {
            if (sender is not Button btn) return;
            string v = btn.Text.Trim();

            if (v == "2nd")
            {
                ToggleSecondMode();
                return;
            }

            if (v.Length == 1 && char.IsDigit(v[0]))
            {
                if (IsRedundantLeadingZero(v)) return;
                AppendDigit(v);
                return;
            }

            if (v == ".")
            {
                AppendDecimal();
                return;
            }

            if (v == "+/-" || v == "±")
            {
                ToggleSign();
                return;
            }

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

            if (v == "⌫" || v == "←" || v == "Back" || v == "⟵")
            {
                Backspace();
                return;
            }

            if (v == "%")
            {
                Percent();
                return;
            }

            if (v == "1/x")
            {
                Reciprocal();
                return;
            }

            if (v == "x²" || v == "x^2")
            {
                Square();
                return;
            }

            if (v == "²√x" || v == "√x" || v == "2√x")
            {
                Sqrt();
                return;
            }

            if (v == "|x|")
            {
                Abs();
                return;
            }

            if (v == "π")
            {
                InsertConstant(Math.PI);
                return;
            }

            if (v == "e")
            {
                InsertConstant(Math.E);
                return;
            }

            if (v == "exp")
            {
                Exp();
                return;
            }

            if (v == "ln")
            {
                Ln();
                return;
            }

            if (v == "log")
            {
                Log10();
                return;
            }

            if (v == "10ˣ" || v == "10^x")
            {
                TenPowerX();
                return;
            }

            if (v == "sin")
            {
                Sin();
                return;
            }

            if (v == "cos")
            {
                Cos();
                return;
            }

            if (v == "tan")
            {
                Tan();
                return;
            }

            if (v == "n!")
            {
                FactorialUnary();
                return;
            }

            if (v == "(")
            {
                AppendParen("(");
                return;
            }

            if (v == ")")
            {
                AppendParen(")");
                return;
            }

            if (v == "+" || v == "-" || v == "X" || v == "x" || v == "÷" || v == "/")
            {
                if (v == "x") v = "X";
                PressOperator(v);
                return;
            }

            if (v == "mod")
            {
                PressOperator("mod");
                return;
            }

            if (v == "xʸ" || v == "x^y")
            {
                PressOperator("xʸ");
                return;
            }

            if (v == "=")
            {
                PressEquals();
                return;
            }
        }

        private async void Button_Click_FX(object sender, EventArgs e)
        {
            if (sender is not Button btn) return;
            string v = btn.Text.Trim();

            if (v.Length == 1 && char.IsDigit(v[0]))
            {
                FxAppendDigit(v);
                return;
            }

            if (v == ".")
            {
                FxAppendDecimal();
                return;
            }

            if (v == "C")
            {
                FxClearAll();
                return;
            }

            if (v == "⌫" || v == "←" || v == "Back" || v == "⟵")
            {
                FxBackspace();
                return;
            }

            if (v.Equals("Copy", StringComparison.OrdinalIgnoreCase))
            {
                FxCopy();
                return;
            }

            if (v.Equals("Paste", StringComparison.OrdinalIgnoreCase))
            {
                FxPaste();
                return;
            }

            if (btn.Name == "btn_fx_convert" || v.Equals("Convert", StringComparison.OrdinalIgnoreCase))
            {
                await FxConvertAsync();
                return;
            }
        }

        #endregion

        #region private methods

        #region helper methods

        private double CurrentValue()
        {
            if (double.TryParse(lbl_result.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double v))
                return v;

            if (double.TryParse(lbl_result.Text, out v))
                return v;

            return 0;
        }

        private void SetResult(double v)
        {
            if (double.IsNaN(v) || double.IsInfinity(v))
            {
                ShowError("Invalid input");
                return;
            }

            lbl_result.Text = v.ToString(CultureInfo.InvariantCulture);
        }

        private void ShowError(string message)
        {
            lbl_expression.Text = "";
            lbl_result.Text = message;

            leftOperand = 0;
            pendingOperator = "";
            lastRightOperand = 0;
            lastOperator = "";
            isEnteringNumber = false;
        }

        private bool IsRedundantLeadingZero(string digit)
        {
            return digit == "0" && lbl_result.Text == "0" && !isEnteringNumber;
        }

        private double Evaluate(double a, double b, string op)
        {
            return op switch
            {
                "+" => a + b,
                "-" => a - b,
                "X" => a * b,
                "x" => a * b,
                "÷" => b == 0 ? throw new DivideByZeroException() : a / b,
                "/" => b == 0 ? throw new DivideByZeroException() : a / b,
                "mod" => b == 0 ? throw new DivideByZeroException() : a % b,
                "xʸ" => Math.Pow(a, b),
                _ => b
            };
        }

        private static bool IsInteger(double x)
        {
            return Math.Abs(x - Math.Round(x)) < 1e-12;
        }

        private static double Factorial(double n)
        {
            if (n < 0) throw new ArgumentOutOfRangeException(nameof(n));
            if (!IsInteger(n)) throw new ArgumentException("Factorial requires integer input.");
            if (n > 170) throw new OverflowException("Result too large.");

            double r = 1;
            for (int i = 2; i <= (int)Math.Round(n); i++)
                r *= i;
            return r;
        }

        private void WireAllButtonsToOneHandlerIterative(Control root, EventHandler handler)
        {
            var stack = new Stack<Control>();
            stack.Push(root);

            while (stack.Count > 0)
            {
                Control current = stack.Pop();

                if (current is Button b)
                {
                    b.Click -= handler;
                    b.Click += handler;
                }

                foreach (Control child in current.Controls)
                    stack.Push(child);
            }
        }

        private double FxCurrentValue()
        {
            if (double.TryParse(lbl_fx_display.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double v))
                return v;

            if (double.TryParse(lbl_fx_display.Text, out v))
                return v;

            return 0;
        }

        private void FxSetDisplay(double v)
        {
            if (double.IsNaN(v) || double.IsInfinity(v))
            {
                lbl_fx_status.Text = "Invalid input";
                return;
            }

            lbl_fx_display.Text = v.ToString(CultureInfo.InvariantCulture);
        }

        private static string NormalizeCurrency(string code)
        {
            return (code ?? "").Trim().ToUpperInvariant();
        }

        #endregion

        #region button handlers

        private void AppendDigit(string digit)
        {
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
            if (!isEnteringNumber)
            {
                lbl_result.Text = "0.";
                isEnteringNumber = true;
                return;
            }

            if (!lbl_result.Text.Contains("."))
                lbl_result.Text += ".";
        }

        private void Backspace()
        {
            if (!isEnteringNumber)
            {
                ClearEntry();
                return;
            }

            if (lbl_result.Text.Length <= 1 || (lbl_result.Text.Length == 2 && lbl_result.Text.StartsWith("-")))
            {
                lbl_result.Text = "0";
                isEnteringNumber = false;
                return;
            }

            lbl_result.Text = lbl_result.Text.Substring(0, lbl_result.Text.Length - 1);

            if (lbl_result.Text == "-" || lbl_result.Text.Length == 0)
            {
                lbl_result.Text = "0";
                isEnteringNumber = false;
            }
        }

        private void ToggleSign()
        {
            if (lbl_result.Text.StartsWith("-"))
                lbl_result.Text = lbl_result.Text.Substring(1);
            else if (lbl_result.Text != "0")
                lbl_result.Text = "-" + lbl_result.Text;

            isEnteringNumber = true;
        }

        private void ClearAll()
        {
            leftOperand = 0;
            pendingOperator = "";
            lbl_expression.Text = "";
            lbl_result.Text = "0";
            isEnteringNumber = false;

            lastRightOperand = 0;
            lastOperator = "";
        }

        private void ClearEntry()
        {
            lbl_result.Text = "0";
            isEnteringNumber = false;
        }

        private void PressOperator(string op)
        {
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

        private void PressEquals()
        {
            if (pendingOperator == "")
            {
                if (lastOperator != "")
                {
                    try
                    {
                        double current = CurrentValue();
                        double r2 = Evaluate(current, lastRightOperand, lastOperator);
                        lbl_expression.Text = $"{current} {lastOperator} {lastRightOperand} =";
                        SetResult(r2);

                        leftOperand = r2;
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
                return;
            }

            double right = CurrentValue();

            try
            {
                double r = Evaluate(leftOperand, right, pendingOperator);
                lbl_expression.Text = $"{leftOperand} {pendingOperator} {right} =";
                SetResult(r);

                lastRightOperand = right;
                lastOperator = pendingOperator;

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

        private void Percent()
        {
            double current = CurrentValue();

            if (pendingOperator != "")
            {
                double percentValue = leftOperand * (current / 100.0);
                SetResult(percentValue);
            }
            else
            {
                SetResult(current / 100.0);
            }

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
            try
            {
                SetResult(Factorial(x));
                isEnteringNumber = true;
            }
            catch
            {
                ShowError("Invalid input");
            }
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

        private void AppendParen(string paren)
        {
            lbl_expression.Text = $"{lbl_expression.Text} {paren}".Trim();
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

        private void FxAppendDigit(string digit)
        {
            if (!fxIsEnteringAmount || lbl_fx_display.Text == "0")
            {
                lbl_fx_display.Text = digit;
                fxIsEnteringAmount = true;
            }
            else
            {
                lbl_fx_display.Text += digit;
            }
        }

        private void FxAppendDecimal()
        {
            if (!fxIsEnteringAmount)
            {
                lbl_fx_display.Text = "0.";
                fxIsEnteringAmount = true;
                return;
            }

            if (!lbl_fx_display.Text.Contains("."))
                lbl_fx_display.Text += ".";
        }

        private void FxBackspace()
        {
            if (!fxIsEnteringAmount)
            {
                lbl_fx_display.Text = "0";
                fxIsEnteringAmount = false;
                return;
            }

            if (lbl_fx_display.Text.Length <= 1 || (lbl_fx_display.Text.Length == 2 && lbl_fx_display.Text.StartsWith("-")))
            {
                lbl_fx_display.Text = "0";
                fxIsEnteringAmount = false;
                return;
            }

            lbl_fx_display.Text = lbl_fx_display.Text.Substring(0, lbl_fx_display.Text.Length - 1);

            if (lbl_fx_display.Text == "-" || lbl_fx_display.Text.Length == 0)
            {
                lbl_fx_display.Text = "0";
                fxIsEnteringAmount = false;
            }
        }

        private void FxClearAll()
        {
            fxIsEnteringAmount = false;

            fxCachedRate = 0;
            fxCachedPair = "";
            fxCachedAt = DateTime.MinValue;

            lbl_fx_display.Text = "0";
            lbl_fx_status.Text = "";

            if (cmb_fx_from != null) cmb_fx_from.SelectedIndex = 0;
            if (cmb_fx_to != null) cmb_fx_to.SelectedIndex = 1;
        }

        private void FxCopy()
        {
            try
            {
                Clipboard.SetText(lbl_fx_display.Text);
                lbl_fx_status.Text = "Copied";
            }
            catch
            {
                lbl_fx_status.Text = "Copy failed";
            }
        }

        private void FxPaste()
        {
            try
            {
                string t = Clipboard.GetText()?.Trim() ?? "";
                if (t.Length == 0)
                {
                    lbl_fx_status.Text = "Clipboard empty";
                    return;
                }

                if (!double.TryParse(t, NumberStyles.Float, CultureInfo.InvariantCulture, out double v) &&
                    !double.TryParse(t, NumberStyles.Float, CultureInfo.CurrentCulture, out v))
                {
                    lbl_fx_status.Text = "Invalid paste";
                    return;
                }

                FxSetDisplay(v);
                fxIsEnteringAmount = true;
                lbl_fx_status.Text = "";
            }
            catch
            {
                lbl_fx_status.Text = "Paste failed";
            }
        }

        private async Task FxConvertAsync()
        {
            string from = cmb_fx_from?.SelectedItem?.ToString() ?? "";
            string to = cmb_fx_to?.SelectedItem?.ToString() ?? "";

            from = NormalizeCurrency(from);
            to = NormalizeCurrency(to);

            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
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

            lbl_fx_status.Text = "Loading rate...";

            try
            {
                double rate = await FxGetRateAsync(from, to);
                double converted = amount * rate;

                FxSetDisplay(converted);
                fxIsEnteringAmount = false;

                lbl_fx_status.Text = $"{from}->{to} rate={rate.ToString(CultureInfo.InvariantCulture)}";
            }
            catch (Exception ex)
            {
                lbl_fx_status.Text = "Conversion failed: " + ex.Message;
            }
        }

        private async Task<double> FxGetRateAsync(string from, string to)
        {
            string f = NormalizeCurrency(from);
            string t = NormalizeCurrency(to);
            string pair = $"{f}->{t}";

            if (fxCachedPair == pair && fxCachedRate > 0 && (DateTime.Now - fxCachedAt).TotalMinutes < 10)
                return fxCachedRate;

            string url = $"https://hexarate.paikama.co/api/rates/{f}/{t}/latest";

            using HttpResponseMessage resp = await fxHttp.GetAsync(url);
            resp.EnsureSuccessStatusCode();

            string json = await resp.Content.ReadAsStringAsync();

            using JsonDocument doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("data", out JsonElement dataEl))
                throw new Exception("Missing data");

            if (!dataEl.TryGetProperty("mid", out JsonElement midEl))
                throw new Exception("Missing mid");

            double mid = midEl.GetDouble();

            double unit = 1;
            if (dataEl.TryGetProperty("unit", out JsonElement unitEl) && unitEl.ValueKind == JsonValueKind.Number)
                unit = unitEl.GetDouble();

            if (unit == 0)
                throw new Exception("Invalid unit");

            double rate = mid / unit;

            fxCachedPair = pair;
            fxCachedRate = rate;
            fxCachedAt = DateTime.Now;

            return rate;
        }

        #endregion

        #endregion
    }
}