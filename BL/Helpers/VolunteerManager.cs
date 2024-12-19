
using BO;
using DalApi;
using DO;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;

namespace Helpers;

internal static class VolunteerManager
{
    private static IDal _dal = Factory.Get; //stage 4
    internal static void CheckFormat(BO.Volunteer boVolunteer)
    {
        try
        {
            CheckPhonnumber(boVolunteer.Number_phone);
            CheckEmail(boVolunteer.Email);
            Tools.IsAddressValid(boVolunteer.FullCurrentAddress);
        }
        catch (BO.BlWrongItemtException ex)
        {
            throw new BO.BlWrongItemtException($"the item have logic problem", ex);
        }

    }


    internal static void CheckLogic(BO.Volunteer boVolunteer, BO.Volunteer existingVolunteer, bool isManager)
    {
        // Validate ID
        if (!IsValidIsraeliID(boVolunteer.Id))
        {
            throw new BO.Incompatible_ID("Invalid ID: The ID does not pass validation.");
        }

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

        // Check if the active status has changed - only a manager can change the active status
        if (boVolunteer.Active != existingVolunteer.Active)
        {
            if (requesterRole != DO.Role.Manager)
            {
                throw new BO.BlPermissionException("Only a manager is authorized to change the active status.");
            }
        }

        // Add additional checks if there are any other restricted fields
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
            throw new ArgumentException("PhoneNumber must be a 10-digit number starting with 0.");
        }
    }
    
    internal static void CheckEmail(string Email)
    {
        if (!Regex.IsMatch(Email, @"^(?("")(""[^""]+?""@)|(([0-9a-zA-Z](([\.\-]?)(?![\.\-])))*[0-9a-zA-Z]@))([0-9a-zA-Z][\-0-9a-zA-Z]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,}$"))
        {
            throw new ArgumentException("Invalid Email format.");
        }

    }

    public static BO.Volunteer GetVolunteer(int id)
    {
        DO.Volunteer? doVolunteer = _dal.Volunteer.Read(id) ?? throw new BlDoesNotExistException("eroor id");// ז
        if (CallManager.GetCallInProgress(doVolunteer.Id) == null)
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
                CurrentCall =null
              


            };
        else
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
                CurrentCall = CallManager.GetCallInProgress(doVolunteer.Id), //CallAssignInProgress  בפונקצית עזר של 



            };
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

    // GetVolunteerInList and helper methods for each field
    //public static BO.VolunteerInList GetVolunteerInList(int VolunteerId)
    //{


    //    DO.Volunteer? doVolunteer = _dal.Volunteer.Read(VolunteerId) ?? throw new BlDoesNotExistException("eroor id");// ז

    //    Find the appropriate CALL  and Assignmentn by volunteer ID
    //    var doAssignment = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == VolunteerId && a.EndOfTime == null).FirstOrDefault();
    //    var doCall = _dal.Call.ReadAll().Where(c => c.Id == doAssignment!.CallId).FirstOrDefault();



    //    return new BO.VolunteerInList
    //    {
    //        Id = doVolunteer.Id,
    //        FullName = doVolunteer.Name,
    //        IsActive = doVolunteer.Active,
    //        TotalCallsHandled = Tools.TotalHandledCalls(VolunteerId),
    //        TotalCallsCancelled = Tools.TotalCallsCancelledhelp(VolunteerId),
    //        TotalCallsExpired = Tools.TotalCallsExpiredelo(VolunteerId),
    //        CurrentCallId = Tools.CurrentCallIdhelp(VolunteerId),
    //        CurrentCallType = Tools.CurrentCallType(VolunteerId)


    //    };
    //}


    public static BO.VolunteerInList GetVolunteerInList(int VolunteerId)
    {


        DO.Volunteer? doVolunteer = _dal.Volunteer.Read(VolunteerId) ?? throw new BlDoesNotExistException("eroor id");// ז

        //Find the appropriate CALL  and  Assignmentn by volunteer ID
        var doAssignment = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == VolunteerId && a.EndOfTime == null).FirstOrDefault();
        // var doCall = _dal.Call.ReadAll().Where(c => c.Id == doAssignment!.CallId).FirstOrDefault();

        var calls = _dal.Assignment.ReadAll(ass => ass.VolunteerId == VolunteerId);

        //int totalCallsHandled = calls.Count(ass => ass.TypeEndTreat == DO.TypeEnd.Treated);
        //int totalCallsCanceled = calls.Count(ass => ass.TypeEndTreat == DO.TypeEnd.SelfCancel);
        //int totalCallsExpired = calls.Count(ass => ass.TypeEndTreat == DO.TypeEnd.ExpiredCancel);
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


    // GetClosedCallInList 
    public static BO.ClosedCallInList GetClosedCallInList(int VolunteerId)
    {

        DO.Volunteer? doVolunteer = _dal.Volunteer.Read(VolunteerId) ?? throw new BlDoesNotExistException("eroor id");// ז

        //Find the appropriate CALL  and  Assignmentn by volunteer ID
        var doAssignment = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == VolunteerId && a.EndOfTime == null).FirstOrDefault();
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

    //GetOpenCallInList
    public static BO.OpenCallInList GetOpenCallInList(int VolunteerId)
    {
        DO.Volunteer? doVolunteer = _dal.Volunteer.Read(VolunteerId) ?? throw new BlDoesNotExistException("eroor id");// ז

        //Find the appropriate CALL  and  Assignmentn by volunteer ID
        var doAssignment = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == VolunteerId && a.EndOfTime == null).FirstOrDefault();
        var doCall = _dal.Call.ReadAll().Where(c => c.Id == doAssignment!.CallId).FirstOrDefault();

        if (Tools.IsAddressValid(doVolunteer.FullCurrentAddress)/*.Result */== false)// לא כתובת אמיתית 
        {
            throw new BlInvalidaddress("Invalid address of Volunteer");// 
        }
        double? LatitudeVolunteer = Tools.GetLatitudeAsync(doVolunteer.FullCurrentAddress).Result;
        double? LongitudeVolunteer = Tools.GetLongitudeAsync(doVolunteer.FullCurrentAddress).Result;

        // Create the object
        return new BO.OpenCallInList
        {
            Id = doAssignment.Id, // Call identifier
            CallType = (BO.Calltype)doCall.Calltype, // Enum conversion
            FullAddress = doCall.ReadAddress, // Full address of the call
            OpenTime = doCall.OpeningTime, // Time when the call was opened
            MaxEndTime = doCall.MaxEndTime,
            DistanceFromVolunteer = CallManager.Air_distance_between_2_addresses(doCall.Latitude, doCall.Longitude, LatitudeVolunteer, LongitudeVolunteer),// Air distance between 2 addresses

        };

    }





}
