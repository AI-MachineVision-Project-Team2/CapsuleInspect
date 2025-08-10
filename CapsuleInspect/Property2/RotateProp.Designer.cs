namespace CapsuleInspect.Property2
{
    partial class RotateProp
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
            this.txtRotate = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.vScrollBarRotate = new System.Windows.Forms.VScrollBar();
            this.SuspendLayout();
            // 
            // txtRotate
            // 
            this.txtRotate.Location = new System.Drawing.Point(19, 325);
            this.txtRotate.Margin = new System.Windows.Forms.Padding(5);
            this.txtRotate.Multiline = true;
            this.txtRotate.Name = "txtRotate";
            this.txtRotate.ReadOnly = true;
            this.txtRotate.Size = new System.Drawing.Size(103, 34);
            this.txtRotate.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(61, 297);
            this.label5.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(21, 21);
            this.label5.TabIndex = 11;
            this.label5.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(52, 10);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 21);
            this.label4.TabIndex = 10;
            this.label4.Text = "360";
            // 
            // vScrollBarRotate
            // 
            this.vScrollBarRotate.Location = new System.Drawing.Point(19, 40);
            this.vScrollBarRotate.Maximum = 360;
            this.vScrollBarRotate.Name = "vScrollBarRotate";
            this.vScrollBarRotate.Size = new System.Drawing.Size(103, 248);
            this.vScrollBarRotate.TabIndex = 9;
            this.vScrollBarRotate.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBarRotate_Scroll);
            // 
            // RotateProp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtRotate);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.vScrollBarRotate);
            this.Name = "RotateProp";
            this.Size = new System.Drawing.Size(150, 385);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtRotate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.VScrollBar vScrollBarRotate;
    }
}
