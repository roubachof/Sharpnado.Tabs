using System.Globalization;
using MauiSample.Domain;

namespace MauiSample.Presentation.Converters
{
    public class ExceptionToErrorMessageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var exception = value as Exception;

            if (value == null)
            {
                return null;
            }

            return ApplicationExceptions.ToString(exception);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // One-Way converter only
            throw new NotImplementedException();
        }
    }
}
