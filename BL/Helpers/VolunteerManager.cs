
using BlApi;
using BlImplementation;
using BO;
using DalApi;
using DO;
using Microsoft.VisualBasic;
using System;
using System.Text.RegularExpressions;

namespace Helpers;

internal static class VolunteerManager
{
    private static DalApi.IDal _dal = DalApi.Factory.Get; //stage 4
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    internal static ObserverManager Observers = new(); //stage 5 
    internal static void CheckFormat(BO.Volunteer boVolunteer)
    {
        try
        {
            CheckPhonnumber(boVolunteer.Number_phone);
            CheckEmail(boVolunteer.Email);
            Tools.IsAddressValidAsync(boVolunteer.FullCurrentAddress);
            IsStrongPassword(boVolunteer.Password);
        }
        catch(BO.InvalidOperationException ex)
        {
            throw new BO.InvalidOperationException("",ex);
        }
        catch (BO.BlWrongItemtException ex)
        {
            throw new BO.BlWrongItemtException($"the item have logic problem", ex);
        }

    }
    internal static void CheckEmail(string Email)
    {
        if (!Regex.IsMatch(Email, @"^(?("")(""[^""]+?""@)|(([0-9a-zA-Z](([\.\-]?)(?![\.\-])))*[0-9a-zA-Z]@))([0-9a-zA-Z][\-0-9a-zA-Z]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,}$"))
        {
            throw new ArgumentException("Invalid Email format.");
        }

    }

    public static bool IsValidIsraeliID(int id)
    {
        // Convert the integer to a string to validate length
        string idStr = id.ToString();

        // Ensure the ID is exactly 9 digits
        if (idStr.Length != 9)
            return false;

        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            // Extract each digit
            int digit = idStr[i] - '0'; // Convert character to integer

            // Multiply alternately by 1 or 2
            int multiplied = digit * (i % 2 + 1);

            // If the result is greater than 9, sum its digits (e.g., 18 -> 1 + 8)
            sum += (multiplied > 9) ? multiplied - 9 : multiplied;
        }

