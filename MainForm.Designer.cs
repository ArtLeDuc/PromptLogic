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
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnWebDebugger = new System.Windows.Forms.Button();
            this.btnCollapse = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmbStartSlide = new System.Windows.Forms.ComboBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.traSpeed = new System.Windows.Forms.TrackBar();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.pnlCollapsed = new System.Windows.Forms.Panel();
            this.btnExpand = new System.Windows.Forms.Button();
            this.btnCollapsedStop = new System.Windows.Forms.Button();
            this.btnCollapsedPause = new System.Windows.Forms.Button();
            this.btnCollapsedStart = new System.Windows.Forms.Button();
            this.btnCollapsedConnect = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.pnlControl.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.traSpeed)).BeginInit();
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
            this.webView.Name = "webView";
            this.webView.Size = new System.Drawing.Size(518, 450);
            this.webView.TabIndex = 0;
            this.webView.ZoomFactor = 1D;
            // 
            // pnlControl
            // 
            this.pnlControl.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pnlControl.Controls.Add(this.btnSettings);
            this.pnlControl.Controls.Add(this.btnWebDebugger);
            this.pnlControl.Controls.Add(this.btnCollapse);
            this.pnlControl.Controls.Add(this.groupBox1);
            this.pnlControl.Controls.Add(this.groupBox2);
            this.pnlControl.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlControl.Location = new System.Drawing.Point(553, 0);
            this.pnlControl.Name = "pnlControl";
            this.pnlControl.Size = new System.Drawing.Size(258, 450);
            this.pnlControl.TabIndex = 1;
            // 
            // btnSettings
            // 
            this.btnSettings.Location = new System.Drawing.Point(53, 386);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(156, 23);
            this.btnSettings.TabIndex = 13;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // btnWebDebugger
            // 
            this.btnWebDebugger.Location = new System.Drawing.Point(53, 415);
            this.btnWebDebugger.Name = "btnWebDebugger";
            this.btnWebDebugger.Size = new System.Drawing.Size(156, 23);
            this.btnWebDebugger.TabIndex = 12;
            this.btnWebDebugger.Text = "Web Debugger";
            this.btnWebDebugger.UseVisualStyleBackColor = true;
            this.btnWebDebugger.Click += new System.EventHandler(this.btnWebDebugger_Click);
            // 
            // btnCollapse
            // 
            this.btnCollapse.Location = new System.Drawing.Point(0, 332);
            this.btnCollapse.Name = "btnCollapse";
            this.btnCollapse.Size = new System.Drawing.Size(32, 23);
            this.btnCollapse.TabIndex = 11;
            this.btnCollapse.Text = "▶";
            this.btnCollapse.UseVisualStyleBackColor = true;
            this.btnCollapse.Click += new System.EventHandler(this.btnCollapse_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnConnect);
            this.groupBox1.Location = new System.Drawing.Point(6, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(242, 61);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " Select Presentation ";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(47, 21);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(156, 23);
            this.btnConnect.TabIndex = 3;
            this.btnConnect.Text = "🔌 Connect To";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmbStartSlide);
            this.groupBox2.Controls.Add(this.btnStop);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.traSpeed);
            this.groupBox2.Controls.Add(this.btnPause);
            this.groupBox2.Controls.Add(this.btnStart);
            this.groupBox2.Location = new System.Drawing.Point(6, 77);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(242, 240);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = " Show Controls ";
            // 
            // cmbStartSlide
            // 
            this.cmbStartSlide.FormattingEnabled = true;
            this.cmbStartSlide.Location = new System.Drawing.Point(92, 21);
            this.cmbStartSlide.Name = "cmbStartSlide";
            this.cmbStartSlide.Size = new System.Drawing.Size(144, 24);
            this.cmbStartSlide.TabIndex = 8;
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(77, 113);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "■ Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 16);
            this.label1.TabIndex = 7;
            this.label1.Text = "Starting Slide";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(93, 206);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Speed";
            // 
            // traSpeed
            // 
            this.traSpeed.Location = new System.Drawing.Point(63, 166);
            this.traSpeed.Maximum = 100;
            this.traSpeed.Minimum = 1;
            this.traSpeed.Name = "traSpeed";
            this.traSpeed.Size = new System.Drawing.Size(104, 56);
            this.traSpeed.TabIndex = 0;
            this.traSpeed.Value = 1;
            this.traSpeed.ValueChanged += new System.EventHandler(this.traSpeed_ValueChanged);
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(77, 84);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(75, 23);
            this.btnPause.TabIndex = 1;
            this.btnPause.Text = "|| Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(77, 57);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "▶ Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // pnlCollapsed
            // 
            this.pnlCollapsed.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pnlCollapsed.Controls.Add(this.btnExpand);
            this.pnlCollapsed.Controls.Add(this.btnCollapsedStop);
            this.pnlCollapsed.Controls.Add(this.btnCollapsedPause);
            this.pnlCollapsed.Controls.Add(this.btnCollapsedStart);
            this.pnlCollapsed.Controls.Add(this.btnCollapsedConnect);
            this.pnlCollapsed.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlCollapsed.Location = new System.Drawing.Point(518, 0);
            this.pnlCollapsed.Name = "pnlCollapsed";
            this.pnlCollapsed.Size = new System.Drawing.Size(35, 450);
            this.pnlCollapsed.TabIndex = 2;
            // 
            // btnExpand
            // 
            this.btnExpand.Location = new System.Drawing.Point(0, 332);
            this.btnExpand.Name = "btnExpand";
            this.btnExpand.Size = new System.Drawing.Size(32, 23);
            this.btnExpand.TabIndex = 4;
            this.btnExpand.Text = "◀";
            this.btnExpand.UseVisualStyleBackColor = true;
            this.btnExpand.Click += new System.EventHandler(this.btnExpand_Click);
            // 
            // btnCollapsedStop
            // 
            this.btnCollapsedStop.Location = new System.Drawing.Point(0, 190);
            this.btnCollapsedStop.Name = "btnCollapsedStop";
            this.btnCollapsedStop.Size = new System.Drawing.Size(32, 23);
            this.btnCollapsedStop.TabIndex = 3;
            this.btnCollapsedStop.Text = "■";
            this.btnCollapsedStop.UseVisualStyleBackColor = true;
            this.btnCollapsedStop.Click += new System.EventHandler(this.btnCollapsedStop_Click);
            // 
            // btnCollapsedPause
            // 
            this.btnCollapsedPause.Location = new System.Drawing.Point(0, 160);
            this.btnCollapsedPause.Name = "btnCollapsedPause";
            this.btnCollapsedPause.Size = new System.Drawing.Size(32, 23);
            this.btnCollapsedPause.TabIndex = 2;
            this.btnCollapsedPause.Text = "||";
            this.btnCollapsedPause.UseVisualStyleBackColor = true;
            this.btnCollapsedPause.Click += new System.EventHandler(this.btnCollapsedPause_Click);
            // 
            // btnCollapsedStart
            // 
            this.btnCollapsedStart.Location = new System.Drawing.Point(0, 133);
            this.btnCollapsedStart.Name = "btnCollapsedStart";
            this.btnCollapsedStart.Size = new System.Drawing.Size(32, 23);
            this.btnCollapsedStart.TabIndex = 1;
            this.btnCollapsedStart.Text = "▶";
            this.btnCollapsedStart.UseVisualStyleBackColor = true;
            this.btnCollapsedStart.Click += new System.EventHandler(this.btnCollapsedStart_Click);
            // 
            // btnCollapsedConnect
            // 
            this.btnCollapsedConnect.Location = new System.Drawing.Point(0, 34);
            this.btnCollapsedConnect.Name = "btnCollapsedConnect";
            this.btnCollapsedConnect.Size = new System.Drawing.Size(32, 23);
            this.btnCollapsedConnect.TabIndex = 0;
            this.btnCollapsedConnect.Text = "🔌";
            this.btnCollapsedConnect.UseVisualStyleBackColor = true;
            this.btnCollapsedConnect.Click += new System.EventHandler(this.btnCollapsedConnect_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(811, 450);
            this.Controls.Add(this.webView);
            this.Controls.Add(this.pnlCollapsed);
            this.Controls.Add(this.pnlControl);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            this.pnlControl.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.traSpeed)).EndInit();
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
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar traSpeed;
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
    }
}

