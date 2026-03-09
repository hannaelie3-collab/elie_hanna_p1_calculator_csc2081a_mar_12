using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace elie_hanna_p1_calculator_csc2081a_mar_12
{
    public partial class Form1 : Form
    {
        private readonly List<string> tokens = new();   // e.g. ["8", "+", "(", "2", "+", "5", ")"]
        private string lastBinaryOp = "";
        private double lastBinaryRight = 0;
        private bool canRepeatEquals = false;
        private bool isEnteringNumber = false;

        private double leftOperand = 0;
        private string pendingOperator = "";
        private double lastRightOperand = 0;
        private string lastOperator = "";

        private bool isSecondMode = false;
        private Color secondDefaultBackColor = SystemColors.ActiveBorder;
        private bool fxIsEnteringAmount = false;
        private static readonly HttpClient fxHttp = new HttpClient();
        private const int MAX_RESULT_LEN = 14;
        private const string SCI_FORMAT = "0.###E+0";
        private const double INTEGER_TOLERANCE = 1e-12;
        private const int MAX_FACTORIAL_INPUT = 170;

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

        private void Button_Click(object sender, EventArgs e)
        {
            if (sender is not Button btn) return;
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

                case "âŒ«":
                    BackspaceCommon(lbl_result, ref isEnteringNumber, ClearEntry);
                    break;

                case "%":
                    Percent();
                    break;

                case "1/x":
                    Reciprocal();
                    break;

                case "xÂ²":
                    Square();
                    break;

                case "Â²âˆšx":
                    Sqrt();
                    break;

                case "|x|":
                    Abs();
                    break;

                case "Ï€":
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

                case "10Ë£" or "10^x":
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

                case "+" or "-" or "X" or "Ã·" or "mod" or "xÊ¸":
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

        private async void Button_Click_FX(object sender, EventArgs e)
        {
            if (sender is not Button btn) return;
            string v = btn.Text.Trim();

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

                case "âŒ«" or "â†" or "Back" or "âŸµ":
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

        private void AppendDecimalCommon(Label displayLabel, ref bool isEnteringFlag)
        {
            if (!isEnteringFlag)
            {
                displayLabel.Text = "0.";
                isEnteringFlag = true;
                return;
            }

            if (!displayLabel.Text.Contains("."))
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

            if (displayLabel.Text.Length <= 1 || (displayLabel.Text.Length == 2 && displayLabel.Text.StartsWith("-")))
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
            if (double.TryParse(displayLabel.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double v))
                return v;

            if (double.TryParse(displayLabel.Text, out v))
                return v;

            return 0;
        }

        private static int Precedence(string op) => op switch
        {
            "xÊ¸" => 3,
            "X" or "Ã·" or "mod" => 2,
            "+" or "-" => 1,
            _ => 0
        };

        private static double Apply(double a, double b, string op) => op switch
        {
            "+" => a + b,
            "-" => a - b,
            "X" => a * b,
            "Ã·" => b == 0 ? throw new DivideByZeroException() : a / b,
            "mod" => b == 0 ? throw new DivideByZeroException() : a % b,
            "xÊ¸" => Math.Pow(a, b),
            _ => throw new InvalidOperationException($"Unknown operator: {op}")
        };


        private void CaptureRepeatEqualsMemory(List<string> working)
        {
            canRepeatEquals = false;
            if (working.Count < 3) return;
            int i = working.Count - 1;
            if (!double.TryParse(working[i], NumberStyles.Float, CultureInfo.InvariantCulture, out double right))
                return;
            string op = working[i - 1];
            bool opIsBinary = op is "+" or "-" or "X" or "Ã·" or "mod" or "xÊ¸";
            if (!opIsBinary) return;
            string leftTok = working[i - 2];
            bool leftOk =
                double.TryParse(leftTok, NumberStyles.Float, CultureInfo.InvariantCulture, out _) ||
                leftTok == ")";

            if (!leftOk) return;

            lastBinaryOp = op;
            lastBinaryRight = right;
            canRepeatEquals = true;
        }

        private double EvaluateTokens(List<string> tks)
        {
            var values = new Stack<double>();
            var ops = new Stack<string>();

            void ReduceOnce()
            {
                string op = ops.Pop();
                double b = values.Pop();
                double a = values.Pop();
                values.Push(Apply(a, b, op));
            }

            foreach (string tk in tks)
            {
                if (double.TryParse(tk, NumberStyles.Float, CultureInfo.InvariantCulture, out double num))
                {
                    values.Push(num);
                }
                else if (tk == "(")
                {
                    ops.Push(tk);
                }
                else if (tk == ")")
                {
                    while (ops.Count > 0 && ops.Peek() != "(") ReduceOnce();
                    if (ops.Count == 0) throw new InvalidOperationException("Mismatched parentheses");
                    ops.Pop();
                }
                else
                {
                    while (ops.Count > 0 && ops.Peek() != "(" && Precedence(ops.Peek()) >= Precedence(tk))
                        ReduceOnce();
                    ops.Push(tk);
                }
            }

            while (ops.Count > 0)
            {
                if (ops.Peek() == "(") throw new InvalidOperationException("Mismatched parentheses");
                ReduceOnce();
            }

            if (values.Count != 1) throw new InvalidOperationException("Invalid expression");
            return values.Pop();
        }

        private List<string> GetInnermostParenSlice(List<string> tks)
        {
            int start = tks.LastIndexOf("(");
            if (start < 0) return new List<string>(tks);
            return tks.Skip(start + 1).ToList();
        }
        private void RefreshDisplay()
        {
            var working = new List<string>(tokens);

            if (isEnteringNumber)
                working.Add(lbl_result.Text);

            var slice = GetInnermostParenSlice(working);
            if (slice.Count == 0) return;
            string last = slice[^1];
            bool lastIsOp = last is "+" or "-" or "X" or "Ã·" or "mod" or "xÊ¸";
            if (lastIsOp)
                slice.RemoveAt(slice.Count - 1);

            if (slice.Count == 0) return;
            if (slice[^1] == "(") return;

            try
            {
                double v = EvaluateTokens(slice);
                lbl_result.Text = FormatForDisplay(v);
            }
            catch
            {
            }
        }
        private void PushCurrentEntryIfNeeded()
        {
            if (isEnteringNumber)
            {
                tokens.Add(lbl_result.Text);
                isEnteringNumber = false;
            }
        }

        private bool IsOperator(string tk) => tk is "+" or "-" or "X" or "Ã·" or "mod" or "xÊ¸";

        private void PressLeftParen()
        {
            if (tokens.Count > 0)
            {
                string last = tokens[^1];
                bool lastIsNumber = double.TryParse(last, NumberStyles.Float, CultureInfo.InvariantCulture, out _);
                if (lastIsNumber || last == ")")
                    tokens.Add("X");
            }

            tokens.Add("(");
            lbl_expression.Text = string.Join(" ", tokens);
            lbl_result.Text = "0";
            isEnteringNumber = false;
        }

        private void PressRightParen()
        {
            PushCurrentEntryIfNeeded();
            if (!tokens.Contains("(")) return;
            if (tokens.Count > 0)
            {
                string last = tokens[^1];
                if (last == "(" || IsOperator(last))
                    return;
            }

            tokens.Add(")");
            lbl_expression.Text = string.Join(" ", tokens);
            RefreshDisplay();
        }

        private void PressOperator(string op)
        {
            canRepeatEquals = false;

            PushCurrentEntryIfNeeded();

            if (tokens.Count == 0)
            {
                tokens.Add(CurrentValue().ToString(CultureInfo.InvariantCulture));
                tokens.Add(op);
            }
            else
            {
                string last = tokens[^1];
                if (IsOperator(last))
                {
                    tokens[^1] = op;
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
                    tokens.Add(op);
                }
            }

            lbl_expression.Text = string.Join(" ", tokens);
            RefreshDisplay();
        }

        private void PressEquals()
        {
            if (tokens.Count == 0 && !isEnteringNumber && canRepeatEquals && lastBinaryOp != "")
            {
                try
                {
                    double current = CurrentValue();
                    double rRepeat = Apply(current, lastBinaryRight, lastBinaryOp);
                    lbl_expression.Text = $"{FormatForDisplay(current)} {lastBinaryOp} {FormatForDisplay(lastBinaryRight)} =";
                    SetResult(rRepeat);
                    isEnteringNumber = false;
                    return;
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

            var working = new List<string>(tokens);

            if (isEnteringNumber)
                working.Add(lbl_result.Text);
            while (working.Count > 0)
            {
                string last = working[^1];
                bool lastIsOp = last is "+" or "-" or "X" or "Ã·" or "mod" or "xÊ¸";
                if (lastIsOp || last == "(")
                {
                    working.RemoveAt(working.Count - 1);
                    continue;
                }
                break;
            }

            if (working.Count == 0) return;

            try
            {
                double r = EvaluateTokens(working);
                lbl_expression.Text = string.Join(" ", working) + " =";
                SetResult(r);
                CaptureRepeatEqualsMemory(working);
                tokens.Clear();
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

        private double CurrentValue() => ParseDisplayValue(lbl_result);

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
            leftOperand = 0;
            pendingOperator = "";
            lastRightOperand = 0;
            lastOperator = "";

            tokens.Clear();
            isEnteringNumber = false;
            lastBinaryOp = "";
            lastBinaryRight = 0;
            canRepeatEquals = false;
        }

        private static bool IsInteger(double x)
        {
            return Math.Abs(x - Math.Round(x)) < INTEGER_TOLERANCE;
        }

        private static double Factorial(double n)
        {
            if (n < 0) throw new ArgumentOutOfRangeException(nameof(n));
            if (!IsInteger(n)) throw new ArgumentException("Factorial requires integer input.");
            if (n > MAX_FACTORIAL_INPUT) throw new OverflowException("Result too large.");

            double r = 1;
            for (int i = 2; i <= (int)Math.Round(n); i++)
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
            lastRightOperand = 0;
            lastOperator = "";
            tokens.Clear();
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

        private void SetSecondMode(bool enabled)
        {
            isSecondMode = enabled;

            btn_second.BackColor = enabled
                ? Color.FromArgb(255, 205, 145)
                : secondDefaultBackColor;

            btn_power_10.Text = enabled ? "sin" : "10Ë£";
            btn_log10.Text = enabled ? "cos" : "log";
            btn_ln.Text = enabled ? "tan" : "ln";
        }

        private void ToggleSecondMode()
        {
            SetSecondMode(!isSecondMode);
        }

        private double FxCurrentValue() => ParseDisplayValue(lbl_fx_display);

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
            return (code ?? "").Trim().ToUpperInvariant();
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
            string url = $"https://hexarate.paikama.co/api/rates/{from}/{to}/latest";

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

            return mid / unit;
        }

    }
}
