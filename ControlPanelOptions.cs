using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PromptLogic
{
    public partial class ControlPanelOptions : Form
    {
        private readonly ITeleprompterControl preview;

        public ControlPanelOptions(ITeleprompterControl preview)
        {
            this.preview = preview;
            InitializeComponent();
            UpdateControls();

            ContextMenuStrip menu = new ContextMenuStrip();
            var closeItem = new ToolStripMenuItem("Close");
            closeItem.Click += (s, ev) => this.Close();
            menu.Items.Add(closeItem);

            this.ContextMenuStrip = menu;
        }

        public void UpdateControls()
        {
            chkControlShow.Checked = SettingsManager.Settings.controlPanelVisible;
            chkControlHide.Checked = !SettingsManager.Settings.controlPanelVisible;
            chkControlCompress.Checked = SettingsManager.Settings.controlPanelCompressed;
            chkControlUncompress.Checked = !SettingsManager.Settings.controlPanelCompressed;

            chkBorderShow.Checked = SettingsManager.Settings.MainFormBorderStyle == FormBorderStyle.Sizable;
            chkBorderHide.Checked = SettingsManager.Settings.MainFormBorderStyle == FormBorderStyle.None;
        }

        private void Visibility_Changed(object sender, EventArgs e)
        {
            var clicked = sender as RadioButton;
            if (clicked == null || !clicked.Checked)
                return;

            // Use Tag or Name to determine which option was selected
            preview.SetControlPanelVisible(clicked.Name == "chkControlShow");
        }

        private void CompressionChanged(object sender, EventArgs e)
        {
            var clicked = sender as RadioButton;
            if (clicked == null || !clicked.Checked)
                return;

            preview.SetControlPanelCompressed(clicked.Name == "chkControlCompress");

        }

        private void BorderChanged(object sender, EventArgs e)
        {
            var clicked = sender as RadioButton;
            if (clicked == null || !clicked.Checked)
                return;

            preview.ApplyMainBorderStyle((clicked.Name == "chkBorderShow") ? FormBorderStyle.Sizable : FormBorderStyle.None);
        }
    }
}
