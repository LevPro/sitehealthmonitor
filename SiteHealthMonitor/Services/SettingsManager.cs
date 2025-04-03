using Newtonsoft.Json;
using SiteHealthMonitor.Models;
using System.IO;

namespace SiteHealthMonitor.Services
{
    public class SettingsManager
    {
        private readonly string _settingsDirectory = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "data"
        );
        
        private string _settingsFilePath;

        public SettingsManager()
        {
            if (Directory.Exists(_settingsDirectory) == false)
            {
                Directory.CreateDirectory(_settingsDirectory);
            }

            _settingsFilePath = Path.Combine(
                _settingsDirectory,
                "settings.json"
            );
        }

        public AppSettings LoadSettings()
        {
            if (File.Exists(_settingsFilePath))
            {
                return JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(_settingsFilePath));
            }
            else
            {
                return new AppSettings();
            }
        }

        public void SaveSettings(AppSettings settings)
        {
            File.WriteAllText(_settingsFilePath, JsonConvert.SerializeObject(settings, Formatting.Indented));
        }
    }
}