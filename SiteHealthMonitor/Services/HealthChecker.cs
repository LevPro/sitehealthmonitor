using System.Net.Http;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using Timer = System.Timers.Timer;

namespace SiteHealthMonitor.Services
{
    public class HealthChecker
    {
        private Dictionary<string, bool> _messages = new();
        private readonly HttpClient _httpClient = new();
        private Timer? _timer;
        private bool _isRunning;
        private readonly TaskbarIcon _taskbarIcon;
        
        public HealthChecker(TaskbarIcon taskbarIcon)
        {
            _taskbarIcon = taskbarIcon;
        }

        public void Start(int intervalSeconds)
        {
            if (_isRunning) return;
            
            _timer = new Timer(intervalSeconds * 1000);
            _timer.Elapsed += async (s, e) => await CheckAllSites();
            _timer.Start();
            _isRunning = true;
        }

        public void Stop()
        {
            _timer?.Stop();
            _isRunning = false;
        }

        private async Task CheckAllSites()
        {
            var settManager = new SettingsManager();
            var settings = settManager.LoadSettings();
            foreach (var site in settings.Websites)
            {
                try
                {
                    var response = await _httpClient.GetAsync(site.Url);
                    site.IsAvailable = response.IsSuccessStatusCode;
                }
                catch (HttpRequestException)
                {
                    site.IsAvailable = false;
                }
                catch (TaskCanceledException)
                {
                    site.IsAvailable = false;
                }
                catch (Exception)
                {
                    site.IsAvailable = false;
                }
                finally
                {
                    site.LastChecked = DateTime.Now;
                }

                if (site.IsAvailable == false)
                {
                    ShowTrayMessage($"Сайт {site.Url} не доступен");
                    
                    _messages.Add(site.Url, site.IsAvailable);
                }
                else
                {
                    if (_messages.ContainsKey(site.Url))
                    {
                        _messages.Remove(site.Url);
                        
                        ShowTrayMessage($"Сайт {site.Url} снова доступен", BalloonIcon.Info);
                    }
                }
            }
            settManager.SaveSettings(settings);
        }
        
        private void ShowTrayMessage(string message, BalloonIcon icon = BalloonIcon.Error)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _taskbarIcon.ShowBalloonTip("Site Health Monitor", message, icon);
            });
        }
    }
}