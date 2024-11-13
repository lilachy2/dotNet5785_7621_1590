namespace DalTest;
using DalApi;
using DO;
using Microsoft.VisualBasic;
using System.Data;
using System.Net;

/// <param name="Random"> A field that all entities will use, 
/// to generate random numbers while filling in the values ​​of the objects.
/// <param name=""> 
/// <param name=""> 
public static class Initialization
{
    /// <param name=""> fields of the appropriate interface type.
    private static IVolunteer? s_dalVolunteer;
    private static ICall? s_dalCall;
    private static IAssignment? s_dalAssignment;
    private static IConfig? s_dalConfig;


    private static readonly Random s_rand = new();
    private static int MIN_ID = 10000000;
    private static int MAX_ID = 99999999;

    static string[] Names =

{
    "Avi Cohen", "Sara Levi", "Yael Ben David", "Nir Azulay", "Tamar Gilad",
    "Roni Shaked", "Lior Peretz", "Meir Shechter", "Efrat Halevi", "Ofir Bar",
    "Maya Tzur", "Noa Mizrahi", "David Erez", "Tal Koren", "Hila Malka"
};
    static string[] volunteerAddresses =
{
    "7 Presidents St, Petah Tikva, Israel", "Lev Academic Center, Jerusalem, Israel", "45 Rothschild Blvd, Tel Aviv, Israel",
    "12 Herzl St, Haifa, Israel", "20 King David St, Jerusalem, Israel", "3 HaNasi Blvd, Be'er Sheva, Israel",
    "14 Jabotinsky St, Rishon LeZion, Israel", "10 Arlozorov St, Tel Aviv, Israel",
    "22 Hillel St, Jerusalem, Israel", "8 Dizengoff St, Tel Aviv, Israel", "5 Ben Yehuda St, Haifa, Israel",
    "16 Weizmann St, Rehovot, Israel", "9 Begin Ave, Ashdod, Israel",  "11 Allenby St, Tel Aviv, Israel",
    "4 Hanegev St, Eilat, Israel"
};

    public static void CreateVolunteers()
    {
        List<Volunteer> volunteers = new List<Volunteer>();



        foreach (var name in Names)
        {
            int id;
            int i = new int(); // for the arr
            i = 0;
            do
            {
                id = s_rand.Next(MIN_ID, MAX_ID);
            } while (id % 10 == 0); // לבדוק האם הת.ז תקין, לדוגמה, לפי ספרת ביקורת


            string phone = "05" + s_rand.Next(0, 8).ToString() + s_rand.Next(1000000, 9999999).ToString();
            int p1 = int.Parse(phone); // כדי לבצע המרה בשביל הבנאי
            string email = name.Replace(" ", ".").ToLower() + "@volunteer.org";
            //string? password = null; // סיסמא ראשונית או null עד שהמתנדב יעדכן
            string? address = volunteerAddresses[i];
            Role role = Role.Volunteer;
            distance_type distanceType = distance_type.Aerial_distance;

            double? maxDistance = s_rand.Next(5, 20); // מרחק ברירת מחדל בקילומטר

            Random rand = new Random(); // לקווי אורך ורוחב
            bool active = true;

            // טווחי קו רוחב ואורך בישראל
            double minLatitude = 29.5;
            double maxLatitude = 33.5;
            double minLongitude = 34.3;
            double maxLongitude = 35.9;
            double randomLatitude = rand.NextDouble() * (maxLatitude - minLatitude) + minLatitude;
            double randomLongitude = rand.NextDouble() * (maxLongitude - minLongitude) + minLongitude;

            volunteers.Add(new Volunteer(id, name, p1, email, role, distanceType, address, randomLatitude, randomLongitude, active, maxDistance));
            i = i + 1;

        }

    }

    public static void CreateCalls()
    {
        List<Call> calls = new List<Call>();

        foreach (var name in Names)
        {
            int i = new int(); // for the arr
            i = 0;
            string? address = volunteerAddresses[i];

            Calltype calltype = Calltype.birth;

            string? VerbalDescription = null;
            Random rand = new Random(); // לקווי אורך ורוחב
            bool active = true;

            // טווחי קו רוחב ואורך בישראל
            double minLatitude = 29.5;
            double maxLatitude = 33.5;
            double minLongitude = 34.3;
            double maxLongitude = 35.9;
            double randomLatitude = rand.NextDouble() * (maxLatitude - minLatitude) + minLatitude;
            double randomLongitude = rand.NextDouble() * (maxLongitude - minLongitude) + minLongitude;


            //    // יש פה בעיה 
            //    // מגדירים את זמן הפתיחה מתוך שעון המערכת ב-IConfig
            //    IConfig config = new IConfig();  // יצירת מופע של מחלקת Config

            //    DateTime OpenTime = IConfig.Clock;

            //// מגדירים זמן מקסימלי לסיום לפי OpenTime עם רווח בטווח סיכון רנדומלי
            //TimeSpan riskSpan = IConfig.RiskRange;

            //// להוספת רווח רנדומלי בטווח זמן סיכון מסוים (אם MaxEndTime לא null)
            //Random random = new Random();
            //int extraHours = random.Next(1, 24); // אקראי בין שעה ל-24 שעות, בהתאם לצורך
            //int extraMinutes = random.Next(1, 60);

            //MaxEndTime = OpenTime.Add(riskSpan).AddHours(extraHours).AddMinutes(extraMinutes);
            i = i + 1;
        }

        // להשלים
        //DateTime OpeningTime = default,
        //DateTime? MaxEndTime = null

       

        calls.Add(new Call(maxLatitude, randomLongitude, calltype, 0, VerbalDescription, address, null, null));


    }
}

    


