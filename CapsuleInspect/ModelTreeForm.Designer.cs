namespace CapsuleInspect
{
    partial class ModelTreeForm
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
            this.tvModelTree = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // tvModelTree
            // 
            this.tvModelTree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.tvModelTree.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tvModelTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvModelTree.Location = new System.Drawing.Point(0, 0);
            this.tvModelTree.Margin = new System.Windows.Forms.Padding(5);
            this.tvModelTree.Name = "tvModelTree";
            this.tvModelTree.Size = new System.Drawing.Size(635, 422);
            this.tvModelTree.TabIndex = 2;
            this.tvModelTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tvModelTree_MouseDown);
            // 
            // ModelTreeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(635, 422);
            this.Controls.Add(this.tvModelTree);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Font = new System.Drawing.Font("Noto Sans KR", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "ModelTreeForm";
            this.Text = "ROI창";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvModelTree;
    }
}