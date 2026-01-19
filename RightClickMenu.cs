using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PromptLogic
{
    public class RightClickMenu : ContextMenu
    {
        MenuItem pauseResumeMenuItem = null;
        MenuItem stopStartMenuItem = null;
        MenuItem connectMenuItem = null;
        MenuItem settingsMenuItem = null;
        MenuItem ControlPanelCompressed = null;
        MenuItem ControlPanelUnCompressed = null;
        MenuItem ControlPanelHide = null;
        MenuItem ControlPanelShow = null;
        MenuItem BorderShow = null;
        MenuItem BorderHide = null;
        MenuItem loadSampleScript = null;
        ITeleprompterControl _ui;

        public RightClickMenu(ITeleprompterControl ui)
        {
            _ui = ui;

            //            var menu = new ContextMenu();

            connectMenuItem = MenuItems.Add("Connect", ConnectSlideShow);
            loadSampleScript = MenuItems.Add("Load Sample Script", LoadSampleScript);
            MenuItems.Add("-");

            stopStartMenuItem = MenuItems.Add("Start", StartSlideShow);
            pauseResumeMenuItem = MenuItems.Add("Pause", PauseSlideShow);

            MenuItems.Add("-");
            MenuItems.Add("Speed...", OpenSpeedSetting);
            MenuItems.Add("Highlight...", OpenHighlightSetting);
            settingsMenuItem = MenuItems.Add("Settings...", OpenSettings);
            MenuItems.Add("-");

            var viewOptions = new MenuItem { Text = "Control Panel" };
            ControlPanelShow = viewOptions.MenuItems.Add("Show", ControlPanelVisibleClicked);
            ControlPanelHide = viewOptions.MenuItems.Add("Hide", ControlPanelVisibleClicked);
            viewOptions.MenuItems.Add("-");
            ControlPanelCompressed = viewOptions.MenuItems.Add("Compress", ControlPanelCompressedClicked);
            ControlPanelUnCompressed = viewOptions.MenuItems.Add("Uncompress", ControlPanelCompressedClicked);
            MenuItems.Add(viewOptions);

            ControlPanelShow.Checked = true;

            var borderOptions = new MenuItem { Text = "Border" };
            BorderShow = borderOptions.MenuItems.Add("Show", BorderButtonClicked);
            BorderShow.Tag = FormBorderStyle.Sizable;
            BorderHide = borderOptions.MenuItems.Add("Hide", BorderButtonClicked);
            BorderHide.Tag = FormBorderStyle.None;
            MenuItems.Add(borderOptions);
            BorderShow.Checked = true;

            MenuItems.Add("-");

            MenuItems.Add("Close", CloseApplication);
            Popup += ContextMenu_Popup;
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
            MenuItem clickedItem = sender as MenuItem;
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
            MenuItem clickedItem = sender as MenuItem;
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
            MenuItem clickedItem = sender as MenuItem;
            if (clickedItem != null)
            {
                clickedItem.Checked = true;
            }

            bool isVisible = clickedItem.Text == "Show";
            bool isCompressed = SettingsManager.Settings.controlPanelCompressed;
            _ui.ApplySetControlPanelState(isVisible, isCompressed);
        }

        private void ContextMenu_Popup(object sender, EventArgs e)
        {
            if (((MainForm)_ui).InputLocked)
                    return;

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
        private void ConnectSlideShow(object seder, EventArgs e)
        {
            _ui.ConnectSlideShow();
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
