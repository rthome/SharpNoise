namespace NoiseTester
{
    partial class NoiseControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.greyRadioButton = new System.Windows.Forms.RadioButton();
            this.terrainRadioButton = new System.Windows.Forms.RadioButton();
            this.minLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // propertyGrid
            // 
            this.propertyGrid.HelpVisible = false;
            this.propertyGrid.Location = new System.Drawing.Point(3, 26);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.propertyGrid.Size = new System.Drawing.Size(147, 128);
            this.propertyGrid.TabIndex = 3;
            this.propertyGrid.ToolbarVisible = false;
            this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.OnPropertyValueChanged);
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(155, 26);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(128, 128);
            this.pictureBox.TabIndex = 2;
            this.pictureBox.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Perlin";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.greyRadioButton);
            this.panel1.Controls.Add(this.terrainRadioButton);
            this.panel1.Location = new System.Drawing.Point(227, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(56, 23);
            this.panel1.TabIndex = 5;
            // 
            // greyRadioButton
            // 
            this.greyRadioButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.greyRadioButton.AutoSize = true;
            this.greyRadioButton.Location = new System.Drawing.Point(30, 0);
            this.greyRadioButton.Name = "greyRadioButton";
            this.greyRadioButton.Size = new System.Drawing.Size(25, 23);
            this.greyRadioButton.TabIndex = 1;
            this.greyRadioButton.TabStop = true;
            this.greyRadioButton.Text = "G";
            this.greyRadioButton.UseVisualStyleBackColor = true;
            this.greyRadioButton.CheckedChanged += new System.EventHandler(this.OnCheckedChanged);
            // 
            // terrainRadioButton
            // 
            this.terrainRadioButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.terrainRadioButton.AutoSize = true;
            this.terrainRadioButton.Checked = true;
            this.terrainRadioButton.Location = new System.Drawing.Point(0, 0);
            this.terrainRadioButton.Margin = new System.Windows.Forms.Padding(0);
            this.terrainRadioButton.Name = "terrainRadioButton";
            this.terrainRadioButton.Size = new System.Drawing.Size(24, 23);
            this.terrainRadioButton.TabIndex = 0;
            this.terrainRadioButton.TabStop = true;
            this.terrainRadioButton.Text = "T";
            this.terrainRadioButton.UseVisualStyleBackColor = true;
            this.terrainRadioButton.CheckedChanged += new System.EventHandler(this.OnCheckedChanged);
            // 
            // minLabel
            // 
            this.minLabel.AutoSize = true;
            this.minLabel.Location = new System.Drawing.Point(97, 4);
            this.minLabel.Name = "minLabel";
            this.minLabel.Size = new System.Drawing.Size(99, 13);
            this.minLabel.TabIndex = 6;
            this.minLabel.Text = "Min/Max 0.11/0.22";
            // 
            // NoiseControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.minLabel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.propertyGrid);
            this.Name = "NoiseControl";
            this.Size = new System.Drawing.Size(286, 157);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton terrainRadioButton;
        private System.Windows.Forms.RadioButton greyRadioButton;
        private System.Windows.Forms.Label minLabel;
    }
}
