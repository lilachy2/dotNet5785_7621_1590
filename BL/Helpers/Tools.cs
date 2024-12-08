
using DO;

namespace Helpers;
// for help fun
internal static class Tools
{
    //public static string ToStringProperty<T>(this T t)
    //{


    //}

    private static bool HelpCheck(BO.Volunteer volunteer) ////// להשלים
    {
              DalApi.IDal _dal = DalApi.Factory.Get;

        // 1. בדיקת אימייל - חייב לכלול @gmail.com
        if (string.IsNullOrEmpty(volunteer.Email) || !volunteer.Email.Contains("@gmail.com"))
        {
            return false; // אימייל לא תקין
        }

        // 2. בדיקת מספר טלפון - חייב להיות אורך 10 ספרות בלבד כולל ספרת ביקורת
        if (string.IsNullOrEmpty(volunteer.Number_phone) || volunteer.Number_phone.Length != 10 || !volunteer.Number_phone.All(char.IsDigit) || !IsValidPhoneNumber(volunteer.Number_phone))
        {
            return false; // מספר טלפון לא תקין
        }

        // 3. בדיקת ת.ז - חייב להיות ערך מספרי עם ספרת ביקורת תקינה
        if (volunteer.Id <= 0 || !IsValidID(volunteer.Id)) // יש להוסיף לוגיקה לבדוק אם ת.ז תקינה
        {
            return false; // ת.ז לא תקינה
        }

        // 4. בדיקת כתובת - חייבת לכלול קווי אורך ורוחב תקינים
        if (string.IsNullOrEmpty(volunteer.FullCurrentAddress) || volunteer.Latitude == null || volunteer.Longitude == null)
        {
            return false; // כתובת לא תקינה או חסר קווי אורך/רוחב
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





}

