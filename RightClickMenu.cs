#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PromptLogic
{
    public class RightClickMenu : ContextMenuStrip
    {
        ToolStripMenuItem pauseResumeMenuItem;
        ToolStripMenuItem stopStartMenuItem;
        ToolStripMenuItem connectMenuItem;
        ToolStripMenuItem settingsMenuItem;
        ToolStripMenuItem ControlPanelCompressed;
        ToolStripMenuItem ControlPanelUnCompressed;
        ToolStripMenuItem ControlPanelHide;
        ToolStripMenuItem ControlPanelShow;
        ToolStripMenuItem BorderShow;
        ToolStripMenuItem BorderHide;
        ToolStripMenuItem loadSampleScript;
        ITeleprompterControl _ui;

        public RightClickMenu(ITeleprompterControl ui)
        {
            _ui = ui;

            connectMenuItem = new ToolStripMenuItem("Connect");
            connectMenuItem.Click += ConnectSlideShow;
            Items.Add(connectMenuItem); 

            loadSampleScript = new ToolStripMenuItem("Load Sample Script");
            loadSampleScript.Click += LoadSampleScript;
            Items.Add(loadSampleScript);

            Items.Add(new ToolStripSeparator());

            stopStartMenuItem = new ToolStripMenuItem("Start");
            stopStartMenuItem.Click += StartSlideShow;
            Items.Add(stopStartMenuItem);

            pauseResumeMenuItem = new ToolStripMenuItem("Pause");
            pauseResumeMenuItem.Click += PauseSlideShow;
            Items.Add(pauseResumeMenuItem);

            Items.Add(new ToolStripSeparator());

            Items.Add(new ToolStripMenuItem("Speed...", null, OpenSpeedSetting));
            Items.Add(new ToolStripMenuItem("Highlight...", null, OpenHighlightSetting));

            settingsMenuItem = new ToolStripMenuItem("Settings...");
            settingsMenuItem.Click += OpenSettings;
            Items.Add(settingsMenuItem);

            Items.Add(new ToolStripSeparator());

            var viewOptions = new ToolStripMenuItem("Control Panel");
            ControlPanelShow = new ToolStripMenuItem("Show");
            ControlPanelShow.Click += ControlPanelVisibleClicked;
            viewOptions.DropDownItems.Add(ControlPanelShow);
            ControlPanelHide = new ToolStripMenuItem("Hide");
            ControlPanelHide.Click += ControlPanelVisibleClicked;
            viewOptions.DropDownItems.Add(ControlPanelHide);
            viewOptions.DropDownItems.Add(new ToolStripSeparator());
            ControlPanelCompressed = new ToolStripMenuItem("Compress");
            ControlPanelCompressed.Click += ControlPanelCompressedClicked;
            viewOptions.DropDownItems.Add(ControlPanelCompressed);
            ControlPanelUnCompressed = new ToolStripMenuItem("Uncompress");
            ControlPanelUnCompressed.Click += ControlPanelCompressedClicked;
            viewOptions.DropDownItems.Add(ControlPanelUnCompressed);
            Items.Add(viewOptions);

            ControlPanelShow.Checked = true;

            var borderOptions = new ToolStripMenuItem( "Border" );
            BorderShow = new ToolStripMenuItem("Show");
            BorderShow.Click += BorderButtonClicked;
            BorderShow.Tag = FormBorderStyle.Sizable;
            borderOptions.DropDownItems.Add(BorderShow);
            BorderHide = new ToolStripMenuItem("Hide");
            BorderHide.Click += BorderButtonClicked;
            BorderHide.Tag = FormBorderStyle.None;
            borderOptions.DropDownItems.Add(BorderHide);

            BorderShow.Checked = true;
            Items.Add(borderOptions);

            Items.Add(new ToolStripSeparator());

            Items.Add(new ToolStripMenuItem("Close", null, CloseApplication));

            Opening += (s, e) => { ContextMenuPopup(s, e); };

        }

        public void UpdateBorderStyleMenuChecks(bool borderVisible)
        {
            BorderShow.Checked = borderVisible;
            BorderHide.Checked = !borderVisible;
        }

        private void BorderButtonClicked(object sender, EventArgs e)
        {
            BorderHide.Checked = false;
            BorderShow.Checked = false;
            // Check the clicked radio button
            ToolStripMenuItem clickedItem = sender as ToolStripMenuItem;
            if (clickedItem != null)
            {
                clickedItem.Checked = true;
            }

             _ui.ApplyMainBorderStyle((FormBorderStyle)clickedItem.Tag);
        }
        public void UpdateControlPanelMenuChecks(bool isVisible, bool isCompressed)
        {
            ControlPanelShow.Checked = isVisible;
            ControlPanelHide.Checked = !isVisible;
            ControlPanelCompressed.Checked = isCompressed;
            ControlPanelUnCompressed.Checked = !isCompressed;
        }

        private void ControlPanelCompressedClicked(object sender, EventArgs e)
        {
            ToolStripMenuItem clickedItem = sender as ToolStripMenuItem;
            if (clickedItem != null)
            {
                clickedItem.Checked = true;
            }


            bool isVisible = SettingsManager.Settings.controlPanelVisible;
            bool isCompressed = clickedItem.Text == "Compress";

            _ui.ApplySetControlPanelState(isVisible, isCompressed);
        }

        private void ControlPanelVisibleClicked(object sender, EventArgs e)
        {
            ToolStripMenuItem clickedItem = sender as ToolStripMenuItem;
            if (clickedItem != null)
            {
                clickedItem.Checked = true;
            }

            bool isVisible = clickedItem.Text == "Show";
            bool isCompressed = SettingsManager.Settings.controlPanelCompressed;
            _ui.ApplySetControlPanelState(isVisible, isCompressed);
        }

        private void ContextMenuPopup(object sender, EventArgs e)
        {
            bool isStopped = ((MainForm)_ui).IsStopped;
            bool isPaused = ((MainForm)_ui).IsPaused;

            connectMenuItem.Enabled = isStopped;
            settingsMenuItem.Enabled = isStopped;
            loadSampleScript.Enabled = isStopped;

            string s;

            if (isStopped)
                s = "Start";
            else
                s = "Stop";
            stopStartMenuItem.Text = s;

            if (isPaused)
                s = "Resume";
            else
                s = "Pause";
            pauseResumeMenuItem.Text = s;
            pauseResumeMenuItem.Enabled = !isStopped;
        }
        private void ConnectSlideShow(object sender, EventArgs e)
        {
            _ui.EnableController("ppt");
        }
        private void PauseSlideShow(object sender, EventArgs e)
        {
            _ui.PauseSlideShow();
        }
        private void StopSlideShow(object sender, EventArgs e)
        {
            _ui.StopSlideShow();
        }
        private void StartSlideShow(object sender, EventArgs e)
        {
            bool isStopped = ((MainForm)_ui).IsStopped;

            if (isStopped)
                _ui.StartSlideShow();
            else
                _ui.StopSlideShow();
        }
        private void CloseApplication(object sender, EventArgs e)
        {
            _ui.CloseApplication();
        }

        private void OpenSpeedSetting(object sender, EventArgs e)
        {
            _ui.OpenSpeedSettings();
        }
        private void OpenHighlightSetting(object sender, EventArgs e)
        {
            _ui.OpenHighlightSettings();
        }
        private void OpenSettings(object sender, EventArgs e)
        {
            _ui.OpenSettings();
        }

        private void LoadSampleScript(object sender, EventArgs e)
        {
            _ui.LoadSampleScript();
        }
    }
}
