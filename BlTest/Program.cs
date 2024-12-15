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
        RESET,
        INITIALIZATION,
    }

    public enum IVolunteer
    {
        EXIT,
        ENTER_SYSTEM,
        GET_VOLUNTEER_LIST,
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
2 - Get Volunteer List
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
                            Console.WriteLine("Are you sure you want to reset the database? (y/n)");
                            if (Console.ReadLine()?.ToLower() == "y")
                            {
                                s_bl.Admin.ResetDB();
                                Console.WriteLine("Database reset successfully.");
                            }
                            break;

                        case IAdmin.INITIALIZATION:
                            Console.WriteLine("Are you sure you want to initialize the database? (y/n)");
                            if (Console.ReadLine()?.ToLower() == "y")
                            {
                                s_bl.Admin.InitializeDB();
                                Console.WriteLine("Database initialized successfully.");
                            }
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

                        case IVolunteer.GET_VOLUNTEER_LIST: // 2
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
                            Console.WriteLine("Is the Volunteer active? (true/false): ");
                            bool newActive = bool.Parse(Console.ReadLine());

                            BO.Volunteer newVolunteer = new BO.Volunteer
                            {
                                Id = newId,
                                Name = newName,
                                Number_phone = newPhone,
                                Email = newEmail,
                                FullCurrentAddress = newAddress,
                                Active = newActive
                            };

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
                ICall option = ShowCallMenu();
                while (option != ICall.EXIT)
                {
                    // Handle options similarly as above
                    option = ShowCallMenu();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
