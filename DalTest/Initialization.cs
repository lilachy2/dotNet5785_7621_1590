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
    static string[] Addresses =
{
    "7 Presidents St, Petah Tikva, Israel", "Lev Academic Center, Jerusalem, Israel", "45 Rothschild Blvd, Tel Aviv, Israel",
    "12 Herzl St, Haifa, Israel", "20 King David St, Jerusalem, Israel", "3 HaNasi Blvd, Be'er Sheva, Israel",
    "14 Jabotinsky St, Rishon LeZion, Israel", "10 Arlozorov St, Tel Aviv, Israel",
    "22 Hillel St, Jerusalem, Israel", "8 Dizengoff St, Tel Aviv, Israel", "5 Ben Yehuda St, Haifa, Israel",
    "16 Weizmann St, Rehovot, Israel", "9 Begin Ave, Ashdod, Israel",  "11 Allenby St, Tel Aviv, Israel",
    "4 Hanegev St, Eilat, Israel"
};
    static readonly string[] Descriptions =
  {
        "A situation where someone has fainted and needs assistance.",            // fainting
        "A birth event requiring medical personnel to assist with delivery.",      // birth
        "A resuscitation call where CPR or other life-saving measures are needed.", // resuscitation
        "An allergy attack, possibly involving severe reactions such as anaphylaxis.", // allergy
        "A heart attack event requiring urgent medical intervention.",             // heartattack
        "A call for a broken bone injury that needs stabilization and treatment.", // broken_bone
        "A security event involving a threat or emergency requiring a quick response." // security_event
   };


    public static void CreateVolunteers()
    {
        bool isFirst = true;
        int id;
        int i = new int(); // for the arr
        i = 0;

        foreach (var name in Names)
        {
        
            do
            {
                id = s_rand.Next(MIN_ID, MAX_ID);
            } while (s_dalVolunteer!.Read(id) != null);



            string phone = "05" + s_rand.Next(0, 8).ToString() + s_rand.Next(1000000, 9999999).ToString();
            int p1 = int.Parse(phone); // כדי לבצע המרה בשביל הבנאי
            string email = name.Replace(" ", ".").ToLower() + "@volunteer.org";
            //string? password = null; // סיסמא ראשונית או null עד שהמתנדב יעדכן
            string? address = Addresses[i];
            Role role;
            if (isFirst)
            {
                role = Role.Manager;//one Manager
                isFirst = false;
            }
            role = Role.Volunteer;
            distance_type distanceType = distance_type.Aerial_distance;

            double? maxDistance = s_rand.Next(5, 20); // מרחק ברירת מחדל בקילומטר

            Random rand = new Random(); // לקווי אורך ורוחב
            bool active = true;

            // טווחי קו רוחב ואורך בישראל
            double minLatitude = 29.5;
            double maxLatitude = 33.5;
            double minLongitude = 34.3;
            double maxLongitude = 35.9;
            double randomLatitude = s_rand.NextDouble() * (maxLatitude - minLatitude) + minLatitude;
            double randomLongitude = s_rand.NextDouble() * (maxLongitude - minLongitude) + minLongitude;

            s_dalVolunteer.Create(new Volunteer(id, name, p1, email, role, distanceType, address, randomLatitude, randomLongitude, active, maxDistance));
            i = i + 1;

        }


    }

    public static void CreateCalls()
    {
        int index1 = s_rand.Next(0, 15);
        int index2 = s_rand.Next(0, 6);
        int tempID = new int(); // for the arr

        for (int i = 0; i < 50; i++)
        {
         

            do
            {
                tempID = s_dalConfig!.NextCallId;
            } while (s_dalCall!.Read(tempID) != null);


            string? address = Addresses[index1];
            Calltype calltype = (Calltype)index2;

            string? VerbalDescription = Descriptions[index2];
            bool active = true;

            // טווחי קו רוחב ואורך בישראל
            double minLatitude = 29.5;
            double maxLatitude = 33.5;
            double minLongitude = 34.3;
            double maxLongitude = 35.9;
            double randomLatitude = s_rand.NextDouble() * (maxLatitude - minLatitude) + minLatitude;
            double randomLongitude = s_rand.NextDouble() * (maxLongitude - minLongitude) + minLongitude;

            DateTime currentTime = s_dalConfig.Clock; // The system clock representing the current time
            TimeSpan maxTimeSpanBackwards = TimeSpan.FromDays(30); // Maximum range for setting call open time, e.g., up to 30 days ago

            // Generate a random opening time that is before the current time, within a maximum of 30 days in the past
            TimeSpan randomOffset = new TimeSpan(
                s_rand.Next(0, (int)maxTimeSpanBackwards.TotalDays),    // Random days backwards
                s_rand.Next(0, 24),                                     // Random hours
                s_rand.Next(0, 60),                                     // Random minutes
                s_rand.Next(0, 60)                                      // Random seconds
            );

            DateTime openTime = currentTime - randomOffset; // Calculate a random open time

            // Define a random max end time or leave it null if there's no end time
            DateTime? maxEndTime = null;
            bool hasEndTime = s_rand.Next(0, 2) == 1; // Randomly decide if there's an end time, 50% chance

            if (hasEndTime)
            {
                TimeSpan riskSpan = s_dalConfig.RiskRange; // The base risk range to add
                int extraHours = s_rand.Next(1, 24);       // Random number of hours
                int extraMinutes = s_rand.Next(1, 60);     // Random number of minutes

                // Calculate max end time based on open time and adding random time
                maxEndTime = openTime.Add(riskSpan).AddHours(extraHours).AddMinutes(extraMinutes);
            }

            s_dalCall.Create(new Call(randomLatitude, randomLongitude, calltype, tempID, VerbalDescription, address, openTime, maxEndTime));
        }




    }

    public static void CreateAssignment()
    {
        List<Call> callist = s_dalCall!.ReadAll();
        List<Volunteer?> volunteerlist = s_dalVolunteer!.ReadAll();
        for (int i = 0; i < 50; i++)
        {
            int index1 = s_rand.Next(0, 15);
            int index2 = s_rand.Next(0, 6);
            int tempID = new int(); // Assignment
            Call tempCall = callist[i]; //1 call from the list 
            Volunteer? tempVolunteer = volunteerlist[index1]; // volunteer

            do
            {
                tempID = s_dalConfig!.NextAssignmentId;
            } while (s_dalAssignment!.Read(tempID) != null);






            DateTime openTime = tempCall.OpeningTime;         // Retrieve open time from the call
            DateTime? maxEndTime = tempCall.MaxEndTime;    // Retrieve max end time from the call

            // Generate a random entry time between openTime and maxEndTime
            DateTime entryTime;
            if (maxEndTime.HasValue)
            {
                entryTime = openTime.AddMinutes(s_rand.Next(1, (int)(maxEndTime.Value - openTime).TotalMinutes));
            }
            else
            {
                // If no max end time is defined, use the current time as an upper bound
                entryTime = openTime.AddMinutes(s_rand.Next(1, (int)(DateTime.Now - openTime).TotalMinutes));
            }

            // Determine end time and status
            DateTime? endTime = null;
            AssignmentCompletionType? endOfTreatment = null;

            bool isExpired = maxEndTime.HasValue && entryTime > maxEndTime.Value;

            //for the times will make sence
            if (!isExpired)
            {
                // Randomly decide if treatment ends on time, is cancelled, or remains active
                int endTypeDecision = s_rand.Next(0, 4); // 0: Treated on Time, 1: VolunteerCancelled, 2: AdminCancelled, 3: Active

                switch (endTypeDecision)
                {
                    case 0:
                        endOfTreatment = AssignmentCompletionType.TreatedOnTime;
                        endTime = entryTime.AddMinutes(s_rand.Next(1, 60)); // Random end time within an hour
                        break;
                    case 1:
                        endOfTreatment = AssignmentCompletionType.VolunteerCancelled;
                        endTime = entryTime.AddMinutes(s_rand.Next(1, 30)); // Random end time within 30 minutes
                        break;
                    case 2:
                        endOfTreatment = AssignmentCompletionType.AdminCancelled;
                        endTime = entryTime.AddMinutes(s_rand.Next(1, 30)); // Random end time within 30 minutes
                        break;
                    case 3:
                        // Active call - no end time, no completion type
                        break;
                }
            }
            else
            {
                // If the call has expired, set status to Expired and leave endTime as null
                endOfTreatment = AssignmentCompletionType.Expired;
            }


            s_dalAssignment.Create(new Assignment(entryTime, tempID, tempCall.Id, tempVolunteer?.id ?? 0, endTime, endOfTreatment));
        }
    }

    public static void Do(IVolunteer? dalVolunteer, ICall? dalCall, IAssignment? dalAssignment, IConfig? dalConfig)
    {
        s_dalVolunteer = dalVolunteer ?? throw new NullReferenceException("DAL object can not be null!"); 
        s_dalCall = dalCall ?? throw new NullReferenceException("DAL object can not be null!"); 
        s_dalAssignment = dalAssignment ?? throw new NullReferenceException("DAL object can not be null!"); 
        s_dalConfig = dalConfig ?? throw new NullReferenceException("DAL object can not be null!");

        Console.WriteLine("Reset Configuration values and List values");
        s_dalConfig.Reset();
        Console.WriteLine("Reset Volunteer values and List values");
        s_dalVolunteer.DeleteAll();
        Console.WriteLine("Reset Call values and List values");
        s_dalCall.DeleteAll();
        Console.WriteLine("Reset Assignment values and List values");
        s_dalAssignment.DeleteAll();

        Console.WriteLine("Initializing Volunteers list ...");
        CreateVolunteers(); ///////////////////////////////////////////////////////////////
        Console.WriteLine("Initializing Calls list ...");
        CreateCalls();
        Console.WriteLine("Initializing Assignments list ...");
        CreateAssignment();


    }



}






