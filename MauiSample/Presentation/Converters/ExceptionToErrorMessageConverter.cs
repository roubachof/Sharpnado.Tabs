using System.Globalization;
using MauiSample.Domain;

namespace MauiSample.Presentation.Converters
{
    public class ExceptionToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            string imageName;

            switch (value)
            {
                case ServerException serverException:
                    imageName = "server.png";
                    break;
                case NetworkException networkException:
                    imageName = "the_internet.png";
                    break;
                default:
                    imageName = "richmond.png";
                    break;
            }

            return ImageSource.FromFile(imageName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // One-Way converter only
            throw new NotImplementedException();
        }
    }
}
