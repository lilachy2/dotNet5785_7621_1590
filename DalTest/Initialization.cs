﻿namespace DalTest;
using DalApi;
using DO;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Numerics;
using System.Xml.Linq;

// <param name="s_dalVolunteer">fields of the appropriate interface type.</param>
// <param name="s_dalCall">fields of the appropriate interface type.</param>
// <param name="s_dalAssignment">fields of the appropriate interface type.</param>
// <param name="s_dalConfig">fields of the appropriate interface type.</param>
// <param name="s_rand">Random number generator.</param>
// <param name="MIN_ID">Minimum ID value.</param
// <param name="MAX_ID">Maximum ID value.</param>
//<param name = "Names" > List of possible names.</param>
public static class Initialization
{
    // stage1
    //private static IVolunteer? s_dalVolunteer;  
    //private static ICall? s_dalCall;  
    //private static IAssignment? s_dalAssignment; 
    //private static IConfig? s_dalConfig;

    private static IDal? s_dal; //stage 2


    private static readonly Random s_rand = new Random();
    private static int MIN_ID = 100000000;
    private static int MAX_ID = 999999999;
    static int indexcalltype1 = s_rand.Next(0, 6);


    static string[] Names =
    {
        "Avi Cohen", "Sara Levi", "Yael Ben David", "Nir Azulay", "Tamar Gilad",
        "Roni Shaked", "Lior Peretz", "Meir Shechter", "Efrat Halevi", "Ofir Bar",
        "Maya Tzur", "Noa Mizrahi", "David Erez", "Tal Koren", "Hila Malka"
    };

    static string[] israeliAddressesHebrow = new string[]
{
    "שדרות רוטשילד 1, תל אביב",
    "ז'בוטינסקי 2, רמת גן",
    "הרצל 3, חיפה",
    "בן יהודה 4, ירושלים",
    "ויצמן 5, רחובות",
    "דיזנגוף 6, תל אביב",
    "מלך ג'ורג' 7, ירושלים",
    "סוקולוב 8, חיפה",
    "שדרות רוקח 9, באר שבע",
    "המרכבה 10, נתניה",
    "שדרות ירושלים 11, חולון",
    "דב הוז 12, בת ים",
    "כיכר רבין 13, ראשון לציון",
    "אבן גבירול 14, פתח תקווה",
    "ז'בוטינסקי 15, בני ברק",
    "ברזילאי 16, אשקלון",
    "המרפא 17, הרצליה",
    "מרכז העיר 18, כפר סבא",
    "קיבוץ גלויות 19, חדרה",
    "שדרות ירושלים 20, מודיעין",
    "המרכז 21, נצרת",
    "שדרות רוטשילד 22, רמת גן",
    "שושן 23, רעננה",
    "חיים ויצמן 24, גבעתיים",
    "הכחול 25, עכו",
    "הילד 26, אילת",
    "קיבוץ 27, קריית גת",
    "הסופר 28, קריית מוצקין",
    "שדרות ירושלים 29, קריית ים",
    "הרב הרצוג 30, קריית ביאליק",
    "הגלבוע 31, קריית אתא",
    "עין כרם 32, נהריה",
    "הפועל 33, טבריה",
    "המרכז 34, ראש העין",
    "השלום 35, יבנה",
    "הדר 36, אור יהודה",
    "לוד 37, רמלה",
    "מרכז 38, כפר יונה",
    "הצפון 39, שדרות",
    "אלוני 40, דימונה",
    "יונתן 41, ערד",
    "זבולון 42, ירושלים",
    "סיני 43, מגדל העמק",
    "לבון 44, כרמיאל"
};


    static readonly string[] Descriptions =  // <param name="Descriptions">List of event descriptions.</param>
    {
        "A situation where someone has fainted and needs assistance.",            // fainting
        "A birth event requiring medical personnel to assist with delivery.",      // birth
        "A resuscitation call where CPR or other life-saving measures are needed.", // resuscitation
        "An allergy attack, possibly involving severe reactions such as anaphylaxis.", // allergy
        "A heart attack event requiring urgent medical intervention.",             // heartattack
        "A call for a broken bone injury that needs stabilization and treatment.", // broken_bone
        "A security event involving a threat or emergency requiring a quick response." ,// security_event
            "there is not call yet"
    };

    static readonly Calltype[] CalltypeArray =
{
    Calltype.fainting,
    Calltype.birth,
    Calltype.resuscitation,
    Calltype.allergy,
    Calltype.heartattack,
    Calltype.broken_bone,
    Calltype.security_event,
    Calltype.None
};


