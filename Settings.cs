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
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click_1(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
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
    }
}
