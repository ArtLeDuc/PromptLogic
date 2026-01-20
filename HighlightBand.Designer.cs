namespace PromptLogic
{
    partial class HighlightBand
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.trkHighlightbandDistanceFromTop = new System.Windows.Forms.TrackBar();
            this.trkHighlightBandOpacity = new System.Windows.Forms.TrackBar();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.numHighLightBandLinesCustom = new System.Windows.Forms.NumericUpDown();
            this.radHighLightBandCustom = new System.Windows.Forms.RadioButton();
            this.radHighLightBand5Lines = new System.Windows.Forms.RadioButton();
            this.radHighLightBand3Lines = new System.Windows.Forms.RadioButton();
            this.label9 = new System.Windows.Forms.Label();
            this.chkHighlightDisplay = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkHighlightbandDistanceFromTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkHighlightBandOpacity)).BeginInit();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHighLightBandLinesCustom)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.trkHighlightbandDistanceFromTop);
            this.panel1.Controls.Add(this.trkHighlightBandOpacity);
            this.panel1.Controls.Add(this.groupBox8);
            this.panel1.Controls.Add(this.chkHighlightDisplay);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(324, 269);
            this.panel1.TabIndex = 0;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(126, 150);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(54, 13);
            this.label10.TabIndex = 54;
            this.label10.Text = "Opacity %";
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(104, 97);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(104, 13);
            this.label12.TabIndex = 55;
            this.label12.Text = "Distance from top %:";
            // 
            // trkHighlightbandDistanceFromTop
            // 
            this.trkHighlightbandDistanceFromTop.Location = new System.Drawing.Point(69, 62);
            this.trkHighlightbandDistanceFromTop.Margin = new System.Windows.Forms.Padding(2);
            this.trkHighlightbandDistanceFromTop.Maximum = 80;
            this.trkHighlightbandDistanceFromTop.Name = "trkHighlightbandDistanceFromTop";
            this.trkHighlightbandDistanceFromTop.Size = new System.Drawing.Size(175, 56);
            this.trkHighlightbandDistanceFromTop.TabIndex = 53;
            this.trkHighlightbandDistanceFromTop.Value = 10;
            this.trkHighlightbandDistanceFromTop.Scroll += new System.EventHandler(this.trkHighlightbandDistanceFromTop_Scroll);
            // 
            // trkHighlightBandOpacity
            // 
            this.trkHighlightBandOpacity.Location = new System.Drawing.Point(69, 111);
            this.trkHighlightBandOpacity.Margin = new System.Windows.Forms.Padding(2);
            this.trkHighlightBandOpacity.Maximum = 100;
            this.trkHighlightBandOpacity.Name = "trkHighlightBandOpacity";
            this.trkHighlightBandOpacity.Size = new System.Drawing.Size(175, 56);
            this.trkHighlightBandOpacity.TabIndex = 52;
            this.trkHighlightBandOpacity.Scroll += new System.EventHandler(this.trkHighlightBandOpacity_Scroll);
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.numHighLightBandLinesCustom);
            this.groupBox8.Controls.Add(this.radHighLightBandCustom);
            this.groupBox8.Controls.Add(this.radHighLightBand5Lines);
            this.groupBox8.Controls.Add(this.radHighLightBand3Lines);
            this.groupBox8.Controls.Add(this.label9);
            this.groupBox8.Location = new System.Drawing.Point(58, 168);
            this.groupBox8.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox8.Size = new System.Drawing.Size(198, 90);
            this.groupBox8.TabIndex = 51;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = " Height ";
            // 
            // numHighLightBandLinesCustom
            // 
            this.numHighLightBandLinesCustom.Enabled = false;
            this.numHighLightBandLinesCustom.Location = new System.Drawing.Point(110, 63);
            this.numHighLightBandLinesCustom.Margin = new System.Windows.Forms.Padding(2);
            this.numHighLightBandLinesCustom.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numHighLightBandLinesCustom.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numHighLightBandLinesCustom.Name = "numHighLightBandLinesCustom";
            this.numHighLightBandLinesCustom.Size = new System.Drawing.Size(43, 22);
            this.numHighLightBandLinesCustom.TabIndex = 4;
            this.numHighLightBandLinesCustom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numHighLightBandLinesCustom.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numHighLightBandLinesCustom.ValueChanged += new System.EventHandler(this.numHighLightBandLinesCustom_ValueChanged);
            // 
            // radHighLightBandCustom
            // 
            this.radHighLightBandCustom.Location = new System.Drawing.Point(20, 63);
            this.radHighLightBandCustom.Margin = new System.Windows.Forms.Padding(2);
            this.radHighLightBandCustom.Name = "radHighLightBandCustom";
            this.radHighLightBandCustom.Size = new System.Drawing.Size(60, 17);
            this.radHighLightBandCustom.TabIndex = 3;
            this.radHighLightBandCustom.TabStop = true;
            this.radHighLightBandCustom.Text = "Custom";
            this.radHighLightBandCustom.UseVisualStyleBackColor = true;
            this.radHighLightBandCustom.CheckedChanged += new System.EventHandler(this.radHighLightBandCustom_CheckedChanged);
            // 
            // radHighLightBand5Lines
            // 
            this.radHighLightBand5Lines.Location = new System.Drawing.Point(20, 42);
            this.radHighLightBand5Lines.Margin = new System.Windows.Forms.Padding(2);
            this.radHighLightBand5Lines.Name = "radHighLightBand5Lines";
            this.radHighLightBand5Lines.Size = new System.Drawing.Size(59, 17);
            this.radHighLightBand5Lines.TabIndex = 2;
            this.radHighLightBand5Lines.TabStop = true;
            this.radHighLightBand5Lines.Text = "5 Lines";
            this.radHighLightBand5Lines.UseVisualStyleBackColor = true;
            this.radHighLightBand5Lines.CheckedChanged += new System.EventHandler(this.radHighLightBand5Lines_CheckedChanged);
            // 
            // radHighLightBand3Lines
            // 
            this.radHighLightBand3Lines.Location = new System.Drawing.Point(20, 20);
            this.radHighLightBand3Lines.Margin = new System.Windows.Forms.Padding(2);
            this.radHighLightBand3Lines.Name = "radHighLightBand3Lines";
            this.radHighLightBand3Lines.Size = new System.Drawing.Size(59, 17);
            this.radHighLightBand3Lines.TabIndex = 1;
            this.radHighLightBand3Lines.TabStop = true;
            this.radHighLightBand3Lines.Tag = "";
            this.radHighLightBand3Lines.Text = "3 Lines";
            this.radHighLightBand3Lines.UseVisualStyleBackColor = true;
            this.radHighLightBand3Lines.CheckedChanged += new System.EventHandler(this.radHighLightBand3Lines_CheckedChanged);
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(77, 65);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(38, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Lines :";
            // 
            // chkHighlightDisplay
            // 
            this.chkHighlightDisplay.Location = new System.Drawing.Point(58, 38);
            this.chkHighlightDisplay.Name = "chkHighlightDisplay";
            this.chkHighlightDisplay.Size = new System.Drawing.Size(79, 19);
            this.chkHighlightDisplay.TabIndex = 1;
            this.chkHighlightDisplay.Text = "Display";
            this.chkHighlightDisplay.UseVisualStyleBackColor = true;
            this.chkHighlightDisplay.CheckedChanged += new System.EventHandler(this.chkHighlightDisplay_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(324, 23);
            this.panel2.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label1.Location = new System.Drawing.Point(13, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Highlight Bar";
            // 
            // HighlightBand
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.ClientSize = new System.Drawing.Size(324, 269);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "HighlightBand";
            this.Text = "HighlightBand";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkHighlightbandDistanceFromTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkHighlightBandOpacity)).EndInit();
            this.groupBox8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numHighLightBandLinesCustom)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkHighlightDisplay;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.NumericUpDown numHighLightBandLinesCustom;
        private System.Windows.Forms.RadioButton radHighLightBandCustom;
        private System.Windows.Forms.RadioButton radHighLightBand5Lines;
        private System.Windows.Forms.RadioButton radHighLightBand3Lines;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TrackBar trkHighlightbandDistanceFromTop;
        private System.Windows.Forms.TrackBar trkHighlightBandOpacity;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label12;
    }
}