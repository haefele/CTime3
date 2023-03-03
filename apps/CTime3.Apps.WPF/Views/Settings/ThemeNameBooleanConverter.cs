using System.Globalization;
using System.Windows.Data;
using CommunityToolkit.Diagnostics;

namespace CTime3.Apps.WPF.Views.Settings
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "The converter is used in XAML files.")]
    internal class ThemeNameBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Guard.IsOfType<string>(parameter);

            if (!Enum.IsDefined(typeof(Wpf.Ui.Appearance.ThemeType), value))
                throw new ArgumentException("ExceptionEnumToBooleanConverterValueMustBeAnEnum");

            var enumValue = Enum.Parse(typeof(Wpf.Ui.Appearance.ThemeType), (string)parameter);

            return enumValue.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Guard.IsOfType<string>(parameter);

            return Enum.Parse(typeof(Wpf.Ui.Appearance.ThemeType), (string)parameter);
        }
    }
}
