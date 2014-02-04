using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Finances
{
    public class StringTruncateConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int maxLength;
            if (int.TryParse(parameter.ToString(), out maxLength))
            {
                string val = (value == null) ? null : value.ToString();
                if (val != null && val.Length > maxLength)
                {
                    return val.Substring(0, maxLength) + "..";
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
