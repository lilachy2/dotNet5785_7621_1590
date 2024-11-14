using Dal;
using DalApi;
using DO;
using System.Security.Cryptography.X509Certificates;

namespace DalTest
{
    public class Program
    {
        private static IVolunteer? s_dalVolunteer;
        private static ICall? s_dalCall;
        private static IAssignment? s_dalAssignment;
        private static IConfig? s_dalConfig;

        public enum MainMenuOption
        {
            Exit, VolunteerMenu, CallMenu, AssignmentMenu, ConfigMenu, InitializeData, ShowAllData, ResetDatabase
        }

        public enum VolunteerMenuOption
        {
            Exit,
            Create,
            Read,
            ReadAll,
            Update,
            Delete,
            DeleteAll
        }

        public enum CallMenuOption
        {
            Exit,
            Create,
            Read,
            ReadAll,
            Update,
            Delete,
            DeleteAll
        }

        public enum AssignmentMenuOption
        {
            Exit,
            Create,
            Read,
            ReadAll,
            Update,
            Delete,
            DeleteAll
        }

        public enum ConfigMenuOption
        {
            Exit,
            AdvanceSystemClock,
            ShowSystemClock,
            SetConfigVariable,
            ShowConfigVariable,
            ResetConfig
        }

        static void Main()
        {
            try
            {
                s_dalVolunteer = new VolunteerImplementation();
                s_dalCall = new CallImplementation();
                s_dalAssignment = new AssignmentImplementation();
                s_dalConfig = new ConfigImplementation();

                bool exit = false;
                while (!exit)
                {
                    Console.WriteLine("\nMain Menu:");
                    Console.WriteLine("1. Volunteer Menu");
                    Console.WriteLine("2. Call Menu");
                    Console.WriteLine("3. Assignment Menu");
                    Console.WriteLine("4. Config Menu");
                    Console.WriteLine("5. Initialize Data");
                    Console.WriteLine("6. Show All Data");
                    Console.WriteLine("7. Reset Database");
                    Console.WriteLine("0. Exit");

                    MainMenuOption choice = (MainMenuOption)int.Parse(Console.ReadLine() ?? "0");

                    switch (choice)
                    {
                        case MainMenuOption.VolunteerMenu:
                            VolunteerMenu();
                            break;
                        case MainMenuOption.CallMenu:
                            CallMenu();
                            break;
                        case MainMenuOption.AssignmentMenu:
                            AssignmentMenu();
                            break;
                        case MainMenuOption.ConfigMenu:
                            ConfigMenu();
                            break;
                        case MainMenuOption.InitializeData:
                            // קריאה לפונקציית אתחול
                            Initialization.Do(s_dalVolunteer, s_dalCall, s_dalAssignment , s_dalConfig);
                            break;
                        case MainMenuOption.ShowAllData:
                            // הצגת כל הנתונים
                          print(s_dalVolunteer, s_dalCall, s_dalAssignment, s_dalConfig);
                            break;
                        case MainMenuOption.ResetDatabase:
                            s_dalVolunteer.ReadAll();
                            s_dalCall.ReadAll();
                            s_dalAssignment.ReadAll();
                            s_dalConfig.Reset();

                            // איפוס בסיס הנתונים והתצורה
                            break;
                        case MainMenuOption.Exit:
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Invalid choice, try again.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during initialization: {ex.Message}");
            }
        }

        private static void VolunteerMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\nVolunteer Menu:");
                Console.WriteLine("1. Add Volunteer");
                Console.WriteLine("2. Display Volunteer by ID");
                Console.WriteLine("3. Display All Volunteers");
                Console.WriteLine("4. Update Volunteer");
                Console.WriteLine("5. Delete Volunteer");
                Console.WriteLine("6. Delete All Volunteers");
                Console.WriteLine("0. Exit");

                VolunteerMenuOption choice = (VolunteerMenuOption)int.Parse(Console.ReadLine() ?? "0");

                switch (choice)
                {
                    case VolunteerMenuOption.Create:
                        // מתודה להוספת מתנדב
                        // קליטת נתונים מהמשתמש

                        Volunteer volunteer = helpV();
                        s_dalVolunteer!.Create(volunteer);

                        break;
                    case VolunteerMenuOption.Read:
                        Console.WriteLine("enter id you want read" );
                        int tempid = int.Parse(Console.ReadLine() ?? "0");
                        s_dalVolunteer!.Read(tempid);

                        // מתודה לתצוגת מתנדב לפי מזהה
                        break;
                    case VolunteerMenuOption.ReadAll:
          
                        s_dalVolunteer!.ReadAll();

                        // מתודה להצגת כל המתנדבים
                        break;
                    case VolunteerMenuOption.Update:
                        Volunteer volunteer1 = helpV();
                        s_dalVolunteer!.Update(volunteer1);

                        // מתודה לעדכון מתנדב קיים
                        break;
                    case VolunteerMenuOption.Delete:
                        Console.WriteLine("enter id you want read");
                        int tempid2 = int.Parse(Console.ReadLine() ?? "0");
                        s_dalVolunteer!.Delete(tempid2);
                        // מתודה למחיקת מתנדב לפי מזהה
                        break;
                    case VolunteerMenuOption.DeleteAll:
                        s_dalVolunteer!.DeleteAll();
                        // מתודה למחיקת כל המתנדבים
                        break;
                    case VolunteerMenuOption.Exit:
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice, try again.");
                        break;
                }
            }
        }

