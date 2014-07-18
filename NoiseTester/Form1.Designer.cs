namespace NoiseTester
{
    partial class Form1
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.xNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.wNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.hNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.yNumericUpDown = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.xNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.hNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.AutoScroll = true;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 46);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(857, 479);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // xNumericUpDown
            // 
            this.xNumericUpDown.DecimalPlaces = 2;
            this.xNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.xNumericUpDown.Location = new System.Drawing.Point(32, 12);
            this.xNumericUpDown.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.xNumericUpDown.Name = "xNumericUpDown";
            this.xNumericUpDown.Size = new System.Drawing.Size(51, 20);
            this.xNumericUpDown.TabIndex = 4;
            this.xNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            // 
            // wNumericUpDown
            // 
            this.wNumericUpDown.DecimalPlaces = 2;
            this.wNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.wNumericUpDown.Location = new System.Drawing.Point(113, 12);
            this.wNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.wNumericUpDown.Name = "wNumericUpDown";
            this.wNumericUpDown.Size = new System.Drawing.Size(50, 20);
            this.wNumericUpDown.TabIndex = 5;
            this.wNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "X";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(89, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "W";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(246, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(15, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "H";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(169, 14);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Y";
            // 
            // hNumericUpDown
            // 
            this.hNumericUpDown.DecimalPlaces = 2;
            this.hNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.hNumericUpDown.Location = new System.Drawing.Point(270, 12);
            this.hNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.hNumericUpDown.Name = "hNumericUpDown";
            this.hNumericUpDown.Size = new System.Drawing.Size(50, 20);
            this.hNumericUpDown.TabIndex = 9;
            this.hNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            // 
            // yNumericUpDown
            // 
            this.yNumericUpDown.DecimalPlaces = 2;
            this.yNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.yNumericUpDown.Location = new System.Drawing.Point(189, 12);
            this.yNumericUpDown.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.yNumericUpDown.Name = "yNumericUpDown";
            this.yNumericUpDown.Size = new System.Drawing.Size(51, 20);
            this.yNumericUpDown.TabIndex = 8;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(881, 537);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.hNumericUpDown);
            this.Controls.Add(this.yNumericUpDown);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.wNumericUpDown);
            this.Controls.Add(this.xNumericUpDown);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form1";
            this.Text = "NoiseTester";
            ((System.ComponentModel.ISupportInitialize)(this.xNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.hNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.NumericUpDown xNumericUpDown;
        private System.Windows.Forms.NumericUpDown wNumericUpDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown hNumericUpDown;
        private System.Windows.Forms.NumericUpDown yNumericUpDown;

    }
}