    public static void CreateVolunteers()
    {
        // Latitude and longitude range in Israel
        double minLatitude = 29.5;
        double maxLatitude = 33.5;
        double minLongitude = 34.3;
        double maxLongitude = 35.9;
        /// <param name="i"> // For the array index </param>
        /// <param name="p1"> // To perform conversion for the constructor </param>
        /// <param name="address"> // Initial password or null until the volunteer updates it </param>
        /// <param name="maxDistance"> // Default distance in kilometers </param>
        /// <param name="rand"> // For latitude and longitude </param>
        /// <param name="minLatitude"> // Latitude range in Israel </param>
        /// <param name="maxLatitude"> // Latitude range in Israel </param>
        /// <param name="minLongitude"> // Longitude range in Israel </param>
        /// <param name="maxLongitude"> // Longitude range in Israel </param>
        /// <param name="randomLatitude"> // Random latitude in Israel </param>
        /// <param name="randomLongitude"> // Random longitude in Israel </param>

        bool isFirst = true;
        int id;
        int i = new int(); // For the array index
        i = 0;

        foreach (var name in Names)
        {
            do
            {
                id = s_rand.Next(MIN_ID, MAX_ID);

            }
            //while (s_dalVolunteer!.Read(id) != null); //stage1
            while (s_dal!.Volunteer.Read(id) != null); //stage2
            //while (s_dal!.Volunteer.Read(v => v.Id == id) != null);//stage2

            string phone = "05" + s_rand.Next(0, 8).ToString() + s_rand.Next(1000000, 9999999).ToString();
            ////string p1 = phone; // To perform conversion for the constructor
            //int p1 = int.Parse(phone); // To perform conversion for the constructor
            string email = name.Replace(" ", ".").ToLower() + "@volunteer.org";
            string? password = "Hey1234@"; // Initial password or null until the volunteer updates it
            string? address = /*Addresses*/israeliAddressesHebrow[i];
            //string? address = "Eiffel Tower, Paris, France";
            Role role;
            role = Role.Volunteer;

            if (isFirst)
            {
                role = Role.Manager; // One Manager
                isFirst = false;
            }
            distance_type distanceType = distance_type.Aerial_distance;

            double? maxDistance = s_rand.Next(5, 20); // Default distance in kilometers

            //Random rand = new Random(); // For latitude and longitude
            bool active = true;


            double randomLatitude = s_rand.NextDouble() * (maxLatitude - minLatitude) + minLatitude;
            double randomLongitude = s_rand.NextDouble() * (maxLongitude - minLongitude) + minLongitude;

            //s_dalVolunteer.Create(new Volunteer(id, name, p1, email, role, distanceType, address, randomLatitude, randomLongitude, active, maxDistance)); //stage1
            s_dal!.Volunteer.Create(new Volunteer(id, name, phone, email, password, role, distanceType, active, address, randomLatitude, randomLongitude, maxDistance)); //stage2
            i = i + 1;
        }
        s_dal!.Volunteer.Create(new Volunteer(326017621, "lilach", "0524031048", "l@gmail.com", "Mamy1234@", Role.Manager, distance_type.Aerial_distance, true, "חיים תורן 25, ירושלים", 30, 30, 12)); //stage2
        s_dal!.Volunteer.Create(new Volunteer(324291590, "yalin", "0528050630", "y@gmail.com", "Yalin1234@", Role.Manager, distance_type.Aerial_distance, true, "צופים", 30, 30, 12)); //stage2
        s_dal!.Volunteer.Create(new Volunteer(325530103, "גלעד", "0547901655", "g@gmail.com", "Gilad1234@", Role.Volunteer, distance_type.Aerial_distance, true, "חיים תורן 25, ירושלים", 30, 30, 12)); //stage2

    }

