namespace GaoYaXianShu.UserControls
{
    partial class JogButton
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
            this.Btn_JogButton = new Sunny.UI.UIButton();
            this.SuspendLayout();
            // 
            // Btn_JogButton
            // 
            this.Btn_JogButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Btn_JogButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Btn_JogButton.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(190)))), ((int)(((byte)(172)))));
            this.Btn_JogButton.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(190)))), ((int)(((byte)(172)))));
            this.Btn_JogButton.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(203)))), ((int)(((byte)(189)))));
            this.Btn_JogButton.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(152)))), ((int)(((byte)(138)))));
            this.Btn_JogButton.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(152)))), ((int)(((byte)(138)))));
            this.Btn_JogButton.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Btn_JogButton.LightColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(251)))), ((int)(((byte)(250)))));
            this.Btn_JogButton.Location = new System.Drawing.Point(0, 0);
            this.Btn_JogButton.MinimumSize = new System.Drawing.Size(1, 1);
            this.Btn_JogButton.Name = "Btn_JogButton";
            this.Btn_JogButton.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(190)))), ((int)(((byte)(172)))));
            this.Btn_JogButton.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(203)))), ((int)(((byte)(189)))));
            this.Btn_JogButton.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(152)))), ((int)(((byte)(138)))));
            this.Btn_JogButton.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(152)))), ((int)(((byte)(138)))));
            this.Btn_JogButton.Size = new System.Drawing.Size(600, 80);
            this.Btn_JogButton.Style = Sunny.UI.UIStyle.Custom;
            this.Btn_JogButton.TabIndex = 1;
            this.Btn_JogButton.Text = "名字";
            this.Btn_JogButton.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // JogButton
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.Btn_JogButton);
            this.Name = "JogButton";
            this.Size = new System.Drawing.Size(600, 80);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UIButton Btn_JogButton;
    }
}
