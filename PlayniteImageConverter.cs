using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace FilterMaster
{
    internal class PlayniteImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null || (value is string path && string.IsNullOrEmpty(path)))
                {
                    if (ResourceProvider.GetResource("DefaultGameIcon") != null)
                    {
                        return  ResourceProvider.GetResource("DefaultGameIcon");
                    }
                }
                else
                {
                    if (!(value is string))
                    {
                        return  value;
                    }

                    if (!File.Exists((string)value))
                    {
                        if (File.Exists(API.Instance.Database.GetFullFilePath((string)value)))
                        { 
                            return API.Instance.Database.GetFullFilePath((string)value);
                        }
                        else
                        {
                            if (ResourceProvider.GetResource("DefaultGameIcon") != null)
                            {
                                return ResourceProvider.GetResource("DefaultGameIcon");
                            }
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
