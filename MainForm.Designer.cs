namespace Teleprompter
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
            this.components = new System.ComponentModel.Container();
            this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.pnlControl = new System.Windows.Forms.Panel();
            this.btnLoadSampleScript = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnWebDebugger = new System.Windows.Forms.Button();
            this.btnCollapse = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.grpShowControls = new System.Windows.Forms.GroupBox();
            this.cmbStartSlide = new System.Windows.Forms.ComboBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.pnlCollapsed = new System.Windows.Forms.Panel();
            this.btnExpand = new System.Windows.Forms.Button();
            this.btnCollapsedStop = new System.Windows.Forms.Button();
            this.btnCollapsedPause = new System.Windows.Forms.Button();
            this.btnCollapsedStart = new System.Windows.Forms.Button();
            this.btnCollapsedConnect = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.pnlControl.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpShowControls.SuspendLayout();
            this.pnlCollapsed.SuspendLayout();
            this.SuspendLayout();
            // 
            // webView
            // 
            this.webView.AllowExternalDrop = true;
            this.webView.CreationProperties = null;
            this.webView.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView.Location = new System.Drawing.Point(0, 0);
            this.webView.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.webView.Name = "webView";
            this.webView.Size = new System.Drawing.Size(388, 366);
            this.webView.TabIndex = 0;
            this.webView.ZoomFactor = 1D;
            // 
            // pnlControl
            // 
            this.pnlControl.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pnlControl.Controls.Add(this.btnLoadSampleScript);
            this.pnlControl.Controls.Add(this.btnSettings);
            this.pnlControl.Controls.Add(this.btnWebDebugger);
            this.pnlControl.Controls.Add(this.btnCollapse);
            this.pnlControl.Controls.Add(this.groupBox1);
            this.pnlControl.Controls.Add(this.grpShowControls);
            this.pnlControl.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlControl.Location = new System.Drawing.Point(414, 0);
            this.pnlControl.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pnlControl.Name = "pnlControl";
            this.pnlControl.Size = new System.Drawing.Size(194, 366);
            this.pnlControl.TabIndex = 1;
            // 
            // btnLoadSampleScript
            // 
            this.btnLoadSampleScript.Location = new System.Drawing.Point(40, 290);
            this.btnLoadSampleScript.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnLoadSampleScript.Name = "btnLoadSampleScript";
            this.btnLoadSampleScript.Size = new System.Drawing.Size(117, 19);
            this.btnLoadSampleScript.TabIndex = 14;
            this.btnLoadSampleScript.TabStop = false;
            this.btnLoadSampleScript.Text = "Load Sample Script";
            this.btnLoadSampleScript.UseVisualStyleBackColor = true;
            this.btnLoadSampleScript.Click += new System.EventHandler(this.btnLoadSampleScript_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Location = new System.Drawing.Point(40, 314);
            this.btnSettings.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(117, 19);
            this.btnSettings.TabIndex = 13;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // btnWebDebugger
            // 
            this.btnWebDebugger.Location = new System.Drawing.Point(40, 337);
            this.btnWebDebugger.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnWebDebugger.Name = "btnWebDebugger";
            this.btnWebDebugger.Size = new System.Drawing.Size(117, 19);
            this.btnWebDebugger.TabIndex = 12;
            this.btnWebDebugger.Text = "Web Debugger";
            this.btnWebDebugger.UseVisualStyleBackColor = true;
            this.btnWebDebugger.Click += new System.EventHandler(this.btnWebDebugger_Click);
            // 
            // btnCollapse
            // 
            this.btnCollapse.Location = new System.Drawing.Point(0, 270);
            this.btnCollapse.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnCollapse.Name = "btnCollapse";
            this.btnCollapse.Size = new System.Drawing.Size(24, 19);
            this.btnCollapse.TabIndex = 11;
            this.btnCollapse.Text = "▶";
            this.btnCollapse.UseVisualStyleBackColor = true;
            this.btnCollapse.Click += new System.EventHandler(this.btnCollapse_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnConnect);
            this.groupBox1.Location = new System.Drawing.Point(4, 11);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Size = new System.Drawing.Size(182, 50);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " Select Presentation ";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(35, 17);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(117, 19);
            this.btnConnect.TabIndex = 3;
            this.btnConnect.Text = "🔌 Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // grpShowControls
            // 
            this.grpShowControls.Controls.Add(this.button3);
            this.grpShowControls.Controls.Add(this.button1);
            this.grpShowControls.Controls.Add(this.cmbStartSlide);
            this.grpShowControls.Controls.Add(this.btnStop);
            this.grpShowControls.Controls.Add(this.label1);
            this.grpShowControls.Controls.Add(this.btnPause);
            this.grpShowControls.Controls.Add(this.btnStart);
            this.grpShowControls.Location = new System.Drawing.Point(4, 63);
            this.grpShowControls.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.grpShowControls.Name = "grpShowControls";
            this.grpShowControls.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.grpShowControls.Size = new System.Drawing.Size(182, 195);
            this.grpShowControls.TabIndex = 9;
            this.grpShowControls.TabStop = false;
            this.grpShowControls.Text = " Show Controls ";
            // 
            // cmbStartSlide
            // 
            this.cmbStartSlide.FormattingEnabled = true;
            this.cmbStartSlide.Location = new System.Drawing.Point(69, 17);
            this.cmbStartSlide.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cmbStartSlide.Name = "cmbStartSlide";
            this.cmbStartSlide.Size = new System.Drawing.Size(109, 21);
            this.cmbStartSlide.TabIndex = 8;
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(49, 92);
            this.btnStop.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(82, 23);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "■ Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Starting Slide";
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(49, 68);
            this.btnPause.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(82, 23);
            this.btnPause.TabIndex = 1;
            this.btnPause.Text = "|| Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(49, 46);
            this.btnStart.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(82, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "▶ Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // pnlCollapsed
            // 
            this.pnlCollapsed.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pnlCollapsed.Controls.Add(this.button4);
            this.pnlCollapsed.Controls.Add(this.button2);
            this.pnlCollapsed.Controls.Add(this.btnExpand);
            this.pnlCollapsed.Controls.Add(this.btnCollapsedStop);
            this.pnlCollapsed.Controls.Add(this.btnCollapsedPause);
            this.pnlCollapsed.Controls.Add(this.btnCollapsedStart);
            this.pnlCollapsed.Controls.Add(this.btnCollapsedConnect);
            this.pnlCollapsed.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlCollapsed.Location = new System.Drawing.Point(388, 0);
            this.pnlCollapsed.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pnlCollapsed.Name = "pnlCollapsed";
            this.pnlCollapsed.Size = new System.Drawing.Size(26, 366);
            this.pnlCollapsed.TabIndex = 2;
            // 
            // btnExpand
            // 
            this.btnExpand.Location = new System.Drawing.Point(0, 270);
            this.btnExpand.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnExpand.Name = "btnExpand";
            this.btnExpand.Size = new System.Drawing.Size(24, 19);
            this.btnExpand.TabIndex = 4;
            this.btnExpand.Text = "◀";
            this.btnExpand.UseVisualStyleBackColor = true;
            this.btnExpand.Click += new System.EventHandler(this.btnExpand_Click);
            // 
            // btnCollapsedStop
            // 
            this.btnCollapsedStop.Location = new System.Drawing.Point(0, 154);
            this.btnCollapsedStop.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnCollapsedStop.Name = "btnCollapsedStop";
            this.btnCollapsedStop.Size = new System.Drawing.Size(24, 19);
            this.btnCollapsedStop.TabIndex = 3;
            this.btnCollapsedStop.Text = "■";
            this.btnCollapsedStop.UseVisualStyleBackColor = true;
            this.btnCollapsedStop.Click += new System.EventHandler(this.btnCollapsedStop_Click);
            // 
            // btnCollapsedPause
            // 
            this.btnCollapsedPause.Location = new System.Drawing.Point(0, 130);
            this.btnCollapsedPause.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnCollapsedPause.Name = "btnCollapsedPause";
            this.btnCollapsedPause.Size = new System.Drawing.Size(24, 19);
            this.btnCollapsedPause.TabIndex = 2;
            this.btnCollapsedPause.Text = "||";
            this.btnCollapsedPause.UseVisualStyleBackColor = true;
            this.btnCollapsedPause.Click += new System.EventHandler(this.btnCollapsedPause_Click);
            // 
            // btnCollapsedStart
            // 
            this.btnCollapsedStart.Location = new System.Drawing.Point(0, 108);
            this.btnCollapsedStart.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnCollapsedStart.Name = "btnCollapsedStart";
            this.btnCollapsedStart.Size = new System.Drawing.Size(24, 19);
            this.btnCollapsedStart.TabIndex = 1;
            this.btnCollapsedStart.Text = "▶";
            this.btnCollapsedStart.UseVisualStyleBackColor = true;
            this.btnCollapsedStart.Click += new System.EventHandler(this.btnCollapsedStart_Click);
            // 
            // btnCollapsedConnect
            // 
            this.btnCollapsedConnect.Location = new System.Drawing.Point(0, 28);
            this.btnCollapsedConnect.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnCollapsedConnect.Name = "btnCollapsedConnect";
            this.btnCollapsedConnect.Size = new System.Drawing.Size(24, 19);
            this.btnCollapsedConnect.TabIndex = 0;
            this.btnCollapsedConnect.Text = "🔌";
            this.btnCollapsedConnect.UseVisualStyleBackColor = true;
            this.btnCollapsedConnect.Click += new System.EventHandler(this.btnCollapsedConnect_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(49, 133);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(82, 23);
            this.button1.TabIndex = 9;
            this.button1.Text = "⏩Speed...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(0, 196);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(24, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "⏩";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(49, 163);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(82, 23);
            this.button3.TabIndex = 10;
            this.button3.Text = "▬ Highlight";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(0, 226);
            this.button4.Margin = new System.Windows.Forms.Padding(2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(24, 23);
            this.button4.TabIndex = 6;
            this.button4.Text = "▬";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 366);
            this.Controls.Add(this.webView);
            this.Controls.Add(this.pnlCollapsed);
            this.Controls.Add(this.pnlControl);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "MainForm";
            this.Text = "PromptLogic";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            this.pnlControl.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.grpShowControls.ResumeLayout(false);
            this.grpShowControls.PerformLayout();
            this.pnlCollapsed.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private System.Windows.Forms.Panel pnlControl;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox grpShowControls;
        private System.Windows.Forms.ComboBox cmbStartSlide;
        private System.Windows.Forms.Button btnCollapse;
        private System.Windows.Forms.Panel pnlCollapsed;
        private System.Windows.Forms.Button btnExpand;
        private System.Windows.Forms.Button btnCollapsedStop;
        private System.Windows.Forms.Button btnCollapsedPause;
        private System.Windows.Forms.Button btnCollapsedStart;
        private System.Windows.Forms.Button btnCollapsedConnect;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnWebDebugger;
        private System.Windows.Forms.Button btnLoadSampleScript;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
    }
}

