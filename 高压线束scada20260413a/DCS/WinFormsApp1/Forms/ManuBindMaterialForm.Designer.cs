namespace WinFormsApp1.Forms
{
    partial class ManuBindMaterialForm
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
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle7 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle8 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle9 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle10 = new DataGridViewCellStyle();
            uiTableLayoutPanel2 = new Sunny.UI.UITableLayoutPanel();
            uiTableLayoutPanel1 = new Sunny.UI.UITableLayoutPanel();
            uiTableLayoutPanel4 = new Sunny.UI.UITableLayoutPanel();
            Btn_Subject = new Sunny.UI.UIButton();
            Btn_Cancel = new Sunny.UI.UIButton();
            Dgv_MaterialCode = new Sunny.UI.UIDataGridView();
            uiTableLayoutPanel3 = new Sunny.UI.UITableLayoutPanel();
            Tb_SnInput = new Sunny.UI.UITextBox();
            uiLabel1 = new Sunny.UI.UILabel();
            uiTableLayoutPanel1.SuspendLayout();
            uiTableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Dgv_MaterialCode).BeginInit();
            uiTableLayoutPanel3.SuspendLayout();
            SuspendLayout();
            // 
            // uiTableLayoutPanel2
            // 
            uiTableLayoutPanel2.ColumnCount = 2;
            uiTableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            uiTableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            uiTableLayoutPanel2.Dock = DockStyle.Fill;
            uiTableLayoutPanel2.Location = new Point(0, 0);
            uiTableLayoutPanel2.Name = "uiTableLayoutPanel2";
            uiTableLayoutPanel2.RowCount = 1;
            uiTableLayoutPanel2.Size = new Size(200, 100);
            uiTableLayoutPanel2.TabIndex = 0;
            uiTableLayoutPanel2.TagString = null;
            // 
            // uiTableLayoutPanel1
            // 
            uiTableLayoutPanel1.ColumnCount = 1;
            uiTableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            uiTableLayoutPanel1.Controls.Add(uiTableLayoutPanel4, 0, 2);
            uiTableLayoutPanel1.Controls.Add(Dgv_MaterialCode, 0, 1);
            uiTableLayoutPanel1.Controls.Add(uiTableLayoutPanel3, 0, 0);
            uiTableLayoutPanel1.Dock = DockStyle.Fill;
            uiTableLayoutPanel1.Location = new Point(0, 35);
            uiTableLayoutPanel1.Name = "uiTableLayoutPanel1";
            uiTableLayoutPanel1.RowCount = 3;
            uiTableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            uiTableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            uiTableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            uiTableLayoutPanel1.Size = new Size(800, 415);
            uiTableLayoutPanel1.TabIndex = 0;
            uiTableLayoutPanel1.TagString = null;
            // 
            // uiTableLayoutPanel4
            // 
            uiTableLayoutPanel4.ColumnCount = 2;
            uiTableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            uiTableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            uiTableLayoutPanel4.Controls.Add(Btn_Subject, 0, 0);
            uiTableLayoutPanel4.Controls.Add(Btn_Cancel, 1, 0);
            uiTableLayoutPanel4.Dock = DockStyle.Fill;
            uiTableLayoutPanel4.Location = new Point(3, 318);
            uiTableLayoutPanel4.Name = "uiTableLayoutPanel4";
            uiTableLayoutPanel4.RowCount = 1;
            uiTableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            uiTableLayoutPanel4.Size = new Size(794, 94);
            uiTableLayoutPanel4.TabIndex = 2;
            uiTableLayoutPanel4.TagString = null;
            // 
            // Btn_Subject
            // 
            Btn_Subject.Dock = DockStyle.Fill;
            Btn_Subject.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            Btn_Subject.Location = new Point(3, 3);
            Btn_Subject.MinimumSize = new Size(1, 1);
            Btn_Subject.Name = "Btn_Subject";
            Btn_Subject.Size = new Size(391, 88);
            Btn_Subject.TabIndex = 1;
            Btn_Subject.Text = "确定";
            Btn_Subject.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            Btn_Subject.Click += Btn_Subject_Click;
            // 
            // Btn_Cancel
            // 
            Btn_Cancel.Dock = DockStyle.Fill;
            Btn_Cancel.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            Btn_Cancel.Location = new Point(400, 3);
            Btn_Cancel.MinimumSize = new Size(1, 1);
            Btn_Cancel.Name = "Btn_Cancel";
            Btn_Cancel.Size = new Size(391, 88);
            Btn_Cancel.TabIndex = 0;
            Btn_Cancel.Text = "取消";
            Btn_Cancel.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            Btn_Cancel.Click += Btn_Cancel_Click;
            // 
            // Dgv_MaterialCode
            // 
            dataGridViewCellStyle6.BackColor = Color.FromArgb(235, 243, 255);
            Dgv_MaterialCode.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle6;
            Dgv_MaterialCode.BackgroundColor = Color.White;
            Dgv_MaterialCode.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle7.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.BackColor = Color.FromArgb(80, 160, 255);
            dataGridViewCellStyle7.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            dataGridViewCellStyle7.ForeColor = Color.White;
            dataGridViewCellStyle7.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = DataGridViewTriState.True;
            Dgv_MaterialCode.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            Dgv_MaterialCode.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle8.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = SystemColors.Window;
            dataGridViewCellStyle8.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            dataGridViewCellStyle8.ForeColor = Color.FromArgb(48, 48, 48);
            dataGridViewCellStyle8.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = DataGridViewTriState.False;
            Dgv_MaterialCode.DefaultCellStyle = dataGridViewCellStyle8;
            Dgv_MaterialCode.Dock = DockStyle.Fill;
            Dgv_MaterialCode.EnableHeadersVisualStyles = false;
            Dgv_MaterialCode.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            Dgv_MaterialCode.GridColor = Color.FromArgb(80, 160, 255);
            Dgv_MaterialCode.Location = new Point(3, 103);
            Dgv_MaterialCode.Name = "Dgv_MaterialCode";
            Dgv_MaterialCode.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle9.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = Color.FromArgb(235, 243, 255);
            dataGridViewCellStyle9.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            dataGridViewCellStyle9.ForeColor = Color.FromArgb(48, 48, 48);
            dataGridViewCellStyle9.SelectionBackColor = Color.FromArgb(80, 160, 255);
            dataGridViewCellStyle9.SelectionForeColor = Color.White;
            dataGridViewCellStyle9.WrapMode = DataGridViewTriState.True;
            Dgv_MaterialCode.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            Dgv_MaterialCode.RowHeadersWidth = 51;
            dataGridViewCellStyle10.BackColor = Color.White;
            dataGridViewCellStyle10.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            Dgv_MaterialCode.RowsDefaultCellStyle = dataGridViewCellStyle10;
            Dgv_MaterialCode.SelectedIndex = -1;
            Dgv_MaterialCode.Size = new Size(794, 209);
            Dgv_MaterialCode.StripeOddColor = Color.FromArgb(235, 243, 255);
            Dgv_MaterialCode.TabIndex = 0;
            // 
            // uiTableLayoutPanel3
            // 
            uiTableLayoutPanel3.ColumnCount = 2;
            uiTableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
            uiTableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            uiTableLayoutPanel3.Controls.Add(Tb_SnInput, 1, 0);
            uiTableLayoutPanel3.Controls.Add(uiLabel1, 0, 0);
            uiTableLayoutPanel3.Dock = DockStyle.Fill;
            uiTableLayoutPanel3.Location = new Point(3, 3);
            uiTableLayoutPanel3.Name = "uiTableLayoutPanel3";
            uiTableLayoutPanel3.RowCount = 1;
            uiTableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            uiTableLayoutPanel3.Size = new Size(794, 94);
            uiTableLayoutPanel3.TabIndex = 1;
            uiTableLayoutPanel3.TagString = null;
            // 
            // Tb_SnInput
            // 
            Tb_SnInput.Dock = DockStyle.Fill;
            Tb_SnInput.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            Tb_SnInput.Location = new Point(104, 5);
            Tb_SnInput.Margin = new Padding(4, 5, 4, 5);
            Tb_SnInput.MinimumSize = new Size(1, 16);
            Tb_SnInput.Name = "Tb_SnInput";
            Tb_SnInput.Padding = new Padding(5);
            Tb_SnInput.ShowText = false;
            Tb_SnInput.Size = new Size(686, 84);
            Tb_SnInput.TabIndex = 1;
            Tb_SnInput.TextAlignment = ContentAlignment.MiddleLeft;
            Tb_SnInput.Watermark = "";
            // 
            // uiLabel1
            // 
            uiLabel1.Dock = DockStyle.Fill;
            uiLabel1.Font = new Font("宋体", 18F);
            uiLabel1.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel1.Location = new Point(3, 0);
            uiLabel1.Name = "uiLabel1";
            uiLabel1.Size = new Size(94, 94);
            uiLabel1.TabIndex = 2;
            uiLabel1.Text = "物料Sn号";
            uiLabel1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // ManuBindMaterialForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(238, 251, 250);
            ClientSize = new Size(800, 450);
            ControlBoxFillHoverColor = Color.FromArgb(51, 203, 189);
            Controls.Add(uiTableLayoutPanel1);
            Name = "ManuBindMaterialForm";
            RectColor = Color.FromArgb(0, 190, 172);
            Style = Sunny.UI.UIStyle.Custom;
            Text = "手动绑定物料窗口";
            TitleColor = Color.FromArgb(0, 190, 172);
            ZoomScaleRect = new Rectangle(19, 19, 800, 450);
            Load += ManuBindMaterialForm_Load;
            uiTableLayoutPanel1.ResumeLayout(false);
            uiTableLayoutPanel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)Dgv_MaterialCode).EndInit();
            uiTableLayoutPanel3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel2;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel1;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel4;
        private Sunny.UI.UIDataGridView Dgv_MaterialCode;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel3;
        private Sunny.UI.UIButton Btn_Subject;
        private Sunny.UI.UIButton Btn_Cancel;
        private Sunny.UI.UITextBox Tb_SnInput;
        private Sunny.UI.UILabel uiLabel1;
    }
}