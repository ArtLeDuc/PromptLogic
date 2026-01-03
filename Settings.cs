using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Teleprompter
{
    public partial class Settings : Form
    {
        AppSettings pending = new AppSettings(SettingsManager.Settings);
        private readonly ITeleprompterPreview preview;
        public Settings(ITeleprompterPreview preview)
        {
            InitializeComponent();

            this.Load += Settings_Load;
            this.preview = preview;

            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }
        private void Settings_Load(object sender, EventArgs e)
        {
            InstalledFontCollection fonts = new InstalledFontCollection();

            foreach (FontFamily family in fonts.Families)
            {
                cmbFontName.Items.Add(family.Name);
            }

            // Optional: sort alphabetically
            cmbFontName.Sorted = true;

            // Load saved setting (if any)
            cmbFontName.SelectedItem = pending.TeleprompterFont;
            numFontSize.Value = pending.FontSize;
            pnlTextColor.BackColor = ColorTranslator.FromHtml(pending.TextColor);
            pnlBackColor.BackColor = ColorTranslator.FromHtml(pending.BackColor);
            traScrollSpeed.Value = pending.ScrollSpeed;
            trkLineSpacing.Value = pending.LineSpacing;
            trkParagraphSpacing.Value = pending.ParagraphSpacing;
            trkBreakSpacing1.Value = pending.BreakSpacing1;
            trkBreakSpacing2.Value = pending.BreakSpacing2;
            trkBreakSpacing3.Value = pending.BreakSpacing3;
            trkHighlightbandDistanceFromTop.Value = pending.HighlightBandDistanceFromTop;
            trkHighlightBandOpacity.Value = (int)(pending.HighlightBandOpacity * 100.0);
            pnlHighlightBandColor.BackColor = ColorTranslator.FromHtml(pending.HighlightBandColor);
            numHighLightBandLinesCustom.Value = pending.HighlightHeightLines;
            numHighLightBandLinesCustom.Enabled = false;
            if (pending.HighlightHeightLines == 3)
            {
                radHighLightBand3Lines.Checked = true;
            }
            else if (pending.HighlightHeightLines == 5)
            {
                radHighLightBand5Lines.Checked = true;
            }
            else
            {
                radHighLightBandCustom.Checked = true;
                numHighLightBandLinesCustom.Enabled = true;
            }
            trkHighLightBandTriggerOffset.Value = (int)(pending.HighlightBandTriggerPoint/100.0);
            chkHighlightbandVisible.Checked = pending.HighlightBandVisible;
        }

        private void cmbFontName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbFontName.SelectedItem is string fontName)
            {
                pending.TeleprompterFont = fontName;
                preview.ApplyFont(fontName);   // <-- THIS is the call
            }
        }

        private void btnOk_Click_1(object sender, EventArgs e)
        {
            SettingsManager.Save(pending);
            this.Close();
        }

        private void btnCancel_Click_1(object sender, EventArgs e)
        {
            preview.ApplyAllSettings();   // restore saved settings
            this.Close();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            SettingsManager.Save(pending);
        }

        private void numFontSize_ValueChanged(object sender, EventArgs e)
        {
            int size = (int)numFontSize.Value;

            pending.FontSize = size;        // update pending settings
            preview.ApplyFontSize(size);    // live preview
        }

        private void btnBackgroundColor_Click(object sender, EventArgs e)
        {
            using (var dlg = new ColorDialog())
            {
                dlg.Color = pnlBackColor.BackColor; // start with current color

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    // Update preview square
                    pnlBackColor.BackColor = dlg.Color;

                    // Convert to CSS color
                    string css = ColorTranslator.ToHtml(dlg.Color);

                    // Update pending settings
                    pending.TextColor = css;

                    // Live preview
                    preview.ApplyBackgroundColor(css);
                }
            }
        }

        private void btnTextColor_Click(object sender, EventArgs e)
        {
            using (var dlg = new ColorDialog())
            {
                dlg.Color = pnlTextColor.BackColor; // start with current color

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    // Update preview square
                    pnlTextColor.BackColor = dlg.Color;

                    // Convert to CSS color
                    string css = ColorTranslator.ToHtml(dlg.Color);

                    // Update pending settings
                    pending.TextColor = css;

                    // Live preview
                    preview.ApplyTextColor(css);
                }
            }
        }

        private void trkLineSpacing_Scroll(object sender, EventArgs e)
        {
            int spacing = trkLineSpacing.Value;
            pending.LineSpacing = spacing;
            preview.ApplyLineSpacing(spacing);
        }

        private void trkParagraphSpacing_Scroll(object sender, EventArgs e)
        {
            int spacing = trkParagraphSpacing.Value;
            pending.ParagraphSpacing = spacing;
            preview.ApplyParagraphSpacing(spacing);

        }

        private void trkBreakSpacing1_Scroll(object sender, EventArgs e)
        {
            int spacing = trkBreakSpacing1.Value;
            pending.BreakSpacing1 = spacing;
            preview.ApplyBreakSpacing1(spacing);
        }

        private void trkBreakSpacing2_Scroll(object sender, EventArgs e)
        {
            int spacing = trkBreakSpacing2.Value;
            pending.BreakSpacing2 = spacing;
            preview.ApplyBreakSpacing2(spacing);
        }

        private void trkBreakSpacing3_Scroll(object sender, EventArgs e)
        {
            int spacing = trkBreakSpacing3.Value;
            pending.BreakSpacing3 = spacing;
            preview.ApplyBreakSpacing3(spacing);
        }

        private void chkHighlightbandVisible_CheckedChanged(object sender, EventArgs e)
        {
            bool visible = chkHighlightbandVisible.Checked;
            pending.HighlightBandVisible = visible;
            grpHighlightBandSettings.Enabled = visible;
            preview.ApplyHighlightVisible(visible);
        }

        private void trkHighlightBandOpacity_Scroll(object sender, EventArgs e)
        {
            double opacity = (double)trkHighlightBandOpacity.Value / 100.0;
            pending.HighlightBandOpacity = opacity;
            preview.ApplyHighlightOpacity(opacity);
        }

        private void btnHighLightBandColor_Click(object sender, EventArgs e)
        {
            using (var dlg = new ColorDialog())
            {
                dlg.Color = pnlHighlightBandColor.BackColor; // start with current color

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    // Update preview square
                    pnlHighlightBandColor.BackColor = dlg.Color;

                    // Convert to CSS color
                    string css = ColorTranslator.ToHtml(dlg.Color);

                    // Update pending settings
                    pending.HighlightBandColor = css;

                    // Live preview
                    preview.ApplyHighlightColor(css);
                }
            }

        }

        private void radHighLightBand3Lines_CheckedChanged(object sender, EventArgs e)
        {
            int lines = 3;
            pending.HighlightHeightLines = lines;
            preview.ApplyHighlightLines(lines);

        }

        private void radHighLightBand5Lines_CheckedChanged(object sender, EventArgs e)
        {
            int lines = 5;
            pending.HighlightHeightLines = lines;
            preview.ApplyHighlightLines(lines);
        }

        private void radHighLightBandCustom_CheckedChanged(object sender, EventArgs e)
        {
            var rb = (RadioButton)sender;
            numHighLightBandLinesCustom.Enabled = rb.Checked;
            if (rb.Checked)
            {
                int lines = (int)numHighLightBandLinesCustom.Value;
                pending.HighlightHeightLines = lines;
                preview.ApplyHighlightLines(lines);
            }
        }

        private void numHighLightBandLinesCustom_ValueChanged(object sender, EventArgs e)
        {
            int lines = (int)numHighLightBandLinesCustom.Value;
            pending.HighlightHeightLines = lines;
            preview.ApplyHighlightLines(lines);
        }

        private void trkHighLightBandTriggerOffset_Scroll(object sender, EventArgs e)
        {
            double offset = trkHighLightBandTriggerOffset.Value;
            pending.HighlightBandTriggerPoint = offset;
            preview.ApplyHighlightTriggerPoint(offset);
        }

        private void trkHighlightbandDistanceFromTop_Scroll(object sender, EventArgs e)
        {
            int distanceFromTop = trkHighlightbandDistanceFromTop.Value;
            pending.HighlightBandDistanceFromTop = distanceFromTop;
            preview.ApplyHighlightTop(distanceFromTop);
        }
    }
}
