namespace BlTest
{
    using BlApi;
    using BO;
    using DalApi;
    using System.ComponentModel;
    // test
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
                                Console.WriteLine(
     $"ID: {v.Id}\n" +
     $"Full Name: {v.FullName}\n" +
     $"Active: {v.IsActive}\n" +
     $"Total Calls Handled: {v.TotalCallsHandled}\n" +
     $"Total Calls Cancelled: {v.TotalCallsCancelled}\n" +
     $"Total Calls Expired: {v.TotalCallsExpired}\n" +
     $"Current Call ID: {v.CurrentCallId}\n" +
     $"Current Call Type: {v.CurrentCallType}"
 );

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


                        case IVolunteer.CREATE:
                            {

                                Console.WriteLine("Please enter volunteer details:");
                                Console.WriteLine("Please enter the ID :");
                                string idCreat = Console.ReadLine();  // לוקחים את הקלט מהמשתמש

                                if (!int.TryParse(idCreat, out int idC))  // מנסים להמיר את הקלט למספר
                                {
                                    throw new BO.Incompatible_ID($"Invalid ID{idCreat} format");  // זורקים חריגה אם המזהה לא תקני
                                }
                                // Full Name
                                Console.WriteLine("Full Name:");
                                string fullNameUp = Console.ReadLine();

                                // Phone Number
                                Console.WriteLine("Phone Number:");
                                string phoneNumberUp = Console.ReadLine();

                                // Email
                                Console.WriteLine("Email:");
                                string emailUp = Console.ReadLine();

                                // Distance Type
                                Console.WriteLine("Distance Type (Aerial_distance, walking_distance, driving_distance):");
                                string distanceTypeInputUp = Console.ReadLine();
                                BO.DistanceType distanceType;
                                if (!Enum.TryParse(distanceTypeInputUp, true, out distanceType))
                                {
                                    throw new BO.Incompatible_ID("Invalid distance type. Defaulting to Aerial.");

                                }

                                // Role
                                Console.WriteLine("Role (Volunteer, Manager):");
                                string roleUp = Console.ReadLine();
                                BO.Role roleup;
                                if (!Enum.TryParse(roleUp, true, out roleup))
                                {
                                    throw new BO.Incompatible_ID("Invalid role. Defaulting to Volunteer.");

                                }

                                // Active
                                Console.WriteLine("Active (true/false):");
                                bool IsActive;
                                if (!bool.TryParse(Console.ReadLine(), out IsActive))
                                {
                                    throw new BO.Incompatible_ID("Invalid input for Active. Defaulting to false.");
                                }

                                // Password
                                Console.WriteLine("Password:");
                                string passwordUp = Console.ReadLine();

                                // Full Address
                                Console.WriteLine("Full Address:");
                                string fullAddressUp = Console.ReadLine();

                                // Max Reading
                                Console.WriteLine("Max Reading:");
                                int maxReading;
                                if (!int.TryParse(Console.ReadLine(), out maxReading))
                                {
                                    throw new BO.Incompatible_ID("Invalid input for Max Reading. Defaulting to 0.");

                                }

                                // Create the new Volunteer object
                                BO.Volunteer newVolunteer = new BO.Volunteer
                                {
                                    Id = idC,
                                    Name = fullNameUp,
                                    Number_phone = phoneNumberUp,
                                    Email = emailUp,
                                    DistanceType = distanceType,
                                    Role = roleup,
                                    Active = IsActive,
                                    Password = passwordUp,
                                    FullCurrentAddress = fullAddressUp,
                                    Latitude = 0,
                                    Longitude = 0,
                                    Distance = maxReading,
                                    TotalHandledCalls = 0,
                                    TotalCancelledCalls = 0,
                                    TotalExpiredCalls = 0,
                                    CurrentCall = null,
                                };

                                // Call the Create method
                                s_bl.Volunteer.Create(newVolunteer);
                                break;

                            }



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


        private static void CountCalls()
        {
            var counts = s_bl.Call.GetCallStatusesCounts();
            Console.WriteLine($" Open calls: {counts[0]}  Closed calls: {counts[1]}  InProgress calls: {counts[2]}  Expired calls: {counts[3]}  InProgressAtRisk calls: {counts[4]}  OpenAtRisk calls: {counts[5]} Total calls: {counts[6]} ");
    
        }

