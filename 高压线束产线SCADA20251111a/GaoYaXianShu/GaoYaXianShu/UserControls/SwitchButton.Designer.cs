namespace GaoYaXianShu.UserControls
{
    partial class SwitchButton
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.uiTableLayoutPanel1 = new Sunny.UI.UITableLayoutPanel();
            this.Lb_InStation = new Sunny.UI.UILabel();
            this.Sw_kaiguan = new Sunny.UI.UISwitch();
            this.uiTableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiTableLayoutPanel1
            // 
            this.uiTableLayoutPanel1.ColumnCount = 2;
            this.uiTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65.03198F));
            this.uiTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34.96802F));
            this.uiTableLayoutPanel1.Controls.Add(this.Lb_InStation, 0, 0);
            this.uiTableLayoutPanel1.Controls.Add(this.Sw_kaiguan, 1, 0);
            this.uiTableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.uiTableLayoutPanel1.Name = "uiTableLayoutPanel1";
            this.uiTableLayoutPanel1.RowCount = 1;
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel1.Size = new System.Drawing.Size(600, 80);
            this.uiTableLayoutPanel1.Style = Sunny.UI.UIStyle.Custom;
            this.uiTableLayoutPanel1.TabIndex = 2;
            this.uiTableLayoutPanel1.TagString = null;
            // 
            // Lb_InStation
            // 
            this.Lb_InStation.BackColor = System.Drawing.Color.Turquoise;
            this.Lb_InStation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Lb_InStation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Lb_InStation.Font = new System.Drawing.Font("宋体", 18F);
            this.Lb_InStation.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.Lb_InStation.Location = new System.Drawing.Point(3, 0);
            this.Lb_InStation.Name = "Lb_InStation";
            this.Lb_InStation.Size = new System.Drawing.Size(384, 80);
            this.Lb_InStation.Style = Sunny.UI.UIStyle.Custom;
            this.Lb_InStation.TabIndex = 5;
            this.Lb_InStation.Text = "名字";
            this.Lb_InStation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Sw_kaiguan
            // 
            this.Sw_kaiguan.ActiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(190)))), ((int)(((byte)(172)))));
            this.Sw_kaiguan.BackColor = System.Drawing.Color.Turquoise;
            this.Sw_kaiguan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Sw_kaiguan.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Sw_kaiguan.Location = new System.Drawing.Point(393, 3);
            this.Sw_kaiguan.MinimumSize = new System.Drawing.Size(1, 1);
            this.Sw_kaiguan.Name = "Sw_kaiguan";
            this.Sw_kaiguan.Size = new System.Drawing.Size(204, 74);
            this.Sw_kaiguan.Style = Sunny.UI.UIStyle.Custom;
            this.Sw_kaiguan.TabIndex = 1;
            this.Sw_kaiguan.Text = "uiSwitch1";
            this.Sw_kaiguan.ValueChanged += new Sunny.UI.UISwitch.OnValueChanged(this.Sw_kaiguan_ValueChanged);
            // 
            // SwitchButton
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.uiTableLayoutPanel1);
            this.Name = "SwitchButton";
            this.Size = new System.Drawing.Size(600, 80);
            this.Style = Sunny.UI.UIStyle.Custom;
            this.uiTableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel1;
        private Sunny.UI.UILabel Lb_InStation;
        private Sunny.UI.UISwitch Sw_kaiguan;
    }
}
