
using System.Reflection;

namespace Helpers;
// for help fun
internal static class Tools
{
    private static readonly DalApi.IDal _dal = DalApi.Factory.Get;

    //public static string ToStringProperty<T>(this T t)
    //{


    //}

    public static bool HelpCheck(BO.Volunteer volunteer) ////// להשלים
    public static string ToStringProperty<T>(this T t)
    {
        string str = "";
        foreach (PropertyInfo item in t.GetType().GetProperties())
            str += "\n" + item.Name + ": " + item.GetValue(t, null);
        return str;
    }

    public static string ToStringPropertyArray<T>(this T[] t)
    {
        string str = "";
        foreach (var elem in t)
        {
            foreach (PropertyInfo item in t.GetType().GetProperties())
                str += "\n" + item.Name + ": " + item.GetValue(t, null);
        }
        return str;
    }

        // 5. בדיקת קווי רוחב ואורך - חייב להיות בין -90 ל-90 עבור Latitude ו- בין -180 ל-180 עבור Longitude
        if (volunteer.Latitude < -90 || volunteer.Latitude > 90 || volunteer.Longitude < -180 || volunteer.Longitude > 180)
        {
            return false; // קווי אורך/רוחב לא תקינים
        }

        // 6. בדיקת תפקיד - רק מנהל יכול לשנות את תפקיד המתנדב
        if (volunteer.Role != Role.Manager &)
        {
            return false; // מתנדב לא יכול לשנות את תפקידו
        }

        // 7. בדיקת מרחק - אם קיים, חייב להיות ערך חיובי
        if (volunteer.Distance.HasValue && volunteer.Distance <= 0)
        {
            return false; // אם המרחק לא תקין
        }

        // 8. בדיקת סטטוס פעיל - חייב להיות אמת או שקר
        if (volunteer.Active != true && volunteer.Active != false)
        {
            return false; // אם הסטטוס לא תקין
        }

        // 9. בדיקת סיסמה - אם קיימת, חייבת להיות באורך מינימלי של 6 תווים
        if (!string.IsNullOrEmpty(volunteer.Password) && volunteer.Password.Length < 6)
        {
            return false; // אם הסיסמה קצרה מדי
        }

        // 10. בדיקת שדות מספריים - לדוגמה, TotalHandledCalls, TotalCancelledCalls, TotalExpiredCalls
        if (volunteer.TotalHandledCalls < 0 || volunteer.TotalCancelledCalls < 0 || volunteer.TotalExpiredCalls < 0)
        {
            return false; // שדות מספריים לא תקינים
        }

        return true; // כל הבדיקות עברו בהצלחה


    }

    private static bool HelpCheckdelete(BO.Volunteer volunteer)
    {
        DO.Volunteer doVolunteer=_dal.Volunteer.Read(volunteer.Id);
        



    }




}

