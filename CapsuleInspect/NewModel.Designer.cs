namespace CapsuleInspect
{
    partial class NewModel
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
            this.btnCreate = new System.Windows.Forms.Button();
            this.txtModelInfo = new System.Windows.Forms.RichTextBox();
            this.txtModelName = new System.Windows.Forms.TextBox();
            this.lbModelInfo = new System.Windows.Forms.Label();
            this.lbModelName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(264, 222);
            this.btnCreate.Margin = new System.Windows.Forms.Padding(4);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(113, 45);
            this.btnCreate.TabIndex = 14;
            this.btnCreate.Text = "만들기";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // txtModelInfo
            // 
            this.txtModelInfo.Location = new System.Drawing.Point(107, 58);
            this.txtModelInfo.Margin = new System.Windows.Forms.Padding(4);
            this.txtModelInfo.Name = "txtModelInfo";
            this.txtModelInfo.Size = new System.Drawing.Size(270, 154);
            this.txtModelInfo.TabIndex = 13;
            this.txtModelInfo.Text = "";
            // 
            // txtModelName
            // 
            this.txtModelName.Location = new System.Drawing.Point(107, 17);
            this.txtModelName.Margin = new System.Windows.Forms.Padding(4);
            this.txtModelName.Name = "txtModelName";
            this.txtModelName.Size = new System.Drawing.Size(270, 28);
            this.txtModelName.TabIndex = 12;
            // 
            // lbModelInfo
            // 
            this.lbModelInfo.AutoSize = true;
            this.lbModelInfo.Location = new System.Drawing.Point(14, 62);
            this.lbModelInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbModelInfo.Name = "lbModelInfo";
            this.lbModelInfo.Size = new System.Drawing.Size(86, 18);
            this.lbModelInfo.TabIndex = 11;
            this.lbModelInfo.Text = "모델 정보";
            // 
            // lbModelName
            // 
            this.lbModelName.AutoSize = true;
            this.lbModelName.Location = new System.Drawing.Point(14, 22);
            this.lbModelName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbModelName.Name = "lbModelName";
            this.lbModelName.Size = new System.Drawing.Size(62, 18);
            this.lbModelName.TabIndex = 10;
            this.lbModelName.Text = "모델명";
            // 
            // NewModel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(401, 280);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.txtModelInfo);
            this.Controls.Add(this.txtModelName);
            this.Controls.Add(this.lbModelInfo);
            this.Controls.Add(this.lbModelName);
            this.Name = "NewModel";
            this.Text = "NewModel";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.RichTextBox txtModelInfo;
        private System.Windows.Forms.TextBox txtModelName;
        private System.Windows.Forms.Label lbModelInfo;
        private System.Windows.Forms.Label lbModelName;
    }
}