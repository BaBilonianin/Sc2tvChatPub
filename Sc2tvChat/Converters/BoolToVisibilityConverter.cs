using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace RatChat.Converters {

    class MySupportConverter : IValueConverter {
        public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture ) {
            bool ret = ((string)value).Contains((string)parameter);
            return ret;
                
        }

        public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture ) {
            throw new NotImplementedException();
        }
    }

    class BoolToVisibilityConverter: IValueConverter {
        public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture ) {
            if ((bool)value) {
                return Visibility.Visible;
            } else {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture ) {
            return ((Visibility)value) == Visibility.Visible;
        }
    }
   

}
