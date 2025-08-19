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
            this.btnModelSave = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnModelOpen = new System.Windows.Forms.Button();
            this.btnZoomIn = new System.Windows.Forms.Button();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.butCycleone = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // toolboxImgList
            // 
            this.toolboxImgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("toolboxImgList.ImageStream")));
            this.toolboxImgList.TransparentColor = System.Drawing.Color.Transparent;
            this.toolboxImgList.Images.SetKeyName(0, "free-icon-help-round-button-61079.png");
            this.toolboxImgList.Images.SetKeyName(1, "free-icon-zoom-out-159096.png");
            this.toolboxImgList.Images.SetKeyName(2, "free-icon-zoom-in-597207.png");
            this.toolboxImgList.Images.SetKeyName(3, "free-icon-image-download-14367262.png");
            this.toolboxImgList.Images.SetKeyName(4, "free-icon-open-folder-25302.png");
            this.toolboxImgList.Images.SetKeyName(5, "ChatGPT Image 2025년 8월 18일 오후 03_20_15.png");
            // 
            // btnModelSave
            // 
            this.btnModelSave.BackColor = System.Drawing.Color.Transparent;
            this.btnModelSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnModelSave.ImageIndex = 3;
            this.btnModelSave.ImageList = this.toolboxImgList;
            this.btnModelSave.Location = new System.Drawing.Point(44, 0);
            this.btnModelSave.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.btnModelSave.Name = "btnModelSave";
            this.btnModelSave.Size = new System.Drawing.Size(36, 30);
            this.btnModelSave.TabIndex = 11;
            this.btnModelSave.UseVisualStyleBackColor = false;
            this.btnModelSave.Click += new System.EventHandler(this.btnModelSave_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.BackColor = System.Drawing.Color.Transparent;
            this.btnHelp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnHelp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnHelp.ImageIndex = 0;
            this.btnHelp.ImageList = this.toolboxImgList;
            this.btnHelp.Location = new System.Drawing.Point(196, 0);
            this.btnHelp.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(36, 30);
            this.btnHelp.TabIndex = 10;
            this.btnHelp.UseVisualStyleBackColor = false;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnModelOpen
            // 
            this.btnModelOpen.BackColor = System.Drawing.Color.Transparent;
            this.btnModelOpen.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnModelOpen.ImageIndex = 4;
            this.btnModelOpen.ImageList = this.toolboxImgList;
            this.btnModelOpen.Location = new System.Drawing.Point(6, 0);
            this.btnModelOpen.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.btnModelOpen.Name = "btnModelOpen";
            this.btnModelOpen.Size = new System.Drawing.Size(36, 30);
            this.btnModelOpen.TabIndex = 9;
            this.btnModelOpen.UseVisualStyleBackColor = false;
            this.btnModelOpen.Click += new System.EventHandler(this.btnModelOpen_Click);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.BackColor = System.Drawing.Color.Transparent;
            this.btnZoomIn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnZoomIn.ImageIndex = 2;
            this.btnZoomIn.ImageList = this.toolboxImgList;
            this.btnZoomIn.Location = new System.Drawing.Point(82, 0);
            this.btnZoomIn.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(36, 30);
            this.btnZoomIn.TabIndex = 12;
            this.btnZoomIn.UseVisualStyleBackColor = false;
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.BackColor = System.Drawing.Color.Transparent;
            this.btnZoomOut.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnZoomOut.ImageIndex = 1;
            this.btnZoomOut.ImageList = this.toolboxImgList;
            this.btnZoomOut.Location = new System.Drawing.Point(120, 0);
            this.btnZoomOut.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(36, 30);
            this.btnZoomOut.TabIndex = 13;
            this.btnZoomOut.UseVisualStyleBackColor = false;
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // butCycleone
            // 
            this.butCycleone.BackColor = System.Drawing.Color.Transparent;
            this.butCycleone.Cursor = System.Windows.Forms.Cursors.Hand;
            this.butCycleone.ImageIndex = 5;
            this.butCycleone.ImageList = this.toolboxImgList;
            this.butCycleone.Location = new System.Drawing.Point(158, 0);
            this.butCycleone.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.butCycleone.Name = "butCycleone";
            this.butCycleone.Size = new System.Drawing.Size(36, 30);
            this.butCycleone.TabIndex = 14;
            this.butCycleone.UseVisualStyleBackColor = false;
            this.butCycleone.Click += new System.EventHandler(this.butCycleone_Click);
            // 
            // ToolboxCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.butCycleone);
            this.Controls.Add(this.btnZoomOut);
            this.Controls.Add(this.btnZoomIn);
            this.Controls.Add(this.btnModelSave);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.btnModelOpen);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Name = "ToolboxCtrl";
            this.Size = new System.Drawing.Size(829, 36);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ImageList toolboxImgList;
        private System.Windows.Forms.Button btnModelSave;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Button btnModelOpen;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.Button butCycleone;
    }
}
