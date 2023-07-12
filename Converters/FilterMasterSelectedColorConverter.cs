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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is FilterMasterProperty.SelectedState prop)) return gray;
            switch (prop)
            {
                case FilterMasterProperty.SelectedState.Selected: return green;
                case FilterMasterProperty.SelectedState.NotSelected: return red;
                case FilterMasterProperty.SelectedState.NotPossible: return darkGray;
                case FilterMasterProperty.SelectedState.Maybe: return orange;
                default: return gray;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
