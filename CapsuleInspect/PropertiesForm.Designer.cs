namespace CapsuleInspect
{
    partial class PropertiesForm
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
            this.tabPropControl = new System.Windows.Forms.TabControl();
            this.SuspendLayout();
            // 
            // tabPropControl
            // 
            this.tabPropControl.Cursor = System.Windows.Forms.Cursors.PanNW;
            this.tabPropControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabPropControl.Location = new System.Drawing.Point(0, 0);
            this.tabPropControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPropControl.Name = "tabPropControl";
            this.tabPropControl.SelectedIndex = 0;
            this.tabPropControl.Size = new System.Drawing.Size(914, 638);
            this.tabPropControl.TabIndex = 1;
            // 
            // PropertiesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(914, 638);
            this.Controls.Add(this.tabPropControl);
            this.Cursor = System.Windows.Forms.Cursors.PanNW;
            this.Font = new System.Drawing.Font("한컴 말랑말랑 Bold", 9.749998F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "PropertiesForm";
            this.Text = "속성창";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabPropControl;
    }
}