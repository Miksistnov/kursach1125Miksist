using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace SalesDataAnalysisApp.Converters
{
    public class ReadStatusToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isRead = value is bool b && b;
            string iconPath = isRead ? "pack://application:,,,/Resources/read.png" : "pack://application:,,,/Resources/unread.png";
            return new BitmapImage(new Uri(iconPath));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
