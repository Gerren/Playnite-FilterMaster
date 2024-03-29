using FilterMaster.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FilterMaster.Converters
{
    internal class FilterMasterFilterVisibliltyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is SelectedState prop)) return Visibility.Visible;
            switch (prop)
            {
                case SelectedState.NotPossible: return  FilterMasterSelectGameViewModel.showUnavailable ? Visibility.Visible : Visibility.Collapsed;
                default: return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
