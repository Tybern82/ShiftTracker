using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using Avalonia.Data.Converters;

namespace EnumBindingWithDescription {
    public class EnumDescriptionConverter : IValueConverter {

        object? IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
            if (value == null) return string.Empty;
            Enum myEnum = (Enum)value;
            string description = com.tybern.ShiftTracker.EnumConverter.GetEnumDescription(myEnum);
            return description;
        }

        object IValueConverter.ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
            return string.Empty;
        }
    }
}