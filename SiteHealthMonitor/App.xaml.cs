using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;
using SiteHealthMonitor.Models;
using SiteHealthMonitor.Services;
using SiteHealthMonitor.Utilities;
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
    private ActualVersionWindow? _actualVersionWindow;
    private NewVersionWindow? _newVersionWindow;
    private MenuItem? _startMenuItem;
    private MenuItem? _stopMenuItem;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
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

    private async void CheckUpdates(object sender, RoutedEventArgs e)
    {
        try
        {
            var checker = new VersionChecker(
                "levpro",
                "sitehealthmonitor",
                Assembly.GetEntryAssembly()!.GetName().Version!.ToString()
            );

            if (!await checker.IsUpdateAvailableAsync())
            {
                if (_actualVersionWindow != null)
                {
                    // Если окно скрыто - показываем снова
                    if (!_actualVersionWindow.IsVisible)
                    {
                        _actualVersionWindow.Show();
                    }
                    _actualVersionWindow.Activate();
                    return;
                }
        
                try
                {
                    _actualVersionWindow = new ActualVersionWindow();
                    _actualVersionWindow.Closed += (s, args) => 
                    {
                        _actualVersionWindow = null; // Важно: обнуляем ссылку при закрытии
                    };
        
                    // Для обработки закрытия через системный крестик
                    _actualVersionWindow.Closing += (s, args) => 
                    {
                        args.Cancel = true; // Отменяем настоящее закрытие
                        _actualVersionWindow.Hide(); // Скрываем вместо закрытия
                    };

                    _actualVersionWindow.ShowDialog();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error opening actual version window: {ex.Message}");
                }
                
                return;
            }
            
            if (_newVersionWindow != null)
            {
                // Если окно скрыто - показываем снова
                if (!_newVersionWindow.IsVisible)
                {
                    _newVersionWindow.Show();
                }
                _newVersionWindow.Activate();
                return;
            }
        
            try
            {
                _newVersionWindow = new NewVersionWindow();
                _newVersionWindow.Closed += (s, args) => 
                {
                    _newVersionWindow = null; // Важно: обнуляем ссылку при закрытии
                };
        
                // Для обработки закрытия через системный крестик
                _newVersionWindow.Closing += (s, args) => 
                {
                    args.Cancel = true; // Отменяем настоящее закрытие
                    _newVersionWindow.Hide(); // Скрываем вместо закрытия
                };

                _newVersionWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error opening new version window: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Update check failed: {ex.Message}");
        }
    }
    
    private void AboutProgram(object sender, RoutedEventArgs e)
    {
        Runtime.OpenUrl("https://levpro.ru/solutions/kruglosutochnyj-monitoring-dostupnosti-sajtov");
    }

    private void OpenSettings(object sender, RoutedEventArgs e)
    {
        if (_settingsWindow != null)
        {
            // Если окно скрыто - показываем снова
            if (!_settingsWindow.IsVisible)
            {
                _settingsWindow.Show();
            }
            _settingsWindow.Activate();
            return;
        }

        try
        {
            _settingsWindow = new SettingsWindow();
            _settingsWindow.Closed += (s, args) => 
            {
                _settingsWindow = null; // Важно: обнуляем ссылку при закрытии
            };
        
            // Для обработки закрытия через системный крестик
            _settingsWindow.Closing += (s, args) => 
            {
                args.Cancel = true; // Отменяем настоящее закрытие
                _settingsWindow.Hide(); // Скрываем вместо закрытия
            };

            _settingsWindow.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка открытия настроек: {ex.Message}");
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