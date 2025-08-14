namespace CapsuleInspect.UIControl
{
    partial class ToolboxCtrl
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ToolboxCtrl));
            this.toolboxImgList = new System.Windows.Forms.ImageList(this.components);
            this.btnLive = new System.Windows.Forms.Button();
            this.btnInsp = new System.Windows.Forms.Button();
            this.btnGrab = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // toolboxImgList
            // 
            this.toolboxImgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("toolboxImgList.ImageStream")));
            this.toolboxImgList.TransparentColor = System.Drawing.Color.Transparent;
            this.toolboxImgList.Images.SetKeyName(0, "imageopen-removebg-preview.png");
            this.toolboxImgList.Images.SetKeyName(1, "imagesave-removebg-preview.png");
            this.toolboxImgList.Images.SetKeyName(2, "help-removebg-preview.png");
            // 
            // btnLive
            // 
            this.btnLive.BackColor = System.Drawing.Color.Transparent;
            this.btnLive.Cursor = System.Windows.Forms.Cursors.PanNW;
            this.btnLive.ImageIndex = 1;
            this.btnLive.ImageList = this.toolboxImgList;
            this.btnLive.Location = new System.Drawing.Point(52, 4);
            this.btnLive.Margin = new System.Windows.Forms.Padding(4);
            this.btnLive.Name = "btnLive";
            this.btnLive.Size = new System.Drawing.Size(25, 25);
            this.btnLive.TabIndex = 11;
            this.btnLive.UseVisualStyleBackColor = false;
            // 
            // btnInsp
            // 
            this.btnInsp.BackColor = System.Drawing.Color.Transparent;
            this.btnInsp.Cursor = System.Windows.Forms.Cursors.PanNW;
            this.btnInsp.ImageIndex = 2;
            this.btnInsp.ImageList = this.toolboxImgList;
            this.btnInsp.Location = new System.Drawing.Point(88, 4);
            this.btnInsp.Margin = new System.Windows.Forms.Padding(4);
            this.btnInsp.Name = "btnInsp";
            this.btnInsp.Size = new System.Drawing.Size(25, 25);
            this.btnInsp.TabIndex = 10;
            this.btnInsp.UseVisualStyleBackColor = false;
            // 
            // btnGrab
            // 
            this.btnGrab.BackColor = System.Drawing.Color.Transparent;
            this.btnGrab.Cursor = System.Windows.Forms.Cursors.PanNW;
            this.btnGrab.ImageIndex = 0;
            this.btnGrab.ImageList = this.toolboxImgList;
            this.btnGrab.Location = new System.Drawing.Point(14, 4);
            this.btnGrab.Margin = new System.Windows.Forms.Padding(4);
            this.btnGrab.Name = "btnGrab";
            this.btnGrab.Size = new System.Drawing.Size(25, 25);
            this.btnGrab.TabIndex = 9;
            this.btnGrab.UseVisualStyleBackColor = false;
            // 
            // ToolboxCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnLive);
            this.Controls.Add(this.btnInsp);
            this.Controls.Add(this.btnGrab);
            this.Name = "ToolboxCtrl";
            this.Size = new System.Drawing.Size(828, 32);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ImageList toolboxImgList;
        private System.Windows.Forms.Button btnLive;
        private System.Windows.Forms.Button btnInsp;
        private System.Windows.Forms.Button btnGrab;
    }
}
