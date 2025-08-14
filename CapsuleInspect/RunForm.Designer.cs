namespace CapsuleInspect
{
    partial class RunForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RunForm));
            this.runImageList = new System.Windows.Forms.ImageList(this.components);
            this.btnStop = new System.Windows.Forms.Button();
            this.btnLive = new System.Windows.Forms.Button();
            this.btnInsp = new System.Windows.Forms.Button();
            this.btnGrab = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // runImageList
            // 
            this.runImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("runImageList.ImageStream")));
            this.runImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.runImageList.Images.SetKeyName(0, "free-icon-photo-camera-interface-symbol-for-button-45010.png");
            this.runImageList.Images.SetKeyName(1, "video-camera.png");
            this.runImageList.Images.SetKeyName(2, "free-icon-play-button-109197.png");
            this.runImageList.Images.SetKeyName(3, "free-icon-pause-play-9513367.png");
            // 
            // btnStop
            // 
            this.btnStop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnStop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStop.ImageIndex = 3;
            this.btnStop.ImageList = this.runImageList;
            this.btnStop.Location = new System.Drawing.Point(173, 15);
            this.btnStop.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(45, 45);
            this.btnStop.TabIndex = 8;
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnLive
            // 
            this.btnLive.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLive.ImageIndex = 1;
            this.btnLive.ImageList = this.runImageList;
            this.btnLive.Location = new System.Drawing.Point(67, 15);
            this.btnLive.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnLive.Name = "btnLive";
            this.btnLive.Size = new System.Drawing.Size(45, 45);
            this.btnLive.TabIndex = 7;
            this.btnLive.UseVisualStyleBackColor = true;
            this.btnLive.Click += new System.EventHandler(this.btnLive_Click);
            // 
            // btnInsp
            // 
            this.btnInsp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnInsp.ImageIndex = 2;
            this.btnInsp.ImageList = this.runImageList;
            this.btnInsp.Location = new System.Drawing.Point(120, 15);
            this.btnInsp.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnInsp.Name = "btnInsp";
            this.btnInsp.Size = new System.Drawing.Size(45, 45);
            this.btnInsp.TabIndex = 6;
            this.btnInsp.UseVisualStyleBackColor = true;
            this.btnInsp.Click += new System.EventHandler(this.btnInsp_Click);
            // 
            // btnGrab
            // 
            this.btnGrab.BackColor = System.Drawing.Color.White;
            this.btnGrab.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGrab.ImageIndex = 0;
            this.btnGrab.ImageList = this.runImageList;
            this.btnGrab.Location = new System.Drawing.Point(13, 15);
            this.btnGrab.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnGrab.Name = "btnGrab";
            this.btnGrab.Size = new System.Drawing.Size(45, 45);
            this.btnGrab.TabIndex = 5;
            this.btnGrab.UseVisualStyleBackColor = false;
            this.btnGrab.Click += new System.EventHandler(this.btnGrab_Click);
            // 
            // RunForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(233, 68);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnLive);
            this.Controls.Add(this.btnInsp);
            this.Controls.Add(this.btnGrab);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Font = new System.Drawing.Font("Noto Sans KR", 9F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "RunForm";
            this.Text = " ";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.ImageList runImageList;
        private System.Windows.Forms.Button btnLive;
        private System.Windows.Forms.Button btnInsp;
        private System.Windows.Forms.Button btnGrab;
    }
}