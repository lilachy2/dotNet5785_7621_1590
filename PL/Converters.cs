using System;
using System.Globalization;
using System.Security;
using System.Windows;
using System.Windows.Data;

using System.Windows.Media;

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
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return !(bool)value;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return !(bool)value;
            }
            return true;
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


    public class ClosedCallInListEnumToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BO.ClosedCallInListEnum callEnum = (BO.ClosedCallInListEnum)value;

                switch (callEnum)
                {
                    case BO.ClosedCallInListEnum.Id:
                        return System.Windows.Media.Brushes.LightGoldenrodYellow; // צבע עבור Id
                    case BO.ClosedCallInListEnum.FullAddress:
                        return System.Windows.Media.Brushes.LightSkyBlue; // צבע עבור Name
                    case BO.ClosedCallInListEnum.CompletionStatus:
                        return System.Windows.Media.Brushes.LightCoral; // צבע עבור IsActive
                     case BO.ClosedCallInListEnum.EnterTime:
                        return System.Windows.Media.Brushes.LightGreen; // צבע עבור IsActive
                     case BO.ClosedCallInListEnum.OpenTime:
                        return System.Windows.Media.Brushes.LightSalmon; // צבע עבור IsActive
                   case BO.ClosedCallInListEnum.CallType:
                        return System.Windows.Media.Brushes.LightSteelBlue; // צבע עבור IsActive
                    default:
                        return System.Windows.Media.Brushes.LightCyan; // ברירת מחדל
                }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value; // המרת חזרה לא נדרשת במקרה זה
        }
    } 
    public class OpenCallInListEnumToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BO.OpenCallInListEnum callEnum = (BO.OpenCallInListEnum)value;

                switch (callEnum)
                {
                    case BO.OpenCallInListEnum.Id:
                        return System.Windows.Media.Brushes.LightGoldenrodYellow; // צבע עבור Id
                    case BO.OpenCallInListEnum.FullAddress:
                        return System.Windows.Media.Brushes.LightSkyBlue; // צבע עבור Name
                    case BO.OpenCallInListEnum.DistanceFromVolunteer:
                        return System.Windows.Media.Brushes.LightCoral; // צבע עבור IsActive
                     case BO.OpenCallInListEnum.MaxEndTime:
                        return System.Windows.Media.Brushes.LightGreen; // צבע עבור IsActive
                     case BO.OpenCallInListEnum.OpenTime:
                        return System.Windows.Media.Brushes.LightSalmon; // צבע עבור IsActive
                   case BO.OpenCallInListEnum.CallType:
                        return System.Windows.Media.Brushes.LightSteelBlue; // צבע עבור IsActive
                   case BO.OpenCallInListEnum.Description:
                        return System.Windows.Media.Brushes.LightSteelBlue; // צבע עבור IsActive
                    default:
                        return System.Windows.Media.Brushes.LightCyan; // ברירת מחדל
                }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value; // המרת חזרה לא נדרשת במקרה זה
        }
    }
    public class CallInListEnumToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BO.CallInListField callEnum = (BO.CallInListField)value;

                switch (callEnum)
                {
                    case BO.CallInListField.Id:
                        return System.Windows.Media.Brushes.LightGoldenrodYellow; // צבע עבור Id
                    case BO.CallInListField.CompletionTime:
                        return System.Windows.Media.Brushes.LightSkyBlue; // צבע עבור Name
                    case BO.CallInListField.VolunteerName:
                        return System.Windows.Media.Brushes.LightCoral; // צבע עבור IsActive
                     case BO.CallInListField.Status:
                        return System.Windows.Media.Brushes.LightGreen; // צבע עבור IsActive
                     case BO.CallInListField.OpenTime:
                        return System.Windows.Media.Brushes.LightSalmon; // צבע עבור IsActive
                   case BO.CallInListField.CallType:
                        return System.Windows.Media.Brushes.LightSteelBlue; // צבע עבור IsActive
                   case BO.CallInListField.TimeRemaining:
                        return System.Windows.Media.Brushes.Violet; // צבע עבור IsActive
                   case BO.CallInListField.TotalAssignments:
                        return System.Windows.Media.Brushes.Tomato; // צבע עבור IsActive
                    default:
                        return System.Windows.Media.Brushes.LightCyan; // ברירת מחדל
                }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value; // המרת חזרה לא נדרשת במקרה זה
        }
    }

    public class CalltypeToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BO.Calltype callTypeEnum = (BO.Calltype)value;

            switch (callTypeEnum)
            {
                case BO.Calltype.allergy:
                    return System.Windows.Media.Brushes.LightGoldenrodYellow; // צבע עבור Id
                case BO.Calltype.broken_bone:
                    return System.Windows.Media.Brushes.LightSkyBlue; // צבע עבור Name
                case BO.Calltype.birth:
                    return System.Windows.Media.Brushes.LightCoral; // צבע עבור IsActive
                case BO.Calltype.security_event:
                    return System.Windows.Media.Brushes.LightGreen; // צבע עבור IsActive
                case BO.Calltype.heartattack:
                    return System.Windows.Media.Brushes.LightSalmon; // צבע עבור IsActive
                case BO.Calltype.resuscitation:
                    return System.Windows.Media.Brushes.Lavender; // צבע עבור IsActive
                case BO.Calltype.fainting:
                    return System.Windows.Media.Brushes.Violet; // צבע עבור IsActive

               default:
                    return System.Windows.Media.Brushes.LightCyan; // ברירת מחדל
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

    public class ConverterSimulatorState : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "Stop Simulator" : "Start Simulator";
        }
        //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    if (value is bool booleanValue)
        //        return booleanValue ? "Stop Simulator" : "Start Simulator";

        //    return "Start Simulator"; // ערך ברירת מחדל אם הערך אינו תקין
        //}

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    /// Checking whether it is possible to delete a call (CanDelete)
    /// If so → returns Visibility.Visible(the element will be visible).
    /// If not → returns Visibility.Collapsed(the element is hidden).
    /// </summary>
    public class ConvertIsCanDeletCall : IValueConverter
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (s_bl.Call.CanDelete((int)value))
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

   
}
