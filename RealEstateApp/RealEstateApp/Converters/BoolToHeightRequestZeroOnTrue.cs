using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp.Converters
{
  public class BoolToHeightRequestZeroOnTrue : IValueConverter, IMarkupExtension
  {
    public object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      // Validate and convert input.
      if (value is bool boolean && boolean)
      {
        return 0;
      }

      return -1;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }
}
