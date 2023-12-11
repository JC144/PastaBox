using System.Collections;

namespace PastaBox.Helpers
{
    public sealed class NullOrEmptyToVisibilityConverter : System.Windows.Data.IValueConverter
    {
        public bool Inverted { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility visibility = Visibility.Visible;

            if (value == null
                || (value is string && String.IsNullOrEmpty((string)value))
                || (value is IEnumerable && !((IEnumerable)value).Cast<object>().Any())
                || (value is int && (int)value == 0))
            {
                visibility = Visibility.Collapsed;
            }

            if (Inverted)
            {
                visibility = visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            }

            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                  System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