    private static (double latitude, double longitude) GetHardcodedCoordinates(string address)
    {
        double minLatitude = 29.5;
        double maxLatitude = 33.5;
        double minLongitude = 34.3;
        double maxLongitude = 35.9;
        // מיפוי כתובות לקואורדינטות קבועות
        Dictionary<string, (double lat, double lon)> addressMap = new()
    {
        {"7 Presidents St, Petah Tikva, Israel", (32.0898, 34.8867)},
        {"Lev Academic Center, Jerusalem, Israel", (31.7650, 35.1897)},
        {"45 Rothschild Blvd, Tel Aviv, Israel", (32.0634, 34.7750)},
        {"12 Herzl St, Haifa, Israel", (32.8191, 34.9983)},
        {"20 King David St, Jerusalem, Israel", (31.7767, 35.2234)},
        {"3 HaNasi Blvd, Be'er Sheva, Israel", (31.2516, 34.7915)},
        {"14 Jabotinsky St, Rishon LeZion, Israel", (31.9644, 34.8044)},
        {"10 Arlozorov St, Tel Aviv, Israel", (32.0873, 34.7817)},
        {"22 Hillel St, Jerusalem, Israel", (31.7800, 35.2200)},
        {"8 Dizengoff St, Tel Aviv, Israel", (32.0753, 34.7737)},
        {"5 Ben Yehuda St, Haifa, Israel", (32.8184, 34.9885)},
        {"16 Weizmann St, Rehovot, Israel", (31.8928, 34.8113)},
        {"9 Begin Ave, Ashdod, Israel", (31.7892, 34.6400)},
        {"11 Allenby St, Tel Aviv, Israel", (32.0664, 34.7703)},
        {"4 Hanegev St, Eilat, Israel", (29.5581, 34.9482)}
    };

        if (addressMap.TryGetValue(address, out var coordinates))
        {
            return coordinates;
        }

        // אם הכתובת לא נמצאה, נחזיר קואורדינטות רנדומליות כגיבוי
        return ((s_rand.NextDouble() * (maxLatitude - minLatitude) + minLatitude), (s_rand.NextDouble() * (maxLongitude - minLongitude) + minLongitude)
        );
    }



    /// <param name="index1"> // Random index for address selection </param>
    /// <param name="index2"> // Random index for call type selection </param>
    /// <param name="tempID"> // For the array </param>
    /// <param name="address"> // Random address selected from the list </param>
    /// <param name="calltype"> // Random call type assigned </param>
    /// <param name="VerbalDescription"> // Description for the selected call type </param>
    /// <param name="active"> // Call status (always active in this case) </param>
    /// <param name="randomLatitude"> // Random latitude in Israel </param>
    /// <param name="randomLongitude"> // Random longitude in Israel </param>
    /// <param name="currentTime"> // System clock representing the current time </param>
    /// <param name="maxTimeSpanBackwards"> // Maximum range for setting call open time, e.g., up to 30 days ago </param>
    /// <param name="randomOffset"> // Random offset to generate a random open time </param>
    /// <param name="openTime"> // Randomly generated open time for the call </param>
    /// <param name="maxEndTime"> // Max end time, may be null if no end time is assigned </param>
    /// <param name="hasEndTime"> // Randomly decides if there should be an end time (50% chance) </param>
    /// <param name="riskSpan"> // Base risk range to add to calculate end time </param>
    /// <param name="extraHours"> // Random extra hours added to the max end time </param>
    /// <param name="extraMinutes"> // Random extra minutes added to the max end time </param>




    public static void CreateCalls()
    {
        // Latitude and longitude range in Israel
        double minLatitude = 29.5;
        double maxLatitude = 33.5;
        double minLongitude = 34.3;
        double maxLongitude = 35.9;

        for (int i = 0; i < 50; i++)
        {
            // יצירת אינדקסים רנדומליים בכל איטרציה
            int index1 = s_rand.Next(0, israeliAddressesHebrow.Length);
            int index2 = s_rand.Next(0, CalltypeArray.Length);

            string? address = israeliAddressesHebrow[index1];
            Calltype calltype = CalltypeArray[index2];
            string? VerbalDescription = Descriptions[index2];

            double randomLatitude = s_rand.NextDouble() * (maxLatitude - minLatitude) + minLatitude;
            double randomLongitude = s_rand.NextDouble() * (maxLongitude - minLongitude) + minLongitude;

            // יצירת זמן פתיחה רנדומלי ב-48 השעות האחרונות
            DateTime openTime = s_dal!.Config.Clock.AddMinutes(-s_rand.Next(0, 2880));

            // יצירת זמן סיום רנדומלי בהתאם לטווח זמנים שונים ליצירת כל הסטטוסים
            DateTime? maxEndTime = null;

            int randomStatus = s_rand.Next(0, 6); // בחירה רנדומלית של סטטוס

            switch (randomStatus)
            {
                case 0: // Open
                    maxEndTime = openTime.AddHours(1);

                    break;

                case 1: // OpenAtRisk
                    maxEndTime = openTime.AddHours(1);
                    openTime = s_dal.Config.Clock.AddMinutes(-s_rand.Next(0, 30)); // זמן פתיחה קרוב מאוד לזמן הנוכחי
                    break;

                case 2: // InProgress
                    openTime = s_dal.Config.Clock.AddMinutes(-s_rand.Next(30, 1440)); // פתיחה בטווח של עד 24 שעות
                    break;

                case 3: // InProgressAtRisk
                    openTime = s_dal.Config.Clock.AddMinutes(-s_rand.Next(1440, 2880)); // פתיחה רחוקה מאוד
                    break;

                case 4: // Closed
                    maxEndTime = openTime.AddMinutes(s_rand.Next(10, 30)); // זמן סיום תקין בין 10 ל-30 דקות אחרי זמן הפתיחה
                    break;

                case 5: // Expired
                    maxEndTime = openTime.AddMinutes(s_rand.Next(10, 30)); // זמן סיום אחרי זמן הפתיחה אבל בטווח בטוח
                    break;
            }

            // יצירת קריאה חדשה והוספתה ל-DAL
            s_dal.Call.Create(new Call(randomLatitude, randomLongitude, calltype, default, VerbalDescription, address, openTime, maxEndTime));
        }
    }


