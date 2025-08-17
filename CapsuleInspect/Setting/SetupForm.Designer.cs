namespace CapsuleInspect.Setting
{
    partial class SetupForm
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
            this.tabSetting = new System.Windows.Forms.TabControl();
            this.SuspendLayout();
            // 
            // tabSetting
            // 
            this.tabSetting.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tabSetting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabSetting.Location = new System.Drawing.Point(0, 0);
            this.tabSetting.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabSetting.Name = "tabSetting";
            this.tabSetting.SelectedIndex = 0;
            this.tabSetting.Size = new System.Drawing.Size(604, 395);
            this.tabSetting.TabIndex = 2;
            // 
            // SetupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 395);
            this.Controls.Add(this.tabSetting);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Font = new System.Drawing.Font("Noto Sans KR", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "SetupForm";
            this.Text = "환경설정창";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabSetting;
    }
}