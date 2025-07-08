using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
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
}
