
namespace RCC_DataEval_App
{
    partial class RawDataView
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadRCCsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadRLFsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadPKCsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(2304, 42);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadRCCsToolStripMenuItem,
            this.loadRLFsToolStripMenuItem,
            this.loadPKCsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(72, 38);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadRCCsToolStripMenuItem
            // 
            this.loadRCCsToolStripMenuItem.Name = "loadRCCsToolStripMenuItem";
            this.loadRCCsToolStripMenuItem.Size = new System.Drawing.Size(359, 44);
            this.loadRCCsToolStripMenuItem.Text = "Load RCCs";
            this.loadRCCsToolStripMenuItem.Tag = 0;
            this.loadRCCsToolStripMenuItem.Click += new System.EventHandler(this.loadFilesToolStripMenuItem_Click);
            // 
            // loadRLFsToolStripMenuItem
            // 
            this.loadRLFsToolStripMenuItem.Name = "loadRLFsToolStripMenuItem";
            this.loadRLFsToolStripMenuItem.Size = new System.Drawing.Size(359, 44);
            this.loadRLFsToolStripMenuItem.Text = "Load RLFs";
            this.loadRLFsToolStripMenuItem.Tag = 1;
            this.loadRLFsToolStripMenuItem.Click += new System.EventHandler(this.loadFilesToolStripMenuItem_Click);
            // 
            // loadPKCsToolStripMenuItem
            // 
            this.loadPKCsToolStripMenuItem.Name = "loadPKCsToolStripMenuItem";
            this.loadPKCsToolStripMenuItem.Size = new System.Drawing.Size(359, 44);
            this.loadPKCsToolStripMenuItem.Text = "Load PKCs";
            this.loadPKCsToolStripMenuItem.Tag = 2;
            this.loadRLFsToolStripMenuItem.Click += new System.EventHandler(this.loadFilesToolStripMenuItem_Click);
            // 
            // RawDataView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2304, 1279);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "RawDataView";
            this.Text = "RawDataView";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadRCCsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadRLFsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadPKCsToolStripMenuItem;
    }
}