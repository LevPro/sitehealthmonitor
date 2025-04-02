using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SiteHealthMonitor.Models
{
    public class Website : INotifyPropertyChanged
    {
        public required string Url { get; set; }
        public DateTime LastChecked { get; set; }
        public bool IsAvailable { get; set; }
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }
        
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}