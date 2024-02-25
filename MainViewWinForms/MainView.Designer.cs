﻿
namespace MainViewWinForms
{
    partial class MainView
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
            this.importRCCsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importRLFsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importPKCsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preferrenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.rawDataPage = new System.Windows.Forms.TabPage();
            this.analysesPage = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.rawDataPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.preferrenceToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(2363, 48);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importRCCsToolStripMenuItem,
            this.importRLFsToolStripMenuItem,
            this.importPKCsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(72, 44);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // importRCCsToolStripMenuItem
            // 
            this.importRCCsToolStripMenuItem.Name = "importRCCsToolStripMenuItem";
            this.importRCCsToolStripMenuItem.Size = new System.Drawing.Size(359, 44);
            this.importRCCsToolStripMenuItem.Text = "Import RCCs";
            this.importRCCsToolStripMenuItem.Click += new System.EventHandler(this.LoadRCCsToolStripMenuItem_Click);
            // 
            // importRLFsToolStripMenuItem
            // 
            this.importRLFsToolStripMenuItem.Name = "importRLFsToolStripMenuItem";
            this.importRLFsToolStripMenuItem.Size = new System.Drawing.Size(359, 44);
            this.importRLFsToolStripMenuItem.Text = "Import RLFs";
            this.importRLFsToolStripMenuItem.Click += new System.EventHandler(this.LoadRLFsToolStripMenuItem_Click);
            // 
            // importPKCsToolStripMenuItem
            // 
            this.importPKCsToolStripMenuItem.Name = "importPKCsToolStripMenuItem";
            this.importPKCsToolStripMenuItem.Size = new System.Drawing.Size(359, 44);
            this.importPKCsToolStripMenuItem.Text = "Import PKCs";
            this.importPKCsToolStripMenuItem.Click += new System.EventHandler(this.LoadPKCsToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(75, 44);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // preferrenceToolStripMenuItem
            // 
            this.preferrenceToolStripMenuItem.Name = "preferrenceToolStripMenuItem";
            this.preferrenceToolStripMenuItem.Size = new System.Drawing.Size(159, 44);
            this.preferrenceToolStripMenuItem.Text = "Preferences";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(85, 44);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.rawDataPage);
            this.tabControl1.Controls.Add(this.analysesPage);
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(0, 43);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(2363, 1171);
            this.tabControl1.TabIndex = 1;
            // 
            // rawDataPage
            // 
            this.rawDataPage.Controls.Add(this.splitContainer1);
            this.rawDataPage.Location = new System.Drawing.Point(8, 45);
            this.rawDataPage.Name = "rawDataPage";
            this.rawDataPage.Padding = new System.Windows.Forms.Padding(3);
            this.rawDataPage.Size = new System.Drawing.Size(2347, 1118);
            this.rawDataPage.TabIndex = 0;
            this.rawDataPage.Text = "Raw Data";
            this.rawDataPage.UseVisualStyleBackColor = true;
            // 
            // analysesPage
            // 
            this.analysesPage.Location = new System.Drawing.Point(8, 47);
            this.analysesPage.Name = "analysesPage";
            this.analysesPage.Padding = new System.Windows.Forms.Padding(3);
            this.analysesPage.Size = new System.Drawing.Size(2347, 1116);
            this.analysesPage.TabIndex = 1;
            this.analysesPage.Text = "Analyses";
            this.analysesPage.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Size = new System.Drawing.Size(2341, 1112);
            this.splitContainer1.SplitterDistance = 534;
            this.splitContainer1.TabIndex = 0;
            // 
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2363, 1295);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainView";
            this.Text = "Rcc Analysis Tool";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.rawDataPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importRCCsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem preferrenceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importRLFsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importPKCsToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage rawDataPage;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabPage analysesPage;
    }
}