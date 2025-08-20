namespace CapsuleInspect.Property
{
    partial class BinaryProp
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
                if (binRangeTrackbar != null)
                    binRangeTrackbar.RangeChanged -= Range_RangeChanged;

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
            this.chkUse = new System.Windows.Forms.CheckBox();
            this.grpBinary = new System.Windows.Forms.GroupBox();
            this.cbChannel = new System.Windows.Forms.ComboBox();
            this.lbChannel = new System.Windows.Forms.Label();
            this.binRangeTrackbar = new CapsuleInspect.UIControl.RangeTrackbar();
            this.cbHighlight = new System.Windows.Forms.ComboBox();
            this.lbHighlight = new System.Windows.Forms.Label();
            this.cbBinMethod = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridViewFilter = new System.Windows.Forms.DataGridView();
            this.chkRotatedRect = new System.Windows.Forms.CheckBox();
            this.chkMeasure = new System.Windows.Forms.CheckBox();
            this.grpBinary.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFilter)).BeginInit();
            this.SuspendLayout();
            // 
            // chkUse
            // 
            this.chkUse.AutoSize = true;
            this.chkUse.Checked = true;
            this.chkUse.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUse.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.chkUse.Location = new System.Drawing.Point(12, 5);
            this.chkUse.Name = "chkUse";
            this.chkUse.Size = new System.Drawing.Size(49, 21);
            this.chkUse.TabIndex = 6;
            this.chkUse.Text = "검사";
            this.chkUse.UseVisualStyleBackColor = true;
            this.chkUse.CheckedChanged += new System.EventHandler(this.chkUse_CheckedChanged);
            // 
            // grpBinary
            // 
            this.grpBinary.Controls.Add(this.cbChannel);
            this.grpBinary.Controls.Add(this.lbChannel);
            this.grpBinary.Controls.Add(this.binRangeTrackbar);
            this.grpBinary.Controls.Add(this.cbHighlight);
            this.grpBinary.Controls.Add(this.lbHighlight);
            this.grpBinary.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.grpBinary.Location = new System.Drawing.Point(8, 28);
            this.grpBinary.Name = "grpBinary";
            this.grpBinary.Size = new System.Drawing.Size(295, 141);
            this.grpBinary.TabIndex = 5;
            this.grpBinary.TabStop = false;
            this.grpBinary.Text = "이진화";
            // 
            // cbChannel
            // 
            this.cbChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbChannel.FormattingEnabled = true;
            this.cbChannel.Location = new System.Drawing.Point(90, 76);
            this.cbChannel.Name = "cbChannel";
            this.cbChannel.Size = new System.Drawing.Size(117, 25);
            this.cbChannel.TabIndex = 9;
            this.cbChannel.SelectedIndexChanged += new System.EventHandler(this.cbChannel_SelectedIndexChanged);
            // 
            // lbChannel
            // 
            this.lbChannel.AutoSize = true;
            this.lbChannel.Location = new System.Drawing.Point(11, 79);
            this.lbChannel.Name = "lbChannel";
            this.lbChannel.Size = new System.Drawing.Size(66, 17);
            this.lbChannel.TabIndex = 10;
            this.lbChannel.Text = "이미지 채널";
            // 
            // binRangeTrackbar
            // 
            this.binRangeTrackbar.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.binRangeTrackbar.Location = new System.Drawing.Point(8, 20);
            this.binRangeTrackbar.Name = "binRangeTrackbar";
            this.binRangeTrackbar.Size = new System.Drawing.Size(281, 53);
            this.binRangeTrackbar.TabIndex = 8;
            this.binRangeTrackbar.ValueLeft = 80;
            this.binRangeTrackbar.ValueRight = 200;
            // 
            // cbHighlight
            // 
            this.cbHighlight.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbHighlight.FormattingEnabled = true;
            this.cbHighlight.Location = new System.Drawing.Point(90, 107);
            this.cbHighlight.Name = "cbHighlight";
            this.cbHighlight.Size = new System.Drawing.Size(117, 25);
            this.cbHighlight.TabIndex = 2;
            this.cbHighlight.SelectedIndexChanged += new System.EventHandler(this.cbHighlight_SelectedIndexChanged);
            // 
            // lbHighlight
            // 
            this.lbHighlight.AutoSize = true;
            this.lbHighlight.Location = new System.Drawing.Point(11, 110);
            this.lbHighlight.Name = "lbHighlight";
            this.lbHighlight.Size = new System.Drawing.Size(63, 17);
            this.lbHighlight.TabIndex = 7;
            this.lbHighlight.Text = "하이라이트";
            // 
            // cbBinMethod
            // 
            this.cbBinMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBinMethod.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.cbBinMethod.FormattingEnabled = true;
            this.cbBinMethod.Location = new System.Drawing.Point(100, 172);
            this.cbBinMethod.Name = "cbBinMethod";
            this.cbBinMethod.Size = new System.Drawing.Size(118, 25);
            this.cbBinMethod.TabIndex = 15;
            this.cbBinMethod.SelectedIndexChanged += new System.EventHandler(this.cbBinMethod_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.label1.Location = new System.Drawing.Point(21, 174);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 17);
            this.label1.TabIndex = 16;
            this.label1.Text = "검사 타입";
            // 
            // dataGridViewFilter
            // 
            this.dataGridViewFilter.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewFilter.Location = new System.Drawing.Point(8, 199);
            this.dataGridViewFilter.Name = "dataGridViewFilter";
            this.dataGridViewFilter.RowHeadersWidth = 62;
            this.dataGridViewFilter.RowTemplate.Height = 23;
            this.dataGridViewFilter.Size = new System.Drawing.Size(295, 118);
            this.dataGridViewFilter.TabIndex = 14;
            this.dataGridViewFilter.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewFilter_CellValueChanged);
            this.dataGridViewFilter.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridViewFilter_CurrentCellDirtyStateChanged);
            // 
            // chkRotatedRect
            // 
            this.chkRotatedRect.AutoSize = true;
            this.chkRotatedRect.Checked = true;
            this.chkRotatedRect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRotatedRect.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.chkRotatedRect.Location = new System.Drawing.Point(11, 320);
            this.chkRotatedRect.Name = "chkRotatedRect";
            this.chkRotatedRect.Size = new System.Drawing.Size(82, 21);
            this.chkRotatedRect.TabIndex = 17;
            this.chkRotatedRect.Text = "회전사각형";
            this.chkRotatedRect.UseVisualStyleBackColor = true;
            this.chkRotatedRect.CheckedChanged += new System.EventHandler(this.chkRotatedRect_CheckedChanged);
            // 
            // chkMeasure
            // 
            this.chkMeasure.AutoSize = true;
            this.chkMeasure.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.chkMeasure.Location = new System.Drawing.Point(119, 320);
            this.chkMeasure.Name = "chkMeasure";
            this.chkMeasure.Size = new System.Drawing.Size(49, 21);
            this.chkMeasure.TabIndex = 18;
            this.chkMeasure.Text = "측정";
            this.chkMeasure.UseVisualStyleBackColor = true;
            this.chkMeasure.CheckedChanged += new System.EventHandler(this.chkMeasure_CheckedChanged);
            // 
            // BinaryProp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkMeasure);
            this.Controls.Add(this.chkRotatedRect);
            this.Controls.Add(this.cbBinMethod);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridViewFilter);
            this.Controls.Add(this.chkUse);
            this.Controls.Add(this.grpBinary);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Name = "BinaryProp";
            this.Size = new System.Drawing.Size(351, 369);
            this.grpBinary.ResumeLayout(false);
            this.grpBinary.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFilter)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkUse;
        private System.Windows.Forms.GroupBox grpBinary;
        private UIControl.RangeTrackbar binRangeTrackbar;
        private System.Windows.Forms.ComboBox cbHighlight;
        private System.Windows.Forms.Label lbHighlight;
        private System.Windows.Forms.ComboBox cbBinMethod;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridViewFilter;
        private System.Windows.Forms.CheckBox chkRotatedRect;
        private System.Windows.Forms.ComboBox cbChannel;
        private System.Windows.Forms.Label lbChannel;
        private System.Windows.Forms.CheckBox chkMeasure;
    }
}
