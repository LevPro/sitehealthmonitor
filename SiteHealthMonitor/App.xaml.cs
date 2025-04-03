using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;
using SiteHealthMonitor.Models;
using SiteHealthMonitor.Services;
using SiteHealthMonitor.Views;

namespace SiteHealthMonitor;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private HealthChecker? _healthChecker;
    private SettingsManager? _settings;
    private SettingsWindow? _settingsWindow;
    private MenuItem? _startMenuItem;
    private MenuItem? _stopMenuItem;

    protected override void OnStartup(StartupEventArgs e)
    {
        var taskbarIcon = new TaskbarIcon
        {
            Icon = LoadTrayIcon(),
            ToolTipText = "Мониторинг сайтов",
            ContextMenu = (ContextMenu)FindResource("TrayMenu")
        };

        _healthChecker = new HealthChecker(taskbarIcon);
        
        _settings = new SettingsManager();
        
        var trayMenu = (ContextMenu)FindResource("TrayMenu");
        
        _startMenuItem = trayMenu.Items
            .OfType<MenuItem>()
            .FirstOrDefault(m => m.Name == "StartMonitoring");
        _stopMenuItem = trayMenu.Items
            .OfType<MenuItem>()
            .FirstOrDefault(m => m.Name == "StopMonitoring");
    }
    
    private static System.Drawing.Icon LoadTrayIcon()
    {
        try
        {
            var assemblyName = Assembly.GetEntryAssembly()!.GetName().Name;
            
            var uri = new Uri(
                $"pack://application:,,,/{assemblyName};component/resources/icon.ico", 
                UriKind.Absolute
            );

            using var stream = GetResourceStream(uri)!.Stream;
            
            return new System.Drawing.Icon(stream);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading icon: {ex.Message}");
            return System.Drawing.SystemIcons.Application;
        }
    }

    private void OpenSettings(object sender, RoutedEventArgs e)
    {
        if (_settingsWindow != null)
        {
            _settingsWindow.Activate();
            return;
        }
        
        try
        {
            _settingsWindow = new SettingsWindow();
            _settingsWindow.Closed += (s, args) => _settingsWindow = null;
            _settingsWindow.Closing += (s, args) =>
            {
                args.Cancel = true;
                _settingsWindow.Hide();
            };
            _settingsWindow.Show();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error opening settings window: {ex.Message}");
        }
    }

    private void StartMonitoring(object sender, RoutedEventArgs e)
    {
        AppSettings appSettings = new();
        try
        {
            appSettings = _settings.LoadSettings();
        }
        catch
        {
            // ignored
        }
        _healthChecker!.Start(appSettings.CheckIntervalSeconds);
        
        _startMenuItem.Visibility = Visibility.Collapsed;
        _stopMenuItem.Visibility = Visibility.Visible;
    }

    private void StopMonitoring(object sender, RoutedEventArgs e)
    {
        _healthChecker?.Stop();
        
        _startMenuItem.Visibility = Visibility.Visible;
        _stopMenuItem.Visibility = Visibility.Collapsed;
    }

    private void ExitApp(object sender, RoutedEventArgs e)
    {
        Current.Shutdown();
    }
}