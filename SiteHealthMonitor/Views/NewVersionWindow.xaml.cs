using System.Diagnostics;
using System.Windows;
using SiteHealthMonitor.Utilities;

namespace SiteHealthMonitor.Views
{
    public partial class NewVersionWindow : Window
    {
        public NewVersionWindow()
        {
            InitializeComponent();
        }

        private void CancelButton(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Download(object sender, RoutedEventArgs e)
        {
            Runtime.OpenUrl("https://github.com/your/repo/releases/latest");
            Close();
        }
    }
}