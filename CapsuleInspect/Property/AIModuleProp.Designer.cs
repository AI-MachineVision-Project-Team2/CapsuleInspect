namespace CapsuleInspect.Property
{
    partial class AIModuleProp
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
            this.lbl_SEG = new System.Windows.Forms.Label();
            this.dataGridViewFilter = new System.Windows.Forms.DataGridView();
            this.chkUse = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFilter)).BeginInit();
            this.SuspendLayout();
            // 
            // lbl_SEG
            // 
            this.lbl_SEG.AutoSize = true;
            this.lbl_SEG.BackColor = System.Drawing.Color.Transparent;
            this.lbl_SEG.Font = new System.Drawing.Font("Noto Sans KR", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbl_SEG.ForeColor = System.Drawing.SystemColors.MenuText;
            this.lbl_SEG.Location = new System.Drawing.Point(3, 0);
            this.lbl_SEG.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_SEG.Name = "lbl_SEG";
            this.lbl_SEG.Size = new System.Drawing.Size(176, 30);
            this.lbl_SEG.TabIndex = 18;
            this.lbl_SEG.Text = "SEGMENTATION";
            // 
            // dataGridViewFilter
            // 
            this.dataGridViewFilter.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewFilter.Location = new System.Drawing.Point(8, 63);
            this.dataGridViewFilter.Name = "dataGridViewFilter";
            this.dataGridViewFilter.RowHeadersWidth = 62;
            this.dataGridViewFilter.RowTemplate.Height = 23;
            this.dataGridViewFilter.Size = new System.Drawing.Size(291, 118);
            this.dataGridViewFilter.TabIndex = 19;
            // 
            // chkUse
            // 
            this.chkUse.AutoSize = true;
            this.chkUse.Checked = true;
            this.chkUse.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUse.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.chkUse.Location = new System.Drawing.Point(8, 34);
            this.chkUse.Name = "chkUse";
            this.chkUse.Size = new System.Drawing.Size(49, 21);
            this.chkUse.TabIndex = 20;
            this.chkUse.Text = "검사";
            this.chkUse.UseVisualStyleBackColor = true;
            this.chkUse.CheckedChanged += new System.EventHandler(this.chkUse_CheckedChanged);
            // 
            // AIModuleProp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkUse);
            this.Controls.Add(this.dataGridViewFilter);
            this.Controls.Add(this.lbl_SEG);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Name = "AIModuleProp";
            this.Size = new System.Drawing.Size(417, 331);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFilter)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lbl_SEG;
        private System.Windows.Forms.DataGridView dataGridViewFilter;
        private System.Windows.Forms.CheckBox chkUse;
    }
}
