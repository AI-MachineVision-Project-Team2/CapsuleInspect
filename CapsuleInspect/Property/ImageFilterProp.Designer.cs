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
            this.btnRedo = new System.Windows.Forms.Button();
            this.checkFilter = new System.Windows.Forms.CheckBox();
            this.btnUndo = new System.Windows.Forms.Button();
            this.btnSrc = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.cbFilterType = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnRedo
            // 
            this.btnRedo.Location = new System.Drawing.Point(188, 140);
            this.btnRedo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnRedo.Name = "btnRedo";
            this.btnRedo.Size = new System.Drawing.Size(78, 44);
            this.btnRedo.TabIndex = 30;
            this.btnRedo.Text = "다음";
            this.btnRedo.UseVisualStyleBackColor = true;
            this.btnRedo.Click += new System.EventHandler(this.btnRedo_Click);
            // 
            // checkFilter
            // 
            this.checkFilter.AutoSize = true;
            this.checkFilter.Location = new System.Drawing.Point(16, 82);
            this.checkFilter.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkFilter.Name = "checkFilter";
            this.checkFilter.Size = new System.Drawing.Size(169, 25);
            this.checkFilter.TabIndex = 29;
            this.checkFilter.Text = "누적 필터 적용";
            this.checkFilter.UseVisualStyleBackColor = true;
            this.checkFilter.CheckedChanged += new System.EventHandler(this.checkFilter_CheckedChanged);
            // 
            // btnUndo
            // 
            this.btnUndo.Location = new System.Drawing.Point(104, 140);
            this.btnUndo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(78, 44);
            this.btnUndo.TabIndex = 28;
            this.btnUndo.Text = "이전";
            this.btnUndo.UseVisualStyleBackColor = true;
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // btnSrc
            // 
            this.btnSrc.Location = new System.Drawing.Point(273, 140);
            this.btnSrc.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSrc.Name = "btnSrc";
            this.btnSrc.Size = new System.Drawing.Size(78, 44);
            this.btnSrc.TabIndex = 27;
            this.btnSrc.Text = "원본";
            this.btnSrc.UseVisualStyleBackColor = true;
            this.btnSrc.Click += new System.EventHandler(this.btnSrc_Click);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(16, 140);
            this.btnApply.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(78, 44);
            this.btnApply.TabIndex = 26;
            this.btnApply.Text = "적용";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // cbFilterType
            // 
            this.cbFilterType.BackColor = System.Drawing.SystemColors.Info;
            this.cbFilterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFilterType.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbFilterType.ForeColor = System.Drawing.SystemColors.InfoText;
            this.cbFilterType.FormattingEnabled = true;
            this.cbFilterType.Location = new System.Drawing.Point(17, 26);
            this.cbFilterType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbFilterType.Name = "cbFilterType";
            this.cbFilterType.Size = new System.Drawing.Size(334, 24);
            this.cbFilterType.TabIndex = 31;
            this.cbFilterType.SelectedIndexChanged += new System.EventHandler(this.cbFilterType_SelectedIndexChanged);
            // 
            // ImageFilterProp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbFilterType);
            this.Controls.Add(this.btnRedo);
            this.Controls.Add(this.checkFilter);
            this.Controls.Add(this.btnUndo);
            this.Controls.Add(this.btnSrc);
            this.Controls.Add(this.btnApply);
            this.Name = "ImageFilterProp";
            this.Size = new System.Drawing.Size(376, 299);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRedo;
        private System.Windows.Forms.CheckBox checkFilter;
        private System.Windows.Forms.Button btnUndo;
        private System.Windows.Forms.Button btnSrc;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.ComboBox cbFilterType;
    }
}
