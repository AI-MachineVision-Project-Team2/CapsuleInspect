namespace CapsuleInspect.Setting
{
    partial class PathSetting
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSelImageDir = new System.Windows.Forms.Button();
            this.txtImageDir = new System.Windows.Forms.TextBox();
            this.lbImageDir = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnSelModelDir = new System.Windows.Forms.Button();
            this.txtModelDir = new System.Windows.Forms.TextBox();
            this.lbModelDir = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnSelImageDir
            // 
            this.btnSelImageDir.Location = new System.Drawing.Point(458, 62);
            this.btnSelImageDir.Margin = new System.Windows.Forms.Padding(5);
            this.btnSelImageDir.Name = "btnSelImageDir";
            this.btnSelImageDir.Size = new System.Drawing.Size(61, 37);
            this.btnSelImageDir.TabIndex = 20;
            this.btnSelImageDir.Text = "...";
            this.btnSelImageDir.UseVisualStyleBackColor = true;
            this.btnSelImageDir.Click += new System.EventHandler(this.btnSelImageDir_Click);
            // 
            // txtImageDir
            // 
            this.txtImageDir.Location = new System.Drawing.Point(136, 64);
            this.txtImageDir.Margin = new System.Windows.Forms.Padding(5);
            this.txtImageDir.Name = "txtImageDir";
            this.txtImageDir.Size = new System.Drawing.Size(287, 32);
            this.txtImageDir.TabIndex = 19;
            // 
            // lbImageDir
            // 
            this.lbImageDir.AutoSize = true;
            this.lbImageDir.Location = new System.Drawing.Point(17, 69);
            this.lbImageDir.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbImageDir.Name = "lbImageDir";
            this.lbImageDir.Size = new System.Drawing.Size(122, 21);
            this.lbImageDir.TabIndex = 18;
            this.lbImageDir.Text = "이미지 경로";
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(427, 111);
            this.btnApply.Margin = new System.Windows.Forms.Padding(5);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(93, 46);
            this.btnApply.TabIndex = 17;
            this.btnApply.Text = "적용";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnSelModelDir
            // 
            this.btnSelModelDir.Location = new System.Drawing.Point(458, 15);
            this.btnSelModelDir.Margin = new System.Windows.Forms.Padding(5);
            this.btnSelModelDir.Name = "btnSelModelDir";
            this.btnSelModelDir.Size = new System.Drawing.Size(61, 37);
            this.btnSelModelDir.TabIndex = 16;
            this.btnSelModelDir.Text = "...";
            this.btnSelModelDir.UseVisualStyleBackColor = true;
            this.btnSelModelDir.Click += new System.EventHandler(this.btnSelModelDir_Click);
            // 
            // txtModelDir
            // 
            this.txtModelDir.Location = new System.Drawing.Point(136, 15);
            this.txtModelDir.Margin = new System.Windows.Forms.Padding(5);
            this.txtModelDir.Name = "txtModelDir";
            this.txtModelDir.Size = new System.Drawing.Size(287, 32);
            this.txtModelDir.TabIndex = 15;
            // 
            // lbModelDir
            // 
            this.lbModelDir.AutoSize = true;
            this.lbModelDir.Location = new System.Drawing.Point(17, 22);
            this.lbModelDir.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbModelDir.Name = "lbModelDir";
            this.lbModelDir.Size = new System.Drawing.Size(101, 21);
            this.lbModelDir.TabIndex = 14;
            this.lbModelDir.Text = "모델 경로";
            // 
            // PathSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnSelImageDir);
            this.Controls.Add(this.txtImageDir);
            this.Controls.Add(this.lbImageDir);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnSelModelDir);
            this.Controls.Add(this.txtModelDir);
            this.Controls.Add(this.lbModelDir);
            this.Name = "PathSetting";
            this.Size = new System.Drawing.Size(537, 187);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSelImageDir;
        private System.Windows.Forms.TextBox txtImageDir;
        private System.Windows.Forms.Label lbImageDir;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnSelModelDir;
        private System.Windows.Forms.TextBox txtModelDir;
        private System.Windows.Forms.Label lbModelDir;
    }
}
