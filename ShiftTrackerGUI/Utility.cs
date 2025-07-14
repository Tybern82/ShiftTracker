using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using com.tybern.ShiftTracker;
using com.tybern.ShiftTracker.data;

namespace ShiftTrackerGUI {
    internal class Utility {

        public static WindowPosition getPosition(Window wnd) {
            return new WindowPosition() { Height = wnd.Height, Width = wnd.Width, PositionX = wnd.Position.X, PositionY = wnd.Position.Y };
        }

        public static void setPosition(Window wnd, WindowPosition position) {
            wnd.Height = position.Height;
            wnd.Width = position.Width;
            wnd.Position = new Avalonia.PixelPoint(position.PositionX, position.PositionY);
        }
    }

    public class PercentageConverter : MarkupExtension, IValueConverter {
        private static PercentageConverter? _instance;

        #region IValueConverter Members  

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
            return System.Convert.ToDouble(value) * System.Convert.ToDouble(parameter);
        }
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }

        #endregion

        public override object ProvideValue(IServiceProvider serviceProvider) {
            return _instance ??= new PercentageConverter();
        }
    }
}
