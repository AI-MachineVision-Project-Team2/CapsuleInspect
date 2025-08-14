namespace CapsuleInspect
{
    partial class LogForm
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
            this.listBoxLogs = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // listBoxLogs
            // 
            this.listBoxLogs.Cursor = System.Windows.Forms.Cursors.Hand;
            this.listBoxLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxLogs.Font = new System.Drawing.Font("Noto Sans KR", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.listBoxLogs.FormattingEnabled = true;
            this.listBoxLogs.ItemHeight = 29;
            this.listBoxLogs.Location = new System.Drawing.Point(0, 0);
            this.listBoxLogs.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.listBoxLogs.Name = "listBoxLogs";
            this.listBoxLogs.Size = new System.Drawing.Size(640, 425);
            this.listBoxLogs.TabIndex = 2;
            // 
            // LogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.ClientSize = new System.Drawing.Size(640, 425);
            this.Controls.Add(this.listBoxLogs);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Font = new System.Drawing.Font("Noto Sans KR", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "LogForm";
            this.Text = "로그창";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LogForm_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxLogs;
    }
}