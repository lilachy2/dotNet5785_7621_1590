using System;
using System.Globalization;
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
    public class EnumToStringConverter : IValueConverter
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

    public class EnumToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum enumValue)
            {
                var fieldName = parameter as string;

                // התאמה ל-Enums שונים על פי השדה המועבר כ-parameter
                switch (fieldName)
                {
                    case "Role.Manager":
                        return System.Windows.Media.Brushes.LightGoldenrodYellow; // צבע למנהל
                    case "Role.Volunteer":
                        return System.Windows.Media.Brushes.LightSkyBlue; // צבע לוולונטר

                    case "DistanceType.Aerial_distance":
                        return System.Windows.Media.Brushes.LightYellow;
                    case "DistanceType.walking_distance":
                        return System.Windows.Media.Brushes.LightCoral;
                    case "DistanceType.driving_distance":
                        return System.Windows.Media.Brushes.LightSteelBlue;
                    case "DistanceType.change_distance_type":
                        return System.Windows.Media.Brushes.LightPink;

                    default:
                        return System.Windows.Media.Brushes.Transparent; // ברירת מחדל
                }
            }
            return System.Windows.Media.Brushes.Transparent; // ברירת מחדל אם אין תוצאה
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value; // המרת חזרה לא נדרשת במקרה זה
        }
    }

}
