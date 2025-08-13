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
            this.tabPropControl2.Name = "tabPropControl2";
            this.tabPropControl2.SelectedIndex = 0;
            this.tabPropControl2.Size = new System.Drawing.Size(727, 386);
            this.tabPropControl2.TabIndex = 0;
            // 
            // FilterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(727, 386);
            this.Controls.Add(this.tabPropControl2);
            this.Name = "FilterForm";
            this.Text = "FilterForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabPropControl2;
    }
}