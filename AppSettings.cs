using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;


namespace Teleprompter
{
    public class AppSettings
    {
        public string TeleprompterFont { get; set; } = "Segoe UI";
        public int FontSize { get; set; } = 32;
        public int ScrollSpeed { get; set; } = 50;
        public string LastScriptPath { get; set; } = "";
        public SlideEngine SelectedEngine { get; set; } = SlideEngine.PowerPoint;
        public int StartSlideIndex { get; set; } = 0;
        public bool IsCollapsed { get; set; } = false;
        public string TextColor { get; set; } = "#FFFFFF"; // default white
        public string BackColor { get; set; } = "#000000";

        public int LineSpacing { get; set; } = 5;
        public int ParagraphSpacing { get; set; } = 12;

        public int HighlightHeightLines { get; set; } = 3;
        public bool HighlightBandVisible {  get; set; } = true;
        public double HighlightBandOpacity { get; set; } = 0.45;
        public string HighlightBandColor { get; set; } = "#C8C8C8";
        public double HighlightBandTriggerPoint { get; set; } = 0.0;
        public int HighlightBandDistanceFromTop { get; set; } = 10;
        public bool HighlightBandTriggerPointVisible { get; set; } = false;
        public string HighlightBandTriggerPointColor { get; set; } = "#FF0000";
        public FormBorderStyle MainFormBorderStyle { get; set; } = FormBorderStyle.Sizable;
        public bool ShowControlSidebar { get; set; } = true;
        public bool MirrorText { get; set; } = false;
        public bool AlwaysOnTop { get; set; } = false;
        public bool NonActivating { get; set; } = false;
        public Size BorderlessSize { get; set; } = new Size(0, 0);
        public Point BorderlessLocation { get; set; } = Point.Empty;
        public Rectangle WindowBounds { get; set; } = Rectangle.Empty;

        public AppSettings()
        {

        }

        public AppSettings(AppSettings other)
        {
            TeleprompterFont = other.TeleprompterFont;
            FontSize = other.FontSize;
            ScrollSpeed = other.ScrollSpeed;
            LastScriptPath = other.LastScriptPath;
            SelectedEngine = other.SelectedEngine;
            StartSlideIndex = other.StartSlideIndex;
            IsCollapsed = other.IsCollapsed;
            LineSpacing = other.LineSpacing;
            ParagraphSpacing = other.ParagraphSpacing;
            HighlightHeightLines = other.HighlightHeightLines;
            HighlightBandVisible = other.HighlightBandVisible;
            HighlightBandOpacity = other.HighlightBandOpacity;
            HighlightBandColor = other.HighlightBandColor;
            HighlightBandTriggerPoint = other.HighlightBandTriggerPoint;
            HighlightBandDistanceFromTop = other.HighlightBandDistanceFromTop;
            HighlightBandTriggerPointVisible = other.HighlightBandTriggerPointVisible;
            HighlightBandTriggerPointColor = other.HighlightBandTriggerPointColor;
            MainFormBorderStyle = other.MainFormBorderStyle;
            MirrorText = other.MirrorText;
            AlwaysOnTop = other.AlwaysOnTop;
            NonActivating = other.NonActivating;
            ShowControlSidebar = other.ShowControlSidebar;
            BorderlessSize = other.BorderlessSize;
            BorderlessLocation = other.BorderlessLocation;
            WindowBounds = other.WindowBounds;
        }
    }


    public static class SettingsManager
    {
        private static readonly string SettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

        public static AppSettings Settings { get; private set; }

        public static void Load()
        {
            if (File.Exists(SettingsPath))
            {
                string json = File.ReadAllText(SettingsPath);
                Settings = JsonConvert.DeserializeObject<AppSettings>(json)
                           ?? new AppSettings();
            }
            else
            {
                Settings = new AppSettings();
                Save(); // create default file
            }
        }

        public static void Save()
        {
            string json = JsonConvert.SerializeObject(Settings, Formatting.Indented);
            File.WriteAllText(SettingsPath, json);
        }
        public static void Save(AppSettings newSettings)
        {
            Settings = newSettings;
            Save();
        }

    }

}
