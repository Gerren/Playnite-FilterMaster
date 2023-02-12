using FilterMaster.Models;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace FilterMaster.Converters
{
    internal class FilterMasterGameVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length<2 || values[0] == null || !(values[0] is Game game) || values[1] == null || !(values[1] is IGameVisibilityDeterminer determiner)) return Visibility.Visible;
            return determiner.IsVisible(game) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
