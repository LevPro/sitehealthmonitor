using System.Collections.ObjectModel;
using System.ComponentModel;
using SiteHealthMonitor.Models;
using SiteHealthMonitor.Services;

namespace SiteHealthMonitor.ViewModel;

public sealed class WebsitesViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Website> Websites { get; } = (new SettingsManager()).LoadSettings().Websites;
    public int CheckInterval { get; set; } = (new SettingsManager()).LoadSettings().CheckIntervalSeconds;

    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged(string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}