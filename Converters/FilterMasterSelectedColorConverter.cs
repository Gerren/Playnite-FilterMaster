using FilterMaster.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace FilterMaster.Converters
{
    internal class FilterMasterSelectedColorConverter : IValueConverter
    {
        public static readonly SolidColorBrush darkGray = new SolidColorBrush(Color.FromArgb(128, 34, 36, 38));
        public static readonly SolidColorBrush gray = new SolidColorBrush(Color.FromArgb(128, 155, 170, 186));
        public static readonly SolidColorBrush green = new SolidColorBrush(Color.FromArgb(128, 74, 115, 71));
        public static readonly SolidColorBrush red = new SolidColorBrush(Color.FromArgb(128, 117, 30, 8));
        public static readonly SolidColorBrush orange = new SolidColorBrush(Color.FromArgb(128, 214, 142, 34));


        public static readonly SolidColorBrush cyan = new SolidColorBrush(Color.FromArgb(128, 0, 126, 135));
        public static readonly SolidColorBrush blue = new SolidColorBrush(Color.FromArgb(128, 14, 79, 135));
        public static readonly SolidColorBrush purple = new SolidColorBrush(Color.FromArgb(128, 66, 20, 135));
        public static readonly SolidColorBrush pink = new SolidColorBrush(Color.FromArgb(128, 135, 20, 100));
        public static readonly SolidColorBrush yellow = new SolidColorBrush(Color.FromArgb(128, 130, 135, 34));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is SelectedState prop)) return gray;
            switch (prop)
            {
                case SelectedState.Selected: return green;

                case SelectedState.Bracket1: return cyan;
                case SelectedState.Bracket2: return blue;
                case SelectedState.Bracket3: return purple;
                case SelectedState.Bracket4: return pink;
                case SelectedState.Bracket5: return yellow;

                case SelectedState.NotSelected: return red;
                case SelectedState.NotPossible: return darkGray;
                case SelectedState.Maybe: return orange;
                default: return gray;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
