namespace DalTest;
using DalApi;
using DO;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Data;
using System.Net;

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

    static string[] Addresses =  // <param name="Addresses">List of possible addresses.</param>
    {
         "7 Presidents St, Petah Tikva, Israel","Lev Academic Center, Jerusalem, Israel",
    "45 Rothschild Blvd, Tel Aviv, Israel", "12 Herzl St, Haifa, Israel",
    "20 King David St, Jerusalem, Israel","3 HaNasi Blvd, Be'er Sheva, Israel",
    "14 Jabotinsky St, Rishon LeZion, Israel","10 Arlozorov St, Tel Aviv, Israel",
    "22 Hillel St, Jerusalem, Israel","8 Dizengoff St, Tel Aviv, Israel",
    "5 Ben Yehuda St, Haifa, Israel", "16 Weizmann St, Rehovot, Israel",
    "9 Begin Ave, Ashdod, Israel", "11 Allenby St, Tel Aviv, Israel",
    "4 Hanegev St, Eilat, Israel"
    };

    // Israeli Addresses
    static string[] israeliAddresses = new string[]
    {
    "1 Rothschild Blvd, Tel Aviv",
    "2 Jabotinsky St, Ramat Gan",
    "3 Herzl St, Haifa",
    "4 Ben Yehuda St, Jerusalem",
    "5 Weizmann St, Rehovot",
    "6 Dizengoff St, Tel Aviv",
    "7 Haneviim St, Jerusalem",
    "8 Hahistadrut Blvd, Haifa",
    "9 Begin Blvd, Beersheba",
    "10 Hahagana St, Ashdod",
    "11 Hahistadrut St, Netanya",
    "12 Hahistadrut St, Holon",
    "13 Hahistadrut St, Bat Yam",
    "14 Hahistadrut St, Rishon LeZion",
    "15 Hahistadrut St, Petah Tikva",
    "16 Hahistadrut St, Bnei Brak",
    "17 Hahistadrut St, Ashkelon",
    "18 Hahistadrut St, Herzliya",
    "19 Hahistadrut St, Kfar Saba",
    "20 Hahistadrut St, Hadera",
    "21 Hahistadrut St, Modiin",
    "22 Hahistadrut St, Nazareth",
    "23 Hahistadrut St, Ramat Gan",
    "24 Hahistadrut St, Raanana",
    "25 Hahistadrut St, Givatayim",
    "26 Hahistadrut St, Acre (Akko)",
    "27 Hahistadrut St, Eilat",
    "28 Hahistadrut St, Kiryat Gat",
    "29 Hahistadrut St, Kiryat Motzkin",
    "30 Hahistadrut St, Kiryat Yam",
    "31 Hahistadrut St, Kiryat Bialik",
    "32 Hahistadrut St, Kiryat Ata",
    "33 Hahistadrut St, Nahariya",
    "34 Hahistadrut St, Tiberias",
    "35 Hahistadrut St, Rosh HaAyin",
    "36 Hahistadrut St, Yavne",
    "37 Hahistadrut St, Or Yehuda",
    "38 Hahistadrut St, Lod",
    "39 Hahistadrut St, Ramla",
    "40 Hahistadrut St, Kfar Yona",
    "41 Hahistadrut St, Sderot",
    "42 Hahistadrut St, Dimona",
    "43 Hahistadrut St, Arad",
    "44 Hahistadrut St, Ma'alot-Tarshiha",
    "45 Hahistadrut St, Migdal HaEmek",
    "46 Hahistadrut St, Karmiel",
    "47 Hahistadrut St, Sakhnin",
    "48 Hahistadrut St, Tamra",
    "49 Hahistadrut St, Umm al-Fahm",
    "50 Hahistadrut St, Qalansawe"
    };

    // International Addresses
    static string[] internationalAddresses = new string[]
    {
    // ארצות הברית
    "123 5th Avenue, New York, NY, USA",
    "456 Michigan Avenue, Chicago, IL, USA",
    "789 Market Street, San Francisco, CA, USA",
    "321 Boylston Street, Boston, MA, USA",
    "654 Broadway, Nashville, TN, USA",
    
    // בריטניה
    "10 Downing Street, London, UK",
    "45 Oxford Street, London, UK",
    "78 Prince Street, Edinburgh, Scotland, UK",
    "23 Castle Street, Cardiff, Wales, UK",
    "56 Victoria Square, Birmingham, UK",
    
    // צרפת
    "12 Rue de Rivoli, Paris, France",
    "34 Avenue des Champs-Élysées, Paris, France",
    "67 Rue Paradis, Marseille, France",
    "89 Rue de la République, Lyon, France",
    "23 Place Bellecour, Lyon, France",
    
    // גרמניה
    "45 Kurfürstendamm, Berlin, Germany",
    "78 Maximilianstrasse, Munich, Germany",
    "90 Zeil, Frankfurt, Germany",
    "12 Neuer Wall, Hamburg, Germany",
    "34 Königsallee, Düsseldorf, Germany",
    
    // איטליה
    "56 Via del Corso, Rome, Italy",
    "78 Via Monte Napoleone, Milan, Italy",
    "90 Via Roma, Florence, Italy",
    "23 Via Toledo, Naples, Italy",
    "45 Via Garibaldi, Venice, Italy",
    
    // ספרד
    "67 Gran Via, Madrid, Spain",
    "89 Las Ramblas, Barcelona, Spain",
    "12 Calle Sierpes, Seville, Spain",
    "34 Plaza Mayor, Valencia, Spain",
    "56 Plaza del Pilar, Zaragoza, Spain",
    
    // קנדה
    "78 Yonge Street, Toronto, ON, Canada",
    "90 Rue Sainte-Catherine, Montreal, QC, Canada",
    "23 Robson Street, Vancouver, BC, Canada",
    "45 Stephen Avenue, Calgary, AB, Canada",
    "67 Spring Garden Road, Halifax, NS, Canada",
    
    // אוסטרליה
    "89 George Street, Sydney, NSW, Australia",
    "12 Collins Street, Melbourne, VIC, Australia",
    "34 Queen Street, Brisbane, QLD, Australia",
    "56 Rundle Mall, Adelaide, SA, Australia",
    "78 Murray Street, Perth, WA, Australia",
    
    // יפן
    "90 Ginza Street, Chuo City, Tokyo, Japan",
    "23 Shijo Dori, Kyoto, Japan",
    "45 Midosuji, Osaka, Japan",
    "67 Tenjin, Fukuoka, Japan",
    "89 Motomachi, Yokohama, Japan",
    
    // נורבגיה
    "12 Karl Johans Gate, Oslo, Norway",
    "34 Torgallmenningen, Bergen, Norway",
    "56 Olav Tryggvasons Gate, Trondheim, Norway",
    "78 Strandgata, Tromsø, Norway",
    "90 Kirkegata, Stavanger, Norway"
    };
    static string[] internationalAddresses2 = new string[]
  {
    // ארצות הברית
    "1200 Pennsylvania Avenue, Washington, DC, USA",
    "1501 15th Street, Denver, CO, USA",
    "800 N Michigan Avenue, Chicago, IL, USA",
    "2000 Market Street, Philadelphia, PA, USA",
    "1025 Connecticut Avenue, Washington, DC, USA",

    // בריטניה
    "1 Piccadilly Circus, London, UK",
    "56 Regent Street, London, UK",
    "88 Southwark Street, London, UK",
    "10 Princes Street, Edinburgh, Scotland, UK",
    "22 The Parade, Cardiff, Wales, UK",

    // צרפת
    "5 Rue de la Paix, Paris, France",
    "22 Rue Saint-Antoine, Paris, France",
    "33 Boulevard Saint-Germain, Paris, France",
    "54 Rue de la Liberté, Marseille, France",
    "91 Rue de la République, Lyon, France",

    // גרמניה
    "24 Unter den Linden, Berlin, Germany",
    "34 Maximilianstrasse, Munich, Germany",
    "55 Friedrichstrasse, Berlin, Germany",
    "77 Königstrasse, Stuttgart, Germany",
    "102 Mönckebergstraße, Hamburg, Germany",

    // איטליה
    "101 Via del Babuino, Rome, Italy",
    "88 Via Spiga, Milan, Italy",
    "44 Via San Gregorio, Milan, Italy",
    "12 Piazza del Duomo, Florence, Italy",
    "56 Via San Vitale, Bologna, Italy",

    // ספרד
    "34 Paseo de la Castellana, Madrid, Spain",
    "12 Gran Via de les Corts Catalanes, Barcelona, Spain",
    "8 Calle de Preciados, Madrid, Spain",
    "45 Calle de Fuencarral, Madrid, Spain",
    "67 Avenida Diagonal, Barcelona, Spain",

    // קנדה
    "120 Queen Street West, Toronto, ON, Canada",
    "30 King Street West, Toronto, ON, Canada",
    "88 Avenue du Parc, Montreal, QC, Canada",
    "60 Granville Street, Vancouver, BC, Canada",
    "70 Bay Street, Toronto, ON, Canada",

    // אוסטרליה
    "101 Pitt Street, Sydney, NSW, Australia",
    "200 George Street, Sydney, NSW, Australia",
    "100 Swanston Street, Melbourne, VIC, Australia",
    "10 Collins Street, Melbourne, VIC, Australia",
    "15 King Street, Brisbane, QLD, Australia",

    // יפן
    "45 Roppongi, Minato City, Tokyo, Japan",
    "12 Omotesando, Shibuya, Tokyo, Japan",
    "78 Shinjuku, Tokyo, Japan",
    "56 Namba, Osaka, Japan",
    "34 Sapporo, Hokkaido, Japan",

    // נורבגיה
    "18 Dronningens Gate, Oslo, Norway",
    "12 Markens Gate, Kristiansand, Norway",
    "22 Torggata, Oslo, Norway",
    "33 Søndre Gate, Trondheim, Norway",
    "67 Lillehammer, Norway"
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
            string? address = /*Addresses*/internationalAddresses[i];
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

    //public static void CreateCalls()
    //{
    //    int index1 = s_rand.Next(0, 15);
    //    int index2 = s_rand.Next(0, 8);

    //    // Latitude and longitude range in Israel
    //    double minLatitude = 29.5;
    //    double maxLatitude = 33.5;
    //    double minLongitude = 34.3;
    //    double maxLongitude = 35.9;

    //    for (int i = 0; i < 50; i++)
    //    {

    //        string? address = internationalAddresses2[index1];
    //        Calltype calltype = (Calltype)index2;


    //        calltype = CalltypeArray[index2];

    //        string? VerbalDescription = Descriptions[index2];
    //        bool active = true;

    //        double randomLatitude = s_rand.NextDouble() * (maxLatitude - minLatitude) + minLatitude;
    //        double randomLongitude = s_rand.NextDouble() * (maxLongitude - minLongitude) + minLongitude;

    //        // run num
    //        ///DateTime currentTime = s_dal!.Config.Clock;//stage2  
    //        //DateTime currentTime = s_dalConfig.Clock;//stage1
    //        DateTime currentTime = new DateTime(s_dal!.Config.Clock.Year, s_dal!.Config.Clock.Month, s_dal!.Config.Clock.Hour);//stage2  

    //        DateTime openTime = s_dal!.Config.Clock.AddDays(-1);

    //        // Calculate the number of minutes since the start time until now
    //        int totalMinutesInLastDay = (int)(s_dal!.Config.Clock - openTime).TotalMinutes;
    //        // Random opening time within the last 24 hours
    //        DateTime RndomStart = openTime.AddMinutes(s_rand.Next(0, totalMinutesInLastDay));
    //        DateTime? maxEndTime = null;



    //        //s_dalCall.Create(new Call(randomLatitude, randomLongitude, calltype, tempID, VerbalDescription, address, openTime, maxEndTime)); //stage1
    //        s_dal!.Call.Create(new Call(randomLatitude, randomLongitude, calltype, default, VerbalDescription, address, openTime, maxEndTime)); //stage
    //    }
    //}


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
            int index1 = s_rand.Next(0, internationalAddresses2.Length);
            int index2 = s_rand.Next(0, CalltypeArray.Length);

            string? address = internationalAddresses2[index1];
            Calltype calltype = CalltypeArray[index2];
            string? VerbalDescription = Descriptions[index2];

            double randomLatitude = s_rand.NextDouble() * (maxLatitude - minLatitude) + minLatitude;
            double randomLongitude = s_rand.NextDouble() * (maxLongitude - minLongitude) + minLongitude;

            // יצירת זמן פתיחה רנדומלי ב-48 השעות האחרונות
            DateTime openTime = s_dal!.Config.Clock.AddMinutes(-s_rand.Next(0, 2880));

            // יצירת זמן סיום רנדומלי בהתאם לטווח זמנים שונים ליצירת כל הסטטוסים
            DateTime? maxEndTime;

            int randomStatus = s_rand.Next(0, 6); // בחירה רנדומלית של סטטוס

            switch (randomStatus)
            {
                case 0: // Open
                    maxEndTime = null;
                    break;

                case 1: // OpenAtRisk
                    maxEndTime = null;
                    openTime = s_dal.Config.Clock.AddMinutes(-s_rand.Next(0, 30)); // זמן פתיחה קרוב מאוד לזמן הנוכחי
                    break;

                case 2: // InProgress
                    maxEndTime = null;
                    openTime = s_dal.Config.Clock.AddMinutes(-s_rand.Next(30, 1440)); // פתיחה בטווח של עד 24 שעות
                    break;

                case 3: // InProgressAtRisk
                    maxEndTime = null;
                    openTime = s_dal.Config.Clock.AddMinutes(-s_rand.Next(1440, 2880)); // פתיחה רחוקה מאוד
                    break;

                case 4: // Closed
                    maxEndTime = openTime.AddMinutes(s_rand.Next(10, 60)); // זמן סיום תקין אחרי זמן הפתיחה
                    break;

                case 5: // Expired
                    maxEndTime = openTime.AddMinutes(-s_rand.Next(10, 60)); // זמן סיום שפג לפני זמן ה-Clock
                    break;

                default:
                    maxEndTime = null;
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
                finish = AssignmentCompletionType.Expired;

            }
            else
            {
                int randFinish = s_rand.Next(0, 3); 
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
                    case 3:
                        finish = AssignmentCompletionType.Open;
                        finishTime = null;
                        break;
                   
                     

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