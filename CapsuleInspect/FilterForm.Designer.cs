namespace CapsuleInspect
{
    partial class FilterForm
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
            this.tabPropControl2 = new System.Windows.Forms.TabControl();
            this.SuspendLayout();
            // 
            // tabPropControl2
            // 
            this.tabPropControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabPropControl2.Location = new System.Drawing.Point(0, 0);
            this.tabPropControl2.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tabPropControl2.Name = "tabPropControl2";
            this.tabPropControl2.SelectedIndex = 0;
            this.tabPropControl2.Size = new System.Drawing.Size(604, 395);
            this.tabPropControl2.TabIndex = 0;
            // 
            // FilterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(604, 395);
            this.Controls.Add(this.tabPropControl2);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Font = new System.Drawing.Font("Noto Sans KR", 10F, System.Drawing.FontStyle.Bold);
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "FilterForm";
            this.Text = "이미지필터창";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabPropControl2;
    }
}