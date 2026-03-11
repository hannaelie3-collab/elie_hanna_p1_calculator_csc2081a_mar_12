using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace elie_hanna_p1_calculator_csc2081a_mar_12
{
    public partial class Form1 : Form
    {
        // =========================
        // Initial variables / fields
        // =========================

        private const int MAX_TOKENS = 256;
        private readonly string[] tokens = new string[MAX_TOKENS];
        private int tokenCount = 0;

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

        private void GetInnermostParenSlice(string[] src, int srcCount, out int startIndex, out int sliceCount)
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
            {
                startIndex = 0;
                sliceCount = srcCount;
                return;
            }

            startIndex = start + 1;
            sliceCount = srcCount - startIndex;
        }

        private void RefreshDisplay()
        {
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

            int sliceStart, sliceLen;
            GetInnermostParenSlice(working, workingCount, out sliceStart, out sliceLen);

            if (sliceLen <= 0) return;

            int lastIndex = sliceStart + sliceLen - 1;
            string last = working[lastIndex];
            bool lastIsOp = IsOperator(last);
            if (lastIsOp)
            {
                sliceLen--;
            }

            if (sliceLen <= 0) return;
            if (working[sliceStart] == "(") return;

            int evalCount = sliceStart + sliceLen;
            double v = EvaluateTokens(working, evalCount);
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
            else if (v.Equals("Paste", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    string t = Clipboard.GetText();
                    if (t == null) t = "";
                    t = t.Trim();
                    if (t.Length == 0)
                    {
                        lbl_fx_status.Text = "Clipboard empty";
                        return;
                    }

                    double vv;
                    if (!double.TryParse(t, NumberStyles.Float, CultureInfo.InvariantCulture, out vv) &&
                        !double.TryParse(t, NumberStyles.Float, CultureInfo.CurrentCulture, out vv))
                    {
                        lbl_fx_status.Text = "Invalid paste";
                        return;
                    }

                    FxSetDisplay(vv);
                    fxIsEnteringAmount = true;
                    lbl_fx_status.Text = "";
                }
                catch
                {
                    lbl_fx_status.Text = "Paste failed";
                }
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
    }
}