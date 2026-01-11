using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
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
            panel2.Top = 3;
            panel2.Left = 3;
            panel2.Width = this.Width - 6;
            panel2.Height = this.Height - 6;

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
            trkHighlightbandDistanceFromTop.Value = pending.HighlightBandDistanceFromTop;
            trkHighlightBandOpacity.Value = (int)(pending.HighlightBandOpacity * 100.0);
            pnlHighlightBandColor.BackColor = ColorTranslator.FromHtml(pending.HighlightBandColor);
            chkTrigger.Checked = pending.HighlightBandTriggerPointVisible;

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
            trkHighLightBandTriggerOffset.Value = (int)(pending.HighlightBandTriggerPoint / 100.0);
            chkTrigger.Checked = (pending.HighlightBandTriggerPointVisible);
            pnlTriggerColor.BackColor = ColorTranslator.FromHtml(pending.HighlightBandTriggerPointColor);

            chkHighlightbandVisible.Checked = pending.HighlightBandVisible;

            radBorderless.CheckedChanged += radBorder_CheckChanged;
            radNormalBorder.CheckedChanged += radBorder_CheckChanged;
            if (pending.MainFormBorderStyle == FormBorderStyle.None)
            {
                radBorderless.Checked = true;
                radNormalBorder.Checked = false;
            }
            else
            {
                radBorderless.Checked = false;
                radNormalBorder.Checked = true;
            }

            chkShowControlSidebar.Checked = pending.ShowControlSidebar;

            lblDescription.Text = "A Windows‑native teleprompter engine built for clarity, control, and a clean workflow.\r\nLocal‑first, predictable, and modular—designed for creators and educators.";
            lblCopyright.Text = "© 2026 Vermont Creative Technologies.";
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

        private void chkHighlightbandVisible_CheckedChanged(object sender, EventArgs e)
        {
            bool visible = chkHighlightbandVisible.Checked;
            pending.HighlightBandVisible = visible;
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

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

            int thickness = 1;
            Color borderColor = Color.Black;

            using (var pen = new Pen(borderColor, thickness))
            {
                // Adjust rectangle so the full border is visible
                Rectangle rect = new Rectangle(
                    thickness / 2,
                    thickness / 2,
                    panel2.Width - thickness,
                    panel2.Height - thickness);

                e.Graphics.DrawRectangle(pen, rect);
            }

            using (var pen = new Pen(Color.Black, 1))
            {
                int y = lblTitle.Bottom + 2; // vertical position of the line
                e.Graphics.DrawLine(pen, 0, y, panel2.Width, y);
            }

        }

        private void chkTrigger_CheckedChanged(object sender, EventArgs e)
        {
            bool visible = chkTrigger.Checked;
            pending.HighlightBandTriggerPointVisible = visible;
            preview.ApplyHighlightBandTriggerPointVisible(visible);
        }

        private void btnTriggerPointColor_Click(object sender, EventArgs e)
        {
            using (var dlg = new ColorDialog())
            {
                dlg.Color = pnlTriggerColor.BackColor; // start with current color

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    // Update preview square
                    pnlTriggerColor.BackColor = dlg.Color;

                    // Convert to CSS color
                    string css = ColorTranslator.ToHtml(dlg.Color);

                    // Update pending settings
                    pending.HighlightBandTriggerPointColor = css;

                    // Live preview
                    preview.ApplyHighlightBandTriggerPointColor(css);
                }

            }
        }

        private void radBorder_CheckChanged(object sender, EventArgs e)
        {
            var rb = (RadioButton)sender;
            if (!rb.Checked)
                return;

            switch (rb.Name)
            {
                case "radBorderless":
                    pending.MainFormBorderStyle = FormBorderStyle.None;
                    break;
                case "radNormalBorder":
                    pending.MainFormBorderStyle = FormBorderStyle.Sizable;
                    break;
            }

            preview.ApplyMainBorderStyle(pending.MainFormBorderStyle);
        }

        private void chkShowControlSidebar_CheckedChanged(object sender, EventArgs e)
        {
            bool bShowControlBar = chkShowControlSidebar.Checked;
            pending.ShowControlSidebar = bShowControlBar;
            preview.ApplyShowControlSidebar(bShowControlBar);
        }
    }
}
