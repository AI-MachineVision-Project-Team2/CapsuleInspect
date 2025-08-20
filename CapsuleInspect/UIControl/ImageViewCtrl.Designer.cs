using System.Windows.Forms;

namespace CapsuleInspect.UIControl
{
    partial class ImageViewCtrl
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
            this.SuspendLayout();
            // 
            // ImageViewCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Name = "ImageViewCtrl";
            this.Size = new System.Drawing.Size(215, 325);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ImageViewCtrl_Paint);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ImageViewCtrl_MouseDoubleClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ImageViewCtrl_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ImageViewCtrl_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ImageViewCtrl_MouseUp);
            this.Resize += new System.EventHandler(this.ImageViewCtrl_Resize);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
