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

    // Converter לממיר את ה-Enum לערך צבע עבור כל שדה (לפי ENUMs שונים)
    public class EnumToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum enumValue)
            {
                // כל שדה יקבל צבע לפי שם השדה
                var fieldName = parameter as string;

                switch (fieldName)
                {
                    case "Id":
                        return System.Windows.Media.Brushes.LightGray;
                    case "Name":
                        return System.Windows.Media.Brushes.LightBlue;
                    case "IsActive":
                        return System.Windows.Media.Brushes.LightGreen;
                    case "None":
                        return System.Windows.Media.Brushes.Transparent;

                    // עבור ה-ENUM VolInList
                    case "VolInList.Id":
                        return System.Windows.Media.Brushes.LightGray;
                    case "VolInList.Name":
                        return System.Windows.Media.Brushes.LightBlue;
                    case "VolInList.IsActive":
                        return System.Windows.Media.Brushes.LightGreen;
                    case "VolInList.None":
                        return System.Windows.Media.Brushes.Transparent;

                    // עבור ה-ENUM DistanceType
                    case "DistanceType.Aerial_distance":
                        return System.Windows.Media.Brushes.LightYellow;
                    case "DistanceType.walking_distance":
                        return System.Windows.Media.Brushes.LightCoral;
                    case "DistanceType.driving_distance":
                        return System.Windows.Media.Brushes.LightSteelBlue;
                    case "DistanceType.change_distance_type":
                        return System.Windows.Media.Brushes.LightPink;

                    // צבעים עבור ENUMים אחרים
                    default:
                        return System.Windows.Media.Brushes.Transparent;
                }
            }

            return System.Windows.Media.Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value; // לא נחוץ במקרה הזה
        }
    }
}
