#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PromptLogic
{
    public partial class ScrollSpeed : Form
    {
        private readonly ITeleprompterControl preview;

        public ScrollSpeed(ITeleprompterControl preview)
        {
            InitializeComponent();

            double speedValue = SettingsManager.Settings.ScrollSpeed;
            traSpeed.Value = (int)speedValue;

            this.preview = preview;

            ContextMenuStrip menu = new ContextMenuStrip();
            var closeItem = new ToolStripMenuItem("Close");
            closeItem.Click += (s, ev) => this.Close();
            menu.Items.Add(closeItem);
            this.ContextMenuStrip = menu;

        }

        public ScrollSpeed()
        {
            InitializeComponent();
        }
        private void traSpeed_Scroll(object sender, EventArgs e)
        {
            SettingsManager.Settings.ScrollSpeed = traSpeed.Value;
            //The range wants to be between 0.01 and 1
            double speed = (double)traSpeed.Value / 100.0;
            preview.ApplyScrollSpeed(speed);
        }

        private void ScrollSpeed_Load(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(traSpeed, "Adjust scrolling speed");
        }
/*
        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                NativeMethods.ReleaseCapture();
                NativeMethods.SendMessage(this.Handle, NativeMethods.WM_NCLBUTTONDOWN, (IntPtr)NativeMethods.HTCAPTION, IntPtr.Zero);
            }
        }
*/
    }
}
