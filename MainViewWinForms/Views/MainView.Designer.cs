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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainView));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importRCCsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importRLFsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importPKCsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearRCCsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.associatePKCsWithRCCsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataAssessmentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.targetCountsTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rawCountsPlateViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sampleVsSampleCorrelationScatterplotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.geneCountBinsStackedBarChartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pCAOverviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analysisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preferrenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setThresholdsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cSVCellToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rawCountTablePreferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.clearButton = new System.Windows.Forms.ToolStripButton();
            this.helpToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.identifyIdealHousekeepersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.dataAssessmentToolStripMenuItem,
            this.analysisToolStripMenuItem,
            this.preferrenceToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1624, 42);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importRCCsToolStripMenuItem,
            this.importRLFsToolStripMenuItem,
            this.importPKCsToolStripMenuItem,
            this.clearRCCsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(72, 38);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // importRCCsToolStripMenuItem
            // 
            this.importRCCsToolStripMenuItem.Name = "importRCCsToolStripMenuItem";
            this.importRCCsToolStripMenuItem.Size = new System.Drawing.Size(280, 44);
            this.importRCCsToolStripMenuItem.Text = "Import RCCs";
            this.importRCCsToolStripMenuItem.Click += new System.EventHandler(this.LoadRCCsToolStripMenuItem_Click);
            // 
            // importRLFsToolStripMenuItem
            // 
            this.importRLFsToolStripMenuItem.Name = "importRLFsToolStripMenuItem";
            this.importRLFsToolStripMenuItem.Size = new System.Drawing.Size(280, 44);
            this.importRLFsToolStripMenuItem.Text = "Import RLFs";
            this.importRLFsToolStripMenuItem.Click += new System.EventHandler(this.LoadRLFsToolStripMenuItem_Click);
            // 
            // importPKCsToolStripMenuItem
            // 
            this.importPKCsToolStripMenuItem.Name = "importPKCsToolStripMenuItem";
            this.importPKCsToolStripMenuItem.Size = new System.Drawing.Size(280, 44);
            this.importPKCsToolStripMenuItem.Text = "Import PKCs";
            this.importPKCsToolStripMenuItem.Click += new System.EventHandler(this.LoadPKCsToolStripMenuItem_Click);
            // 
            // clearRCCsToolStripMenuItem
            // 
            this.clearRCCsToolStripMenuItem.Name = "clearRCCsToolStripMenuItem";
            this.clearRCCsToolStripMenuItem.Size = new System.Drawing.Size(280, 44);
            this.clearRCCsToolStripMenuItem.Text = "Clear RCCs";
            this.clearRCCsToolStripMenuItem.Click += new System.EventHandler(this.clearRCCsToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.associatePKCsWithRCCsToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(75, 38);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // associatePKCsWithRCCsToolStripMenuItem
            // 
            this.associatePKCsWithRCCsToolStripMenuItem.Name = "associatePKCsWithRCCsToolStripMenuItem";
            this.associatePKCsWithRCCsToolStripMenuItem.Size = new System.Drawing.Size(374, 44);
            this.associatePKCsWithRCCsToolStripMenuItem.Text = "Edit PKC Associations";
            this.associatePKCsWithRCCsToolStripMenuItem.ToolTipText = "Edit PKC Associations";
            this.associatePKCsWithRCCsToolStripMenuItem.Click += new System.EventHandler(this.associatePKCsWithRCCsToolStripMenuItem_Click);
            // 
            // dataAssessmentToolStripMenuItem
            // 
            this.dataAssessmentToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.targetCountsTableToolStripMenuItem,
            this.rawCountsPlateViewToolStripMenuItem,
            this.sampleVsSampleCorrelationScatterplotToolStripMenuItem,
            this.geneCountBinsStackedBarChartToolStripMenuItem,
            this.identifyIdealHousekeepersToolStripMenuItem,
            this.pCAOverviewToolStripMenuItem});
            this.dataAssessmentToolStripMenuItem.Name = "dataAssessmentToolStripMenuItem";
            this.dataAssessmentToolStripMenuItem.Size = new System.Drawing.Size(145, 38);
            this.dataAssessmentToolStripMenuItem.Text = "Evaluation";
            // 
            // targetCountsTableToolStripMenuItem
            // 
            this.targetCountsTableToolStripMenuItem.Name = "targetCountsTableToolStripMenuItem";
            this.targetCountsTableToolStripMenuItem.Size = new System.Drawing.Size(580, 44);
            this.targetCountsTableToolStripMenuItem.Text = "Raw Counts Table";
            this.targetCountsTableToolStripMenuItem.Click += new System.EventHandler(this.rawCountsTableToolStripMenuItem_Click);
            // 
            // rawCountsPlateViewToolStripMenuItem
            // 
            this.rawCountsPlateViewToolStripMenuItem.Enabled = false;
            this.rawCountsPlateViewToolStripMenuItem.Name = "rawCountsPlateViewToolStripMenuItem";
            this.rawCountsPlateViewToolStripMenuItem.Size = new System.Drawing.Size(580, 44);
            this.rawCountsPlateViewToolStripMenuItem.Text = "Raw Counts Plate View";
            this.rawCountsPlateViewToolStripMenuItem.Click += new System.EventHandler(this.rawCountsPlateViewToolStripMenuItem_Click);
            // 
            // sampleVsSampleCorrelationScatterplotToolStripMenuItem
            // 
            this.sampleVsSampleCorrelationScatterplotToolStripMenuItem.Name = "sampleVsSampleCorrelationScatterplotToolStripMenuItem";
            this.sampleVsSampleCorrelationScatterplotToolStripMenuItem.Size = new System.Drawing.Size(580, 44);
            this.sampleVsSampleCorrelationScatterplotToolStripMenuItem.Text = "Sample vs sample correlation scatterplot";
            this.sampleVsSampleCorrelationScatterplotToolStripMenuItem.Click += new System.EventHandler(this.sampleVsSampleCorrelationScatterplotToolStripMenuItem_Click);
            // 
            // geneCountBinsStackedBarChartToolStripMenuItem
            // 
            this.geneCountBinsStackedBarChartToolStripMenuItem.Name = "geneCountBinsStackedBarChartToolStripMenuItem";
            this.geneCountBinsStackedBarChartToolStripMenuItem.Size = new System.Drawing.Size(580, 44);
            this.geneCountBinsStackedBarChartToolStripMenuItem.Text = "Count Ranges Bar Chart";
            this.geneCountBinsStackedBarChartToolStripMenuItem.Click += new System.EventHandler(this.geneCountBinsStackedBarChartToolStripMenuItem_Click);
            // 
            // pCAOverviewToolStripMenuItem
            // 
            this.pCAOverviewToolStripMenuItem.Name = "pCAOverviewToolStripMenuItem";
            this.pCAOverviewToolStripMenuItem.Size = new System.Drawing.Size(580, 44);
            this.pCAOverviewToolStripMenuItem.Text = "PCA Overview";
            this.pCAOverviewToolStripMenuItem.Click += new System.EventHandler(this.pCAOverviewToolStripMenuItem_Click);
            // 
            // analysisToolStripMenuItem
            // 
            this.analysisToolStripMenuItem.Name = "analysisToolStripMenuItem";
            this.analysisToolStripMenuItem.Size = new System.Drawing.Size(120, 38);
            this.analysisToolStripMenuItem.Text = "Analysis";
            // 
            // preferrenceToolStripMenuItem
            // 
            this.preferrenceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setThresholdsToolStripMenuItem,
            this.cSVCellToolStripMenuItem,
            this.rawCountTablePreferencesToolStripMenuItem});
            this.preferrenceToolStripMenuItem.Name = "preferrenceToolStripMenuItem";
            this.preferrenceToolStripMenuItem.Size = new System.Drawing.Size(159, 38);
            this.preferrenceToolStripMenuItem.Text = "Preferences";
            // 
            // setThresholdsToolStripMenuItem
            // 
            this.setThresholdsToolStripMenuItem.Name = "setThresholdsToolStripMenuItem";
            this.setThresholdsToolStripMenuItem.Size = new System.Drawing.Size(457, 44);
            this.setThresholdsToolStripMenuItem.Text = "Set Thresholds";
            this.setThresholdsToolStripMenuItem.Click += new System.EventHandler(this.setThresholdsToolStripMenuItem_Click);
            // 
            // cSVCellToolStripMenuItem
            // 
            this.cSVCellToolStripMenuItem.Name = "cSVCellToolStripMenuItem";
            this.cSVCellToolStripMenuItem.Size = new System.Drawing.Size(457, 44);
            this.cSVCellToolStripMenuItem.Text = "CSV Field Delimiter";
            // 
            // rawCountTablePreferencesToolStripMenuItem
            // 
            this.rawCountTablePreferencesToolStripMenuItem.Name = "rawCountTablePreferencesToolStripMenuItem";
            this.rawCountTablePreferencesToolStripMenuItem.Size = new System.Drawing.Size(457, 44);
            this.rawCountTablePreferencesToolStripMenuItem.Text = "Raw Count Table Preferences";
            this.rawCountTablePreferencesToolStripMenuItem.Click += new System.EventHandler(this.rawCountTablePreferencesToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(85, 38);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.Location = new System.Drawing.Point(0, 98);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1624, 960);
            this.panel1.TabIndex = 1;
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.toolStrip1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripButton,
            this.clearButton,
            this.helpToolStripButton,
            this.toolStripSeparator1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 42);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStrip1.Size = new System.Drawing.Size(1624, 52);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // openToolStripButton
            // 
            this.openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripButton.Image")));
            this.openToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.openToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripButton.Name = "openToolStripButton";
            this.openToolStripButton.Size = new System.Drawing.Size(46, 46);
            this.openToolStripButton.Text = "Load RCCs";
            this.openToolStripButton.Click += new System.EventHandler(this.LoadRCCsToolStripMenuItem_Click);
            // 
            // clearButton
            // 
            this.clearButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearButton.Image = global::MainViewWinForms.Properties.Resources.remove_icon_png_7116;
            this.clearButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.clearButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(46, 46);
            this.clearButton.Text = "Clear RCCs";
            this.clearButton.Click += new System.EventHandler(this.clearRCCsToolStripMenuItem_Click);
            // 
            // helpToolStripButton
            // 
            this.helpToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.helpToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("helpToolStripButton.Image")));
            this.helpToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.helpToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.helpToolStripButton.Name = "helpToolStripButton";
            this.helpToolStripButton.Size = new System.Drawing.Size(46, 46);
            this.helpToolStripButton.Text = "He&lp";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 52);
            // 
            // identifyIdealHousekeepersToolStripMenuItem
            // 
            this.identifyIdealHousekeepersToolStripMenuItem.Name = "identifyIdealHousekeepersToolStripMenuItem";
            this.identifyIdealHousekeepersToolStripMenuItem.Size = new System.Drawing.Size(580, 44);
            this.identifyIdealHousekeepersToolStripMenuItem.Text = "Evaluate Housekeepers";
            this.identifyIdealHousekeepersToolStripMenuItem.Click += new System.EventHandler(this.identifyIdealHousekeepersToolStripMenuItem_Click);
            // 
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1624, 1062);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainView";
            this.Text = "Rcc Analysis Tool";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
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
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem setThresholdsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearRCCsToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton openToolStripButton;
        private System.Windows.Forms.ToolStripButton clearButton;
        private System.Windows.Forms.ToolStripButton helpToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem cSVCellToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dataAssessmentToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem targetCountsTableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rawCountTablePreferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rawCountsPlateViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sampleVsSampleCorrelationScatterplotToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem associatePKCsWithRCCsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem geneCountBinsStackedBarChartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem analysisToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pCAOverviewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem identifyIdealHousekeepersToolStripMenuItem;
    }
}