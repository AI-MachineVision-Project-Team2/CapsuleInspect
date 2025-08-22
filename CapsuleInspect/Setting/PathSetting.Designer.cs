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
            this.label1 = new System.Windows.Forms.Label();
            this.txtAIModelPath = new System.Windows.Forms.TextBox();
            this.AIModelPath = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSelImageDir
            // 
            this.btnSelImageDir.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.btnSelImageDir.Location = new System.Drawing.Point(416, 77);
            this.btnSelImageDir.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btnSelImageDir.Name = "btnSelImageDir";
            this.btnSelImageDir.Size = new System.Drawing.Size(55, 46);
            this.btnSelImageDir.TabIndex = 20;
            this.btnSelImageDir.Text = "...";
            this.btnSelImageDir.UseVisualStyleBackColor = true;
            this.btnSelImageDir.Click += new System.EventHandler(this.btnSelImageDir_Click);
            // 
            // txtImageDir
            // 
            this.txtImageDir.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.txtImageDir.Location = new System.Drawing.Point(124, 79);
            this.txtImageDir.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.txtImageDir.Name = "txtImageDir";
            this.txtImageDir.Size = new System.Drawing.Size(261, 34);
            this.txtImageDir.TabIndex = 19;
            // 
            // lbImageDir
            // 
            this.lbImageDir.AutoSize = true;
            this.lbImageDir.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.lbImageDir.Location = new System.Drawing.Point(15, 85);
            this.lbImageDir.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbImageDir.Name = "lbImageDir";
            this.lbImageDir.Size = new System.Drawing.Size(102, 26);
            this.lbImageDir.TabIndex = 18;
            this.lbImageDir.Text = "이미지 경로";
            // 
            // btnApply
            // 
            this.btnApply.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.btnApply.Location = new System.Drawing.Point(386, 209);
            this.btnApply.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(85, 56);
            this.btnApply.TabIndex = 17;
            this.btnApply.Text = "적용";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnSelModelDir
            // 
            this.btnSelModelDir.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.btnSelModelDir.Location = new System.Drawing.Point(416, 19);
            this.btnSelModelDir.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btnSelModelDir.Name = "btnSelModelDir";
            this.btnSelModelDir.Size = new System.Drawing.Size(55, 46);
            this.btnSelModelDir.TabIndex = 16;
            this.btnSelModelDir.Text = "...";
            this.btnSelModelDir.UseVisualStyleBackColor = true;
            this.btnSelModelDir.Click += new System.EventHandler(this.btnSelModelDir_Click);
            // 
            // txtModelDir
            // 
            this.txtModelDir.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.txtModelDir.Location = new System.Drawing.Point(124, 19);
            this.txtModelDir.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.txtModelDir.Name = "txtModelDir";
            this.txtModelDir.Size = new System.Drawing.Size(261, 34);
            this.txtModelDir.TabIndex = 15;
            // 
            // lbModelDir
            // 
            this.lbModelDir.AutoSize = true;
            this.lbModelDir.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.lbModelDir.Location = new System.Drawing.Point(15, 27);
            this.lbModelDir.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbModelDir.Name = "lbModelDir";
            this.lbModelDir.Size = new System.Drawing.Size(85, 26);
            this.lbModelDir.TabIndex = 14;
            this.lbModelDir.Text = "모델 경로";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.label1.Location = new System.Drawing.Point(15, 146);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 26);
            this.label1.TabIndex = 21;
            this.label1.Text = "AI 모델 경로";
            // 
            // txtAIModelPath
            // 
            this.txtAIModelPath.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.txtAIModelPath.Location = new System.Drawing.Point(124, 138);
            this.txtAIModelPath.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.txtAIModelPath.Name = "txtAIModelPath";
            this.txtAIModelPath.Size = new System.Drawing.Size(261, 34);
            this.txtAIModelPath.TabIndex = 22;
            // 
            // AIModelPath
            // 
            this.AIModelPath.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.AIModelPath.Location = new System.Drawing.Point(416, 136);
            this.AIModelPath.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.AIModelPath.Name = "AIModelPath";
            this.AIModelPath.Size = new System.Drawing.Size(55, 46);
            this.AIModelPath.TabIndex = 23;
            this.AIModelPath.Text = "...";
            this.AIModelPath.UseVisualStyleBackColor = true;
            this.AIModelPath.Click += new System.EventHandler(this.AIModelPath_Click);
            // 
            // PathSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 26F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.AIModelPath);
            this.Controls.Add(this.txtAIModelPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSelImageDir);
            this.Controls.Add(this.txtImageDir);
            this.Controls.Add(this.lbImageDir);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnSelModelDir);
            this.Controls.Add(this.txtModelDir);
            this.Controls.Add(this.lbModelDir);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "PathSetting";
            this.Size = new System.Drawing.Size(488, 301);
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtAIModelPath;
        private System.Windows.Forms.Button AIModelPath;
    }
}
