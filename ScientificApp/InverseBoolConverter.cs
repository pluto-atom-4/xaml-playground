using System;
using System.Globalization;
using System.Windows.Data;

namespace ScientificApp;

/// <summary>
/// Converts boolean to inverse boolean (true → false, false → true).
/// Useful for button IsEnabled binding with IsGenerating property.
/// </summary>
public class InverseBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
            return !boolValue;

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
            return !boolValue;

        return value;
    }
}
