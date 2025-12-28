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
        public int ScrollSpeed { get; set; } = 50;
        public string LastScriptPath { get; set; } = "";
        public SlideEngine SelectedEngine { get; set; } = SlideEngine.PowerPoint;

    }

    public static class SettingsManager
    {
        private static readonly string SettingsPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

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
    }

}
