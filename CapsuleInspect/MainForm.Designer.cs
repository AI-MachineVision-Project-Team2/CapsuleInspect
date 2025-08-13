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
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.imageOpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageSaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.modelNewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modelOpenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modelSaveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modelSaveAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SetupMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inspectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cycleModeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.mainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.BackColor = System.Drawing.Color.LightPink;
            this.mainMenu.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainMenu.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.mainMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.setupToolStripMenuItem,
            this.inspectToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Padding = new System.Windows.Forms.Padding(8, 3, 0, 3);
            this.mainMenu.Size = new System.Drawing.Size(1143, 37);
            this.mainMenu.TabIndex = 2;
            this.mainMenu.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.imageOpenToolStripMenuItem,
            this.imageSaveToolStripMenuItem,
            this.toolStripSeparator,
            this.modelNewMenuItem,
            this.modelOpenMenuItem,
            this.modelSaveMenuItem,
            this.modelSaveAsMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(67, 31);
            this.toolStripMenuItem1.Text = "File";
            // 
            // imageOpenToolStripMenuItem
            // 
            this.imageOpenToolStripMenuItem.Name = "imageOpenToolStripMenuItem";
            this.imageOpenToolStripMenuItem.Size = new System.Drawing.Size(272, 36);
            this.imageOpenToolStripMenuItem.Text = "Image Open";
            this.imageOpenToolStripMenuItem.Click += new System.EventHandler(this.imageOpenToolStripMenuItem_Click);
            // 
            // imageSaveToolStripMenuItem
            // 
            this.imageSaveToolStripMenuItem.Name = "imageSaveToolStripMenuItem";
            this.imageSaveToolStripMenuItem.Size = new System.Drawing.Size(272, 36);
            this.imageSaveToolStripMenuItem.Text = "Image Save";
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(269, 6);
            // 
            // modelNewMenuItem
            // 
            this.modelNewMenuItem.Name = "modelNewMenuItem";
            this.modelNewMenuItem.Size = new System.Drawing.Size(272, 36);
            this.modelNewMenuItem.Text = "Model New";
            this.modelNewMenuItem.Click += new System.EventHandler(this.modelNewMenuItem_Click);
            // 
            // modelOpenMenuItem
            // 
            this.modelOpenMenuItem.Name = "modelOpenMenuItem";
            this.modelOpenMenuItem.Size = new System.Drawing.Size(272, 36);
            this.modelOpenMenuItem.Text = "Model Open";
            this.modelOpenMenuItem.Click += new System.EventHandler(this.modelOpenMenuItem_Click);
            // 
            // modelSaveMenuItem
            // 
            this.modelSaveMenuItem.Name = "modelSaveMenuItem";
            this.modelSaveMenuItem.Size = new System.Drawing.Size(272, 36);
            this.modelSaveMenuItem.Text = "Model Save";
            this.modelSaveMenuItem.Click += new System.EventHandler(this.modelSaveMenuItem_Click);
            // 
            // modelSaveAsMenuItem
            // 
            this.modelSaveAsMenuItem.Name = "modelSaveAsMenuItem";
            this.modelSaveAsMenuItem.Size = new System.Drawing.Size(272, 36);
            this.modelSaveAsMenuItem.Text = "Model Save As";
            this.modelSaveAsMenuItem.Click += new System.EventHandler(this.modelSaveAsMenuItem_Click);
            // 
            // setupToolStripMenuItem
            // 
            this.setupToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SetupMenuItem});
            this.setupToolStripMenuItem.Name = "setupToolStripMenuItem";
            this.setupToolStripMenuItem.Size = new System.Drawing.Size(92, 31);
            this.setupToolStripMenuItem.Text = "Setup";
            // 
            // SetupMenuItem
            // 
            this.SetupMenuItem.Name = "SetupMenuItem";
            this.SetupMenuItem.Size = new System.Drawing.Size(178, 36);
            this.SetupMenuItem.Text = "Setup";
            this.SetupMenuItem.Click += new System.EventHandler(this.SetupMenuItem_Click);
            // 
            // inspectToolStripMenuItem
            // 
            this.inspectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cycleModeMenuItem});
            this.inspectToolStripMenuItem.Name = "inspectToolStripMenuItem";
            this.inspectToolStripMenuItem.Size = new System.Drawing.Size(106, 31);
            this.inspectToolStripMenuItem.Text = "Inspect";
            // 
            // cycleModeMenuItem
            // 
            this.cycleModeMenuItem.CheckOnClick = true;
            this.cycleModeMenuItem.Name = "cycleModeMenuItem";
            this.cycleModeMenuItem.Size = new System.Drawing.Size(240, 36);
            this.cycleModeMenuItem.Text = "Cycle Mode";
            // 
            // txtFilePath
            // 
            this.txtFilePath.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFilePath.Location = new System.Drawing.Point(266, 4);
            this.txtFilePath.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(869, 30);
            this.txtFilePath.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1143, 675);
            this.Controls.Add(this.txtFilePath);
            this.Controls.Add(this.mainMenu);
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
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
        private System.Windows.Forms.ToolStripMenuItem modelNewMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modelOpenMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modelSaveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modelSaveAsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SetupMenuItem;
        private System.Windows.Forms.ToolStripMenuItem inspectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cycleModeMenuItem;
        private System.Windows.Forms.TextBox txtFilePath;
    }
}