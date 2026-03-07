namespace elie_hanna_p1_calculator_csc2081a_mar_12
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            WireAllButtonsToOneHandlerIterative();
        }

        private void WireAllButtonsToOneHandlerIterative()
        {
            var stack = new Stack<Control>();
            stack.Push(this);

            while (stack.Count > 0)
            {
                Control current = stack.Pop();

                // Subscribe button clicks
                if (current is Button b)
                {
                    b.Click -= Button_Click;   // avoid double-subscribe
                    b.Click += Button_Click;   // SAME handler for every button
                }

                // Add children to stack
                foreach (Control child in current.Controls)
                    stack.Push(child);
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            if (sender is not Button btn) return;

            string v = btn.Text.Trim();

            // DIGITS
            if (v.Length == 1 && char.IsDigit(v[0]))
            {
                HandleDigit(v);
                return;
            }

            // PLUS / MINUS
            if (v == "+" || v == "-")
            {
                HandlePlusMinus(v);
                return;
            }

            // later: ".", "C", "CE", "=", etc.
        }

        private void HandleDigit(string digit)
        {
            lbl_result.Text = lbl_result.Text == "0" ? digit : lbl_result.Text + digit;

            // mirror into operand-left so you can SEE it
            lbl_operand_left.Text = lbl_result.Text;

            // DEBUG (temporary)
            this.Text = $"result={lbl_result.Text} left={lbl_operand_left.Text}";
        }

        private void HandlePlusMinus(string op)
        {
            bool leftIsEmpty = string.IsNullOrWhiteSpace(lbl_operand_left.Text);

            if (leftIsEmpty)
            {
                // "operand is null": treat + / - as sign assignment to the current entry
                // "+" makes it positive, "-" makes it negative (toggle/force)
                string current = lbl_result.Text.Trim();

                if (op == "+")
                {
                    // remove leading minus if exists
                    if (current.StartsWith("-"))
                        current = current.Substring(1);
                }
                else // op == "-"
                {
                    if (!current.StartsWith("-") && current != "0")
                        current = "-" + current;
                    // if it's "0", keep it "0" (typical calculator behavior)
                }

                lbl_result.Text = current;
                lbl_operand_left.Text = current;
            }
            else
            {
                // left operand exists: assign operator label
                lbl_operator.Text = op;

                // next step (later): start typing the right operand
                // you can decide to clear result or keep it for now
                // lbl_result.Text = "0";
            }
        }


    }
}
