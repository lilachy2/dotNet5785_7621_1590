namespace BlTest
{
    using BlApi;
    using DalApi;

    public enum OPTION
    {
        EXIT,
        ADMIN_MENU,
        VOLUNTEER_MENU,
        CALL_MENU,
    }

    public enum IAdmin
    {
        EXIT,
        GET_CLOCK,
        FORWARD_CLOCK,
        GET_MAX_RANGE,
        RESET,
        INITIALIZATION,
    }

    public enum IVolunteer
    {
        EXIT,
        ENTER_SYSTEM,
        READ_ALL,
        READ,
        UPDATE,
        DELETE,
        CREATE,
    }

    public enum ICall
    {
        EXIT,
        COUNT_CALLS,
        GET_CALLS_LIST,
        READ,
        UPDATE,
        DELETE,
        CREATE,
        GET_CLOSED_CALLS,
        GET_OPEN_CALLS,
        CLOSE_TREATMENT,
        CANCEL_TREATMENT,
        CHOOSE_FOR_TREATMENT,
    }

    internal class Program
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        static void Main(string[] args)
        {
            try
            {
                OPTION option = ShowMainMenu();
                while (OPTION.EXIT != option) // כל עוד לא בחרנו יציאה
                {
                    switch (option)
                    {
                        case OPTION.ADMIN_MENU:
                            HandleAdminOptions();
                            break;
                        case OPTION.VOLUNTEER_MENU:
                            HandleVolunteerOptions();
                            break;
                        case OPTION.CALL_MENU:
                            HandleCallOptions();
                            break;
                    }
                    option = ShowMainMenu();
                }
            }
            catch (Exception ex) // לטיפול בחריגות
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static OPTION ShowMainMenu()
        {
            int choice;
            do
            {
                Console.WriteLine(@"
Main Menu:
0 - Exit
1 - Admin
2 - Volunteer
3 - Call
");
            } while (!int.TryParse(Console.ReadLine(), out choice) || !Enum.IsDefined(typeof(OPTION), choice));

            return (OPTION)choice;
        }

        private static IAdmin ShowAdminMenu()
        {
            int choice;
            do
            {
                Console.WriteLine(@"
Admin Menu:
0 - Exit
1 - Get Clock
2 - Forward Clock
3 - Get Max Range
4 - Reset
5 - Initialization
");
            } while (!int.TryParse(Console.ReadLine(), out choice) || !Enum.IsDefined(typeof(IAdmin), choice));

            return (IAdmin)choice;
        }

        private static IVolunteer ShowVolunteerMenu()
        {
            int choice;
            do
            {
                Console.WriteLine(@"
Volunteer Menu:
0 - Exit
1 - Enter System
2 - Get Volunteer List (ReadAll)
3 - Read
4 - Update
5 - Delete
6 - Create
");
            } while (!int.TryParse(Console.ReadLine(), out choice) || !Enum.IsDefined(typeof(IVolunteer), choice));

            return (IVolunteer)choice;
        }

        private static ICall ShowCallMenu()
        {
            int choice;
            do
            {
                Console.WriteLine(@"
Call Menu:
0 - Exit
1 - Count Calls
2 - Get Calls List
3 - Read
4 - Update
5 - Delete
6 - Create
7 - Get Closed Calls
8 - Get Open Calls
9 - Close Treatment
10 - Cancel Treatment
11 - Choose For Treatment
");
            } while (!int.TryParse(Console.ReadLine(), out choice) || !Enum.IsDefined(typeof(ICall), choice));

            return (ICall)choice;
        }

        private static void HandleAdminOptions()
        {
            try
            {
                IAdmin option = ShowAdminMenu();
                while (option != IAdmin.EXIT)
                {
                    switch (option)
                    {
                        case IAdmin.GET_CLOCK:
                            Console.WriteLine($"Current Clock: {s_bl.Admin.GetClock()}");
                            break;

                        case IAdmin.FORWARD_CLOCK:
                            Console.WriteLine("Enter time unit to forward the clock (Minute, Hour, Day, Month, Year): ");
                            if (Enum.TryParse(Console.ReadLine(), true, out BO.TimeUnit unit))
                            {
                                s_bl.Admin.UpdateClock(unit);
                                Console.WriteLine("Clock forwarded successfully.");
                            }
                            else
                            {
                                Console.WriteLine("Invalid time unit.");
                            }
                            break;

                        case IAdmin.GET_MAX_RANGE:
                            Console.WriteLine($"Max range: {s_bl.Admin.GetMaxRange()}");
                            break;

                        case IAdmin.RESET:
                           
                                s_bl.Admin.ResetDB();
                                Console.WriteLine("Database reset successfully.");
                            
                            break;

                        case IAdmin.INITIALIZATION:
                           
                                s_bl.Admin.InitializeDB();
                                Console.WriteLine("Database initialized successfully.");
                            
                            break;

                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }
                    option = ShowAdminMenu();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void HandleVolunteerOptions()
        {
            try
            {
                IVolunteer option = ShowVolunteerMenu();
                while (option != IVolunteer.EXIT)
                {
                    switch (option)
                    {
                        case IVolunteer.ENTER_SYSTEM: // 1
                            Console.WriteLine("Enter your ID: ");
                            int id = int.Parse(Console.ReadLine());
                            Console.WriteLine("Enter your password: ");
                            string password = Console.ReadLine();
                            var role = s_bl.Volunteer.PasswordEntered(id, password);
                            Console.WriteLine($"Welcome! Role: {role}");
                            break;

                        case IVolunteer.READ_ALL: // 2
                            Console.WriteLine("Enter filter: Active (true/false) or leave blank: ");
                            bool? active = null;
                            var input = Console.ReadLine();
                            if (bool.TryParse(input, out var parsedActive))
                            {
                                active = parsedActive;
                            }

                            Console.WriteLine("Enter sorting criteria: Name, ID, or leave blank: ");
                            var sortByInput = Console.ReadLine();
                            BO.VolInList? sortBy = Enum.TryParse(sortByInput, true, out BO.VolInList parsedSort) ? parsedSort : null;

                            var volunteers = s_bl.Volunteer.ReadAll(active, sortBy);
                            foreach (var v in volunteers)
                            {
                                Console.WriteLine($"ID: {v.Id}, Full Name: {v.FullName}, Active: {v.IsActive}, " +
                                                  $"Total Calls Handled: {v.TotalCallsHandled}, Total Calls Cancelled: {v.TotalCallsCancelled}, " +
                                                  $"Total Calls Expired: {v.TotalCallsExpired}, Current Call ID: {v.CurrentCallId}, " +
                                                  $"Current Call Type: {v.CurrentCallType}");
                            }
                            break;


                        case IVolunteer.READ: // 3
                            
                            Console.WriteLine("Enter Volunteer ID: ");
                            int readId = int.Parse(Console.ReadLine());
                            var volunteer = s_bl.Volunteer.Read(readId);
                            if (volunteer != null)
                            {
                                Console.WriteLine($"ID: {volunteer.Id}, Name: {volunteer.Name}, Phone: {volunteer.Number_phone}, Email: {volunteer.Email}, Address: {volunteer.FullCurrentAddress}, Active: {volunteer.Active}");
                            }
                            else
                            {
                                Console.WriteLine("Volunteer not found.");
                            }
                            break;

                        case IVolunteer.UPDATE: // 4
                            Console.WriteLine("Enter Volunteer ID to update: ");
                            int updateId = int.Parse(Console.ReadLine());
                            Console.WriteLine("Enter updated name: ");
                            string updatedName = Console.ReadLine();
                            Console.WriteLine("Enter updated phone number: ");
                            string updatedPhone = Console.ReadLine();
                            Console.WriteLine("Enter updated email: ");
                            string updatedEmail = Console.ReadLine();
                            Console.WriteLine("Enter updated address: ");
                            string updatedAddress = Console.ReadLine();
                            Console.WriteLine("Enter updated activity status (true/false): ");
                            bool updatedActive = bool.Parse(Console.ReadLine());

                            BO.Volunteer updatedVolunteer = new BO.Volunteer
                            {
                                Id = updateId,
                                Name = updatedName,
                                Number_phone = updatedPhone,
                                Email = updatedEmail,
                                FullCurrentAddress = updatedAddress,
                                Active = updatedActive
                            };

                            s_bl.Volunteer.Update(updatedVolunteer, updateId);
                            Console.WriteLine("Volunteer updated successfully.");
                            break;

                        case IVolunteer.DELETE: // 5
                            Console.WriteLine("Enter Volunteer ID to delete: ");
                            int deleteId = int.Parse(Console.ReadLine());
                            s_bl.Volunteer.Delete(deleteId);
                            Console.WriteLine("Volunteer deleted successfully.");
                            break;

                        case IVolunteer.CREATE: // 6
                                                // בקשת נתונים מהמשתמש
                            Console.WriteLine("Enter new Volunteer ID: ");
                            int newId = int.Parse(Console.ReadLine());

                            Console.WriteLine("Enter new Volunteer Name: ");
                            string newName = Console.ReadLine();

                            Console.WriteLine("Enter new Volunteer Phone: ");
                            string newPhone = Console.ReadLine();

                            Console.WriteLine("Enter new Volunteer Email: ");
                            string newEmail = Console.ReadLine();

                            Console.WriteLine("Enter new Volunteer Address: ");
                            string newAddress = Console.ReadLine();

                            Console.WriteLine("Enter new Volunteer Password: ");
                            string newPassword = Console.ReadLine();

                            Console.WriteLine("Enter new Volunteer Role (e.g. Admin, Member): ");
                            string newRoleString = Console.ReadLine();
                            DO.Role newRole = Enum.TryParse(newRoleString, out DO.Role Role) ? Role : DO.Role.Volunteer; // Default to 'Member' if invalid

                            Console.WriteLine("Enter new Volunteer Distance Type (Aerial_distance,\r\n    walking_distance,\r\n    driving_distance,\r\n    change_distance_type): ");
                            string newDistanceTypeString = Console.ReadLine();
                            DO.distance_type newDistanceType = Enum.TryParse(newDistanceTypeString, out DO.distance_type distanceType) ? distanceType : DO.distance_type.Aerial_distance; // Default to 'Aerial_distance' if invalid

                            Console.WriteLine("Is the Volunteer active? (true/false): ");
                            bool newActive = bool.Parse(Console.ReadLine());
                            //Not OK
                            Console.WriteLine("Enter new Volunteer Latitude: ");
                            double newLatitude = double.Parse(Console.ReadLine());

                            Console.WriteLine("Enter new Volunteer Longitude: ");
                            double newLongitude = double.Parse(Console.ReadLine());

                            Console.WriteLine("Enter new Volunteer Distance: ");
                            double newDistance = double.Parse(Console.ReadLine());

                            // יצירת אובייקט BO.Volunteer עם כל הנתונים
                            BO.Volunteer newVolunteer = new BO.Volunteer
                            {
                                Id = newId,
                                Name = newName,
                                Number_phone = newPhone,
                                Email = newEmail,
                                FullCurrentAddress = newAddress,
                                Password = newPassword,  // סיסמה
                                Role = (BO.Role)newRole,  // תפקיד
                                DistanceType = (BO.DistanceType)newDistanceType,  // סוג המרחק
                                Active = newActive,

                                Latitude = newLatitude,  // קו רוחב
                                Longitude = newLongitude,  // קו אורך
                                Distance = newDistance  // מרחק
                            };

                            // יצירת המתנדב במערכת
                            s_bl.Volunteer.Create(newVolunteer);
                            Console.WriteLine("New Volunteer created successfully.");
                            break;

                        default:
                            Console.WriteLine("Invalid option.");
                            break;

                    }
                    option = ShowVolunteerMenu();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void HandleCallOptions()
        {
            try
            {
                ICall option = ShowCallMenu(); // הצגת התפריט הראשי
                while (option != ICall.EXIT)
                {
                    switch (option)
                    {
                        case ICall.COUNT_CALLS:
                            CountCalls(); // קריאה לפונקציה לספירת קריאות
                            break;

                        case ICall.GET_CALLS_LIST:
                            GetCallsList(); // קריאה לפונקציה להציג את רשימת הקריאות
                            break;

                        case ICall.READ:
                            ReadCall(); // קריאה לפונקציה לקרוא פרטי קריאה לפי ID
                            break;

                        case ICall.UPDATE:
                            UpdateCall(); // קריאה לפונקציה לעדכון קריאה
                            break;

                        case ICall.DELETE:
                            DeleteCall(); // קריאה לפונקציה למחיקת קריאה
                            break;

                        case ICall.CREATE:
                            CreateCall(); // קריאה לפונקציה ליצור קריאה חדשה
                            break;

                        case ICall.GET_CLOSED_CALLS:
                            GetClosedCalls(); // קריאה לפונקציה לקבל קריאות סגורות
                            break;

                        case ICall.GET_OPEN_CALLS:
                            GetOpenCalls(); // קריאה לפונקציה לקבל קריאות פתוחות
                            break;

                        case ICall.CLOSE_TREATMENT:
                            CloseTreatment(); // קריאה לפונקציה לסגור טיפול
                            break;

                        case ICall.CANCEL_TREATMENT:
                            CancelTreatment(); // קריאה לפונקציה לבטל טיפול
                            break;

                        case ICall.CHOOSE_FOR_TREATMENT:
                            ChooseForTreatment(); // קריאה לפונקציה לבחור טיפול
                            break;

                        default:
                            Console.WriteLine("Invalid option selected.");
                            break;
                    }

                    option = ShowCallMenu(); // הצגת התפריט מחדש לאחר ביצוע הפעולה
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}"); // טיפול בשגיאות
            }
        }

        // פונקציות לדוגמא שמטפלות בכל פעולה בתפריט

        private static void CountCalls()
        {
            // קריאה ל-ICall לספירת קריאות
            var counts = s_bl.Call.GetCallStatusesCounts();
            Console.WriteLine($"Total calls: {counts[0]}, Closed calls: {counts[1]}");
        }


        private static void GetCallsList()
        {
            // הצגת תפריט המשתמש
            Console.WriteLine("Testing GetCallsList()...");

            // שדות עבור הסינון והמיון
            BO.CallInListField? filterField = null;
            object? filterValue = null;
            BO.CallInListField? sortField = null;

            // בחירת שדה סינון
            Console.WriteLine("Enter filter field (Status, CallType, VolunteerName, or press Enter to skip):");
            string filterFieldInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(filterFieldInput))
            {
                Enum.TryParse(filterFieldInput, out filterField);
            }

            // בחירת ערך סינון
            Console.WriteLine("Enter filter value (e.g., Status, CallType, VolunteerName or press Enter to skip):");
            filterValue = Console.ReadLine(); // כאן נוכל לשדרג לפי סוגים שונים של סינונים

            // בחירת שדה למיון
            Console.WriteLine("Enter sort field (CallId, OpenTime, Status, VolunteerName or press Enter to skip):");
            string sortFieldInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(sortFieldInput))
            {
                Enum.TryParse(sortFieldInput, out sortField);
            }

            // קריאה לפונקציה מתוך האובייקט המתאים
            try
            {
                var callsList = s_bl.Call.GetCallsList(filterField, filterValue, sortField);

                // הדפסת הקריאות המתקבלות
                if (callsList != null && callsList.Any())
                {
                    Console.WriteLine("Calls List:");
                    foreach (var call in callsList)
                    {
                        Console.WriteLine(call.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("No calls match the given filter and sorting parameters.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }


        private static void ReadCall()
        {
            // קריאה ל-ICall להציג פרטי קריאה לפי ID
            Console.Write("Enter the call ID to read: ");
            int callId = int.Parse(Console.ReadLine());
            var call = s_bl.Call.Read(callId);
            if (call != null)
            {
                Console.WriteLine(call.ToString());
            }
            else
            {
                Console.WriteLine("Call not found.");
            }
        }

        private static void UpdateCall()
        {
            // קריאה ל-ICall לעדכון קריאה
            Console.Write("Enter the call ID to update: ");
            int callId = int.Parse(Console.ReadLine());
            var updatedCall = new BO.Call(); // יצירת אובייקט קריאה חדש (נחוץ למלא את המידע הנכון)
            s_bl.Call.Update(updatedCall);
            Console.WriteLine("Call updated.");
        }

        private static void DeleteCall()
        {
            // קריאה ל-ICall למחיקת קריאה
            Console.Write("Enter the call ID to delete: ");
            int callId = int.Parse(Console.ReadLine());
            s_bl.Call.Delete(callId);
            Console.WriteLine("Call deleted.");
        }

        private static void CreateCall()
        {
            // קריאה ל-ICall ליצירת קריאה חדשה
            var newCall = new BO.Call(); // יצירת אובייקט קריאה חדש (נחוץ למלא את המידע הנכון)
            s_bl.Call.Create(newCall);
            Console.WriteLine("New call created.");
        }

        private static void GetClosedCalls()
        {
            // קריאה ל-ICall לקבלת קריאות סגורות
            Console.Write("Enter volunteer ID: ");
            int volunteerId = int.Parse(Console.ReadLine());
            var closedCalls = s_bl.Call.GetCloseCall(volunteerId, null, null);
            foreach (var closedCall in closedCalls)
            {
                Console.WriteLine(closedCall.ToString());
            }
        }

        private static void GetOpenCalls()
        {
            // קריאה ל-ICall לקבלת קריאות פתוחות
            Console.Write("Enter volunteer ID: ");
            int volunteerId = int.Parse(Console.ReadLine());
            var openCalls = s_bl.Call.GetOpenCall(volunteerId, null, null);
            foreach (var openCall in openCalls)
            {
                Console.WriteLine(openCall.ToString());
            }
        }

        private static void CloseTreatment()
        {
            // קריאה ל-ICall לסגור טיפול
            Console.Write("Enter the call ID to close treatment: ");
            int callId = int.Parse(Console.ReadLine());
            s_bl.Call.UpdateEndTreatment(callId, callId);
            Console.WriteLine("Treatment closed.");
        }

        private static void CancelTreatment()
        {
            // קריאה ל-ICall לבטל טיפול
            Console.Write("Enter the call ID to cancel treatment: ");
            int callId = int.Parse(Console.ReadLine());
            s_bl.Call.UpdateCancelTreatment(callId, callId);
            Console.WriteLine("Treatment canceled.");
        }

        private static void ChooseForTreatment()
        {
            // קריאה ל-ICall לבחור טיפול
            Console.Write("Enter volunteer ID: ");
            int volunteerId = int.Parse(Console.ReadLine());
            Console.Write("Enter the call ID to choose: ");
            int callId = int.Parse(Console.ReadLine());
            s_bl.Call.ChooseCall(volunteerId, callId);
            Console.WriteLine("Call chosen for treatment.");
        }


    }
}
