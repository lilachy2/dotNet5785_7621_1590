namespace BlTest
{
    using BlApi;

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
        DEFINITION,
        RESET,
        INITIALIZATION,
    }

    public enum IVolunteer
    {
        EXIT,
        EnterSystem,
        GET_VOLUNTEER_LIST,
        READ,
        UPDATE,
        DELETE,
        CREATE,
    }

    public enum ICall
    {
        EXIT,
        CountCalls,
        GetCallsList,
        Read,
        Update,
        Delete,
        Create,
        GetClosedCalls,
        GetOpenCalls,
        CloseTreatment,
        CancelTreatment,
        ChooseForTreatment,
    }

    internal class Program
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        static void Main(string[] args)
        {
            try
            {
                OPTION option = ShowMainMenu();
                while (OPTION.EXIT != option) // As long as you haven't chosen an exit
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
            catch (Exception ex)   // If any anomaly is detected
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
OPTION Options:
0 - Exit
1 - Admin
2 - Volunteer
3 - Call
");

            } while (!int.TryParse(Console.ReadLine(), out choice));

            return (OPTION)choice;
        }

        private static IAdmin ShowAdminMenu()
        {
            int choice;
            do
            {
                Console.WriteLine(@$"
Config Options:
0 - Exit
1 - Get clock
2 - Forward Clock
3 - Get Max Range
4 - Definition
5 - Reset
6 - Initialization");

            } while (!int.TryParse(Console.ReadLine(), out choice));

            return (IAdmin)choice;
        }

        private static IVolunteer ShowVolunteerMenu()
        {
            int choice;
            do
            {
                Console.WriteLine(@$"
Volunteer Options:
0 - Exit
1 - Enter System
2 - Get Volunteer List
3 - Read Volunteer
4 - Update Volunteer
5 - Delete Volunteer
6 - Create Volunteer");

            } while (!int.TryParse(Console.ReadLine(), out choice));

            return (IVolunteer)choice;
        }

        private static ICall ShowCallMenu()
        {
            int choice;
            do
            {
                Console.WriteLine(@$"
Call Options:
0 - Exit
1 - Count Calls
2 - Get Calls List
3 - Read Call
4 - Update Call
5 - Delete Call
6 - Create Call
7 - Get Closed Calls
8 - Get Open Calls
9 - Close Treatment
10 - Cancel Treatment
11 - Choose For Treatment");

            } while (!int.TryParse(Console.ReadLine(), out choice));

            return (ICall)choice;
        }

        private static void HandleAdminOptions()
        {
            try
            {
                switch (ShowAdminMenu())
                {
                    case IAdmin.GET_CLOCK:
                        Console.WriteLine(s_bl.Admin.GetClock());
                        break;

                    case IAdmin.FORWARD_CLOCK:
                        // Get the time unit for advancing the clock
                        Console.WriteLine("Enter the time unit to advance the clock (minute, hour, day, month, year): ");
                        var unitInput = Console.ReadLine();
                        if (Enum.TryParse(unitInput, true, out BO.TimeUnit unit))
                        {
                            s_bl.Admin.UpdateClock(unit);
                            Console.WriteLine($"Clock advanced by 1 {unit}.");
                        }
                        else
                        {
                            Console.WriteLine("Invalid time unit.");
                        }
                        break;

                    case IAdmin.GET_MAX_RANGE:
                        Console.WriteLine($"Current risk time range: {s_bl.Admin.GetMaxRange()}");
                        break;

                    case IAdmin.DEFINITION:
                        // Additional logic for definitions if needed
                        break;

                    case IAdmin.RESET:
                        Console.WriteLine("Are you sure you want to reset the database? (y/n)");
                        var confirmReset = Console.ReadLine();
                        if (confirmReset?.ToLower() == "y")
                        {
                            s_bl.Admin.ResetDB();
                            Console.WriteLine("Database has been reset.");
                        }
                        else
                        {
                            Console.WriteLine("Reset cancelled.");
                        }
                        break;

                    case IAdmin.INITIALIZATION:
                        Console.WriteLine("Are you sure you want to initialize the database? (y/n)");
                        var confirmInit = Console.ReadLine();
                        if (confirmInit?.ToLower() == "y")
                        {
                            s_bl.Admin.InitializeDB();
                            Console.WriteLine("Database has been initialized.");
                        }
                        else
                        {
                            Console.WriteLine("Initialization cancelled.");
                        }
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static void HandleVolunteerOptions()
        {
            try
            {
                switch (ShowVolunteerMenu())
                {
                    case IVolunteer.EnterSystem:
                        // Handle system login or authentication for the volunteer
                        Console.WriteLine("Enter your volunteer ID and password to authenticate:");
                        int volunteerId = int.Parse(Console.ReadLine());
                        string password = Console.ReadLine();
                        var role = s_bl.Volunteer.PasswordEntered(volunteerId, password);
                        Console.WriteLine($"Authentication successful, role: {role}");
                        break;

                    case IVolunteer.GET_VOLUNTEER_LIST:
                        Console.WriteLine("Fetching volunteer list...");
                        var volunteers = s_bl.Volunteer.ReadAll(true, BO.VolInList.Name);
                        foreach (var volunteer in volunteers)
                        {
                            Console.WriteLine($"Volunteer ID: {volunteer.Id}, Name: {volunteer.FullName}");
                        }
                        break;

                    case IVolunteer.READ:
                        Console.WriteLine("Enter the volunteer ID to view details:");
                        int volunteerReadId = int.Parse(Console.ReadLine());
                        var volunteerDetails = s_bl.Volunteer.Read(volunteerReadId);
                        if (volunteerDetails != null)
                        {
                            Console.WriteLine($"ID: {volunteerDetails.Id}, Name: {volunteerDetails.Name}, Active: {volunteerDetails.Active}");
                        }
                        else
                        {
                            Console.WriteLine("Volunteer not found.");
                        }
                        break;

                    case IVolunteer.UPDATE:
                        Console.WriteLine("Enter the volunteer ID to update:");
                        int volunteerUpdateId = int.Parse(Console.ReadLine());
                        var volunteerToUpdate = s_bl.Volunteer.Read(volunteerUpdateId);
                        if (volunteerToUpdate != null)
                        {
                            Console.WriteLine("Enter new volunteer name:");
                            volunteerToUpdate.Name = Console.ReadLine();
                            s_bl.Volunteer.Update(volunteerToUpdate, volunteerUpdateId);
                            Console.WriteLine("Volunteer updated.");
                        }
                        break;

                    case IVolunteer.DELETE:
                        Console.WriteLine("Enter the volunteer ID to delete:");
                        int volunteerDeleteId = int.Parse(Console.ReadLine());
                        s_bl.Volunteer.Delete(volunteerDeleteId);
                        Console.WriteLine("Volunteer deleted.");
                        break;

                    case IVolunteer.CREATE:
                        Console.WriteLine("Enter the volunteer name to create:");
                        string volunteerName = Console.ReadLine();
                        var newVolunteer = new BO.Volunteer { Name = volunteerName, Active = true };
                        s_bl.Volunteer.Create(newVolunteer);
                        Console.WriteLine("New volunteer created.");
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static void HandleCallOptions()
        {
            try
            {
                switch (ShowCallMenu())
                {
                    case ICall.CountCalls:
                        Console.WriteLine("Fetching call status counts...");
                        var callStatuses = s_bl.Call.GetCallStatusesCounts();
                        Console.WriteLine($"Open calls: {callStatuses[0]}, In progress: {callStatuses[1]}, Closed: {callStatuses[2]}");
                        break;

                    case ICall.GetCallsList:
                        Console.WriteLine("Fetching call list...");
                        var calls = s_bl.Call.GetCallsList();
                        foreach (var call in calls)
                        {
                            Console.WriteLine($"Call ID: {call.Id}, Status: {call.Status}");
                        }
                        break;

                    case ICall.Read:
                        Console.WriteLine("Enter the call ID to view details:");
                        int callReadId = int.Parse(Console.ReadLine());
                        var callDetails = s_bl.Call.Read(callReadId);
                        if (callDetails != null)
                        {
                            Console.WriteLine($"ID: {callDetails.Id}, Description: {callDetails.Description}, Status: {callDetails.Status}");
                        }
                        else
                        {
                            Console.WriteLine("Call not found.");
                        }
                        break;

                    case ICall.Update:
                        Console.WriteLine("Enter the call ID to update:");
                        int callUpdateId = int.Parse(Console.ReadLine());
                        var callToUpdate = s_bl.Call.Read(callUpdateId);
                        if (callToUpdate != null)
                        {
                            Console.WriteLine("Enter new call description:");
                            callToUpdate.Description = Console.ReadLine();
                            s_bl.Call.Update(callToUpdate);
                            Console.WriteLine("Call updated.");
                        }
                        break;

                    case ICall.Delete:
                        Console.WriteLine("Enter the call ID to delete:");
                        int callDeleteId = int.Parse(Console.ReadLine());
                        s_bl.Call.Delete(callDeleteId);
                        Console.WriteLine("Call deleted.");
                        break;

                    case ICall.Create:
                        Console.WriteLine("Enter new call description:");
                        string newCallDescription = Console.ReadLine();
                        var newCall = new BO.Call { Description = newCallDescription, Status = BO.CallStatus.Open };
                        s_bl.Call.Create(newCall);
                        Console.WriteLine("New call created.");
                        break;

                    case ICall.GetClosedCalls:
                        Console.WriteLine("Enter volunteer ID to view closed calls:");
                        int volunteerClosedId = int.Parse(Console.ReadLine());
                        var closedCalls = s_bl.Call.GetCloseCall(volunteerClosedId, null, null);
                        foreach (var closedCall in closedCalls)
                        {
                            Console.WriteLine($"Closed call ID: {closedCall.Id}, Description: {closedCall.FullAddress}");
                        }
                        break;

                    case ICall.GetOpenCalls:
                        Console.WriteLine("Enter volunteer ID to view open calls:");
                        int volunteerOpenId = int.Parse(Console.ReadLine());
                        var openCalls = s_bl.Call.GetOpenCall(volunteerOpenId, null, null);
                        foreach (var openCall in openCalls)
                        {
                            Console.WriteLine($"Open call ID: {openCall.Id}, Description: {openCall.Description}");
                        }
                        break;

                    case ICall.CloseTreatment:
                        Console.WriteLine("Enter call ID to close treatment:");
                        int closeCallId = int.Parse(Console.ReadLine());
                        s_bl.Call.UpdateEndTreatment(closeCallId, closeCallId);
                        Console.WriteLine("Treatment closed.");
                        break;

                    case ICall.CancelTreatment:
                        Console.WriteLine("Enter call ID to cancel treatment:");
                        int cancelCallId = int.Parse(Console.ReadLine());
                        s_bl.Call.UpdateCancelTreatment(cancelCallId, cancelCallId);
                        Console.WriteLine("Treatment canceled.");
                        break;

                    case ICall.ChooseForTreatment:
                        Console.WriteLine("Enter volunteer ID and call ID to choose treatment:");
                        int chooseVolunteerId = int.Parse(Console.ReadLine());
                        int chooseCallId = int.Parse(Console.ReadLine());
                        s_bl.Call.ChooseCall(chooseVolunteerId, chooseCallId);
                        Console.WriteLine("Call selected for treatment.");
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