        private static void GetCallsList()
        {
            Console.WriteLine("Testing GetCallsList()...");

            BO.CallInListField? filterField = null;
            BO.CallInListField? sortField = null;

            Console.WriteLine("Enter filter field (Id, CallId, CallType, OpenTime, TimeRemaining, VolunteerName, CompletionTime, Status, TotalAssignments, or press Enter to skip):");
            string filterFieldInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(filterFieldInput))
            {
                if (Enum.TryParse<BO.CallInListField>(filterFieldInput, true, out var parsedFilterField))
                {
                    filterField = parsedFilterField;
                }
            }

            Console.WriteLine("Enter filter value (Status, CallType, TimeRemaining, CompletionTime, or press Enter to skip):");
            string filterValueInput = Console.ReadLine();

            object? filterValue = null;

            if (!string.IsNullOrEmpty(filterValueInput))
            {
                if (Enum.TryParse<CallStatus>(filterValueInput, true, out var callStatusEnum))
                {
                    filterValue = callStatusEnum;
                }
                else if (Enum.TryParse<Calltype>(filterValueInput, true, out var callTypeEnum))
                {
                    filterValue = callTypeEnum;
                }
                else if (int.TryParse(filterValueInput, out var intValue))
                {
                    filterValue = intValue; // for TimeRemaining or TotalAssignments
                }
                else if (TimeSpan.TryParse(filterValueInput, out var timeValue))
                {
                    filterValue = timeValue; // for CompletionTime
                }
                else
                {
                    Console.WriteLine("Invalid filter value. Please enter a valid CallStatus, CallType, TimeRemaining, or CompletionTime.");
                }
            }

