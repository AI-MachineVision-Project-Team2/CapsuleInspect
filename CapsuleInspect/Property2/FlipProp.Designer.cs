namespace CapsuleInspect.Property2
{
    partial class FlipProp
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
            this.xFlip = new System.Windows.Forms.RadioButton();
            this.yFlip = new System.Windows.Forms.RadioButton();
            this.xyFlip = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // xFlip
            // 
            this.xFlip.AutoSize = true;
            this.xFlip.Font = new System.Drawing.Font("Noto Sans KR", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.xFlip.Location = new System.Drawing.Point(8, 12);
            this.xFlip.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.xFlip.Name = "xFlip";
            this.xFlip.Size = new System.Drawing.Size(48, 21);
            this.xFlip.TabIndex = 11;
            this.xFlip.TabStop = true;
            this.xFlip.Text = "수평";
            this.xFlip.UseVisualStyleBackColor = true;
            // 
            // yFlip
            // 
            this.yFlip.AutoSize = true;
            this.yFlip.Font = new System.Drawing.Font("Noto Sans KR", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.yFlip.Location = new System.Drawing.Point(8, 32);
            this.yFlip.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.yFlip.Name = "yFlip";
            this.yFlip.Size = new System.Drawing.Size(48, 21);
            this.yFlip.TabIndex = 12;
            this.yFlip.TabStop = true;
            this.yFlip.Text = "수직";
            this.yFlip.UseVisualStyleBackColor = true;
            // 
            // xyFlip
            // 
            this.xyFlip.AutoSize = true;
            this.xyFlip.Font = new System.Drawing.Font("Noto Sans KR", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.xyFlip.Location = new System.Drawing.Point(8, 52);
            this.xyFlip.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.xyFlip.Name = "xyFlip";
            this.xyFlip.Size = new System.Drawing.Size(77, 21);
            this.xyFlip.TabIndex = 13;
            this.xyFlip.TabStop = true;
            this.xyFlip.Text = "수평+수직";
            this.xyFlip.UseVisualStyleBackColor = true;
            // 
            // FlipProp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.xFlip);
            this.Controls.Add(this.yFlip);
            this.Controls.Add(this.xyFlip);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "FlipProp";
            this.Size = new System.Drawing.Size(156, 132);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton xFlip;
        private System.Windows.Forms.RadioButton yFlip;
        private System.Windows.Forms.RadioButton xyFlip;
    }
}
