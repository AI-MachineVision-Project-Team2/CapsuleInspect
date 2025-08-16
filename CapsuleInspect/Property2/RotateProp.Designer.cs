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
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtRotate
            // 
            this.txtRotate.Location = new System.Drawing.Point(146, 19);
            this.txtRotate.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtRotate.Name = "txtRotate";
            this.txtRotate.Size = new System.Drawing.Size(54, 21);
            this.txtRotate.TabIndex = 12;
            this.txtRotate.TextChanged += new System.EventHandler(this.txtRotate_TextChanged);
            this.txtRotate.LostFocus += new System.EventHandler(this.txtRotate_LostFocus);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.label5.Location = new System.Drawing.Point(12, 90);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(15, 17);
            this.label5.TabIndex = 11;
            this.label5.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.label4.Location = new System.Drawing.Point(5, 1);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 17);
            this.label4.TabIndex = 10;
            this.label4.Text = "360";
            // 
            // vScrollBarRotate
            // 
            this.vScrollBarRotate.Location = new System.Drawing.Point(36, 3);
            this.vScrollBarRotate.Maximum = 360;
            this.vScrollBarRotate.Name = "vScrollBarRotate";
            this.vScrollBarRotate.Size = new System.Drawing.Size(56, 96);
            this.vScrollBarRotate.TabIndex = 9;
            this.vScrollBarRotate.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBarRotate_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(106, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 12);
            this.label2.TabIndex = 13;
            this.label2.Text = "각도:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(206, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 12);
            this.label1.TabIndex = 14;
            this.label1.Text = "도 회전";
            // 
            // RotateProp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtRotate);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.vScrollBarRotate);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "RotateProp";
            this.Size = new System.Drawing.Size(384, 117);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtRotate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.VScrollBar vScrollBarRotate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}