        public static Volunteer helpV()
        {
            Console.WriteLine("Enter Volunteer ID:");
            int id = int.Parse(Console.ReadLine() ?? "0");

            Console.WriteLine("Enter Volunteer Name:");
            string name = Console.ReadLine() ?? "";

            Console.WriteLine("Enter Volunteer Phone Number:");
            int numberPhone = int.Parse(Console.ReadLine() ?? "0");

            Console.WriteLine("Enter Volunteer Email:");
            string email = Console.ReadLine() ?? "";

            Console.WriteLine("Enter Role (0 = Volunteer, 1 = Manager):");
            Role role = (Role)int.Parse(Console.ReadLine() ?? "0");

            Console.WriteLine("Enter Distance Type (0 = Aerial_distance, 1 = Driving_distance):");
            distance_type distanceType = (distance_type)int.Parse(Console.ReadLine() ?? "0");

            Console.WriteLine("Enter Full Current Address (or press Enter to skip):");
            string? fullCurrentAddress = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(fullCurrentAddress))
                fullCurrentAddress = null;

            Console.WriteLine("Enter Latitude (or press Enter to skip):");
            string latitudeInput = Console.ReadLine() ?? "";
            double? latitude = string.IsNullOrWhiteSpace(latitudeInput) ? null : double.Parse(latitudeInput);

            Console.WriteLine("Enter Longitude (or press Enter to skip):");
            string longitudeInput = Console.ReadLine() ?? "";
            double? longitude = string.IsNullOrWhiteSpace(longitudeInput) ? null : double.Parse(longitudeInput);

            Console.WriteLine("Is Active? (true/false):");
            bool active = bool.Parse(Console.ReadLine() ?? "true");

            Console.WriteLine("Enter Distance (or press Enter to skip):");
            string distanceInput = Console.ReadLine() ?? "";
            double? distance = string.IsNullOrWhiteSpace(distanceInput) ? null : double.Parse(distanceInput);
            return  new Volunteer(id, name, numberPhone, email, role, distanceType, fullCurrentAddress, latitude, longitude, active, distance);


        }
        public static void print(IVolunteer? s_dalVolunteer, ICall? s_dalCall, IAssignment? s_dalAssignment, IConfig? s_dalConfig)
        {
          
                Console.WriteLine("Volunteer Data:");
                if (s_dalVolunteer != null)
                {
                    foreach (var volunteer in s_dalVolunteer.ReadAll())
                    {
                        Console.WriteLine(volunteer); // Assuming ToString() is overridden for Volunteer
                    }
                }
                else
                {
                    Console.WriteLine("No Volunteer data available.");
                }

                Console.WriteLine("\nCall Data:");
                if (s_dalCall != null)
                {
                    foreach (var call in s_dalCall.ReadAll())
                    {
                        Console.WriteLine(call); // Assuming ToString() is overridden for Call
                    }
                }
                else
                {
                    Console.WriteLine("No Call data available.");
                }

                Console.WriteLine("\nAssignment Data:");
                if (s_dalAssignment != null)
                {
                    foreach (var assignment in s_dalAssignment.ReadAll())
                    {
                        Console.WriteLine(assignment); // Assuming ToString() is overridden for Assignment
                    }
                }
                else
                {
                    Console.WriteLine("No Assignment data available.");
                }

                Console.WriteLine("\nConfig Data:");
                if (s_dalConfig != null)
                {
                    Console.WriteLine($"System Clock: {s_dalConfig.Clock}");
                    // הצגת ערכי תצורה נוספים, אם ישנם
                }
                else
                {
                    Console.WriteLine("No Config data available.");
                }
            



        }



