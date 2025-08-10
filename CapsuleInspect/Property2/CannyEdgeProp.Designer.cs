namespace CapsuleInspect.Property2
{
    partial class CannyEdgeProp
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
            this.rangeSlider = new CapsuleInspect.UIControl.RangeSliderCtrl();
            this.SuspendLayout();
            // 
            // rangeSlider
            // 
            this.rangeSlider.Location = new System.Drawing.Point(14, 26);
            this.rangeSlider.Maximum = 255;
            this.rangeSlider.Minimum = 0;
            this.rangeSlider.Name = "rangeSlider";
            this.rangeSlider.Size = new System.Drawing.Size(292, 52);
            this.rangeSlider.SliderMax = 200;
            this.rangeSlider.SliderMin = 30;
            this.rangeSlider.TabIndex = 0;
            this.rangeSlider.ValueChanged += new System.EventHandler(this.rangeSlider_ValueChanged);
            // 
            // CannyEdgeProp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rangeSlider);
            this.Name = "CannyEdgeProp";
            this.Size = new System.Drawing.Size(446, 173);
            this.ResumeLayout(false);

        }

        #endregion

        private UIControl.RangeSliderCtrl rangeSlider;
    }
}
