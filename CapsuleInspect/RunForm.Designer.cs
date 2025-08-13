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
            this.btnStop = new System.Windows.Forms.Button();
            this.runImageList = new System.Windows.Forms.ImageList(this.components);
            this.btnLive = new System.Windows.Forms.Button();
            this.btnInsp = new System.Windows.Forms.Button();
            this.btnGrab = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnStop
            // 
            this.btnStop.ImageIndex = 3;
            this.btnStop.ImageList = this.runImageList;
            this.btnStop.Location = new System.Drawing.Point(282, 12);
            this.btnStop.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(82, 82);
            this.btnStop.TabIndex = 8;
            this.btnStop.UseVisualStyleBackColor = true;
            // 
            // runImageList
            // 
            this.runImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("runImageList.ImageStream")));
            this.runImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.runImageList.Images.SetKeyName(0, "camera.png");
            this.runImageList.Images.SetKeyName(1, "camera (1).png");
            this.runImageList.Images.SetKeyName(2, "check-mark.png");
            this.runImageList.Images.SetKeyName(3, "stop-button.png");
            // 
            // btnLive
            // 
            this.btnLive.ImageIndex = 1;
            this.btnLive.ImageList = this.runImageList;
            this.btnLive.Location = new System.Drawing.Point(101, 12);
            this.btnLive.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnLive.Name = "btnLive";
            this.btnLive.Size = new System.Drawing.Size(82, 82);
            this.btnLive.TabIndex = 7;
            this.btnLive.UseVisualStyleBackColor = true;
            this.btnLive.Click += new System.EventHandler(this.btnLive_Click);
            // 
            // btnInsp
            // 
            this.btnInsp.ImageIndex = 2;
            this.btnInsp.ImageList = this.runImageList;
            this.btnInsp.Location = new System.Drawing.Point(192, 12);
            this.btnInsp.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnInsp.Name = "btnInsp";
            this.btnInsp.Size = new System.Drawing.Size(82, 82);
            this.btnInsp.TabIndex = 6;
            this.btnInsp.UseVisualStyleBackColor = true;
            this.btnInsp.Click += new System.EventHandler(this.btnInsp_Click);
            // 
            // btnGrab
            // 
            this.btnGrab.ImageIndex = 0;
            this.btnGrab.ImageList = this.runImageList;
            this.btnGrab.Location = new System.Drawing.Point(12, 12);
            this.btnGrab.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnGrab.Name = "btnGrab";
            this.btnGrab.Size = new System.Drawing.Size(82, 82);
            this.btnGrab.TabIndex = 5;
            this.btnGrab.UseVisualStyleBackColor = true;
            this.btnGrab.Click += new System.EventHandler(this.btnGrab_Click);
            // 
            // RunForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(382, 106);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnLive);
            this.Controls.Add(this.btnInsp);
            this.Controls.Add(this.btnGrab);
            this.Name = "RunForm";
            this.Text = "RunForm";
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