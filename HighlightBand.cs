using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PromptLogic
{
    public partial class HighlightBand : Form
    {
        private readonly ITeleprompterControl preview;

        public HighlightBand(ITeleprompterControl preview)
        {
            this.preview = preview; 
            InitializeComponent();

            chkHighlightDisplay.Checked = SettingsManager.Settings.HighlightBandVisible;
            trkHighlightbandDistanceFromTop.Value = SettingsManager.Settings.HighlightBandDistanceFromTop;
            trkHighlightBandOpacity.Value = (int)(SettingsManager.Settings.HighlightBandOpacity * 100.0);

            int lines = SettingsManager.Settings.HighlightHeightLines;

            if (lines == 3)
                radHighLightBand3Lines.Checked = true;
            else if (lines == 5)
                radHighLightBand5Lines.Checked = true;
            else
                radHighLightBandCustom.Checked = true;

            numHighLightBandLinesCustom.Value = lines; 
        }

        private void chkHighlightDisplay_CheckedChanged(object sender, EventArgs e)
        {
            bool visible = chkHighlightDisplay.Checked;
            SettingsManager.Settings.HighlightBandVisible = visible;
            preview.ApplyHighlightVisible(visible);
        }

        private void trkHighlightbandDistanceFromTop_Scroll(object sender, EventArgs e)
        {
            int distanceFromTop = trkHighlightbandDistanceFromTop.Value;
            SettingsManager.Settings.HighlightBandDistanceFromTop = distanceFromTop;
            preview.ApplyHighlightTop(distanceFromTop);
        }

        private void trkHighlightBandOpacity_Scroll(object sender, EventArgs e)
        {
            double opacity = (double)trkHighlightBandOpacity.Value / 100.0;
            SettingsManager.Settings.HighlightBandOpacity = opacity;
            preview.ApplyHighlightOpacity(opacity);
        }

        private void radHighLightBand3Lines_CheckedChanged(object sender, EventArgs e)
        {
            int lines = 3;
            SettingsManager.Settings.HighlightHeightLines = lines;
            preview.ApplyHighlightLines(lines);

        }

        private void radHighLightBand5Lines_CheckedChanged(object sender, EventArgs e)
        {
            int lines = 5;
            SettingsManager.Settings.HighlightHeightLines = lines;
            preview.ApplyHighlightLines(lines);
        }

        private void radHighLightBandCustom_CheckedChanged(object sender, EventArgs e)
        {
            var rb = (RadioButton)sender;
            numHighLightBandLinesCustom.Enabled = rb.Checked;
            if (rb.Checked)
            {
                int lines = (int)numHighLightBandLinesCustom.Value;
                SettingsManager.Settings.HighlightHeightLines = lines;
                preview.ApplyHighlightLines(lines);
            }
        }

        private void numHighLightBandLinesCustom_ValueChanged(object sender, EventArgs e)
        {
            int lines = (int)numHighLightBandLinesCustom.Value;
            SettingsManager.Settings.HighlightHeightLines = lines;
            preview.ApplyHighlightLines(lines);
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
