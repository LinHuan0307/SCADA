namespace GaoYaXianShu.UserControls
{
    partial class IOStatusLight
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
            this.Lb_IOname = new Sunny.UI.UILabel();
            this.Light_IOstatus = new Sunny.UI.UILight();
            this.uiTableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiTableLayoutPanel1
            // 
            this.uiTableLayoutPanel1.ColumnCount = 2;
            this.uiTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.uiTableLayoutPanel1.Controls.Add(this.Lb_IOname, 0, 0);
            this.uiTableLayoutPanel1.Controls.Add(this.Light_IOstatus, 1, 0);
            this.uiTableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel1.Location = new System.Drawing.Point(5, 5);
            this.uiTableLayoutPanel1.Name = "uiTableLayoutPanel1";
            this.uiTableLayoutPanel1.RowCount = 1;
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel1.Size = new System.Drawing.Size(290, 40);
            this.uiTableLayoutPanel1.TabIndex = 0;
            this.uiTableLayoutPanel1.TagString = null;
            // 
            // Lb_IOname
            // 
            this.Lb_IOname.BackColor = System.Drawing.Color.Turquoise;
            this.Lb_IOname.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Lb_IOname.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Lb_IOname.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.Lb_IOname.Location = new System.Drawing.Point(3, 0);
            this.Lb_IOname.Name = "Lb_IOname";
            this.Lb_IOname.Size = new System.Drawing.Size(234, 40);
            this.Lb_IOname.Style = Sunny.UI.UIStyle.Custom;
            this.Lb_IOname.TabIndex = 0;
            this.Lb_IOname.Text = "名字";
            this.Lb_IOname.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Light_IOstatus
            // 
            this.Light_IOstatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Light_IOstatus.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Light_IOstatus.Location = new System.Drawing.Point(243, 3);
            this.Light_IOstatus.MinimumSize = new System.Drawing.Size(1, 1);
            this.Light_IOstatus.Name = "Light_IOstatus";
            this.Light_IOstatus.Radius = 34;
            this.Light_IOstatus.Size = new System.Drawing.Size(44, 34);
            this.Light_IOstatus.TabIndex = 1;
            this.Light_IOstatus.Text = "uiLight1";
            // 
            // IOStatusLight
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.uiTableLayoutPanel1);
            this.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(251)))), ((int)(((byte)(250)))));
            this.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(251)))), ((int)(((byte)(250)))));
            this.Name = "IOStatusLight";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(190)))), ((int)(((byte)(172)))));
            this.Size = new System.Drawing.Size(300, 50);
            this.Style = Sunny.UI.UIStyle.Custom;
            this.Load += new System.EventHandler(this.IOStatusLight_Load);
            this.uiTableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel1;
        private Sunny.UI.UILabel Lb_IOname;
        private Sunny.UI.UILight Light_IOstatus;
    }
}