        private static void CallMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\nCall Menu:");
                Console.WriteLine("1. Create Calls (Auto-generate 50 calls)");
                Console.WriteLine("2. Read Call by ID");
                Console.WriteLine("3. Read All Calls");
                Console.WriteLine("4. Update Call");
                Console.WriteLine("5. Delete Call");
                Console.WriteLine("6. Delete All Calls");
                Console.WriteLine("0. Exit");

                CallMenuOption choice = (CallMenuOption)int.Parse(Console.ReadLine() ?? "0");

                switch (choice)
                {
                    case CallMenuOption.Create:
                        Call call1 = helpC();
                        s_dalCall!.Create(call1);
                        Console.WriteLine("50 calls have been created.");
                        break;

                    case CallMenuOption.Read:
                        Console.Write("Enter Call ID to display: ");
                        int id = int.Parse(Console.ReadLine() ?? "0");
                        var call = s_dalCall?.Read(id);

                        break;

                    case CallMenuOption.ReadAll:
                        s_dalCall?.ReadAll();

                        break;

                    case CallMenuOption.Update:
                        {
                            Call call2 = helpC();
                            s_dalCall!.Update(call2);

                        }
                        break;

                    case CallMenuOption.Delete:
                        Console.Write("Enter Call ID to delete: ");
                        int deleteId = int.Parse(Console.ReadLine() ?? "0");
                        s_dalCall?.Delete(deleteId);
                        break;

                    case CallMenuOption.DeleteAll:
                        s_dalCall?.DeleteAll();
                        break;

                    case CallMenuOption.Exit:
                        exit = true;
                        break;

                    default:
                        Console.WriteLine("Invalid choice, try again.");
                        break;
                }
            }

        }
        public static Call helpC()
        {
            Console.Write("Enter Latitude: ");
            double latitude = double.Parse(Console.ReadLine() ?? "0");

            Console.Write("Enter Longitude: ");
            double longitude = double.Parse(Console.ReadLine() ?? "0");

            Console.Write("Enter Call Type (0 - Type1, 1 - Type2, etc.): ");
            int callTypeInput = int.Parse(Console.ReadLine() ?? "0");
            Calltype callType = (Calltype)callTypeInput;

            Console.Write("Enter ID: ");
            int id = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Enter Verbal Description (optional): ");
            string? verbalDescription = Console.ReadLine();

            Console.Write("Enter Read Address: ");
            string readAddress = Console.ReadLine() ?? "";

            Console.Write("Enter Opening Time (yyyy-MM-dd HH:mm): ");
            DateTime openingTime = DateTime.Parse(Console.ReadLine() ?? DateTime.Now.ToString());

            Console.Write("Enter Max End Time (yyyy-MM-dd HH:mm) or leave empty if not applicable: ");
            string? maxEndTimeInput = Console.ReadLine();
            DateTime? maxEndTime = string.IsNullOrWhiteSpace(maxEndTimeInput) ? null : DateTime.Parse(maxEndTimeInput);

            // יצירת אובייקט Call לפי הבנאי
            return new Call(
                Latitude: latitude,
                Longitude: longitude,
                Calltype: callType,
                Id: id,
                VerbalDescription: verbalDescription,
                ReadAddress: readAddress,
                OpeningTime: openingTime,
                MaxEndTime: maxEndTime
            );


        }


        private static void AssignmentMenu()
        {

            bool exit = false;

            // Loop to keep displaying the menu until the user chooses to exit
            while (!exit)
            {
                Console.WriteLine("\nChoose the method you'd like to perform:");
                Console.WriteLine("0. Exit submenu");
                Console.WriteLine("1. Add a new object of this type to the list (Create)");
                Console.WriteLine("2. Read an object by ID (Read)");
                Console.WriteLine("3. Read the list of all objects of this type (ReadAll)");
                Console.WriteLine("4. Update an existing object's data (Update)");
                Console.WriteLine("5. Delete an existing object from the list (Delete)");
                Console.WriteLine("6. Delete all objects in the list (DeleteAll)");

                Console.Write("Choose an option: ");
                string? choice = Console.ReadLine();

                try
                {
                    // Switch statement to handle each menu option
                    switch (choice)
                    {
                        case "0":
                            // Exit the submenu
                            exit = true;
                            break;

                        case "1":
                            Console.WriteLine("Adding a new object");
                            // Code to create a new object and add it to the list
                            Assignment Assi = helpA();
                            s_dalAssignment!.Create(Assi);

                            break;

                        case "2":
                            Console.WriteLine("Enter the ID of the object to Read:");
                            int idToRead = int.Parse(Console.ReadLine() ?? "0");
                            s_dalAssignment!.Read(idToRead);    
                            // Code to read and display the object by ID

                            break;

                        case "3":
                            s_dalAssignment!.ReadAll();
                            // Code to display all objects of this type
                            break;

                        case "4":
                            Console.WriteLine("Enter the ID of the object to update:");
                         
                            Assignment ToUpdate = helpA();
                            s_dalAssignment!.Create(ToUpdate);

                            // Code to update the object by ID
                            break;

                        case "5":
                            Console.WriteLine("Enter the ID of the object to delete:");
                            int idToDelete = int.Parse(Console.ReadLine() ?? "0");
                            s_dalAssignment!.Delete(idToDelete);
                            // Code to delete the object by ID
                            break;

                        case "6":
                            Console.WriteLine("Deleting all objects in the list");
                            // Code to delete all objects of this type
                            s_dalAssignment!.DeleteAll();
                            break;

                        default:
                            Console.WriteLine("Invalid choice. Please select a valid option.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions and display an error message
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }

           static Assignment helpA()
            {

                Console.Write("Enter Assignment ID (integer): ");
                int id = int.TryParse(Console.ReadLine(), out int parsedId) ? parsedId : 0;

                Console.Write("Enter Call ID (integer): ");
                int callId = int.TryParse(Console.ReadLine(), out int parsedCallId) ? parsedCallId : 0;

                Console.Write("Enter Volunteer ID (integer): ");
                int volunteerId = int.TryParse(Console.ReadLine(), out int parsedVolunteerId) ? parsedVolunteerId : 0;

                Console.Write("Enter the start time of treatment (time_entry_treatment) in format (yyyy-MM-dd HH:mm:ss): ");
                DateTime timeEntryTreatment;
                if (!DateTime.TryParse(Console.ReadLine(), out timeEntryTreatment))
                {
                    Console.WriteLine("Invalid date input. Setting time_entry_treatment to default DateTime.");
                    timeEntryTreatment = default(DateTime);
                }

                Console.Write("Enter the end time of treatment (time_end_treatment) in format (yyyy-MM-dd HH:mm:ss) or leave empty if not applicable: ");
                DateTime? timeEndTreatment = null;
                string endTreatmentInput = Console.ReadLine();
                if (!string.IsNullOrEmpty(endTreatmentInput) && DateTime.TryParse(endTreatmentInput, out DateTime parsedEnd))
                {
                    timeEndTreatment = parsedEnd;
                }

                Console.Write("Enter the completion type (EndOfTime) (e.g., Completed, Cancelled) or leave empty for default: ");
                AssignmentCompletionType? endOfTime = null;
                string endOfTimeInput = Console.ReadLine();
                if (Enum.TryParse(endOfTimeInput, out AssignmentCompletionType parsedCompletionType))
                {
                    endOfTime = parsedCompletionType;
                }

                // Return a new Assignment object with the provided input
                return new Assignment(
                    time_entry_treatment: timeEntryTreatment,
                    Id: id,
                    CallId: callId,
                    VolunteerId: volunteerId,
                    time_end_treatment: timeEndTreatment,
                    EndOfTime: endOfTime
                );
            }



    }

        private static void ConfigMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\nConfig Menu:");
                Console.WriteLine("1. Advance System Clock");
                Console.WriteLine("2. Show System Clock");
                Console.WriteLine("3. Set Config Variable");
                Console.WriteLine("4. Show Config Variable");
                Console.WriteLine("5. Reset Config");
                Console.WriteLine("0. Exit");

                ConfigMenuOption choice = (ConfigMenuOption)int.Parse(Console.ReadLine() ?? "0");

                switch (choice)
                {
                    case ConfigMenuOption.AdvanceSystemClock:
                        // קידום שעון המערכת
                        break;
                    case ConfigMenuOption.ShowSystemClock:
                        Console.WriteLine($"Current System Clock: {s_dalConfig?.Clock}");
                        break;
                    case ConfigMenuOption.SetConfigVariable:
                        // קביעת ערך למשתנה תצורה
                        break;
                    case ConfigMenuOption.ShowConfigVariable:
                        // הצגת ערך נוכחי של משתנה תצורה
                        break;
                    case ConfigMenuOption.ResetConfig:
                        s_dalConfig?.Reset();
                        Console.WriteLine("Config reset to default.");
                        break;
                    case ConfigMenuOption.Exit:
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice, try again.");
                        break;
                }
            }
        }
    }




}
