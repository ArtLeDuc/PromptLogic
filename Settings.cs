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
            cmbFontName.SelectedItem = SettingsManager.Settings.TeleprompterFont;
            numFontSize.Value = SettingsManager.Settings.FontSize;
            pnlTextColor.BackColor = ColorTranslator.FromHtml(pending.TextColor);
            pnlBackColor.BackColor = ColorTranslator.FromHtml(pending.BackColor);
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
            SettingsManager.Settings.LineSpacing = spacing;
            preview.ApplyLineSpacing(spacing);
        }

        private void trkParagraphSpacing_Scroll(object sender, EventArgs e)
        {
            int spacing = trkParagraphSpacing.Value;
            SettingsManager.Settings.ParagraphSpacing = spacing;
            preview.ApplyParagraphSpacing(spacing);

        }

        private void trkBreakSpacing1_Scroll(object sender, EventArgs e)
        {
            int spacing = trkBreakSpacing1.Value;
            SettingsManager.Settings.BreakSpacing1 = spacing;
            preview.ApplyBreakSpacing1(spacing);
        }

        private void trkBreakSpacing2_Scroll(object sender, EventArgs e)
        {
            int spacing = trkBreakSpacing2.Value;
            SettingsManager.Settings.BreakSpacing2 = spacing;
            preview.ApplyBreakSpacing2(spacing);
        }

        private void trkBreakSpacing3_Scroll(object sender, EventArgs e)
        {
            int spacing = trkBreakSpacing3.Value;
            SettingsManager.Settings.BreakSpacing3 = spacing;
            preview.ApplyBreakSpacing3(spacing);
        }
    }
}
