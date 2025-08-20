namespace CapsuleInspect.Property
{
    partial class ImageFilterProp
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
            this.btnSrc = new System.Windows.Forms.Button();
            this.cbFilterType = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnSrc
            // 
            this.btnSrc.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.btnSrc.Location = new System.Drawing.Point(10, 54);
            this.btnSrc.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnSrc.Name = "btnSrc";
            this.btnSrc.Size = new System.Drawing.Size(50, 25);
            this.btnSrc.TabIndex = 27;
            this.btnSrc.Text = "원본";
            this.btnSrc.UseVisualStyleBackColor = true;
            this.btnSrc.Click += new System.EventHandler(this.btnSrc_Click);
            // 
            // cbFilterType
            // 
            this.cbFilterType.BackColor = System.Drawing.SystemColors.Info;
            this.cbFilterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFilterType.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.cbFilterType.ForeColor = System.Drawing.SystemColors.InfoText;
            this.cbFilterType.FormattingEnabled = true;
            this.cbFilterType.Location = new System.Drawing.Point(10, 15);
            this.cbFilterType.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbFilterType.Name = "cbFilterType";
            this.cbFilterType.Size = new System.Drawing.Size(214, 25);
            this.cbFilterType.TabIndex = 31;
            this.cbFilterType.SelectedIndexChanged += new System.EventHandler(this.cbFilterType_SelectedIndexChanged);
            // 
            // ImageFilterProp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbFilterType);
            this.Controls.Add(this.btnSrc);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "ImageFilterProp";
            this.Size = new System.Drawing.Size(239, 171);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnSrc;
        private System.Windows.Forms.ComboBox cbFilterType;
    }
}