            Console.WriteLine("Enter sort field (Id, CallId, CallType, OpenTime, TimeRemaining, VolunteerName, CompletionTime, Status, TotalAssignments, or press Enter to skip):");
            string sortFieldInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(sortFieldInput))
            {
                if (Enum.TryParse<BO.CallInListField>(sortFieldInput, true, out var parsedSortField))
                {
                    sortField = parsedSortField;
                }
            }

            try
            {
                var callsList = s_bl.Call.GetCallsList(filterField, filterValue, sortField);

                if (callsList != null && callsList.Any())
                {
                    Console.WriteLine("Calls List:");
                    foreach (var call in callsList)
                    {
                        Console.WriteLine(call.ToString() + "\n");

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
            Console.Write("Enter the call ID to update: ");
            int callId = int.Parse(Console.ReadLine());


            Console.WriteLine("Call Type (fainting, birth, resuscitation, allergy, heartattack, broken_bone, security_event, None):");
            string callTypeInput = Console.ReadLine();
            BO.Calltype callType;
            if (!Enum.TryParse(callTypeInput, true, out callType))
            {
                callType = BO.Calltype.None;
                throw new BO.Incompatible_ID("Invalid call type. Defaulting to 'None'.");

            }

            // Description
            Console.WriteLine("Description:");
            string description = Console.ReadLine();

            // Full Address
            Console.WriteLine("Full Address:");
            string fullAddress = Console.ReadLine();

            // Open Time
            Console.WriteLine("Open Time (yyyy-MM-dd HH:mm:ss):");
            DateTime openTime;
            if (!DateTime.TryParse(Console.ReadLine(), out openTime))
            {
                throw new BO.Incompatible_ID("Invalid open time format.");
            }

            // Max End Time
            Console.WriteLine("Max End Time (yyyy-MM-dd HH:mm:ss or press Enter for no value):");
            string maxEndTimeInput = Console.ReadLine();
            DateTime? maxEndTime = string.IsNullOrEmpty(maxEndTimeInput) ? (DateTime?)null : DateTime.Parse(maxEndTimeInput);


            // Create the new Call object
            BO.Call updatedCall = new BO.Call
            {
                Id = callId,
                Calltype = callType,
                Description = description,
                FullAddress = fullAddress,
                Latitude = 0,
                Longitude = 0,
                OpenTime = openTime,
                MaxEndTime = maxEndTime
            };


            s_bl.Call.Update(updatedCall);
            Console.WriteLine("Call updated.");
        }


        private static void DeleteCall()
        {
            Console.Write("Enter the call ID to delete: ");
            int callId = int.Parse(Console.ReadLine());
            s_bl.Call.Delete(callId);
            Console.WriteLine("Call deleted.");
        }

        private static void CreateCall()
        {
            Console.WriteLine("Please enter call details:");

            // Call ID
            Console.WriteLine("Please enter the ID:");
            string idInput = Console.ReadLine();
            if (!int.TryParse(idInput, out int callId))
            {
                throw new BO.Incompatible_ID($"Invalid ID {idInput} format");
            }

            // Call Type

            Console.WriteLine("Call Type (fainting, birth, resuscitation, allergy, heartattack, broken_bone, security_event, None):");
            string callTypeInput = Console.ReadLine();
            BO.Calltype callType;
            if (!Enum.TryParse(callTypeInput, true, out callType))
            {
                callType = BO.Calltype.None;
                throw new BO.Incompatible_ID("Invalid call type. Defaulting to 'None'.");

            }

            // Description
            Console.WriteLine("Description:");
            string description = Console.ReadLine();

            // Full Address
            Console.WriteLine("Full Address:");
            string fullAddress = Console.ReadLine();

            // Open Time
            Console.WriteLine("Open Time (yyyy-MM-dd HH:mm:ss):");
            DateTime openTime;
            if (!DateTime.TryParse(Console.ReadLine(), out openTime))
            {
                throw new BO.Incompatible_ID("Invalid open time format.");
            }

            // Max End Time
            Console.WriteLine("Max End Time (yyyy-MM-dd HH:mm:ss or press Enter for no value):");
            string maxEndTimeInput = Console.ReadLine();
            DateTime? maxEndTime = string.IsNullOrEmpty(maxEndTimeInput) ? (DateTime?)null : DateTime.Parse(maxEndTimeInput);


            // Create the new Call object
            BO.Call newCall = new BO.Call
            {
                Id =callId,
                Calltype = callType,
                Description = description,
                FullAddress = fullAddress,
                Latitude = 0,
                Longitude = 0,
                OpenTime = openTime,
                MaxEndTime = maxEndTime
            };
            // status, Latitude, Longitude in creat 

            // Call the Create method
            s_bl.Call.Create(newCall);
            Console.WriteLine("New call created.");

        }



        private static void GetClosedCalls()
        {
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
            // בקשת ת.ז של המתנדב
            Console.Write("Enter volunteer ID: ");
            int volunteerId = int.Parse(Console.ReadLine());

            // בקשת סוג הקריאה (אם יש)
            Console.Write("Enter call type (or press Enter for all types): ");
            string callTypeInput = Console.ReadLine();
            BO.Calltype? callType = string.IsNullOrEmpty(callTypeInput) ? (BO.Calltype?)null : (BO.Calltype)Enum.Parse(typeof(BO.Calltype), callTypeInput);

            // בקשת שדה למיון (אם יש)
            Console.Write("Enter field to sort by (or press Enter for sorting by call ID): ");
            string sortByInput = Console.ReadLine();
            BO.OpenCallInListEnum? sortBy = string.IsNullOrEmpty(sortByInput) ? (BO.OpenCallInListEnum?)null : (BO.OpenCallInListEnum)Enum.Parse(typeof(BO.OpenCallInListEnum), sortByInput);

            // קריאה לפונקציה שמחזירה את הקריאות הפתוחות
            var openCalls = s_bl.Call.GetOpenCall(volunteerId, callType, sortBy);

            // הצגת התוצאות
            foreach (var openCall in openCalls)
            {
                Console.WriteLine(openCall.ToString());
            }
        }


        private static void CloseTreatment()
        {
            Console.Write("Enter the call ID to close treatment: ");
            int callId = int.Parse(Console.ReadLine());
            s_bl.Call.UpdateEndTreatment(callId, callId);
            Console.WriteLine("Treatment closed.");
        }

        private static void CancelTreatment()
        {
            Console.Write("Enter the call ID to cancel treatment: ");
            int callId = int.Parse(Console.ReadLine());
            s_bl.Call.UpdateCancelTreatment(callId, callId);
            Console.WriteLine("Treatment canceled.");
        }

        private static void ChooseForTreatment()
        {
            Console.Write("Enter volunteer ID: ");
            int volunteerId = int.Parse(Console.ReadLine());
            Console.Write("Enter the call ID to choose: ");
            int callId = int.Parse(Console.ReadLine());
            s_bl.Call.ChooseCall(volunteerId, callId);
            Console.WriteLine("Call chosen for treatment.");
        }


    }
}
