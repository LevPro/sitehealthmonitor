using System.Windows.Data;

namespace SiteHealthMonitor.ValueConverter
{
    public class BooleanToYesNoConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            return (value is bool boolValue) 
                ? boolValue ? "Да" : "Нет" 
                : "Нет";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}