        // The ID is valid if the total sum is divisible by 10
        return sum % 10 == 0;
    }

    internal static void CheckPhonnumber(string Number_phone)
    {
        if (string.IsNullOrWhiteSpace(Number_phone) || !Regex.IsMatch(Number_phone, @"^0\d{9}$"))
        {
            throw new BlCheckPhonnumberException("PhoneNumber must be a 10-digit number starting with 0.");
        }
    }
    public static void IsStrongPassword(string password)
    {
        if (password.Length < 8)
            throw new BlIncorrectPasswordException("Password must be at least 8 characters long.");

        if (!Regex.IsMatch(password, @"[A-Z]"))
            throw new BlIncorrectPasswordException("Password must contain at least one uppercase letter.");

        if (!Regex.IsMatch(password, @"[a-z]"))
            throw new BlIncorrectPasswordException("Password must contain at least one lowercase letter.");

        if (!Regex.IsMatch(password, @"\d"))
            throw new BlIncorrectPasswordException("Password must contain at least one number.");
    }


    internal static void CheckLogic(BO.Volunteer boVolunteer, BO.Volunteer existingVolunteer, bool isManager)
    {

        Tools.CheckId(boVolunteer.Id);
        if (existingVolunteer != null)
        {
            // Check if the role was changed
            if (boVolunteer.Role != null)

            {
                if (boVolunteer.Role != existingVolunteer.Role)
                {
                    if (!isManager)
                    {
                        throw new BO.BlPermissionException("Only a manager is authorized to update the volunteer's role.");
                    }
                }
            }
            // Check if the password was changed
            if (boVolunteer.Password != existingVolunteer.Password && !isManager)
            {
                throw new BO.BlIncorrectPasswordException("Only the volunteer or a manager can update the password.");
            }

            // Check if the active status was changed
            if (boVolunteer.Active != existingVolunteer.Active)
            {

                if (!isManager)
                {
                    throw new BO.BlGeneralException("Only a manager is authorized to change the active status.");
                }
            }

            // Add additional checks here for other properties if necessary
        }
    }
    internal static void CheckLogic(BO.Volunteer boVolunteer, BO.Volunteer existingVolunteer, int requesterId, DO.Role requesterRole)
    {
        // Check if the role has changed - only a manager can update the volunteer's role
        if (boVolunteer.Role != existingVolunteer.Role)
        {
            if (requesterRole != DO.Role.Manager)
            {
                throw new BO.BlPermissionException("Only a manager is authorized to update the volunteer's role.");
            }
        }

        // Check if the password has changed - only the volunteer or a manager can update the password
        if (boVolunteer.Password != existingVolunteer.Password)
        {
            if (requesterId != boVolunteer.Id && requesterRole != DO.Role.Manager)
            {
                throw new BO.BlPermissionException("Only the volunteer or a manager can update the password.");
            }
        }

        // Check if the active status has changed - only a manager can change the active
        if ((boVolunteer.Active != existingVolunteer.Active) && (boVolunteer.Role == BO.Role.Volunteer))
        {
            lock (AdminManager.BlMutex) //stage 7
                // mark himself as inactive - provided he is not handling the call at the moment
                if (CallManager.GetCallInProgress(boVolunteer.Id) != null)
                {
                    throw new BO.BlCan_chang_to_NotActivException("The volunteer is currently handling a call and cannot be not Active.");
                }
            //if (requesterRole != DO.Role.Manager)
            //{
            //    throw new BO.BlPermissionException("Only a manager is authorized to change the active status.");
            //}
        }
        if (boVolunteer.Distance < 0)
        {
            throw new BO.BlIsLogicCallException("only positive number");

        }

        // Add additional checks if there are any other restricted fields
    }


    public static BO.Volunteer GetVolunteer(int id)
    {
        lock (AdminManager.BlMutex) //stage 7

        {
            DO.Volunteer? doVolunteer = _dal.Volunteer.Read(id) ?? throw new BlDoesNotExistException("eroor id");

            return new BO.Volunteer
            {
                Id = doVolunteer.Id,
                Name = doVolunteer.Name,
                Number_phone = doVolunteer.Number_phone,
                Email = doVolunteer.Email,
                FullCurrentAddress = doVolunteer.FullCurrentAddress,
                Password = doVolunteer.Password,
                Latitude = doVolunteer.Latitude,
                Longitude = doVolunteer.Longitude,
                Role = (BO.Role)doVolunteer.Role, // המרה ישירה בין ה-Enums
                Active = doVolunteer.Active,
                Distance = doVolunteer.distance,
                DistanceType = (BO.DistanceType)doVolunteer.Distance_Type, // המרה ישירה בין ה-Enums
                TotalHandledCalls = _dal.Assignment.ReadAll().Count(a => a.VolunteerId == doVolunteer.Id && a.EndOfTime == AssignmentCompletionType.TreatedOnTime),
                TotalCancelledCalls = _dal.Assignment.ReadAll().Count(a => a.VolunteerId == doVolunteer.Id &&
                    (a.EndOfTime == AssignmentCompletionType.AdminCancelled || a.EndOfTime == AssignmentCompletionType.VolunteerCancelled)), // ביטול עצמי או מהנל
                TotalExpiredCalls = _dal.Assignment.ReadAll().Count(a => a.VolunteerId == doVolunteer.Id && a.EndOfTime == AssignmentCompletionType.Expired),
                //CurrentCall = CallManager.GetCallInProgress(doVolunteer.Id), //CallAssignInProgress  בפונקצית עזר של 
                CurrentCall = CallManager.GetCallInProgressAsync(doVolunteer.Id).Result, //CallAssignInProgress  בפונקצית עזר של 
            };
        }
    }

    public static DO.Volunteer BOconvertDO(BO.Volunteer Volunteer)
    {
        return new DO.Volunteer
        {
            Id = Volunteer.Id,
            Name = Volunteer.Name,
            Number_phone = Volunteer.Number_phone,
            Email = Volunteer.Email,
            Password = Volunteer.Password,
            Role = (DO.Role)Volunteer.Role, // Assuming Role is of type Role, which is similar in both DO and BO
            Distance_Type = (DO.distance_type)Volunteer.DistanceType, // Converting DistanceType to DO.distance_type
            Active = Volunteer.Active,
            FullCurrentAddress = Volunteer.FullCurrentAddress,
            Latitude = Volunteer.Latitude,
            Longitude = Volunteer.Longitude,
            distance = Volunteer.Distance
        };


    }

    public static BO.VolunteerInList GetVolunteerInList(int VolunteerId)
    {

        lock (AdminManager.BlMutex) //stage 7

        {
            DO.Volunteer? doVolunteer = _dal.Volunteer.Read(VolunteerId) ?? throw new BlDoesNotExistException("eroor id");// ז

            //Find the appropriate CALL  and  Assignmentn by volunteer ID
            var doAssignment = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == VolunteerId && a.EndOfTime == null).FirstOrDefault();

            var calls = _dal.Assignment.ReadAll(ass => ass.VolunteerId == VolunteerId);

            int? currentCallId = calls.FirstOrDefault(ass => ass.EndOfTime == null)?.Id;


            return new BO.VolunteerInList
            {
                Id = doVolunteer.Id,
                FullName = doVolunteer.Name,
                IsActive = doVolunteer.Active,
                TotalCallsHandled = Tools.TotalHandledCalls(VolunteerId),
                TotalCallsCancelled = Tools.TotalCallsCancelledhelp(VolunteerId),
                TotalCallsExpired = Tools.TotalCallsExpiredelo(VolunteerId),
                CurrentCallId = currentCallId,/*Tools.CurrentCallIdhelp(VolunteerId),*/
                CurrentCallType = Tools.CurrentCallType(VolunteerId)
            };
        }
    }

    // GetClosedCallInList 
    public static BO.ClosedCallInList GetClosedCallInList(int VolunteerId)
    {
        lock (AdminManager.BlMutex) //stage 7

        {
            DO.Volunteer? doVolunteer = _dal.Volunteer.Read(VolunteerId) ?? throw new BlDoesNotExistException("eroor id");// ז

            //Find the appropriate CALL  and  Assignmentn by volunteer ID
            var doAssignment = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == VolunteerId/* && a.EndOfTime == null*/).FirstOrDefault();
            var doCall = _dal.Call.ReadAll().Where(c => c.Id == doAssignment!.CallId).FirstOrDefault();

            // Create the object
            return new BO.ClosedCallInList
            {
                Id = doAssignment.Id, // Call identifier
                CallType = (BO.Calltype)doCall.Calltype, // Enum conversion
                FullAddress = doCall.ReadAddress, // Full address of the call
                OpenTime = doCall.OpeningTime, // Time when the call was opened
                EnterTime = doAssignment.time_entry_treatment, // Time when the treatment began
                EndTime = doAssignment.time_end_treatment, // Time when the treatment ended
                CompletionStatus = (BO.CallAssignmentEnum?)doAssignment.EndOfTime // Completion status of the treatment
            };
        }

    }


    //GetOpenCallInList
    public static BO.OpenCallInList GetOpenCallInList(int callId, int volunteerId)
    {
        lock (AdminManager.BlMutex) //stage 7

        {
            DO.Volunteer? doVolunteer = _dal.Volunteer.Read(volunteerId) ?? throw new BlDoesNotExistException("error id");

            // Find the appropriate assignment by volunteer ID and call ID
            var doAssignment = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == volunteerId && a.CallId == callId).FirstOrDefault();
            if (doAssignment == null)
            {
                throw new BlDoesNotExistException("Assignment not found for the given call and volunteer.");
            }

            var doCall = _dal.Call.ReadAll().Where(c => c.Id == callId).FirstOrDefault();
            if (doCall == null)
            {
                throw new BlDoesNotExistException("Call not found.");
            }

            //Get latitude and longitude of the volunteer's address asynchronously
            double? latitudeVolunteer = Tools.GetLatitudeAsync(doVolunteer.FullCurrentAddress).Result;
            double? longitudeVolunteer = Tools.GetLongitudeAsync(doVolunteer.FullCurrentAddress).Result;

            // Ensure latitude and longitude are valid before using them
            if (!latitudeVolunteer.HasValue || !longitudeVolunteer.HasValue)
            {
                throw new BlGeneralException("Could not retrieve valid coordinates for the volunteer.");
            }

            // Create and return the OpenCallInList object
            return new BO.OpenCallInList
            {
                Id = doAssignment.Id, // Assignment identifier
                CallType = (BO.Calltype)doCall.Calltype, // Enum conversion
                FullAddress = doCall.ReadAddress, // Full address of the call
                OpenTime = doCall.OpeningTime, // Time when the call was opened
                MaxEndTime = doCall.MaxEndTime, // Maximum end time for the call
                DistanceFromVolunteer = CallManager.Air_distance_between_2_addresses(
                    doCall.Latitude, doCall.Longitude, latitudeVolunteer, longitudeVolunteer
                ) // Air distance between 2 addresses (call and volunteer)
            };
        }
    }




    //internal static void SimulateVolunteerActivity() //stage 7
    //{
    //    // var volunteerImplementation = new VolunteerImplementation();
    //    Thread.CurrentThread.Name = $"Simulator{++s_simulatorCounter}";

    //    // var volunteerList = volunteerImplementation.GetVolunteerList(true,null);
    //    // var volunteerlist = GetVolunteerListHelp(true, null)/.ToList()/;
    //    double probability = 0.2;

    //    // יצירת מספר אקראי בטווח 0 עד 1
    //    double randomValue = s_rand.NextDouble(); // מספר בין 0.0 ל-1.0
    //    var volunteerList = GetVolunteerListHelp(true, null).ToList();
    //    int size = volunteerList.Count();
    //    // בדיקה אם המספר האקראי קטן מההסתברות
    //    for (int i = 0; i < size; i++)
    //    {
    //        var volunteer = readHelp(volunteerList[i].Id);
    //        if (volunteer.CallIn == null && randomValue < probability)
    //        {
    //            var openCallInListsToChose = CallManager.GetOpenCallHelp(volunteer.Id, null, null).ToList();

    //            if (openCallInListsToChose != null && openCallInListsToChose.Count > 0)
    //            {
    //                //choose random call for volunteer
    //                var randomIndex = s_rand.Next(openCallInListsToChose.Count);
    //                var chosenCall = openCallInListsToChose[randomIndex];

    //                CallManager.ChoseForTreatHelp(volunteer.Id, chosenCall.Id);
    //            }
    //        }

    //        else if (volunteer.CallIn != null)    //there is call in treat
    //        {
    //            var callin = readHelp(volunteer.Id).CallIn!;
    //            if ((AdminManager.Now - callin.StartTreat) >= TimeSpan.FromHours(3))
    //            {
    //                CallManager.CloseTreatHelp(volunteer.Id, callin.Id);
    //            }
    //            else
    //            {
    //                int probability1 = s_rand.Next(1, 101); // מספר אקראי בין 1 ל-100

    //                if (probability1 <= 10) // הסתברות של 10%
    //                {
    //                    // ביטול הטיפול
    //                    CallManager.CancelTreatHelp(volunteer.Id, callin.Id);
    //                }
    //            }
    //        }

    //    }
    //}



    internal static List<BO.Call> GetOpenCallsimulator()
    {
        lock (AdminManager.BlMutex)
            return _dal.Call.ReadAll().Select(v=>CallManager.GetViewingCall(v.Id)).Where(v=> v.Status== BO.CallStatus.OpenAtRisk|| v.Status == BO.CallStatus.Open ).Where(c => AreCoordinatesCalculated(c)).ToList();
    }


    internal static void SimulationVolunteerActivity()
    {
        ////??????
        Task.Run(() =>
        {
            try
            {
                // קבלת רשימת המתנדבים הפעילים והפיכתם לרשימה קונקרטית
                List<BO.Volunteer> activeVolunteers;
                lock (AdminManager.BlMutex)
                {
                    activeVolunteers = _dal.Volunteer.ReadAll()
                        .Where(v => v.Active)
                        .Select(v => GetVolunteer(v.Id)).ToList();
                }

                foreach (var volunteer in activeVolunteers)
                {
                    bool hasUpdated = false;

                    // בדיקה אם למתנדב יש קריאה בטיפולו

                    BO.CallInProgress activeAssignment = volunteer.CurrentCall;

                    if (activeAssignment == null)
                    {
                        // למתנדב אין קריאה פעילה
                        //if (new Random().NextDouble() <= 0.5) // הסתברות של 20%
                        {
                            List<BO.Call> availableCalls = null;
                            lock (AdminManager.BlMutex)
                                availableCalls = GetOpenCallsimulator();


                            if (availableCalls.Any())
                            {
                                var randomCall = availableCalls[new Random().Next(availableCalls.Count)];//take random call in index next count
                                lock (AdminManager.BlMutex)
                                {
                                    CallManager.ChooseCall(volunteer.Id, randomCall.Id);

                                }
                                hasUpdated = true;
                            }
                        }

                    }
                    else
                    {
                        // למתנדב יש קריאה פעילה
                        var TotalMinutesOfMyTreatment = (AdminManager.Now - activeAssignment.EnterTime).TotalMinutes;
                        double TimeMakeSance = new Random().Next(10, 30); ;

                        if (TotalMinutesOfMyTreatment >= TimeMakeSance)
                        {
                            // סיום הטיפול
                            lock (AdminManager.BlMutex)
                            {
                                CallManager.UpdateEndTreatment(volunteer.Id, activeAssignment.CallId);

                            }
                            hasUpdated = true;
                        }
                        else
                        {
                            // ביטול הטיפול
                            //if (new Random().NextDouble() <= 0.1)
                            {
                                lock (AdminManager.BlMutex)
                                    CallManager.UpdateCancelTreatment(volunteer.Id, activeAssignment.CallId);

                                hasUpdated = true;
                            }
                        }
                    }

                    // עדכון הודעה על שינוי מחוץ ל-lock
                    if (hasUpdated)
                    {
                        Observers.NotifyItemUpdated(volunteer.Id);
                        Observers.NotifyListUpdated();
                    }
                }

            }
            catch (Exception ex)
            {
                // טיפול בשגיאות
                Console.WriteLine($"Error in SimulationVolunteerActivity: {ex.Message}");
            }
        });
    }


 





    private static bool AreCoordinatesCalculated(BO.Call dalCall)
    {
        // בדיקה אם קווי אורך ורוחב אינם null ואינם שווים לאפס
        return dalCall.Latitude != null && dalCall.Longitude != null
               && dalCall.Latitude != 0 && dalCall.Longitude != 0;
    }

    public static async Task UpdateCoordinatesForVolunteerAsync( string address, BO.Volunteer? boVolunteer , DO.Volunteer? doVolunteer)
    {
        // חישוב אסינכרוני של הקואורדינטות
        if (doVolunteer==null)
       /* DO.Volunteer*/ doVolunteer = VolunteerManager.BOconvertDO(boVolunteer);
        if (address is not null)
        {
            double lat = await Tools.GetLatitudeAsync(address);
            double lot = await Tools.GetLongitudeAsync(address);
            if (address is not null)
            {
                doVolunteer = doVolunteer with { Latitude = lat, Longitude = lot };
                lock (AdminManager.BlMutex)
                    _dal.Volunteer.Update(doVolunteer);
                VolunteerManager.Observers.NotifyListUpdated();
                VolunteerManager.Observers.NotifyItemUpdated(doVolunteer.Id);
            }
        }

    }



}
