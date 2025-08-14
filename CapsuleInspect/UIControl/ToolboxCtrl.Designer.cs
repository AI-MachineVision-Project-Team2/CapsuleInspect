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
            this.btnXmlOpen = new System.Windows.Forms.Button();
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
            // btnModelSave
            // 
            this.btnModelSave.BackColor = System.Drawing.Color.Transparent;
            this.btnModelSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnModelSave.ImageIndex = 1;
            this.btnModelSave.ImageList = this.toolboxImgList;
            this.btnModelSave.Location = new System.Drawing.Point(34, 0);
            this.btnModelSave.Margin = new System.Windows.Forms.Padding(1);
            this.btnModelSave.Name = "btnModelSave";
            this.btnModelSave.Size = new System.Drawing.Size(25, 20);
            this.btnModelSave.TabIndex = 11;
            this.btnModelSave.UseVisualStyleBackColor = false;
            // 
            // btnHelp
            // 
            this.btnHelp.BackColor = System.Drawing.Color.Transparent;
            this.btnHelp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnHelp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnHelp.ImageIndex = 2;
            this.btnHelp.ImageList = this.toolboxImgList;
            this.btnHelp.Location = new System.Drawing.Point(62, 0);
            this.btnHelp.Margin = new System.Windows.Forms.Padding(1);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(25, 20);
            this.btnHelp.TabIndex = 10;
            this.btnHelp.UseVisualStyleBackColor = false;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnXmlOpen
            // 
            this.btnXmlOpen.BackColor = System.Drawing.Color.Transparent;
            this.btnXmlOpen.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnXmlOpen.ImageIndex = 0;
            this.btnXmlOpen.ImageList = this.toolboxImgList;
            this.btnXmlOpen.Location = new System.Drawing.Point(4, 0);
            this.btnXmlOpen.Margin = new System.Windows.Forms.Padding(1);
            this.btnXmlOpen.Name = "btnXmlOpen";
            this.btnXmlOpen.Size = new System.Drawing.Size(25, 20);
            this.btnXmlOpen.TabIndex = 9;
            this.btnXmlOpen.UseVisualStyleBackColor = false;
            // 
            // ToolboxCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnModelSave);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.btnXmlOpen);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "ToolboxCtrl";
            this.Size = new System.Drawing.Size(580, 24);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ImageList toolboxImgList;
        private System.Windows.Forms.Button btnModelSave;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Button btnXmlOpen;
    }
}