    private static void CreateAssignment()
    {
        for (int i = 0; i < 60; i++)
        {
            Volunteer volunteerToAssign = s_dal!.Volunteer.ReadAll()
                                        .OrderBy(v => s_rand.Next()).First();

            Call callToAssign = s_dal.Call!.ReadAll()
                                    .OrderBy(v => s_rand.Next()).First();

            while (callToAssign.OpeningTime > s_dal!.Config!.Clock)
            {
                callToAssign = s_dal.Call.ReadAll().OrderBy(v => s_rand.Next()).First();
            }
            AssignmentCompletionType? finish = null;
            DateTime? finishTime = null;

            if (callToAssign.MaxEndTime != null && callToAssign.MaxEndTime < s_dal!.Config!.Clock)
            {
                finish = AssignmentCompletionType.Open;

            }
            else
            {
                int randFinish = s_rand.Next(0, 2);
                switch (randFinish)
                {
                    case 0:
                        finish = AssignmentCompletionType.TreatedOnTime;
                        finishTime = s_dal!.Config!.Clock.AddSeconds(s_rand.Next(300, 3000));

                        break;
                    case 1:
                        finish = AssignmentCompletionType.VolunteerCancelled;
                        finishTime = s_dal!.Config!.Clock.AddSeconds(s_rand.Next(10, 300));

                        break;
                    case 2:
                        finish = AssignmentCompletionType.AdminCancelled;
                        finishTime = s_dal!.Config!.Clock.AddSeconds(s_rand.Next(10, 300));
                        break;
                    //case 3:
                    //    finish = AssignmentCompletionType.Open;
                    //    finishTime = null;
                    //    break;



                }
            }
            s_dal.Assignment?.Create(new Assignment(
            0,
            callToAssign.Id,
            volunteerToAssign.Id,
            s_dal!.Config!.Clock,
            finishTime,
            finish));
        }
    }
    //public static void Do(IDal dal) //stage 2
    public static void Do() //stage 4
    {
        // stage1
        //s_dalVolunteer = dalVolunteer ?? throw new NullReferenceException("DAL object can not be null!");
        //s_dalCall = dalCall ?? throw new NullReferenceException("DAL object can not be null!");
        //s_dalAssignment = dalAssignment ?? throw new NullReferenceException("DAL object can not be null!");
        //s_dalConfig = dalConfig ?? throw new NullReferenceException("DAL object can not be null!");
        //s_dal = dal ?? throw new NullReferenceException("DAL object can not be null!"); // stage 2
        s_dal = DalApi.Factory.Get; //stage 4

        Console.WriteLine("Reset Configuration values and List values...");
        // stage1
        //Console.WriteLine("Reset Configuration values and List values");
        //s_dalConfig.Reset();
        //Console.WriteLine("Reset Volunteer values and List values");
        //s_dalVolunteer.DeleteAll();
        //Console.WriteLine("Reset Call values and List values");
        //s_dalCall.DeleteAll();
        //Console.WriteLine("Reset Assignment values and List values");
        //s_dalAssignment.DeleteAll();

        s_dal.ResetDB();//stage 2


        Console.WriteLine("Initializing Volunteers list ...");
        CreateVolunteers();
        Console.WriteLine("Initializing Calls list ...");
        CreateCalls();
        Console.WriteLine("Initializing Assignments list ...");
        CreateAssignment();




    }



}