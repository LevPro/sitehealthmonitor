using System.Windows;
using System.Windows.Controls;
using SiteHealthMonitor.Models;
using SiteHealthMonitor.Services;
using SiteHealthMonitor.ViewModel;

namespace SiteHealthMonitor.Views
{
    public partial class SettingsWindow : Window
    {
        private AddWebsiteWindow? _addWebsiteWindow;
        
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void AddUrl(object sender, RoutedEventArgs e)
        {
            if (_addWebsiteWindow != null)
            {
                _addWebsiteWindow.Activate();
                return;
            }
        
            try
            {
                if (DataContext is WebsitesViewModel viewModel)
                {
                    _addWebsiteWindow = new AddWebsiteWindow(viewModel);
                    _addWebsiteWindow.Closed += (s, args) => _addWebsiteWindow = null;
                    _addWebsiteWindow.ShowDialog();
                }
                else
                {
                    throw new Exception("DataContext is not of type WebsitesViewModel");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error opening add url window: {ex.Message}");
            }
        }

        private void CheckWebsite(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is Website website)
            {
                website.IsSelected = true;
            }
            else
            {
                throw new Exception("DataContext is not of type WebsitesViewModel");
            }
        }

        private void UncheckWebsite(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is Website website)
            {
                website.IsSelected = false;
            }
            else
            {
                throw new Exception("DataContext is not of type WebsitesViewModel");
            }
        }

        private void DeleteWebsites(object sender, RoutedEventArgs e)
        {
            if (DataContext is WebsitesViewModel viewModel)
            {
                var websitesToDelete = viewModel.Websites.Where(w => w.IsSelected).ToList();
                if (websitesToDelete.Count == 0)
                {
                    MessageBox.Show("Выберите сайты для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                foreach (var website in websitesToDelete)
                {
                    viewModel.Websites.Remove(website);
                }
                
                var settings = new AppSettings { Websites = viewModel.Websites, CheckIntervalSeconds = viewModel.CheckInterval };
                var settingsManager = new SettingsManager();
                settingsManager.SaveSettings(settings);
                
                viewModel.OnPropertyChanged(nameof(viewModel.Websites));
            }
            else
            {
                throw new Exception("DataContext is not of type WebsitesViewModel");
            }
        }

        private void ChangeInterval(object sender, RoutedEventArgs e)
        {
            if (DataContext is WebsitesViewModel viewModel)
            {
                if (sender is TextBox textBox)
                {
                    if (int.TryParse(textBox.Text, out int interval))
                    {
                        if (interval < 1)
                        {
                            MessageBox.Show("Интервал должен быть больше 0", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        viewModel.CheckInterval = interval;
                        var settings = new AppSettings { Websites = viewModel.Websites, CheckIntervalSeconds = interval };
                        var settingsManager = new SettingsManager();
                        settingsManager.SaveSettings(settings);
                    }
                    else
                    {
                        MessageBox.Show("Введите корректное значение интервала", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    throw new Exception("Sender is not a TextBox");
                }
            }
            else
            {
                throw new Exception("DataContext is not of type WebsitesViewModel");
            }
        }
    }
}