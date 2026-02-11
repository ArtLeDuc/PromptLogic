namespace PromptLogic
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            pnlControl = new Panel();
            btnSettings = new Button();
            btnWebDebugger = new Button();
            btnCollapse = new Button();
            groupBox1 = new GroupBox();
            btnLoadSampleScript = new Button();
            btnConnect = new Button();
            grpShowControls = new GroupBox();
            btnBorderNControl = new Button();
            button3 = new Button();
            button1 = new Button();
            cmbStartSlide = new ComboBox();
            btnStop = new Button();
            txtStartingSlide = new Label();
            btnPause = new Button();
            btnStart = new Button();
            pnlCollapsed = new Panel();
            btnCollapsedControlPnl = new Button();
            btnCollapsedHighlight = new Button();
            btnCollapsedSpeed = new Button();
            btnExpand = new Button();
            btnCollapsedStop = new Button();
            btnCollapsedPause = new Button();
            btnCollapsedStart = new Button();
            btnCollapsedConnect = new Button();
            toolTip1 = new ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)webView).BeginInit();
            pnlControl.SuspendLayout();
            groupBox1.SuspendLayout();
            grpShowControls.SuspendLayout();
            pnlCollapsed.SuspendLayout();
            SuspendLayout();
            // 
            // webView
            // 
            webView.AllowExternalDrop = true;
            webView.CreationProperties = null;
            webView.DefaultBackgroundColor = Color.White;
            webView.Dock = DockStyle.Fill;
            webView.Location = new Point(0, 0);
            webView.Margin = new Padding(3, 2, 3, 2);
            webView.Name = "webView";
            webView.Size = new Size(521, 422);
            webView.TabIndex = 0;
            webView.ZoomFactor = 1D;
            // 
            // pnlControl
            // 
            pnlControl.BackColor = SystemColors.ControlDark;
            pnlControl.Controls.Add(btnSettings);
            pnlControl.Controls.Add(btnWebDebugger);
            pnlControl.Controls.Add(btnCollapse);
            pnlControl.Controls.Add(groupBox1);
            pnlControl.Controls.Add(grpShowControls);
            pnlControl.Dock = DockStyle.Right;
            pnlControl.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            pnlControl.Location = new Point(554, 0);
            pnlControl.Margin = new Padding(3, 2, 3, 2);
            pnlControl.Name = "pnlControl";
            pnlControl.Size = new Size(223, 422);
            pnlControl.TabIndex = 1;
            // 
            // btnSettings
            // 
            btnSettings.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnSettings.Location = new Point(46, 363);
            btnSettings.Margin = new Padding(3, 2, 3, 2);
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new Size(136, 26);
            btnSettings.TabIndex = 13;
            btnSettings.Text = "Settings...";
            btnSettings.UseVisualStyleBackColor = true;
            btnSettings.Click += btnSettings_Click;
            // 
            // btnWebDebugger
            // 
            btnWebDebugger.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnWebDebugger.Location = new Point(46, 390);
            btnWebDebugger.Margin = new Padding(3, 2, 3, 2);
            btnWebDebugger.Name = "btnWebDebugger";
            btnWebDebugger.Size = new Size(136, 26);
            btnWebDebugger.TabIndex = 12;
            btnWebDebugger.Text = "Web Debugger";
            btnWebDebugger.UseVisualStyleBackColor = true;
            btnWebDebugger.Click += btnWebDebugger_Click;
            // 
            // btnCollapse
            // 
            btnCollapse.Location = new Point(0, 367);
            btnCollapse.Margin = new Padding(3, 2, 3, 2);
            btnCollapse.Name = "btnCollapse";
            btnCollapse.Size = new Size(28, 26);
            btnCollapse.TabIndex = 11;
            btnCollapse.Text = "▶";
            btnCollapse.UseVisualStyleBackColor = true;
            btnCollapse.Click += btnCollapse_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnLoadSampleScript);
            groupBox1.Controls.Add(btnConnect);
            groupBox1.Location = new Point(4, 13);
            groupBox1.Margin = new Padding(3, 2, 3, 2);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(3, 2, 3, 2);
            groupBox1.Size = new Size(213, 91);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            groupBox1.Text = " Select Presentation ";
            // 
            // btnLoadSampleScript
            // 
            btnLoadSampleScript.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnLoadSampleScript.Location = new Point(41, 50);
            btnLoadSampleScript.Margin = new Padding(3, 2, 3, 2);
            btnLoadSampleScript.Name = "btnLoadSampleScript";
            btnLoadSampleScript.Size = new Size(136, 26);
            btnLoadSampleScript.TabIndex = 14;
            btnLoadSampleScript.TabStop = false;
            btnLoadSampleScript.Text = "Load Sample Script";
            btnLoadSampleScript.UseVisualStyleBackColor = true;
            btnLoadSampleScript.Click += btnLoadSampleScript_Click;
            // 
            // btnConnect
            // 
            btnConnect.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnConnect.Location = new Point(41, 20);
            btnConnect.Margin = new Padding(3, 2, 3, 2);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(136, 26);
            btnConnect.TabIndex = 3;
            btnConnect.Text = "🔌 Connect";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // grpShowControls
            // 
            grpShowControls.Controls.Add(btnBorderNControl);
            grpShowControls.Controls.Add(button3);
            grpShowControls.Controls.Add(button1);
            grpShowControls.Controls.Add(cmbStartSlide);
            grpShowControls.Controls.Add(btnStop);
            grpShowControls.Controls.Add(txtStartingSlide);
            grpShowControls.Controls.Add(btnPause);
            grpShowControls.Controls.Add(btnStart);
            grpShowControls.Location = new Point(5, 108);
            grpShowControls.Margin = new Padding(3, 2, 3, 2);
            grpShowControls.Name = "grpShowControls";
            grpShowControls.Padding = new Padding(3, 2, 3, 2);
            grpShowControls.Size = new Size(213, 248);
            grpShowControls.TabIndex = 9;
            grpShowControls.TabStop = false;
            grpShowControls.Text = " Show Controls ";
            // 
            // btnBorderNControl
            // 
            btnBorderNControl.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnBorderNControl.Location = new Point(41, 208);
            btnBorderNControl.Name = "btnBorderNControl";
            btnBorderNControl.Size = new Size(136, 26);
            btnBorderNControl.TabIndex = 11;
            btnBorderNControl.Text = "🗔 Control Panel...";
            btnBorderNControl.UseVisualStyleBackColor = true;
            btnBorderNControl.Click += btnBorderNControl_Click;
            // 
            // button3
            // 
            button3.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button3.Location = new Point(41, 182);
            button3.Margin = new Padding(4);
            button3.Name = "button3";
            button3.Size = new Size(136, 26);
            button3.TabIndex = 10;
            button3.Text = "▬ Highlight";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button1.Location = new Point(41, 156);
            button1.Margin = new Padding(4);
            button1.Name = "button1";
            button1.Size = new Size(136, 26);
            button1.TabIndex = 9;
            button1.Text = "⏩Speed...";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // cmbStartSlide
            // 
            cmbStartSlide.FormattingEnabled = true;
            cmbStartSlide.Location = new Point(94, 20);
            cmbStartSlide.Margin = new Padding(3, 2, 3, 2);
            cmbStartSlide.Name = "cmbStartSlide";
            cmbStartSlide.Size = new Size(113, 23);
            cmbStartSlide.TabIndex = 8;
            cmbStartSlide.Visible = false;
            // 
            // btnStop
            // 
            btnStop.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnStop.Location = new Point(41, 107);
            btnStop.Margin = new Padding(3, 2, 3, 2);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(136, 26);
            btnStop.TabIndex = 2;
            btnStop.Text = "■ Stop";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // txtStartingSlide
            // 
            txtStartingSlide.AutoSize = true;
            txtStartingSlide.Location = new Point(3, 23);
            txtStartingSlide.Name = "txtStartingSlide";
            txtStartingSlide.Size = new Size(76, 15);
            txtStartingSlide.TabIndex = 7;
            txtStartingSlide.Text = "Starting Slide";
            txtStartingSlide.Visible = false;
            // 
            // btnPause
            // 
            btnPause.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnPause.Location = new Point(41, 81);
            btnPause.Margin = new Padding(3, 2, 3, 2);
            btnPause.Name = "btnPause";
            btnPause.Size = new Size(136, 26);
            btnPause.TabIndex = 1;
            btnPause.Text = "❚❚  Pause";
            btnPause.UseVisualStyleBackColor = true;
            btnPause.Click += btnPause_Click;
            // 
            // btnStart
            // 
            btnStart.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnStart.Location = new Point(41, 55);
            btnStart.Margin = new Padding(3, 2, 3, 2);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(136, 26);
            btnStart.TabIndex = 0;
            btnStart.Text = "▶ Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // pnlCollapsed
            // 
            pnlCollapsed.BackColor = SystemColors.ControlDark;
            pnlCollapsed.Controls.Add(btnCollapsedControlPnl);
            pnlCollapsed.Controls.Add(btnCollapsedHighlight);
            pnlCollapsed.Controls.Add(btnCollapsedSpeed);
            pnlCollapsed.Controls.Add(btnExpand);
            pnlCollapsed.Controls.Add(btnCollapsedStop);
            pnlCollapsed.Controls.Add(btnCollapsedPause);
            pnlCollapsed.Controls.Add(btnCollapsedStart);
            pnlCollapsed.Controls.Add(btnCollapsedConnect);
            pnlCollapsed.Dock = DockStyle.Right;
            pnlCollapsed.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            pnlCollapsed.Location = new Point(521, 0);
            pnlCollapsed.Margin = new Padding(3, 2, 3, 2);
            pnlCollapsed.Name = "pnlCollapsed";
            pnlCollapsed.Size = new Size(33, 422);
            pnlCollapsed.TabIndex = 2;
            // 
            // btnCollapsedControlPnl
            // 
            btnCollapsedControlPnl.Location = new Point(3, 316);
            btnCollapsedControlPnl.Margin = new Padding(3, 2, 3, 2);
            btnCollapsedControlPnl.Name = "btnCollapsedControlPnl";
            btnCollapsedControlPnl.Size = new Size(28, 26);
            btnCollapsedControlPnl.TabIndex = 7;
            btnCollapsedControlPnl.Text = "🗔";
            btnCollapsedControlPnl.UseVisualStyleBackColor = true;
            btnCollapsedControlPnl.Click += btnCollapsedControlPnl_Click;
            // 
            // btnCollapsedHighlight
            // 
            btnCollapsedHighlight.Location = new Point(3, 290);
            btnCollapsedHighlight.Margin = new Padding(3, 2, 3, 2);
            btnCollapsedHighlight.Name = "btnCollapsedHighlight";
            btnCollapsedHighlight.Size = new Size(28, 26);
            btnCollapsedHighlight.TabIndex = 6;
            btnCollapsedHighlight.Text = "▬";
            btnCollapsedHighlight.UseVisualStyleBackColor = true;
            btnCollapsedHighlight.Click += button4_Click;
            // 
            // btnCollapsedSpeed
            // 
            btnCollapsedSpeed.Location = new Point(3, 263);
            btnCollapsedSpeed.Margin = new Padding(3, 2, 3, 2);
            btnCollapsedSpeed.Name = "btnCollapsedSpeed";
            btnCollapsedSpeed.Size = new Size(28, 26);
            btnCollapsedSpeed.TabIndex = 5;
            btnCollapsedSpeed.Text = "⏩";
            btnCollapsedSpeed.UseVisualStyleBackColor = true;
            btnCollapsedSpeed.Click += button2_Click;
            // 
            // btnExpand
            // 
            btnExpand.Location = new Point(0, 367);
            btnExpand.Margin = new Padding(3, 2, 3, 2);
            btnExpand.Name = "btnExpand";
            btnExpand.Size = new Size(28, 26);
            btnExpand.TabIndex = 4;
            btnExpand.Text = "◀";
            btnExpand.UseVisualStyleBackColor = true;
            btnExpand.Click += btnExpand_Click;
            // 
            // btnCollapsedStop
            // 
            btnCollapsedStop.Location = new Point(3, 215);
            btnCollapsedStop.Margin = new Padding(3, 2, 3, 2);
            btnCollapsedStop.Name = "btnCollapsedStop";
            btnCollapsedStop.Size = new Size(28, 26);
            btnCollapsedStop.TabIndex = 3;
            btnCollapsedStop.Text = "■";
            btnCollapsedStop.UseVisualStyleBackColor = true;
            btnCollapsedStop.Click += btnCollapsedStop_Click;
            // 
            // btnCollapsedPause
            // 
            btnCollapsedPause.Location = new Point(3, 188);
            btnCollapsedPause.Margin = new Padding(3, 2, 3, 2);
            btnCollapsedPause.Name = "btnCollapsedPause";
            btnCollapsedPause.Size = new Size(28, 26);
            btnCollapsedPause.TabIndex = 2;
            btnCollapsedPause.Text = "||";
            btnCollapsedPause.UseVisualStyleBackColor = true;
            btnCollapsedPause.Click += btnCollapsedPause_Click;
            // 
            // btnCollapsedStart
            // 
            btnCollapsedStart.Location = new Point(3, 163);
            btnCollapsedStart.Margin = new Padding(3, 2, 3, 2);
            btnCollapsedStart.Name = "btnCollapsedStart";
            btnCollapsedStart.Size = new Size(28, 26);
            btnCollapsedStart.TabIndex = 1;
            btnCollapsedStart.Text = "▶";
            btnCollapsedStart.UseVisualStyleBackColor = true;
            btnCollapsedStart.Click += btnCollapsedStart_Click;
            // 
            // btnCollapsedConnect
            // 
            btnCollapsedConnect.Location = new Point(3, 33);
            btnCollapsedConnect.Margin = new Padding(3, 2, 3, 2);
            btnCollapsedConnect.Name = "btnCollapsedConnect";
            btnCollapsedConnect.Size = new Size(28, 26);
            btnCollapsedConnect.TabIndex = 0;
            btnCollapsedConnect.Text = "🔌";
            btnCollapsedConnect.UseVisualStyleBackColor = true;
            btnCollapsedConnect.Click += btnCollapsedConnect_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(777, 422);
            Controls.Add(webView);
            Controls.Add(pnlCollapsed);
            Controls.Add(pnlControl);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 2, 3, 2);
            Name = "MainForm";
            Text = "PromptLogic";
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)webView).EndInit();
            pnlControl.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            grpShowControls.ResumeLayout(false);
            grpShowControls.PerformLayout();
            pnlCollapsed.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private System.Windows.Forms.Panel pnlControl;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label txtStartingSlide;
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
        private System.Windows.Forms.Button btnCollapsedSpeed;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button btnCollapsedHighlight;
        private System.Windows.Forms.Button btnBorderNControl;
        private System.Windows.Forms.Button btnCollapsedControlPnl;
    }
}

