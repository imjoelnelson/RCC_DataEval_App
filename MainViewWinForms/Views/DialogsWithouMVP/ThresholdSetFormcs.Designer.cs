
namespace MainViewWinForms
{
    partial class ThresholdSetFormcs
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
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.imagingUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.sprintUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.daUpDown = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.linearityUpDown = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.coeffUpDown = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.countUpDown = new System.Windows.Forms.NumericUpDown();
            this.threshToLodRadioButton = new System.Windows.Forms.RadioButton();
            this.setToValRadioButton = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.imagingUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sprintUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.daUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.linearityUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.coeffUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.countUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(160, 260);
            this.okButton.Margin = new System.Windows.Forms.Padding(2);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(50, 22);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(214, 260);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(2);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(50, 22);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // imagingUpDown
            // 
            this.imagingUpDown.DecimalPlaces = 2;
            this.imagingUpDown.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.imagingUpDown.Location = new System.Drawing.Point(179, 6);
            this.imagingUpDown.Margin = new System.Windows.Forms.Padding(2);
            this.imagingUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.imagingUpDown.Name = "imagingUpDown";
            this.imagingUpDown.Size = new System.Drawing.Size(48, 20);
            this.imagingUpDown.TabIndex = 2;
            this.imagingUpDown.Value = new decimal(new int[] {
            75,
            0,
            0,
            131072});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(82, 7);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Imaging Threshold";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 27);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(166, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Binding Density Threshold (Sprint)";
            // 
            // sprintUpDown
            // 
            this.sprintUpDown.DecimalPlaces = 2;
            this.sprintUpDown.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.sprintUpDown.Location = new System.Drawing.Point(179, 25);
            this.sprintUpDown.Margin = new System.Windows.Forms.Padding(2);
            this.sprintUpDown.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.sprintUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.sprintUpDown.Name = "sprintUpDown";
            this.sprintUpDown.Size = new System.Drawing.Size(48, 20);
            this.sprintUpDown.TabIndex = 4;
            this.sprintUpDown.Value = new decimal(new int[] {
            18,
            0,
            0,
            65536});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 46);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(154, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Binding Density Threshold (DA)";
            // 
            // daUpDown
            // 
            this.daUpDown.DecimalPlaces = 2;
            this.daUpDown.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.daUpDown.Location = new System.Drawing.Point(179, 45);
            this.daUpDown.Margin = new System.Windows.Forms.Padding(2);
            this.daUpDown.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.daUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.daUpDown.Name = "daUpDown";
            this.daUpDown.Size = new System.Drawing.Size(48, 20);
            this.daUpDown.TabIndex = 6;
            this.daUpDown.Value = new decimal(new int[] {
            225,
            0,
            0,
            131072});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(53, 65);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(121, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "POS Linearity Threshold";
            // 
            // linearityUpDown
            // 
            this.linearityUpDown.DecimalPlaces = 2;
            this.linearityUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.linearityUpDown.Location = new System.Drawing.Point(179, 64);
            this.linearityUpDown.Margin = new System.Windows.Forms.Padding(2);
            this.linearityUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.linearityUpDown.Name = "linearityUpDown";
            this.linearityUpDown.Size = new System.Drawing.Size(48, 20);
            this.linearityUpDown.TabIndex = 8;
            this.linearityUpDown.Value = new decimal(new int[] {
            95,
            0,
            0,
            131072});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 84);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(164, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Std Deviation Coefficient for LOD";
            // 
            // coeffUpDown
            // 
            this.coeffUpDown.Location = new System.Drawing.Point(179, 83);
            this.coeffUpDown.Margin = new System.Windows.Forms.Padding(2);
            this.coeffUpDown.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.coeffUpDown.Name = "coeffUpDown";
            this.coeffUpDown.Size = new System.Drawing.Size(48, 20);
            this.coeffUpDown.TabIndex = 10;
            this.coeffUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(89, 110);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(85, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Count Threshold";
            // 
            // countUpDown
            // 
            this.countUpDown.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.countUpDown.Location = new System.Drawing.Point(213, 153);
            this.countUpDown.Margin = new System.Windows.Forms.Padding(2);
            this.countUpDown.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.countUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.countUpDown.Name = "countUpDown";
            this.countUpDown.Size = new System.Drawing.Size(48, 20);
            this.countUpDown.TabIndex = 12;
            this.countUpDown.Value = new decimal(new int[] {
            40,
            0,
            0,
            0});
            // 
            // threshToLodRadioButton
            // 
            this.threshToLodRadioButton.AutoSize = true;
            this.threshToLodRadioButton.Location = new System.Drawing.Point(179, 108);
            this.threshToLodRadioButton.Name = "threshToLodRadioButton";
            this.threshToLodRadioButton.Size = new System.Drawing.Size(82, 17);
            this.threshToLodRadioButton.TabIndex = 14;
            this.threshToLodRadioButton.TabStop = true;
            this.threshToLodRadioButton.Text = "Set To LOD";
            this.threshToLodRadioButton.UseVisualStyleBackColor = true;
            // 
            // setToValRadioButton
            // 
            this.setToValRadioButton.AutoSize = true;
            this.setToValRadioButton.Location = new System.Drawing.Point(179, 131);
            this.setToValRadioButton.Name = "setToValRadioButton";
            this.setToValRadioButton.Size = new System.Drawing.Size(87, 17);
            this.setToValRadioButton.TabIndex = 15;
            this.setToValRadioButton.TabStop = true;
            this.setToValRadioButton.Text = "Set To Value";
            this.setToValRadioButton.UseVisualStyleBackColor = true;
            // 
            // ThresholdSetFormcs
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(273, 293);
            this.Controls.Add(this.setToValRadioButton);
            this.Controls.Add(this.threshToLodRadioButton);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.countUpDown);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.coeffUpDown);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.linearityUpDown);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.daUpDown);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.sprintUpDown);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.imagingUpDown);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ThresholdSetFormcs";
            this.Text = "Set Thresholds and Values";
            ((System.ComponentModel.ISupportInitialize)(this.imagingUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sprintUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.daUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.linearityUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.coeffUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.countUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.NumericUpDown imagingUpDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown sprintUpDown;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown daUpDown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown linearityUpDown;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown coeffUpDown;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown countUpDown;
        private System.Windows.Forms.RadioButton threshToLodRadioButton;
        private System.Windows.Forms.RadioButton setToValRadioButton;
    }
}