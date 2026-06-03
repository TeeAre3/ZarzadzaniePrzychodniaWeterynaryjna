using System;
using System.Globalization;
using System.Windows.Data;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Converters
{
    public class DecimalConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString();
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string wpisanyTekst)
            {
                if (decimal.TryParse(wpisanyTekst.Replace(".", ","), NumberStyles.Any, culture, out decimal wynik))
                {
                    return wynik;
                }
            }
            return null;
        }
    }
}