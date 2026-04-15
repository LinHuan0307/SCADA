namespace WinFormsApp1.Forms
{
    partial class SNInputForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            uiTableLayoutPanel1 = new Sunny.UI.UITableLayoutPanel();
            uiTableLayoutPanel3 = new Sunny.UI.UITableLayoutPanel();
            Btn_Clear = new Sunny.UI.UIButton();
            Btn_Subject = new Sunny.UI.UIButton();
            Tb_InputSN = new Sunny.UI.UITextBox();
            uiTableLayoutPanel1.SuspendLayout();
            uiTableLayoutPanel3.SuspendLayout();
            SuspendLayout();
            // 
            // uiTableLayoutPanel1
            // 
            uiTableLayoutPanel1.ColumnCount = 1;
            uiTableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            uiTableLayoutPanel1.Controls.Add(uiTableLayoutPanel3, 0, 1);
            uiTableLayoutPanel1.Controls.Add(Tb_InputSN, 0, 0);
            uiTableLayoutPanel1.Dock = DockStyle.Fill;
            uiTableLayoutPanel1.Location = new Point(0, 35);
            uiTableLayoutPanel1.Name = "uiTableLayoutPanel1";
            uiTableLayoutPanel1.RowCount = 2;
            uiTableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            uiTableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            uiTableLayoutPanel1.Size = new Size(800, 154);
            uiTableLayoutPanel1.TabIndex = 1;
            uiTableLayoutPanel1.TagString = null;
            // 
            // uiTableLayoutPanel3
            // 
            uiTableLayoutPanel3.ColumnCount = 2;
            uiTableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            uiTableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            uiTableLayoutPanel3.Controls.Add(Btn_Clear, 1, 0);
            uiTableLayoutPanel3.Controls.Add(Btn_Subject, 0, 0);
            uiTableLayoutPanel3.Dock = DockStyle.Fill;
            uiTableLayoutPanel3.Location = new Point(3, 80);
            uiTableLayoutPanel3.Name = "uiTableLayoutPanel3";
            uiTableLayoutPanel3.RowCount = 1;
            uiTableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            uiTableLayoutPanel3.Size = new Size(794, 71);
            uiTableLayoutPanel3.TabIndex = 3;
            uiTableLayoutPanel3.TagString = null;
            // 
            // Btn_Clear
            // 
            Btn_Clear.Cursor = Cursors.Hand;
            Btn_Clear.Dock = DockStyle.Fill;
            Btn_Clear.FillColor = Color.FromArgb(220, 155, 40);
            Btn_Clear.FillColor2 = Color.FromArgb(220, 155, 40);
            Btn_Clear.FillHoverColor = Color.FromArgb(227, 175, 83);
            Btn_Clear.FillPressColor = Color.FromArgb(176, 124, 32);
            Btn_Clear.FillSelectedColor = Color.FromArgb(176, 124, 32);
            Btn_Clear.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            Btn_Clear.LightColor = Color.FromArgb(253, 249, 241);
            Btn_Clear.Location = new Point(400, 3);
            Btn_Clear.MinimumSize = new Size(1, 1);
            Btn_Clear.Name = "Btn_Clear";
            Btn_Clear.RectColor = Color.FromArgb(220, 155, 40);
            Btn_Clear.RectHoverColor = Color.FromArgb(227, 175, 83);
            Btn_Clear.RectPressColor = Color.FromArgb(176, 124, 32);
            Btn_Clear.RectSelectedColor = Color.FromArgb(176, 124, 32);
            Btn_Clear.Size = new Size(391, 65);
            Btn_Clear.Style = Sunny.UI.UIStyle.Custom;
            Btn_Clear.TabIndex = 1;
            Btn_Clear.Text = "清空输入";
            Btn_Clear.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            Btn_Clear.Click += Btn_Clear_Click;
            // 
            // Btn_Subject
            // 
            Btn_Subject.Cursor = Cursors.Hand;
            Btn_Subject.Dock = DockStyle.Fill;
            Btn_Subject.FillColor = Color.FromArgb(110, 190, 40);
            Btn_Subject.FillColor2 = Color.FromArgb(110, 190, 40);
            Btn_Subject.FillHoverColor = Color.FromArgb(139, 203, 83);
            Btn_Subject.FillPressColor = Color.FromArgb(88, 152, 32);
            Btn_Subject.FillSelectedColor = Color.FromArgb(88, 152, 32);
            Btn_Subject.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            Btn_Subject.LightColor = Color.FromArgb(245, 251, 241);
            Btn_Subject.Location = new Point(3, 3);
            Btn_Subject.MinimumSize = new Size(1, 1);
            Btn_Subject.Name = "Btn_Subject";
            Btn_Subject.RectColor = Color.FromArgb(110, 190, 40);
            Btn_Subject.RectHoverColor = Color.FromArgb(139, 203, 83);
            Btn_Subject.RectPressColor = Color.FromArgb(88, 152, 32);
            Btn_Subject.RectSelectedColor = Color.FromArgb(88, 152, 32);
            Btn_Subject.Size = new Size(391, 65);
            Btn_Subject.Style = Sunny.UI.UIStyle.Custom;
            Btn_Subject.TabIndex = 0;
            Btn_Subject.Text = "确认";
            Btn_Subject.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            Btn_Subject.Click += Btn_Subject_Click;
            // 
            // Tb_InputSN
            // 
            Tb_InputSN.Cursor = Cursors.IBeam;
            Tb_InputSN.Dock = DockStyle.Fill;
            Tb_InputSN.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            Tb_InputSN.Location = new Point(4, 5);
            Tb_InputSN.Margin = new Padding(4, 5, 4, 5);
            Tb_InputSN.MinimumSize = new Size(1, 16);
            Tb_InputSN.Name = "Tb_InputSN";
            Tb_InputSN.Padding = new Padding(5);
            Tb_InputSN.ShowText = false;
            Tb_InputSN.Size = new Size(792, 67);
            Tb_InputSN.TabIndex = 1;
            Tb_InputSN.TextAlignment = ContentAlignment.MiddleLeft;
            Tb_InputSN.Watermark = "";
            // 
            // SNInputForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(800, 189);
            Controls.Add(uiTableLayoutPanel1);
            Name = "SNInputForm";
            Text = "请输入SN号";
            ZoomScaleRect = new Rectangle(15, 15, 800, 450);
            FormClosing += ScadaClosedSNInputForm_FormClosing;
            Load += ScadaClosedSNInputForm_Load;
            uiTableLayoutPanel1.ResumeLayout(false);
            uiTableLayoutPanel3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel1;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel3;
        private Sunny.UI.UIButton Btn_Clear;
        private Sunny.UI.UIButton Btn_Subject;
        public Sunny.UI.UITextBox Tb_InputSN;
    }
}