namespace CapsuleInspect.Property2
{
    partial class MorphologyProp
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
            this.btnOpening = new System.Windows.Forms.RadioButton();
            this.btnClosing = new System.Windows.Forms.RadioButton();
            this.lblpixel = new System.Windows.Forms.Label();
            this.txtpixelsize = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnOpening
            // 
            this.btnOpening.AutoSize = true;
            this.btnOpening.Location = new System.Drawing.Point(14, 18);
            this.btnOpening.Name = "btnOpening";
            this.btnOpening.Size = new System.Drawing.Size(99, 22);
            this.btnOpening.TabIndex = 12;
            this.btnOpening.TabStop = true;
            this.btnOpening.Text = "Opening";
            this.btnOpening.UseVisualStyleBackColor = true;
            // 
            // btnClosing
            // 
            this.btnClosing.AutoSize = true;
            this.btnClosing.Location = new System.Drawing.Point(14, 52);
            this.btnClosing.Name = "btnClosing";
            this.btnClosing.Size = new System.Drawing.Size(92, 22);
            this.btnClosing.TabIndex = 13;
            this.btnClosing.TabStop = true;
            this.btnClosing.Text = "Closing";
            this.btnClosing.UseVisualStyleBackColor = true;
            // 
            // lblpixel
            // 
            this.lblpixel.AutoSize = true;
            this.lblpixel.Location = new System.Drawing.Point(15, 91);
            this.lblpixel.Name = "lblpixel";
            this.lblpixel.Size = new System.Drawing.Size(98, 18);
            this.lblpixel.TabIndex = 14;
            this.lblpixel.Text = "픽셀 크기: ";
            // 
            // txtpixelsize
            // 
            this.txtpixelsize.Location = new System.Drawing.Point(119, 88);
            this.txtpixelsize.Name = "txtpixelsize";
            this.txtpixelsize.Size = new System.Drawing.Size(100, 28);
            this.txtpixelsize.TabIndex = 15;
            this.txtpixelsize.TextChanged += new System.EventHandler(this.txtpixelsize_TextChanged);
            // 
            // MorphologyProp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtpixelsize);
            this.Controls.Add(this.lblpixel);
            this.Controls.Add(this.btnOpening);
            this.Controls.Add(this.btnClosing);
            this.Name = "MorphologyProp";
            this.Size = new System.Drawing.Size(258, 143);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton btnOpening;
        private System.Windows.Forms.RadioButton btnClosing;
        private System.Windows.Forms.Label lblpixel;
        private System.Windows.Forms.TextBox txtpixelsize;
    }
}
