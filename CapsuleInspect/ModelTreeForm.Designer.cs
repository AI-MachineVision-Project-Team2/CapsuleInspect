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
            this.tvModelTree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.tvModelTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvModelTree.Location = new System.Drawing.Point(0, 0);
            this.tvModelTree.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.tvModelTree.Name = "tvModelTree";
            this.tvModelTree.Size = new System.Drawing.Size(582, 364);
            this.tvModelTree.TabIndex = 2;
            this.tvModelTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tvModelTree_MouseDown);
            // 
            // ModelTreeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::CapsuleInspect.Properties.Resources._114d7ae1260225d12228170ef4374aae;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(582, 364);
            this.Controls.Add(this.tvModelTree);
            this.Cursor = System.Windows.Forms.Cursors.PanNW;
            this.Font = new System.Drawing.Font("한컴 말랑말랑 Bold", 9.749998F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "ModelTreeForm";
            this.Text = "ROI창";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvModelTree;
    }
}