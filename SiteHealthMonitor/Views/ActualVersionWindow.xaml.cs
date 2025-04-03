using System.Windows;

namespace SiteHealthMonitor.Views;

public partial class ActualVersionWindow : Window
{
    public ActualVersionWindow()
    {
        InitializeComponent();
    }
    
    private void CancelButton(object sender, RoutedEventArgs e)
    {
        Close();
    }
}