using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;


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
        public int WindowLeft { get; set; } = -1;
        public int WindowTop { get; set; } = -1;
        public int WindowWidth { get; set; } = -1;
        public int WindowHeight { get; set; } = -1;
        public bool IsCollapsed { get; set; } = false;

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
            WindowLeft = other.WindowLeft;
            WindowTop = other.WindowTop;
            WindowWidth = other.WindowWidth;
            WindowHeight = other.WindowHeight;
            IsCollapsed = other.IsCollapsed;        
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
