namespace CapsuleInspect
{
    partial class MainForm
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
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.imageOpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageSaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.modelNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modelOpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modelSaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modelSaveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setupToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.inspectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cycleModeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.BackColor = System.Drawing.Color.Khaki;
            this.mainMenu.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.setupToolStripMenuItem,
            this.inspectToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(800, 26);
            this.mainMenu.TabIndex = 2;
            this.mainMenu.Text = "menuStrip1";
            // 
            // txtFilePath
            // 
            this.txtFilePath.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFilePath.Location = new System.Drawing.Point(186, 2);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(610, 22);
            this.txtFilePath.TabIndex = 3;
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.imageOpenToolStripMenuItem,
            this.imageSaveToolStripMenuItem,
            this.toolStripSeparator,
            this.modelNewToolStripMenuItem,
            this.modelOpenToolStripMenuItem,
            this.modelSaveToolStripMenuItem,
            this.modelSaveAsToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(46, 22);
            this.toolStripMenuItem1.Text = "File";
            // 
            // imageOpenToolStripMenuItem
            // 
            this.imageOpenToolStripMenuItem.Name = "imageOpenToolStripMenuItem";
            this.imageOpenToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.imageOpenToolStripMenuItem.Text = "Image Open";
            this.imageOpenToolStripMenuItem.Click += new System.EventHandler(this.imageOpenToolStripMenuItem_Click);
            // 
            // imageSaveToolStripMenuItem
            // 
            this.imageSaveToolStripMenuItem.Name = "imageSaveToolStripMenuItem";
            this.imageSaveToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.imageSaveToolStripMenuItem.Text = "Image Save";
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(178, 6);
            // 
            // modelNewToolStripMenuItem
            // 
            this.modelNewToolStripMenuItem.Name = "modelNewToolStripMenuItem";
            this.modelNewToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.modelNewToolStripMenuItem.Text = "Model New";
            // 
            // modelOpenToolStripMenuItem
            // 
            this.modelOpenToolStripMenuItem.Name = "modelOpenToolStripMenuItem";
            this.modelOpenToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.modelOpenToolStripMenuItem.Text = "Model Open";
            // 
            // modelSaveToolStripMenuItem
            // 
            this.modelSaveToolStripMenuItem.Name = "modelSaveToolStripMenuItem";
            this.modelSaveToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.modelSaveToolStripMenuItem.Text = "Model Save";
            // 
            // modelSaveAsToolStripMenuItem
            // 
            this.modelSaveAsToolStripMenuItem.Name = "modelSaveAsToolStripMenuItem";
            this.modelSaveAsToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.modelSaveAsToolStripMenuItem.Text = "Model Save As";
            // 
            // setupToolStripMenuItem
            // 
            this.setupToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setupToolStripMenuItem1});
            this.setupToolStripMenuItem.Name = "setupToolStripMenuItem";
            this.setupToolStripMenuItem.Size = new System.Drawing.Size(61, 22);
            this.setupToolStripMenuItem.Text = "Setup";
            // 
            // setupToolStripMenuItem1
            // 
            this.setupToolStripMenuItem1.Name = "setupToolStripMenuItem1";
            this.setupToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.setupToolStripMenuItem1.Text = "Setup";
            // 
            // inspectToolStripMenuItem
            // 
            this.inspectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cycleModeMenuItem});
            this.inspectToolStripMenuItem.Name = "inspectToolStripMenuItem";
            this.inspectToolStripMenuItem.Size = new System.Drawing.Size(69, 22);
            this.inspectToolStripMenuItem.Text = "Inspect";
            // 
            // cycleModeMenuItem
            // 
            this.cycleModeMenuItem.CheckOnClick = true;
            this.cycleModeMenuItem.Name = "cycleModeMenuItem";
            this.cycleModeMenuItem.Size = new System.Drawing.Size(180, 22);
            this.cycleModeMenuItem.Text = "Cycle Mode";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.txtFilePath);
            this.Controls.Add(this.mainMenu);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem imageOpenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem imageSaveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem modelNewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modelOpenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modelSaveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modelSaveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setupToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem inspectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cycleModeMenuItem;
        private System.Windows.Forms.TextBox txtFilePath;
    }
}