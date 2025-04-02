using System.Collections.ObjectModel;

namespace SiteHealthMonitor.Models
{
    public class AppSettings
    {
        public ObservableCollection<Website> Websites { get; set; } = new();
        public int CheckIntervalSeconds { get; set; } = 60;
    }
}