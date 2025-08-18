namespace CapsuleInspect
{
    partial class CameraForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.imageViewer = new CapsuleInspect.UIControl.ImageViewCtrl();
            this.mainViewToolbar = new CapsuleInspect.UIControl.MainViewToolbar();
            this.SuspendLayout();
            // 
            // imageViewer
            // 
            this.imageViewer.BackColor = System.Drawing.Color.White;
            this.imageViewer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.imageViewer.Dock = System.Windows.Forms.DockStyle.Left;
            this.imageViewer.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.imageViewer.Location = new System.Drawing.Point(0, 0);
            this.imageViewer.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.imageViewer.Name = "imageViewer";
            this.imageViewer.Size = new System.Drawing.Size(1143, 675);
            this.imageViewer.TabIndex = 0;
            this.imageViewer.WorkingState = "";
            // 
            // mainViewToolbar
            // 
            this.mainViewToolbar.BackColor = System.Drawing.SystemColors.Control;
            this.mainViewToolbar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.mainViewToolbar.Dock = System.Windows.Forms.DockStyle.Right;
            this.mainViewToolbar.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.mainViewToolbar.Location = new System.Drawing.Point(1051, 0);
            this.mainViewToolbar.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.mainViewToolbar.Name = "mainViewToolbar";
            this.mainViewToolbar.Size = new System.Drawing.Size(92, 675);
            this.mainViewToolbar.TabIndex = 1;
            // 
            // CameraForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(211)))), ((int)(((byte)(211)))));
            this.ClientSize = new System.Drawing.Size(1143, 675);
            this.Controls.Add(this.mainViewToolbar);
            this.Controls.Add(this.imageViewer);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "CameraForm";
            this.Text = "이미지창";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CameraForm_FormClosed);
            this.Resize += new System.EventHandler(this.CameraForm_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private UIControl.ImageViewCtrl imageViewer;
        private UIControl.MainViewToolbar mainViewToolbar;
    }
}