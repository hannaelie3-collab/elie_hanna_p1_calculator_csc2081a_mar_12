namespace elie_hanna_p1_calculator_csc2081a_mar_12
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            tableLayoutPanel1 = new TableLayoutPanel();
            btn_abs = new Button();
            btn_second = new Button();
            btn_mod = new Button();
            btn_reciprocal = new Button();
            btn_exp = new Button();
            btn_backspace = new Button();
            btn_clear_all = new Button();
            btn_divide = new Button();
            btn_multiply = new Button();
            btn_subtract = new Button();
            btn_pi = new Button();
            btn_e = new Button();
            btn_add = new Button();
            btn_equals = new Button();
            btn_digit_9 = new Button();
            btn_digit_6 = new Button();
            btn_digit_3 = new Button();
            btn_factorial = new Button();
            btn_decimal = new Button();
            btn_digit_0 = new Button();
            btn_digit_2 = new Button();
            btn_digit_5 = new Button();
            btn_digit_8 = new Button();
            btn_digit_1 = new Button();
            btn_digit_4 = new Button();
            btn_digit_7 = new Button();
            btn_plus_minus = new Button();
            btn_nth_root = new Button();
            btn_square = new Button();
            btn_power = new Button();
            btn_power_10 = new Button();
            btn_log10 = new Button();
            btn_ln = new Button();
            button3 = new Button();
            button1 = new Button();
            lbl_expression = new Label();
            lbl_result = new Label();
            tabPage2 = new TabPage();
            lbl_fx_status = new Label();
            lbl_fx_display = new Label();
            tableLayoutPanel2 = new TableLayoutPanel();
            cmb_fx_from = new ComboBox();
            button2 = new Button();
            btn_fx_digit_7 = new Button();
            btn_fx_digit_4 = new Button();
            btn_fx_digit_1 = new Button();
            btn_fx_decimal = new Button();
            btn_fx_convert = new Button();
            btn_fx_paste = new Button();
            btn_fx_digit_8 = new Button();
            btn_fx_digit_5 = new Button();
            btn_fx_digit_2 = new Button();
            btn_fx_digit_0 = new Button();
            cmb_fx_to = new ComboBox();
            btn_fx_backspace = new Button();
            btn_fx_digit_9 = new Button();
            btn_fx_digit_6 = new Button();
            btn_fx_digit_3 = new Button();
            btn_fx_clear_all = new Button();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            tabPage2.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Dock = DockStyle.Left;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(410, 661);
            tabControl1.TabIndex = 8;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(tableLayoutPanel1);
            tabPage1.Controls.Add(lbl_expression);
            tabPage1.Controls.Add(lbl_result);
            tabPage1.Location = new Point(4, 49);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(402, 608);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Scientific Calculator";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 5;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.Controls.Add(btn_abs, 2, 1);
            tableLayoutPanel1.Controls.Add(btn_second, 0, 0);
            tableLayoutPanel1.Controls.Add(btn_mod, 4, 1);
            tableLayoutPanel1.Controls.Add(btn_reciprocal, 1, 1);
            tableLayoutPanel1.Controls.Add(btn_exp, 3, 1);
            tableLayoutPanel1.Controls.Add(btn_backspace, 4, 0);
            tableLayoutPanel1.Controls.Add(btn_clear_all, 3, 0);
            tableLayoutPanel1.Controls.Add(btn_divide, 4, 2);
            tableLayoutPanel1.Controls.Add(btn_multiply, 4, 3);
            tableLayoutPanel1.Controls.Add(btn_subtract, 4, 4);
            tableLayoutPanel1.Controls.Add(btn_pi, 1, 0);
            tableLayoutPanel1.Controls.Add(btn_e, 2, 0);
            tableLayoutPanel1.Controls.Add(btn_add, 4, 5);
            tableLayoutPanel1.Controls.Add(btn_equals, 4, 6);
            tableLayoutPanel1.Controls.Add(btn_digit_9, 3, 3);
            tableLayoutPanel1.Controls.Add(btn_digit_6, 3, 4);
            tableLayoutPanel1.Controls.Add(btn_digit_3, 3, 5);
            tableLayoutPanel1.Controls.Add(btn_factorial, 3, 2);
            tableLayoutPanel1.Controls.Add(btn_decimal, 3, 6);
            tableLayoutPanel1.Controls.Add(btn_digit_0, 2, 6);
            tableLayoutPanel1.Controls.Add(btn_digit_2, 2, 5);
            tableLayoutPanel1.Controls.Add(btn_digit_5, 2, 4);
            tableLayoutPanel1.Controls.Add(btn_digit_8, 2, 3);
            tableLayoutPanel1.Controls.Add(btn_digit_1, 1, 5);
            tableLayoutPanel1.Controls.Add(btn_digit_4, 1, 4);
            tableLayoutPanel1.Controls.Add(btn_digit_7, 1, 3);
            tableLayoutPanel1.Controls.Add(btn_plus_minus, 1, 6);
            tableLayoutPanel1.Controls.Add(btn_nth_root, 0, 2);
            tableLayoutPanel1.Controls.Add(btn_square, 0, 1);
            tableLayoutPanel1.Controls.Add(btn_power, 0, 3);
            tableLayoutPanel1.Controls.Add(btn_power_10, 0, 4);
            tableLayoutPanel1.Controls.Add(btn_log10, 0, 5);
            tableLayoutPanel1.Controls.Add(btn_ln, 0, 6);
            tableLayoutPanel1.Controls.Add(button3, 2, 2);
            tableLayoutPanel1.Controls.Add(button1, 1, 2);
            tableLayoutPanel1.Dock = DockStyle.Bottom;
            tableLayoutPanel1.Location = new Point(3, 186);
            tableLayoutPanel1.Margin = new Padding(3, 2, 3, 2);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 7;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanel1.Size = new Size(396, 419);
            tableLayoutPanel1.TabIndex = 10;
            // 
            // btn_abs
            // 
            btn_abs.BackColor = SystemColors.ButtonHighlight;
            btn_abs.Dock = DockStyle.Fill;
            btn_abs.Font = new Font("Segoe UI", 14.25F);
            btn_abs.Location = new Point(161, 61);
            btn_abs.Margin = new Padding(3, 2, 3, 2);
            btn_abs.Name = "btn_abs";
            btn_abs.Size = new Size(73, 55);
            btn_abs.TabIndex = 33;
            btn_abs.Text = "|x|";
            btn_abs.UseVisualStyleBackColor = false;
            // 
            // btn_second
            // 
            btn_second.BackColor = SystemColors.ActiveBorder;
            btn_second.Dock = DockStyle.Fill;
            btn_second.Font = new Font("Segoe UI", 14.25F);
            btn_second.Location = new Point(3, 2);
            btn_second.Margin = new Padding(3, 2, 3, 2);
            btn_second.Name = "btn_second";
            btn_second.Size = new Size(73, 55);
            btn_second.TabIndex = 40;
            btn_second.Text = "2nd";
            btn_second.UseVisualStyleBackColor = false;
            // 
            // btn_mod
            // 
            btn_mod.BackColor = SystemColors.ButtonHighlight;
            btn_mod.Dock = DockStyle.Fill;
            btn_mod.Font = new Font("Segoe UI", 14.25F);
            btn_mod.Location = new Point(319, 61);
            btn_mod.Margin = new Padding(3, 2, 3, 2);
            btn_mod.Name = "btn_mod";
            btn_mod.Size = new Size(74, 55);
            btn_mod.TabIndex = 22;
            btn_mod.Text = "mod";
            btn_mod.UseVisualStyleBackColor = false;
            // 
            // btn_reciprocal
            // 
            btn_reciprocal.BackColor = SystemColors.ButtonHighlight;
            btn_reciprocal.Dock = DockStyle.Fill;
            btn_reciprocal.Font = new Font("Microsoft Sans Serif", 14.25F);
            btn_reciprocal.Location = new Point(82, 61);
            btn_reciprocal.Margin = new Padding(3, 2, 3, 2);
            btn_reciprocal.Name = "btn_reciprocal";
            btn_reciprocal.Size = new Size(73, 55);
            btn_reciprocal.TabIndex = 6;
            btn_reciprocal.Text = "1/x";
            btn_reciprocal.UseVisualStyleBackColor = false;
            // 
            // btn_exp
            // 
            btn_exp.BackColor = SystemColors.ButtonHighlight;
            btn_exp.Dock = DockStyle.Fill;
            btn_exp.Font = new Font("Segoe UI", 14.25F);
            btn_exp.Location = new Point(240, 61);
            btn_exp.Margin = new Padding(3, 2, 3, 2);
            btn_exp.Name = "btn_exp";
            btn_exp.Size = new Size(73, 55);
            btn_exp.TabIndex = 23;
            btn_exp.Text = "exp";
            btn_exp.UseVisualStyleBackColor = false;
            // 
            // btn_backspace
            // 
            btn_backspace.BackColor = SystemColors.ButtonHighlight;
            btn_backspace.Dock = DockStyle.Fill;
            btn_backspace.Font = new Font("Segoe UI", 14.25F);
            btn_backspace.Location = new Point(319, 2);
            btn_backspace.Margin = new Padding(3, 2, 3, 2);
            btn_backspace.Name = "btn_backspace";
            btn_backspace.Size = new Size(74, 55);
            btn_backspace.TabIndex = 4;
            btn_backspace.Text = "⌫";
            btn_backspace.UseVisualStyleBackColor = false;
            // 
            // btn_clear_all
            // 
            btn_clear_all.BackColor = SystemColors.ButtonHighlight;
            btn_clear_all.Dock = DockStyle.Fill;
            btn_clear_all.Font = new Font("Segoe UI", 14.25F);
            btn_clear_all.Location = new Point(240, 2);
            btn_clear_all.Margin = new Padding(3, 2, 3, 2);
            btn_clear_all.Name = "btn_clear_all";
            btn_clear_all.Size = new Size(73, 55);
            btn_clear_all.TabIndex = 39;
            btn_clear_all.Text = "C";
            btn_clear_all.UseVisualStyleBackColor = false;
            // 
            // btn_divide
            // 
            btn_divide.BackColor = SystemColors.ButtonHighlight;
            btn_divide.Dock = DockStyle.Fill;
            btn_divide.Font = new Font("Segoe UI", 20.25F);
            btn_divide.Location = new Point(319, 120);
            btn_divide.Margin = new Padding(3, 2, 3, 2);
            btn_divide.Name = "btn_divide";
            btn_divide.Size = new Size(74, 55);
            btn_divide.TabIndex = 9;
            btn_divide.Text = "÷";
            btn_divide.UseVisualStyleBackColor = false;
            // 
            // btn_multiply
            // 
            btn_multiply.BackColor = SystemColors.ButtonHighlight;
            btn_multiply.Dock = DockStyle.Fill;
            btn_multiply.Font = new Font("Segoe UI", 20.25F);
            btn_multiply.Location = new Point(319, 179);
            btn_multiply.Margin = new Padding(3, 2, 3, 2);
            btn_multiply.Name = "btn_multiply";
            btn_multiply.Size = new Size(74, 55);
            btn_multiply.TabIndex = 14;
            btn_multiply.Text = "X";
            btn_multiply.UseVisualStyleBackColor = false;
            // 
            // btn_subtract
            // 
            btn_subtract.BackColor = SystemColors.ButtonHighlight;
            btn_subtract.Dock = DockStyle.Fill;
            btn_subtract.Font = new Font("Segoe UI", 20.25F);
            btn_subtract.Location = new Point(319, 238);
            btn_subtract.Margin = new Padding(3, 2, 3, 2);
            btn_subtract.Name = "btn_subtract";
            btn_subtract.Size = new Size(74, 55);
            btn_subtract.TabIndex = 19;
            btn_subtract.Text = "-";
            btn_subtract.UseVisualStyleBackColor = false;
            // 
            // btn_pi
            // 
            btn_pi.BackColor = SystemColors.ButtonHighlight;
            btn_pi.Dock = DockStyle.Fill;
            btn_pi.Font = new Font("Segoe UI", 14.25F);
            btn_pi.Location = new Point(82, 2);
            btn_pi.Margin = new Padding(3, 2, 3, 2);
            btn_pi.Name = "btn_pi";
            btn_pi.Size = new Size(73, 55);
            btn_pi.TabIndex = 24;
            btn_pi.Text = "π";
            btn_pi.UseVisualStyleBackColor = false;
            // 
            // btn_e
            // 
            btn_e.BackColor = SystemColors.ButtonHighlight;
            btn_e.Dock = DockStyle.Fill;
            btn_e.Font = new Font("Segoe UI", 14.25F);
            btn_e.Location = new Point(161, 2);
            btn_e.Margin = new Padding(3, 2, 3, 2);
            btn_e.Name = "btn_e";
            btn_e.Size = new Size(73, 55);
            btn_e.TabIndex = 38;
            btn_e.Text = "e";
            btn_e.UseVisualStyleBackColor = false;
            // 
            // btn_add
            // 
            btn_add.BackColor = SystemColors.ButtonHighlight;
            btn_add.Dock = DockStyle.Fill;
            btn_add.Font = new Font("Segoe UI", 20.25F);
            btn_add.Location = new Point(319, 297);
            btn_add.Margin = new Padding(3, 2, 3, 2);
            btn_add.Name = "btn_add";
            btn_add.Size = new Size(74, 55);
            btn_add.TabIndex = 24;
            btn_add.Text = "+";
            btn_add.UseVisualStyleBackColor = false;
            // 
            // btn_equals
            // 
            btn_equals.BackColor = SystemColors.MenuHighlight;
            btn_equals.Cursor = Cursors.Hand;
            btn_equals.Dock = DockStyle.Fill;
            btn_equals.Font = new Font("Segoe UI", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btn_equals.ForeColor = SystemColors.ActiveCaptionText;
            btn_equals.Location = new Point(319, 356);
            btn_equals.Margin = new Padding(3, 2, 3, 2);
            btn_equals.Name = "btn_equals";
            btn_equals.Size = new Size(74, 61);
            btn_equals.TabIndex = 29;
            btn_equals.Text = "=";
            btn_equals.UseVisualStyleBackColor = false;
            // 
            // btn_digit_9
            // 
            btn_digit_9.BackColor = SystemColors.ButtonHighlight;
            btn_digit_9.Dock = DockStyle.Fill;
            btn_digit_9.Font = new Font("Segoe UI", 14.25F);
            btn_digit_9.Location = new Point(240, 179);
            btn_digit_9.Margin = new Padding(3, 2, 3, 2);
            btn_digit_9.Name = "btn_digit_9";
            btn_digit_9.Size = new Size(73, 55);
            btn_digit_9.TabIndex = 13;
            btn_digit_9.Text = "9";
            btn_digit_9.UseVisualStyleBackColor = false;
            // 
            // btn_digit_6
            // 
            btn_digit_6.BackColor = SystemColors.ButtonHighlight;
            btn_digit_6.Dock = DockStyle.Fill;
            btn_digit_6.Font = new Font("Segoe UI", 14.25F);
            btn_digit_6.Location = new Point(240, 238);
            btn_digit_6.Margin = new Padding(3, 2, 3, 2);
            btn_digit_6.Name = "btn_digit_6";
            btn_digit_6.Size = new Size(73, 55);
            btn_digit_6.TabIndex = 18;
            btn_digit_6.Text = "6";
            btn_digit_6.UseVisualStyleBackColor = false;
            // 
            // btn_digit_3
            // 
            btn_digit_3.BackColor = SystemColors.ButtonHighlight;
            btn_digit_3.Dock = DockStyle.Fill;
            btn_digit_3.Font = new Font("Segoe UI", 14.25F);
            btn_digit_3.Location = new Point(240, 297);
            btn_digit_3.Margin = new Padding(3, 2, 3, 2);
            btn_digit_3.Name = "btn_digit_3";
            btn_digit_3.Size = new Size(73, 55);
            btn_digit_3.TabIndex = 23;
            btn_digit_3.Text = "3";
            btn_digit_3.UseVisualStyleBackColor = false;
            // 
            // btn_factorial
            // 
            btn_factorial.BackColor = SystemColors.ButtonHighlight;
            btn_factorial.Dock = DockStyle.Fill;
            btn_factorial.Font = new Font("Segoe UI", 14.25F);
            btn_factorial.Location = new Point(240, 120);
            btn_factorial.Margin = new Padding(3, 2, 3, 2);
            btn_factorial.Name = "btn_factorial";
            btn_factorial.Size = new Size(73, 55);
            btn_factorial.TabIndex = 30;
            btn_factorial.Text = "n!";
            btn_factorial.UseVisualStyleBackColor = false;
            // 
            // btn_decimal
            // 
            btn_decimal.BackColor = SystemColors.ButtonHighlight;
            btn_decimal.Dock = DockStyle.Fill;
            btn_decimal.Font = new Font("Segoe UI", 14.25F);
            btn_decimal.Location = new Point(240, 356);
            btn_decimal.Margin = new Padding(3, 2, 3, 2);
            btn_decimal.Name = "btn_decimal";
            btn_decimal.Size = new Size(73, 61);
            btn_decimal.TabIndex = 28;
            btn_decimal.Text = ".";
            btn_decimal.UseVisualStyleBackColor = false;
            // 
            // btn_digit_0
            // 
            btn_digit_0.BackColor = SystemColors.ButtonHighlight;
            btn_digit_0.Dock = DockStyle.Fill;
            btn_digit_0.Font = new Font("Segoe UI", 14.25F);
            btn_digit_0.Location = new Point(161, 356);
            btn_digit_0.Margin = new Padding(3, 2, 3, 2);
            btn_digit_0.Name = "btn_digit_0";
            btn_digit_0.Size = new Size(73, 61);
            btn_digit_0.TabIndex = 27;
            btn_digit_0.Text = "0";
            btn_digit_0.UseVisualStyleBackColor = false;
            // 
            // btn_digit_2
            // 
            btn_digit_2.BackColor = SystemColors.ButtonHighlight;
            btn_digit_2.Dock = DockStyle.Fill;
            btn_digit_2.Font = new Font("Segoe UI", 14.25F);
            btn_digit_2.Location = new Point(161, 297);
            btn_digit_2.Margin = new Padding(3, 2, 3, 2);
            btn_digit_2.Name = "btn_digit_2";
            btn_digit_2.Size = new Size(73, 55);
            btn_digit_2.TabIndex = 22;
            btn_digit_2.Text = "2";
            btn_digit_2.UseVisualStyleBackColor = false;
            // 
            // btn_digit_5
            // 
            btn_digit_5.BackColor = SystemColors.ButtonHighlight;
            btn_digit_5.Dock = DockStyle.Fill;
            btn_digit_5.Font = new Font("Segoe UI", 14.25F);
            btn_digit_5.Location = new Point(161, 238);
            btn_digit_5.Margin = new Padding(3, 2, 3, 2);
            btn_digit_5.Name = "btn_digit_5";
            btn_digit_5.Size = new Size(73, 55);
            btn_digit_5.TabIndex = 17;
            btn_digit_5.Text = "5";
            btn_digit_5.UseVisualStyleBackColor = false;
            // 
            // btn_digit_8
            // 
            btn_digit_8.BackColor = SystemColors.ButtonHighlight;
            btn_digit_8.Dock = DockStyle.Fill;
            btn_digit_8.Font = new Font("Segoe UI", 14.25F);
            btn_digit_8.Location = new Point(161, 179);
            btn_digit_8.Margin = new Padding(3, 2, 3, 2);
            btn_digit_8.Name = "btn_digit_8";
            btn_digit_8.Size = new Size(73, 55);
            btn_digit_8.TabIndex = 12;
            btn_digit_8.Text = "8";
            btn_digit_8.UseVisualStyleBackColor = false;
            // 
            // btn_digit_1
            // 
            btn_digit_1.BackColor = SystemColors.ButtonHighlight;
            btn_digit_1.Dock = DockStyle.Fill;
            btn_digit_1.Font = new Font("Segoe UI", 14.25F);
            btn_digit_1.Location = new Point(82, 297);
            btn_digit_1.Margin = new Padding(3, 2, 3, 2);
            btn_digit_1.Name = "btn_digit_1";
            btn_digit_1.Size = new Size(73, 55);
            btn_digit_1.TabIndex = 21;
            btn_digit_1.Text = "1";
            btn_digit_1.UseVisualStyleBackColor = false;
            // 
            // btn_digit_4
            // 
            btn_digit_4.BackColor = SystemColors.ButtonHighlight;
            btn_digit_4.Dock = DockStyle.Fill;
            btn_digit_4.Font = new Font("Segoe UI", 14.25F);
            btn_digit_4.Location = new Point(82, 238);
            btn_digit_4.Margin = new Padding(3, 2, 3, 2);
            btn_digit_4.Name = "btn_digit_4";
            btn_digit_4.Size = new Size(73, 55);
            btn_digit_4.TabIndex = 16;
            btn_digit_4.Text = "4";
            btn_digit_4.UseVisualStyleBackColor = false;
            // 
            // btn_digit_7
            // 
            btn_digit_7.BackColor = SystemColors.ButtonHighlight;
            btn_digit_7.Dock = DockStyle.Fill;
            btn_digit_7.Font = new Font("Segoe UI", 14.25F);
            btn_digit_7.Location = new Point(82, 179);
            btn_digit_7.Margin = new Padding(3, 2, 3, 2);
            btn_digit_7.Name = "btn_digit_7";
            btn_digit_7.Size = new Size(73, 55);
            btn_digit_7.TabIndex = 11;
            btn_digit_7.Text = "7";
            btn_digit_7.UseVisualStyleBackColor = false;
            // 
            // btn_plus_minus
            // 
            btn_plus_minus.BackColor = SystemColors.ButtonHighlight;
            btn_plus_minus.Dock = DockStyle.Fill;
            btn_plus_minus.Font = new Font("Segoe UI", 14.25F);
            btn_plus_minus.Location = new Point(82, 356);
            btn_plus_minus.Margin = new Padding(3, 2, 3, 2);
            btn_plus_minus.Name = "btn_plus_minus";
            btn_plus_minus.Size = new Size(73, 61);
            btn_plus_minus.TabIndex = 26;
            btn_plus_minus.Text = "+/-";
            btn_plus_minus.UseVisualStyleBackColor = false;
            // 
            // btn_nth_root
            // 
            btn_nth_root.BackColor = SystemColors.ButtonHighlight;
            btn_nth_root.Dock = DockStyle.Fill;
            btn_nth_root.Font = new Font("Microsoft Sans Serif", 14.25F);
            btn_nth_root.Location = new Point(3, 120);
            btn_nth_root.Margin = new Padding(3, 2, 3, 2);
            btn_nth_root.Name = "btn_nth_root";
            btn_nth_root.Size = new Size(73, 55);
            btn_nth_root.TabIndex = 8;
            btn_nth_root.Text = "²√x";
            btn_nth_root.UseVisualStyleBackColor = false;
            // 
            // btn_square
            // 
            btn_square.BackColor = SystemColors.ButtonHighlight;
            btn_square.Dock = DockStyle.Fill;
            btn_square.Font = new Font("Microsoft Sans Serif", 14.25F);
            btn_square.Location = new Point(3, 61);
            btn_square.Margin = new Padding(3, 2, 3, 2);
            btn_square.Name = "btn_square";
            btn_square.Size = new Size(73, 55);
            btn_square.TabIndex = 7;
            btn_square.Text = "x²";
            btn_square.UseVisualStyleBackColor = false;
            // 
            // btn_power
            // 
            btn_power.BackColor = SystemColors.ButtonHighlight;
            btn_power.Dock = DockStyle.Fill;
            btn_power.Font = new Font("Segoe UI", 14.25F);
            btn_power.Location = new Point(3, 179);
            btn_power.Margin = new Padding(3, 2, 3, 2);
            btn_power.Name = "btn_power";
            btn_power.Size = new Size(73, 55);
            btn_power.TabIndex = 34;
            btn_power.Text = "xʸ";
            btn_power.UseVisualStyleBackColor = false;
            // 
            // btn_power_10
            // 
            btn_power_10.BackColor = SystemColors.ButtonHighlight;
            btn_power_10.Dock = DockStyle.Fill;
            btn_power_10.Font = new Font("Segoe UI", 14.25F);
            btn_power_10.Location = new Point(3, 238);
            btn_power_10.Margin = new Padding(3, 2, 3, 2);
            btn_power_10.Name = "btn_power_10";
            btn_power_10.Size = new Size(73, 55);
            btn_power_10.TabIndex = 35;
            btn_power_10.Text = "10ˣ";
            btn_power_10.UseVisualStyleBackColor = false;
            // 
            // btn_log10
            // 
            btn_log10.BackColor = SystemColors.ButtonHighlight;
            btn_log10.Dock = DockStyle.Fill;
            btn_log10.Font = new Font("Segoe UI", 14.25F);
            btn_log10.Location = new Point(3, 297);
            btn_log10.Margin = new Padding(3, 2, 3, 2);
            btn_log10.Name = "btn_log10";
            btn_log10.Size = new Size(73, 55);
            btn_log10.TabIndex = 36;
            btn_log10.Text = "log";
            btn_log10.UseVisualStyleBackColor = false;
            // 
            // btn_ln
            // 
            btn_ln.BackColor = SystemColors.ButtonHighlight;
            btn_ln.Dock = DockStyle.Fill;
            btn_ln.Font = new Font("Segoe UI", 14.25F);
            btn_ln.Location = new Point(3, 356);
            btn_ln.Margin = new Padding(3, 2, 3, 2);
            btn_ln.Name = "btn_ln";
            btn_ln.Size = new Size(73, 61);
            btn_ln.TabIndex = 37;
            btn_ln.Text = "ln";
            btn_ln.UseVisualStyleBackColor = false;
            // 
            // button3
            // 
            button3.BackColor = SystemColors.ButtonHighlight;
            button3.Dock = DockStyle.Fill;
            button3.Font = new Font("Segoe UI", 14.25F);
            button3.Location = new Point(161, 120);
            button3.Margin = new Padding(3, 2, 3, 2);
            button3.Name = "button3";
            button3.Size = new Size(73, 55);
            button3.TabIndex = 39;
            button3.Text = ")";
            button3.UseVisualStyleBackColor = false;
            // 
            // button1
            // 
            button1.BackColor = SystemColors.ButtonHighlight;
            button1.Dock = DockStyle.Fill;
            button1.Font = new Font("Segoe UI", 14.25F);
            button1.Location = new Point(82, 120);
            button1.Margin = new Padding(3, 2, 3, 2);
            button1.Name = "button1";
            button1.Size = new Size(73, 55);
            button1.TabIndex = 38;
            button1.Text = "(";
            button1.UseVisualStyleBackColor = false;
            // 
            // lbl_expression
            // 
            lbl_expression.AutoSize = true;
            lbl_expression.BackColor = SystemColors.ButtonHighlight;
            lbl_expression.Dock = DockStyle.Fill;
            lbl_expression.Location = new Point(3, 99);
            lbl_expression.Name = "lbl_expression";
            lbl_expression.Size = new Size(94, 40);
            lbl_expression.TabIndex = 9;
            lbl_expression.Text = "label1";
            // 
            // lbl_result
            // 
            lbl_result.AutoSize = true;
            lbl_result.Dock = DockStyle.Top;
            lbl_result.Font = new Font("Segoe UI", 36F);
            lbl_result.Location = new Point(3, 3);
            lbl_result.Margin = new Padding(4, 0, 4, 0);
            lbl_result.Name = "lbl_result";
            lbl_result.Size = new Size(79, 96);
            lbl_result.TabIndex = 8;
            lbl_result.Text = "0";
            lbl_result.TextAlign = ContentAlignment.MiddleRight;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(lbl_fx_status);
            tabPage2.Controls.Add(lbl_fx_display);
            tabPage2.Controls.Add(tableLayoutPanel2);
            tabPage2.Location = new Point(4, 34);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(402, 623);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "tabPage2";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // lbl_fx_status
            // 
            lbl_fx_status.AutoSize = true;
            lbl_fx_status.BackColor = SystemColors.ButtonHighlight;
            lbl_fx_status.Dock = DockStyle.Fill;
            lbl_fx_status.Location = new Point(3, 99);
            lbl_fx_status.Name = "lbl_fx_status";
            lbl_fx_status.Size = new Size(94, 40);
            lbl_fx_status.TabIndex = 15;
            lbl_fx_status.Text = "label1";
            // 
            // lbl_fx_display
            // 
            lbl_fx_display.AutoSize = true;
            lbl_fx_display.Dock = DockStyle.Top;
            lbl_fx_display.Font = new Font("Segoe UI", 36F);
            lbl_fx_display.Location = new Point(3, 3);
            lbl_fx_display.Margin = new Padding(4, 0, 4, 0);
            lbl_fx_display.Name = "lbl_fx_display";
            lbl_fx_display.Size = new Size(79, 96);
            lbl_fx_display.TabIndex = 14;
            lbl_fx_display.Text = "0";
            lbl_fx_display.TextAlign = ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 3;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333359F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333359F));
            tableLayoutPanel2.Controls.Add(cmb_fx_from, 0, 0);
            tableLayoutPanel2.Controls.Add(button2, 0, 1);
            tableLayoutPanel2.Controls.Add(btn_fx_digit_7, 0, 2);
            tableLayoutPanel2.Controls.Add(btn_fx_digit_4, 0, 3);
            tableLayoutPanel2.Controls.Add(btn_fx_digit_1, 0, 4);
            tableLayoutPanel2.Controls.Add(btn_fx_decimal, 0, 5);
            tableLayoutPanel2.Controls.Add(btn_fx_convert, 1, 0);
            tableLayoutPanel2.Controls.Add(btn_fx_paste, 1, 1);
            tableLayoutPanel2.Controls.Add(btn_fx_digit_8, 1, 2);
            tableLayoutPanel2.Controls.Add(btn_fx_digit_5, 1, 3);
            tableLayoutPanel2.Controls.Add(btn_fx_digit_2, 1, 4);
            tableLayoutPanel2.Controls.Add(btn_fx_digit_0, 1, 5);
            tableLayoutPanel2.Controls.Add(cmb_fx_to, 2, 0);
            tableLayoutPanel2.Controls.Add(btn_fx_backspace, 2, 1);
            tableLayoutPanel2.Controls.Add(btn_fx_digit_9, 2, 2);
            tableLayoutPanel2.Controls.Add(btn_fx_digit_6, 2, 3);
            tableLayoutPanel2.Controls.Add(btn_fx_digit_3, 2, 4);
            tableLayoutPanel2.Controls.Add(btn_fx_clear_all, 2, 5);
            tableLayoutPanel2.Dock = DockStyle.Bottom;
            tableLayoutPanel2.Location = new Point(3, 298);
            tableLayoutPanel2.Margin = new Padding(3, 2, 3, 2);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 6;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6666641F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6666679F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6666679F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6666679F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6666679F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6666679F));
            tableLayoutPanel2.Size = new Size(396, 322);
            tableLayoutPanel2.TabIndex = 11;
            // 
            // cmb_fx_from
            // 
            cmb_fx_from.FormattingEnabled = true;
            cmb_fx_from.Items.AddRange(new object[] { "USD", "EUR", "LBP" });
            cmb_fx_from.Location = new Point(3, 3);
            cmb_fx_from.Name = "cmb_fx_from";
            cmb_fx_from.Size = new Size(121, 48);
            cmb_fx_from.TabIndex = 16;
            cmb_fx_from.Text = "From";
            // 
            // button2
            // 
            button2.BackColor = SystemColors.ButtonHighlight;
            button2.Dock = DockStyle.Fill;
            button2.Font = new Font("Segoe UI", 14.25F);
            button2.Location = new Point(3, 55);
            button2.Margin = new Padding(3, 2, 3, 2);
            button2.Name = "button2";
            button2.Size = new Size(125, 49);
            button2.TabIndex = 1;
            button2.Text = "Copy";
            button2.UseVisualStyleBackColor = false;
            // 
            // btn_fx_digit_7
            // 
            btn_fx_digit_7.BackColor = SystemColors.ButtonHighlight;
            btn_fx_digit_7.Dock = DockStyle.Fill;
            btn_fx_digit_7.Font = new Font("Segoe UI", 14.25F);
            btn_fx_digit_7.Location = new Point(3, 108);
            btn_fx_digit_7.Margin = new Padding(3, 2, 3, 2);
            btn_fx_digit_7.Name = "btn_fx_digit_7";
            btn_fx_digit_7.Size = new Size(125, 49);
            btn_fx_digit_7.TabIndex = 11;
            btn_fx_digit_7.Text = "7";
            btn_fx_digit_7.UseVisualStyleBackColor = false;
            // 
            // btn_fx_digit_4
            // 
            btn_fx_digit_4.BackColor = SystemColors.ButtonHighlight;
            btn_fx_digit_4.Dock = DockStyle.Fill;
            btn_fx_digit_4.Font = new Font("Segoe UI", 14.25F);
            btn_fx_digit_4.Location = new Point(3, 161);
            btn_fx_digit_4.Margin = new Padding(3, 2, 3, 2);
            btn_fx_digit_4.Name = "btn_fx_digit_4";
            btn_fx_digit_4.Size = new Size(125, 49);
            btn_fx_digit_4.TabIndex = 16;
            btn_fx_digit_4.Text = "4";
            btn_fx_digit_4.UseVisualStyleBackColor = false;
            // 
            // btn_fx_digit_1
            // 
            btn_fx_digit_1.BackColor = SystemColors.ButtonHighlight;
            btn_fx_digit_1.Dock = DockStyle.Fill;
            btn_fx_digit_1.Font = new Font("Segoe UI", 14.25F);
            btn_fx_digit_1.Location = new Point(3, 214);
            btn_fx_digit_1.Margin = new Padding(3, 2, 3, 2);
            btn_fx_digit_1.Name = "btn_fx_digit_1";
            btn_fx_digit_1.Size = new Size(125, 49);
            btn_fx_digit_1.TabIndex = 21;
            btn_fx_digit_1.Text = "1";
            btn_fx_digit_1.UseVisualStyleBackColor = false;
            // 
            // btn_fx_decimal
            // 
            btn_fx_decimal.BackColor = SystemColors.ButtonHighlight;
            btn_fx_decimal.Dock = DockStyle.Fill;
            btn_fx_decimal.Font = new Font("Segoe UI", 14.25F);
            btn_fx_decimal.Location = new Point(3, 267);
            btn_fx_decimal.Margin = new Padding(3, 2, 3, 2);
            btn_fx_decimal.Name = "btn_fx_decimal";
            btn_fx_decimal.Size = new Size(125, 53);
            btn_fx_decimal.TabIndex = 28;
            btn_fx_decimal.Text = ".";
            btn_fx_decimal.UseVisualStyleBackColor = false;
            // 
            // btn_fx_convert
            // 
            btn_fx_convert.BackColor = SystemColors.ButtonHighlight;
            btn_fx_convert.Dock = DockStyle.Fill;
            btn_fx_convert.Font = new Font("Segoe UI", 14.25F);
            btn_fx_convert.Location = new Point(134, 2);
            btn_fx_convert.Margin = new Padding(3, 2, 3, 2);
            btn_fx_convert.Name = "btn_fx_convert";
            btn_fx_convert.Size = new Size(126, 49);
            btn_fx_convert.TabIndex = 3;
            btn_fx_convert.Text = "Convert";
            btn_fx_convert.UseVisualStyleBackColor = false;
            // 
            // btn_fx_paste
            // 
            btn_fx_paste.BackColor = SystemColors.ButtonHighlight;
            btn_fx_paste.Dock = DockStyle.Fill;
            btn_fx_paste.Font = new Font("Segoe UI", 14.25F);
            btn_fx_paste.Location = new Point(134, 55);
            btn_fx_paste.Margin = new Padding(3, 2, 3, 2);
            btn_fx_paste.Name = "btn_fx_paste";
            btn_fx_paste.Size = new Size(126, 49);
            btn_fx_paste.TabIndex = 34;
            btn_fx_paste.Text = "Paste";
            btn_fx_paste.UseVisualStyleBackColor = false;
            // 
            // btn_fx_digit_8
            // 
            btn_fx_digit_8.BackColor = SystemColors.ButtonHighlight;
            btn_fx_digit_8.Dock = DockStyle.Fill;
            btn_fx_digit_8.Font = new Font("Segoe UI", 14.25F);
            btn_fx_digit_8.Location = new Point(134, 108);
            btn_fx_digit_8.Margin = new Padding(3, 2, 3, 2);
            btn_fx_digit_8.Name = "btn_fx_digit_8";
            btn_fx_digit_8.Size = new Size(126, 49);
            btn_fx_digit_8.TabIndex = 12;
            btn_fx_digit_8.Text = "8";
            btn_fx_digit_8.UseVisualStyleBackColor = false;
            // 
            // btn_fx_digit_5
            // 
            btn_fx_digit_5.BackColor = SystemColors.ButtonHighlight;
            btn_fx_digit_5.Dock = DockStyle.Fill;
            btn_fx_digit_5.Font = new Font("Segoe UI", 14.25F);
            btn_fx_digit_5.Location = new Point(134, 161);
            btn_fx_digit_5.Margin = new Padding(3, 2, 3, 2);
            btn_fx_digit_5.Name = "btn_fx_digit_5";
            btn_fx_digit_5.Size = new Size(126, 49);
            btn_fx_digit_5.TabIndex = 17;
            btn_fx_digit_5.Text = "5";
            btn_fx_digit_5.UseVisualStyleBackColor = false;
            // 
            // btn_fx_digit_2
            // 
            btn_fx_digit_2.BackColor = SystemColors.ButtonHighlight;
            btn_fx_digit_2.Dock = DockStyle.Fill;
            btn_fx_digit_2.Font = new Font("Segoe UI", 14.25F);
            btn_fx_digit_2.Location = new Point(134, 214);
            btn_fx_digit_2.Margin = new Padding(3, 2, 3, 2);
            btn_fx_digit_2.Name = "btn_fx_digit_2";
            btn_fx_digit_2.Size = new Size(126, 49);
            btn_fx_digit_2.TabIndex = 22;
            btn_fx_digit_2.Text = "2";
            btn_fx_digit_2.UseVisualStyleBackColor = false;
            // 
            // btn_fx_digit_0
            // 
            btn_fx_digit_0.BackColor = SystemColors.ButtonHighlight;
            btn_fx_digit_0.Dock = DockStyle.Fill;
            btn_fx_digit_0.Font = new Font("Segoe UI", 14.25F);
            btn_fx_digit_0.Location = new Point(134, 267);
            btn_fx_digit_0.Margin = new Padding(3, 2, 3, 2);
            btn_fx_digit_0.Name = "btn_fx_digit_0";
            btn_fx_digit_0.Size = new Size(126, 53);
            btn_fx_digit_0.TabIndex = 27;
            btn_fx_digit_0.Text = "0";
            btn_fx_digit_0.UseVisualStyleBackColor = false;
            // 
            // cmb_fx_to
            // 
            cmb_fx_to.FormattingEnabled = true;
            cmb_fx_to.Items.AddRange(new object[] { "USD", "EUR", "LBP" });
            cmb_fx_to.Location = new Point(266, 3);
            cmb_fx_to.Name = "cmb_fx_to";
            cmb_fx_to.Size = new Size(121, 48);
            cmb_fx_to.TabIndex = 35;
            cmb_fx_to.Text = "To";
            // 
            // btn_fx_backspace
            // 
            btn_fx_backspace.BackColor = SystemColors.ButtonHighlight;
            btn_fx_backspace.Dock = DockStyle.Fill;
            btn_fx_backspace.Font = new Font("Segoe UI", 14.25F);
            btn_fx_backspace.Location = new Point(266, 55);
            btn_fx_backspace.Margin = new Padding(3, 2, 3, 2);
            btn_fx_backspace.Name = "btn_fx_backspace";
            btn_fx_backspace.Size = new Size(127, 49);
            btn_fx_backspace.TabIndex = 31;
            btn_fx_backspace.Text = "⌫";
            btn_fx_backspace.UseVisualStyleBackColor = false;
            // 
            // btn_fx_digit_9
            // 
            btn_fx_digit_9.BackColor = SystemColors.ButtonHighlight;
            btn_fx_digit_9.Dock = DockStyle.Fill;
            btn_fx_digit_9.Font = new Font("Segoe UI", 14.25F);
            btn_fx_digit_9.Location = new Point(266, 108);
            btn_fx_digit_9.Margin = new Padding(3, 2, 3, 2);
            btn_fx_digit_9.Name = "btn_fx_digit_9";
            btn_fx_digit_9.Size = new Size(127, 49);
            btn_fx_digit_9.TabIndex = 13;
            btn_fx_digit_9.Text = "9";
            btn_fx_digit_9.UseVisualStyleBackColor = false;
            // 
            // btn_fx_digit_6
            // 
            btn_fx_digit_6.BackColor = SystemColors.ButtonHighlight;
            btn_fx_digit_6.Dock = DockStyle.Fill;
            btn_fx_digit_6.Font = new Font("Segoe UI", 14.25F);
            btn_fx_digit_6.Location = new Point(266, 161);
            btn_fx_digit_6.Margin = new Padding(3, 2, 3, 2);
            btn_fx_digit_6.Name = "btn_fx_digit_6";
            btn_fx_digit_6.Size = new Size(127, 49);
            btn_fx_digit_6.TabIndex = 18;
            btn_fx_digit_6.Text = "6";
            btn_fx_digit_6.UseVisualStyleBackColor = false;
            // 
            // btn_fx_digit_3
            // 
            btn_fx_digit_3.BackColor = SystemColors.ButtonHighlight;
            btn_fx_digit_3.Dock = DockStyle.Fill;
            btn_fx_digit_3.Font = new Font("Segoe UI", 14.25F);
            btn_fx_digit_3.Location = new Point(266, 214);
            btn_fx_digit_3.Margin = new Padding(3, 2, 3, 2);
            btn_fx_digit_3.Name = "btn_fx_digit_3";
            btn_fx_digit_3.Size = new Size(127, 49);
            btn_fx_digit_3.TabIndex = 23;
            btn_fx_digit_3.Text = "3";
            btn_fx_digit_3.UseVisualStyleBackColor = false;
            // 
            // btn_fx_clear_all
            // 
            btn_fx_clear_all.BackColor = SystemColors.MenuHighlight;
            btn_fx_clear_all.Cursor = Cursors.Hand;
            btn_fx_clear_all.Dock = DockStyle.Fill;
            btn_fx_clear_all.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btn_fx_clear_all.ForeColor = SystemColors.ActiveCaptionText;
            btn_fx_clear_all.Location = new Point(266, 267);
            btn_fx_clear_all.Margin = new Padding(3, 2, 3, 2);
            btn_fx_clear_all.Name = "btn_fx_clear_all";
            btn_fx_clear_all.Size = new Size(127, 53);
            btn_fx_clear_all.TabIndex = 29;
            btn_fx_clear_all.Text = "C";
            btn_fx_clear_all.UseVisualStyleBackColor = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(16F, 40F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(840, 661);
            Controls.Add(tabControl1);
            Font = new Font("Segoe UI", 14.25F);
            ForeColor = SystemColors.ActiveCaptionText;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(3, 2, 3, 2);
            Name = "Form1";
            Text = "Form1";
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TableLayoutPanel tableLayoutPanel1;
        private Button btn_equals;
        private Button btn_decimal;
        private Button btn_digit_0;
        private Button btn_plus_minus;
        private Button btn_backspace;
        private Button btn_add;
        private Button btn_divide;
        private Button btn_digit_3;
        private Button btn_multiply;
        private Button btn_digit_2;
        private Button btn_subtract;
        private Button btn_digit_1;
        private Button btn_digit_6;
        private Button btn_digit_5;
        private Button btn_digit_4;
        private Button btn_digit_9;
        private Button btn_digit_8;
        private Button btn_digit_7;
        private Button btn_nth_root;
        private Button btn_square;
        private Button btn_reciprocal;
        private Label lbl_expression;
        private Label lbl_result;
        private TabPage tabPage2;
        private TableLayoutPanel tableLayoutPanel2;
        private Button button2;
        private Button btn_fx_decimal;
        private Button btn_fx_convert;
        private Button btn_fx_digit_0;
        private Button btn_fx_digit_3;
        private Button btn_fx_digit_2;
        private Button btn_fx_digit_1;
        private Button btn_fx_digit_6;
        private Button btn_fx_digit_5;
        private Button btn_fx_digit_4;
        private Button btn_fx_digit_9;
        private Button btn_fx_digit_8;
        private Button btn_fx_digit_7;
        private Button btn_fx_backspace;
        private Button btn_fx_clear_all;
        private Button btn_exp;
        private Button btn_mod;
        private Button btn_factorial;
        private Button btn_abs;
        private Button btn_power;
        private Button btn_power_10;
        private Button btn_log10;
        private Button btn_ln;
        private Button btn_e;
        private Button btn_pi;
        private Button btn_clear_all;
        private Button btn_second;
        private Label lbl_fx_display;
        private Button btn_fx_paste;
        private Label lbl_fx_status;
        private ComboBox cmb_fx_from;
        private ComboBox cmb_fx_to;
        private Button button3;
        private Button button1;
    }
}
