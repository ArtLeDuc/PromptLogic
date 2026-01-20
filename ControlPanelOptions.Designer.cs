namespace PromptLogic
{
    partial class ControlPanelOptions
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.chkControlUncompress = new System.Windows.Forms.RadioButton();
            this.chkControlCompress = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.chkControlHide = new System.Windows.Forms.RadioButton();
            this.chkControlShow = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkBorderHide = new System.Windows.Forms.RadioButton();
            this.chkBorderShow = new System.Windows.Forms.RadioButton();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(324, 28);
            this.panel2.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label1.Location = new System.Drawing.Point(17, 5);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(192, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Border && Control Panel Options";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel3);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Location = new System.Drawing.Point(12, 43);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(300, 160);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " Control Panel ";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.chkControlUncompress);
            this.panel3.Controls.Add(this.chkControlCompress);
            this.panel3.Location = new System.Drawing.Point(71, 88);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(154, 64);
            this.panel3.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(-3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "Display Size";
            // 
            // chkControlUncompress
            // 
            this.chkControlUncompress.Location = new System.Drawing.Point(36, 41);
            this.chkControlUncompress.Name = "chkControlUncompress";
            this.chkControlUncompress.Size = new System.Drawing.Size(105, 20);
            this.chkControlUncompress.TabIndex = 1;
            this.chkControlUncompress.Text = "Uncompress";
            this.chkControlUncompress.UseVisualStyleBackColor = true;
            this.chkControlUncompress.CheckedChanged += new System.EventHandler(this.CompressionChanged);
            // 
            // chkControlCompress
            // 
            this.chkControlCompress.Location = new System.Drawing.Point(36, 21);
            this.chkControlCompress.Name = "chkControlCompress";
            this.chkControlCompress.Size = new System.Drawing.Size(90, 20);
            this.chkControlCompress.TabIndex = 0;
            this.chkControlCompress.Text = "Compress";
            this.chkControlCompress.UseVisualStyleBackColor = true;
            this.chkControlCompress.CheckedChanged += new System.EventHandler(this.CompressionChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.chkControlHide);
            this.panel1.Controls.Add(this.chkControlShow);
            this.panel1.Location = new System.Drawing.Point(71, 22);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(155, 60);
            this.panel1.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(-3, -4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Visibility";
            // 
            // chkControlHide
            // 
            this.chkControlHide.Location = new System.Drawing.Point(37, 36);
            this.chkControlHide.Name = "chkControlHide";
            this.chkControlHide.Size = new System.Drawing.Size(57, 20);
            this.chkControlHide.TabIndex = 1;
            this.chkControlHide.Text = "Hide";
            this.chkControlHide.UseVisualStyleBackColor = true;
            this.chkControlHide.CheckedChanged += new System.EventHandler(this.Visibility_Changed);
            // 
            // chkControlShow
            // 
            this.chkControlShow.Location = new System.Drawing.Point(37, 17);
            this.chkControlShow.Name = "chkControlShow";
            this.chkControlShow.Size = new System.Drawing.Size(61, 20);
            this.chkControlShow.TabIndex = 0;
            this.chkControlShow.Text = "Show";
            this.chkControlShow.UseVisualStyleBackColor = true;
            this.chkControlShow.CheckedChanged += new System.EventHandler(this.Visibility_Changed);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkBorderHide);
            this.groupBox3.Controls.Add(this.chkBorderShow);
            this.groupBox3.Location = new System.Drawing.Point(12, 210);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(300, 79);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Border";
            // 
            // chkBorderHide
            // 
            this.chkBorderHide.Location = new System.Drawing.Point(107, 47);
            this.chkBorderHide.Name = "chkBorderHide";
            this.chkBorderHide.Size = new System.Drawing.Size(57, 20);
            this.chkBorderHide.TabIndex = 3;
            this.chkBorderHide.Text = "Hide";
            this.chkBorderHide.UseVisualStyleBackColor = true;
            this.chkBorderHide.CheckedChanged += new System.EventHandler(this.BorderChanged);
            // 
            // chkBorderShow
            // 
            this.chkBorderShow.Location = new System.Drawing.Point(107, 22);
            this.chkBorderShow.Name = "chkBorderShow";
            this.chkBorderShow.Size = new System.Drawing.Size(61, 20);
            this.chkBorderShow.TabIndex = 2;
            this.chkBorderShow.Text = "Show";
            this.chkBorderShow.UseVisualStyleBackColor = true;
            this.chkBorderShow.CheckedChanged += new System.EventHandler(this.BorderChanged);
            // 
            // ControlPanelOptions
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(324, 299);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ControlPanelOptions";
            this.Text = "Border and Control Panel Options";
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton chkControlHide;
        private System.Windows.Forms.RadioButton chkControlShow;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton chkControlCompress;
        private System.Windows.Forms.RadioButton chkControlUncompress;
        private System.Windows.Forms.RadioButton chkBorderHide;
        private System.Windows.Forms.RadioButton chkBorderShow;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
    }
}