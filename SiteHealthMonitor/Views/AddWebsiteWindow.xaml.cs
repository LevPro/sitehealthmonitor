using System.Windows;
using SiteHealthMonitor.Models;
using SiteHealthMonitor.Services;
using SiteHealthMonitor.Utilities;
using SiteHealthMonitor.ViewModel;

namespace SiteHealthMonitor.Views
{
    public partial class AddWebsiteWindow : Window
    {
        private readonly WebsitesViewModel _viewModel;
        
        public AddWebsiteWindow(WebsitesViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        private void CancelButton(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddWebsite(object sender, RoutedEventArgs e)
        {
            var url = UrlTextBox.Text;
            
            if (!UriExtensions.IsValidRootUrl(url))
            {
                MessageBox.Show(
                    "Некорректный URL! Введите адрес в формате http://example.com или https://example.com",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            if (_viewModel.Websites.Any(w => w.Url == url))
            {
                MessageBox.Show(
                    $"Сайт {url} уже был добавлен ранее",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }
            
            var normalizedUrl = UriExtensions.NormalizeUrl(url);
            _viewModel.Websites.Add(new Website { Url = normalizedUrl, IsAvailable = true, LastChecked = DateTime.Now });
            var settings = new AppSettings { Websites = _viewModel.Websites };
            
            var settingsManager = new SettingsManager();
            
            settingsManager.SaveSettings(settings);

            _viewModel.OnPropertyChanged(nameof(_viewModel.Websites));
            
            Close();
        }
    }
}