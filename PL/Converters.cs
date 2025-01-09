using System;
using System.Globalization;
using System.Security;
using System.Windows;
using System.Windows.Data;

namespace PL
{
    // Converter שממיר את ערך ה-Id למצב IsReadOnly
    public class ConvertUpdateToTrueKey : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // אם ה-Id לא אפס (לא מצב הוספה), אז לא ניתן לשנות
            return value != null && (int)value != 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    // Converter שממיר את ערך ה-Id למצב Visibility
    public class ConvertUpdateToVisibleKey : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // אם ה-Id קיים (במצב עדכון), אז מראים את האלמנט
            return value != null && (int)value != 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    // Converter שממיר Enum למחרוזת ידידותית
    public class RoleEnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum)
            {
                // ממיר את שם ה-Enum למחרוזת ידידותית
                return value.ToString();
            }
            return string.Empty; // ברירת מחדל: מחרוזת ריקה
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is Type enumType && Enum.IsDefined(enumType, value))
            {
                return Enum.Parse(enumType, value.ToString());
            }
            throw new InvalidOperationException("Cannot convert back to Enum.");
        }
    }

    public class RoleEnumToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            BO.Role role = (BO.Role)value;

            // התאמה ל-Enums שונים על פי השדה המועבר כ-parameter
            switch (role)
            {
                case BO.Role.Manager:
                    return System.Windows.Media.Brushes.LightGoldenrodYellow; // צבע למנהל
                case BO.Role.Volunteer:
                    return System.Windows.Media.Brushes.LightSkyBlue; // צבע לוולונטר
                default:
                    return System.Windows.Media.Brushes.Transparent;
            }
           
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value; // המרת חזרה לא נדרשת במקרה זה
        }
    }
    public class VolInListEnumToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            BO.VolInList vol = (BO.VolInList)value;

            // התאמה ל-Enums שונים על פי השדה המועבר כ-parameter
            switch (vol)
            {
                case BO.VolInList.Id:
                    return System.Windows.Media.Brushes.LightGoldenrodYellow; 
                case BO.VolInList.Name:
                    return System.Windows.Media.Brushes.LightSkyBlue; 
                case BO.VolInList.IsActive:
                    return System.Windows.Media.Brushes.LightPink; 
                default:
                    return System.Windows.Media.Brushes.Transparent;
            }
           
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value; // המרת חזרה לא נדרשת במקרה זה
        }
    }
    public class DistanceTypeEnumToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
               
            BO.DistanceType distance_type = (BO.DistanceType)value;

            // התאמה ל-Enums שונים על פי השדה המועבר כ-parameter
            switch (distance_type)
            {
                case BO.DistanceType.Aerial_distance:
                    return System.Windows.Media.Brushes.LightYellow;
                case BO.DistanceType.walking_distance:
                    return System.Windows.Media.Brushes.LightCoral;
                case BO.DistanceType.driving_distance:
                    return System.Windows.Media.Brushes.LightSteelBlue;
                case BO.DistanceType.change_distance_type:
                    return System.Windows.Media.Brushes.LightPink;

                default:
                    return System.Windows.Media.Brushes.Transparent; // ברירת מחדל
            }


        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value; // המרת חזרה לא נדרשת במקרה זה
        }
    }


    public class PasswordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // אם הערך לא null, ממיר את ה-SecureString ל-string
            if (value is SecureString secureString)
            {
                IntPtr ptr = IntPtr.Zero;
                try
                {
                    ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(secureString);
                    return System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr);
                }
                finally
                {
                    if (ptr != IntPtr.Zero)
                    {
                        System.Runtime.InteropServices.Marshal.FreeBSTR(ptr);
                    }
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // אם הערך לא null, ממיר את ה-string ל-SecureString
            if (value is string str)
            {
                SecureString secureString = new SecureString();
                foreach (char c in str)
                {
                    secureString.AppendChar(c);
                }
                secureString.MakeReadOnly();
                return secureString;
            }
            return null;
        }
    }

}
