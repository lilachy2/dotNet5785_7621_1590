
using BlApi;
using BO;
using DalApi;
using DO;
using Microsoft.VisualBasic;

using Dal;
using System;

namespace Helpers;

internal static class CallManager
{

    private static IDal _dal = DalApi.Factory.Get; //stage 4
    internal static ObserverManager Observers = new(); //stage 5 
    internal static void IsLogicCall(BO.Call boCall)
    {
        // Ensure MaxEndTime is greater than OpenTime.
        if (boCall.MaxEndTime.HasValue && boCall.MaxEndTime.Value <= boCall.OpenTime)
        {
            throw new BlIsLogicCallException("MaxEndTime must be greater than OpenTime.");
        }

        // Check that CallType is valid (assuming enums start at 0).
        if (!Enum.IsDefined(typeof(BO.Calltype), boCall.Calltype))
        {
            throw new BlIsLogicCallException("Invalid call type.");
        }

        // Validate the status (assuming statuses start at 0).
        if (!Enum.IsDefined(typeof(BO.CallStatus), boCall.Status))
        {
            throw new BlIsLogicCallException("Invalid call status.");
        }

    }
    internal static void IsValideCall(BO.Call boCall)
    {
        // Validate that the ID is positive.
        if (boCall.Id <= 0)
        {
            throw new ArgumentException("Call ID must be a positive integer.");
        }

        // Validate that the description is not null or empty.
        if (string.IsNullOrWhiteSpace(boCall.Description))
        {
            throw new ArgumentException("Description cannot be null or empty.");
        }

        // Validate that the latitude is within valid range (-90 to 90).
        if (boCall.Latitude != null && (boCall.Latitude < -90 || boCall.Latitude > 90))
        {
            throw new ArgumentException("Latitude must be between -90 and 90.");
        }

        // Validate that the longitude is within valid range (-180 to 180).
        if (boCall.Longitude != null && (boCall.Longitude < -180 || boCall.Longitude > 180))
        {
            throw new ArgumentException("Longitude must be between -180 and 180.");
        }

        // Validate that the address is not null or empty.
        if (string.IsNullOrWhiteSpace(boCall.FullAddress))
        {
            throw new ArgumentException("Address cannot be null or empty.");
        }

        // Validate the address format using an external API.
        if (!Tools.IsAddressValidAsync(boCall.FullAddress).Result)
        {
            throw new ArgumentException("The address is invalid.");
        }
    }

    //CreateCallInProgress and 3 helper methods


    public static BO.CallInProgress GetCallInProgress(int VolunteerId)
    {

        lock (AdminManager.BlMutex) //stage 7
{
            DO.Volunteer? doVolunteer = _dal.Volunteer.Read(VolunteerId) ?? throw new BlDoesNotExistException("eroor id");// ז

        //Find the appropriate CALL  and  Assignmentn by volunteer ID
        //var doAssignment = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == VolunteerId /*&& a.EndOfTime == null*/).FirstOrDefault();

        //var doAssignment = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == VolunteerId /*&& a.EndOfTime == null*/).FirstOrDefault();
        var doAssignment = _dal.Assignment.ReadAll()
                    .Where(a => a.VolunteerId == VolunteerId)
                    .OrderByDescending(a => a.Id)
                    .FirstOrDefault();


        if (doAssignment == null)
        {
            return null;
        }
        var doCall = _dal.Call.ReadAll().Where(c => c.Id == doAssignment!.CallId).FirstOrDefault();
        // בודק האם הכתובת אמיתית ומחזיר קווי אורך ורוחב עבור הכתובת 

        double LatitudeVolunteer ;
        double LongitudeVolunteer ;
            //if (Task.Run(() => Tools.IsAddressValidAsync(doVolunteer.FullCurrentAddress)).Result == true)//  כתובת אמיתית 
            //{
            //    LongitudeVolunteer = Task.Run(() => Tools.GetLongitudeAsync(doVolunteer.FullCurrentAddress)).Result;
            //    Thread.Sleep(3000);
            //    //LongitudeVolunteer = Task.Run(() => Tools.GetLatitude(doVolunteer.FullCurrentAddress));
            //    LatitudeVolunteer = Task.Run(() => Tools.GetLongitudeAsync(doVolunteer.FullCurrentAddress)).Result;
            //}
            bool isValidAddress =  Tools.IsAddressValidAsync(doVolunteer.FullCurrentAddress).Result;

            if (isValidAddress)
            {
                LongitudeVolunteer =  Tools.GetLongitudeAsync(doVolunteer.FullCurrentAddress).Result;
                LatitudeVolunteer =  Tools.GetLatitudeAsync(doVolunteer.FullCurrentAddress).Result;
            }

            else
            throw new BlInvalidaddress("Invalid address of Volunteer");
            // Ensure latitude and longitude are valid
            //double volunteerLatitude = doVolunteer.Latitude ?? Tools.GetLatitudeAsync(doVolunteer.FullCurrentAddress).Result;
            //double volunteerLongitude = doVolunteer.Longitude ?? Tools.GetLongitudeAsync(doVolunteer.FullCurrentAddress).Result;
            //double? LatitudeVolunteer = Tools.GetLatitudeAsync(doVolunteer.FullCurrentAddress).Result;
            //double? LongitudeVolunteer = Tools.GetLongitudeAsync(doVolunteer.FullCurrentAddress).Result;
            //double? LatitudeVolunteer1 = Task.Run(() => Tools.GetLatitudeAsync(doVolunteer.FullCurrentAddress)).Result;
            //double? LongitudeVolunteer = Task.Run(() => Tools.GetLongitudeAsync(doVolunteer.FullCurrentAddress)).Result;

            //if (CalculateCallStatus(doCall) == CallStatus.Open|| CalculateCallStatus(doCall) ==CallStatus.OpenAtRisk)// status open
            var callStatus = CalculateCallStatus(doCall);
            if ((callStatus == CallStatus.InProgressAtRisk) || (callStatus == CallStatus.InProgress))// status open
                return new BO.CallInProgress

                {
                    Id = doAssignment.Id,
                    CallId = doAssignment.CallId,
                    CallType = (BO.Calltype)doCall.Calltype,
                    Description = doCall.VerbalDescription,
                    FullAddress = doCall.ReadAddress,
                    OpenTime = doCall.OpeningTime,
                    MaxCompletionTime = doCall.MaxEndTime,
                    EnterTime = doAssignment.time_entry_treatment,
                    DistanceFromVolunteer = Air_distance_between_2_addresses(doCall.Latitude, doCall.Longitude, LatitudeVolunteer, LongitudeVolunteer),// Air distance between 2 addresses
                    Status = callStatus

                };
            else
                return null;// not found call open
}
    }

    public static async Task<BO.CallInProgress?> GetCallInProgressAsync(int VolunteerId)
    {
        DO.Volunteer? doVolunteer = null;
        DO.Assignment? doAssignment = null;
        DO.Call? doCall = null;

        // קריאה סינכרונית לבדוק אם המתנדב קיים
        lock (AdminManager.BlMutex)
        {
            doVolunteer = _dal.Volunteer.Read(VolunteerId)
                ?? throw new BlDoesNotExistException("Volunteer with the specified ID does not exist.");
        }

        // קריאה סינכרונית לקרוא את ההקצאות של המתנדב
        lock (AdminManager.BlMutex)
        {
            doAssignment = _dal.Assignment.ReadAll()
                .Where(a => a.VolunteerId == VolunteerId)
                .OrderByDescending(a => a.Id)
                .FirstOrDefault();
        }

        if (doAssignment == null)
            return null;

        // קריאה סינכרונית לקרוא את הקריאה
        lock (AdminManager.BlMutex)
        {
            doCall = _dal.Call.ReadAll().FirstOrDefault(c => c.Id == doAssignment!.CallId);
        }

        // לא לחכות בתוך הלוק, אלא לעדכן את הקואורדינטות בצורה אסינכרונית
        _ = UpdateCoordinatesForCallAsync(doVolunteer);

        // ביצוע בדיקות אחרות בצורה סינכרונית
        BO.CallStatus callStatus = CalculateCallStatus(doCall);

        if (callStatus == BO.CallStatus.InProgressAtRisk || callStatus == BO.CallStatus.InProgress)
        {
            return new BO.CallInProgress
            {
                Id = doAssignment.Id,
                CallId = doAssignment.CallId,
                CallType = (BO.Calltype)doCall.Calltype,
                Description = doCall.VerbalDescription,
                FullAddress = doCall.ReadAddress,
                OpenTime = doCall.OpeningTime,
                MaxCompletionTime = doCall.MaxEndTime,
                EnterTime = doAssignment.time_entry_treatment,
                DistanceFromVolunteer = Air_distance_between_2_addresses(doCall.Latitude, doCall.Longitude, doVolunteer.Latitude, doVolunteer.Longitude),
                Status = callStatus
            };
        }
        else
        {
            return null;
        }
    }

    // פונקציה אסינכרונית לעדכון הקואורדינטות של המתנדב
    public static async Task UpdateCoordinatesForCallAsync(DO.Volunteer doVolunteer)
    {
        if (doVolunteer.FullCurrentAddress is not null)
        {
            // בדוק אם הכתובת תקינה בעולם לפני שמבצע את החישוב
            if (!await Tools.IsAddressValidAsync(doVolunteer.FullCurrentAddress).ConfigureAwait(false))
            {
                throw new BlInvalidaddress("The address is not valid in the real world.");
            }

            // אם הכתובת תקינה, אז חישוב הקואורדינטות
            var coordinates = await Tools.GetCoordinatesAsync(doVolunteer.FullCurrentAddress);
            if (coordinates.HasValue)
            {
                doVolunteer = doVolunteer with { Latitude = coordinates.Value.Latitude, Longitude = coordinates.Value.Longitude };

                lock (AdminManager.BlMutex)
                {
                    _dal.Volunteer.Update(doVolunteer);
                }

                // התראות עדכון
                Observers.NotifyListUpdated();
                Observers.NotifyItemUpdated(doVolunteer.Id);
            }
            else
            {
                throw new BlInvalidaddress("Failed to calculate coordinates for the address.");
            }
        }
    }









    private const double EarthRadiusKm = 6371.0; // Earth's radius in kilometers

    /// <summary>
    /// Calculates the air (great-circle) distance between two geographic points based on their latitude and longitude coordinates.
    /// </summary>
    /// <param name="lat1">Latitude of the first point</param>
    /// <param name="lon1">Longitude of the first point</param>
    /// <param name="lat2">Latitude of the second point</param>
    /// <param name="lon2">Longitude of the second point</param>
    /// <returns>The air distance in kilometers</returns>
    public static double Air_distance_between_2_addresses(double? lat1, double? lon1, double? lat2, double? lon2)
    {
        // Check if any of the coordinates is null
        if (!lat1.HasValue || !lon1.HasValue || !lat2.HasValue || !lon2.HasValue)
        {
            throw new ArgumentNullException("One or more coordinate values are null.");
        }
        // Convert degrees to radians

        double lat1Rad = DegreesToRadians(lat1.Value);
        double lon1Rad = DegreesToRadians(lon1.Value);
        double lat2Rad = DegreesToRadians(lat2.Value);
        double lon2Rad = DegreesToRadians(lon2.Value);

        // Calculate coordinate differences
        double dLat = lat2Rad - lat1Rad;
        double dLon = lon2Rad - lon1Rad;

        // Compute the haversine formula
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        // Calculate the distance
        return EarthRadiusKm * c;
    }

    /// <summary>
    /// Converts degrees to radians.
    /// </summary>
    /// <param name="degrees">The angle in degrees</param>
    /// <returns>The angle in radians</returns>
    private static double DegreesToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }
    public static bool IsInRisk(DO.Call call)
    { //stage 7
        lock (AdminManager.BlMutex) //stage 7
            return call.MaxEndTime - _dal.Config.Clock <= _dal.Config.RiskRange;
    }   
    internal static BO.CallStatus CalculateCallStatus(DO.Call doCall)
    {
        lock (AdminManager.BlMutex) //stage 7

        // 1. Check if the call's maximum allowed time has expired
        if (doCall.MaxEndTime < _dal.Config.Clock)
            return BO.CallStatus.Expired;

        //// 2. Retrieve the latest assignment related to the call
        //var /*last*/Assignment = _dal.Assignment.ReadAll(ass => ass.CallId == doCall.Id)
        //                                     .OrderByDescending(a => a.time_entry_treatment)
        //                                     .FirstOrDefault();

        // עושה בעיות עם סיום טיפול
        //var lastAssignment = _dal.Assignment.ReadAll(ass => ass.CallId == doCall.Id && ass.time_end_treatment == null)
        //                             .OrderByDescending(a => a.time_entry_treatment)
        //                             .FirstOrDefault();

        lock (AdminManager.BlMutex) //stage 7
        {
            var lastAssignment = _dal.Assignment.ReadAll(ass => ass.CallId == doCall.Id).OrderByDescending(a => a.Id).FirstOrDefault();

            // 3. If no assignments exist
            if (lastAssignment == null)
            {
                if (IsInRisk(doCall))
                    return BO.CallStatus.OpenAtRisk; // Open but in risk
                else
                    return BO.CallStatus.Open; // Open and not in risk
            }

            if ((lastAssignment.time_end_treatment != null) && (lastAssignment.EndOfTime == DO.AssignmentCompletionType.TreatedOnTime))
            {
                return BO.CallStatus.Closed; // Closed successfully
            }

            // 5. If the call is in progress but treatment has not ended
            if (lastAssignment.time_entry_treatment != null && lastAssignment.time_end_treatment == null)
            {
                if (IsInRisk(doCall))
                    return BO.CallStatus.InProgressAtRisk; // In progress and in risk
                else
                    return BO.CallStatus.InProgress; // In progress but not in risk
            }
            if (lastAssignment.EndOfTime.ToString() == "SelfCancel" || lastAssignment.EndOfTime.ToString() == "ManagerCancel")
            {

                return BO.CallStatus.Open;

            }

        }
        // 6. Default status - Open
        return BO.CallStatus.Open;


    }

 // CALL - function for viewing, function that checks for correctness (add and update) + helper method
    // helper method- 1- GetCallAssignmentsForCall 2- CalculateCallStatus
    public static BO.Call GetViewingCall(int CallId)
    {
        // DO.Volunteer? doVolunteer = _dal.Volunteer.Read(VolunteerId) ?? throw new BlDoesNotExistException("eroor id");// ז

        //Find the appropriate CALL  and  Assignmentn by volunteer ID
        // var doAssignment = _dal.Assignment.ReadAll().Where(a => a.CallId == CallId /*&& a.EndOfTime == null*/).FirstOrDefault();
        lock (AdminManager.BlMutex) //stage 7

     {       var doAssignment = _dal.Assignment.ReadAll(a => a.CallId == CallId).FirstOrDefault();
        var doCall = _dal.Call.ReadAll().Where(c => c.Id ==/* doAssignment!.*/CallId).FirstOrDefault();


            // Create the object
            return new BO.Call
            {

                Id = doCall.Id, // Call identifier
                Calltype = (BO.Calltype)doCall.Calltype, // Enum conversion
                Description = doCall.VerbalDescription,
                FullAddress = doCall.ReadAddress, // Full address of the call
                Latitude = (double)doCall.Latitude, // Latitude coordinate of the address
                Longitude = (double)doCall.Longitude, // Longitude coordinate of the address
                OpenTime = doCall.OpeningTime, // Time when the call was opened
                MaxEndTime = doCall.MaxEndTime, // Maximum completion time for the call
                Status = CalculateCallStatus(doCall), // Current status of the call
                CallAssignments = CallManager.GetCallAssignmentsForCall(doCall.Id),

            };
        }
    }
    public static List<BO.CallAssignInList> GetCallAssignmentsForCall(int callId)
    {
        // For the CallAssignments field in the GetViewingCall function
        // Search for all assignments related to the given call
        lock (AdminManager.BlMutex) //stage 7

{            var doAssignments = _dal.Assignment.ReadAll().Where(a => a.CallId == callId).ToList();

        // If no assignments are found, return null
        if (doAssignments == null)
        {
            return null; // No assignments found
        }

        // Use LINQ to convert the assignments into BO.CallAssignInList using the GetCallAssignInList function
        var callAssignInList = (from doAssignment in doAssignments
                                let doVolunteer = _dal.Volunteer.Read(doAssignment.VolunteerId)
                                where doVolunteer != null
                                select GetCallAssignInList(doAssignment.VolunteerId)) // Calls the conversion function
                                .ToList();

            // Return the complete list
            return callAssignInList;
        }
    }

    // CALL - function for Add or update
    public static BO.Call GetAdd_update_Call(int VolunteerId)
    {
        DO.Volunteer? doVolunteer = _dal.Volunteer.Read(VolunteerId) ?? throw new BlDoesNotExistException("eroor id");// ז

        //Find the appropriate CALL  and  Assignmentn by volunteer ID
        lock (AdminManager.BlMutex) //stage 7

      {      var doAssignment = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == VolunteerId /*&& a.EndOfTime == null*/).FirstOrDefault();
        var doCall = _dal.Call.ReadAll().Where(c => c.Id == doAssignment!.CallId).FirstOrDefault();

        // logic chack
        if (Tools.IsAddressValidAsync(doCall.ReadAddress).Result==false)
            throw new BlInvalidaddress($"The address = {doCall.ReadAddress}provided is invalid.");
        MaxEndTimeCheck(doCall.MaxEndTime, doCall.OpeningTime);// If not good throw an exception from within the method


            // Create the object
            return new BO.Call
            {

                Id = doCall.Id, // Call identifier
                Calltype = (BO.Calltype)doCall.Calltype, // Enum conversion
                Description = doCall.VerbalDescription,
                FullAddress = doCall.ReadAddress, // Full address that chack above 
                Latitude = Tools.GetLatitudeAsync(doCall.ReadAddress).Result, // Latitude coordinate of the address
                Longitude = Tools.GetLongitudeAsync(doCall.ReadAddress).Result, // Longitude coordinate of the addres

                OpenTime = doCall.OpeningTime, // Time when the call was opened
                MaxEndTime = doCall.MaxEndTime, // Maximum completion time for the call
                Status = CalculateCallStatus(doCall), // Current status of the call
                CallAssignments = CallManager.GetCallAssignmentsForCall(doCall.Id),

            };
        }
    }

    public static void MaxEndTimeCheck(DateTime? MaxEndTime, DateTime OpeningTime)
    {
        if (MaxEndTime < OpeningTime || MaxEndTime < AdminManager.Now)
        {
            throw new BlMaximum_time_to_finish_readingException("The time entered according to the current time or opening time");
        }
    }

    // GetCallAssignInList
    public static BO.CallAssignInList GetCallAssignInList(int Id)
    {
        lock (AdminManager.BlMutex) //stage 7

        {    DO.Volunteer? doVolunteer = _dal.Volunteer.Read(Id) ?? throw new BlDoesNotExistException("eroor id");// ז

        //Find the appropriate CALL  and  Assignmentn by volunteer ID
        var doAssignment = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == Id ).FirstOrDefault();


        if (doAssignment == null)
        {
            return null;
        }


            return new BO.CallAssignInList
            {
                VolunteerId = doAssignment.VolunteerId, // The ID of the volunteer
                VolunteerName = doVolunteer.Name, // The name of the volunteer (helper method can be used)
                EnterTime = doAssignment.time_entry_treatment, // The time the volunteer started handling the call
                CompletionTime = doAssignment.time_end_treatment, // The time the handling of the call was completed
                CompletionStatus = (BO.CallAssignmentEnum?)doAssignment.EndOfTime // Completion status (nullable)
            };
}


    }

    public static BO.CallInList GetCallInList(DO.Call doCall)
    {
        lock (AdminManager.BlMutex) //stage 7

        {
            var assignmentsForCall = _dal.Assignment.ReadAll(a => a.CallId == doCall.Id) ?? Enumerable.Empty<DO.Assignment>();

            var lastAssignmentsForCall = assignmentsForCall
         .OrderByDescending(item => item.time_end_treatment)  // מיון לפי time_entry_treatment בסדר יורד
         .LastOrDefault();  // לוקחים את האחרון אחרי המיון


            // בדיקה אם הקריאה ל-Read מחזירה null
            var volunteer = (lastAssignmentsForCall != null) ? _dal.Volunteer.Read(lastAssignmentsForCall.VolunteerId) : null;

            // var status = CalculateCallStatus(doCall);   
            var callinlist = new CallInList()
            {
                Id = (lastAssignmentsForCall == null) ? null : lastAssignmentsForCall.Id,
                CallId = doCall.Id,
                CallType = (BO.Calltype)doCall.Calltype,
                OpenTime = doCall.OpeningTime,
                //TimeRemaining = doCall.MaxEndTime != null ? doCall.MaxEndTime - _dal.Config.Clock : null,
                TimeRemaining = doCall.MaxEndTime != null
        ? (doCall.MaxEndTime - _dal.Config.Clock).HasValue && (doCall.MaxEndTime - _dal.Config.Clock).Value.TotalMilliseconds >= 0
            ? doCall.MaxEndTime - _dal.Config.Clock
            : null
        : null,

                VolunteerName = (volunteer != null) ? volunteer.Name : null,
                //CompletionTime = (lastAssignmentsForCall != null && lastAssignmentsForCall.time_end_treatment != null && lastAssignmentsForCall.time_entry_treatment != null)
                //                 ? lastAssignmentsForCall.time_end_treatment - lastAssignmentsForCall.time_entry_treatment
                //                 : null, 
                CompletionTime = (lastAssignmentsForCall != null && lastAssignmentsForCall.time_end_treatment != null && lastAssignmentsForCall.time_entry_treatment != null)
                                 ? lastAssignmentsForCall.time_end_treatment - lastAssignmentsForCall.time_entry_treatment
                                 : null,
                Status = CallManager.CalculateCallStatus(doCall),
                TotalAssignments = assignmentsForCall.Count()
            };

            return callinlist;

        }
    }

    // Convert 
    public static DO.Call BOConvertDO_Call(int Id)
    {
        lock (AdminManager.BlMutex) //stage 7

     {       // Retrieve the DO.Call object using the provided ID
            var BOCall = _dal.Call.Read(Id);
        if (BOCall == null)
        {
            throw new BO.Incompatible_ID($"Call with ID {Id} not found.");
        }
        // Convert DO.Call to BO.Call
        var DOCall = new DO.Call
        {
            Id = BOCall.Id,
            Calltype = (DO.Calltype)BOCall.Calltype, // Explicit cast to BO.Calltype enum
            VerbalDescription = BOCall.VerbalDescription,
            ReadAddress = BOCall.ReadAddress,
            Latitude = BOCall.Latitude ?? 0, // Convert nullable to non-nullable
            Longitude = BOCall.Longitude ?? 0, // Convert nullable to non-nullable
            OpeningTime = BOCall.OpeningTime,
            MaxEndTime = BOCall.MaxEndTime
        };

            return DOCall;
        }
    }
    public static TimeSpan? CalculateTimeRemaining(DateTime? maxEndTime)
    {
        if (maxEndTime == null)
            return null;

        return maxEndTime - AdminManager.Now;
    }
    public static string? GetLatestVolunteerNameForCall(int callId)
    {
        lock (AdminManager.BlMutex) //stage 7

          {  // Retrieve all assignments related to the call
            var assignments = _dal.Assignment.ReadAll().Where(a => a.CallId == callId);

        if (!assignments.Any())
            return null;

        // Find the assignment with the latest entry time closest to the current system time
        var latestAssignment = assignments
            .OrderByDescending(a => a.time_entry_treatment)
            .FirstOrDefault();

        if (latestAssignment == null || latestAssignment.VolunteerId == null)
            return null;

        // Retrieve the volunteer associated with the assignment
        var volunteer = _dal.Volunteer.Read((int)latestAssignment.VolunteerId);
            return volunteer?.Name;
        }
    }
    public static TimeSpan? CalculateCompletionTime(int callId)
    {
        lock (AdminManager.BlMutex) //stage 7
{
            // Retrieve all assignments related to the call
            var assignments = _dal.Assignment.ReadAll().Where(a => a.CallId == callId);

        // Check if the call has been completed
        var completedAssignment = assignments
            .Where(a => a.EndOfTime != null && a.time_end_treatment != null)
            .OrderByDescending(a => a.time_end_treatment)
            .FirstOrDefault();

        if (completedAssignment == null)
            return null;

            // Calculate the time taken to complete the call
            return completedAssignment.time_end_treatment - completedAssignment.time_entry_treatment;
        }
    }
    public static void UpdateCallsToExpired(DateTime oldClock, DateTime newClock)
    {
        List<DO.Assignment> assignmentsToUpdate;
        List<DO.Assignment> updatedAssignments;
        List<int> callIdsToUpdate;
        List<DO.Call> callsToUpdate;

        // נעילה עבור קריאה ל-DAL כדי לקרוא את כל ההקצאות שדורשות עדכון
        lock (AdminManager.BlMutex)
        {
            assignmentsToUpdate = _dal.Assignment.ReadAll()
                .Where(a => a.EndOfTime == null && a.time_entry_treatment <= newClock && a.CallId != 0)
                .ToList();
        }

        // יצירת רשימת הקצאות מעודכנות
        updatedAssignments = assignmentsToUpdate
            .Select(a => new DO.Assignment
            {
                Id = a.Id,
                CallId = a.CallId,
                VolunteerId = a.VolunteerId,
                time_entry_treatment = a.time_entry_treatment,
                time_end_treatment = newClock, // עדכון זמן סיום
                EndOfTime = DO.AssignmentCompletionType.Expired // סטטוס "פג תוקף"
            }).ToList();

        // נעילה לעדכון ההקצאות ב-DAL
        lock (AdminManager.BlMutex)
        {
            updatedAssignments.ForEach(a => _dal.Assignment.Update(a));
        }

        // יצירת רשימת מזהי השיחות שדורשות עדכון
        callIdsToUpdate = updatedAssignments.Select(a => a.CallId).Distinct().ToList();

        // נעילה לקריאת כל השיחות שדורשות עדכון
        lock (AdminManager.BlMutex)
        {
            callsToUpdate = _dal.Call.ReadAll()
                .Where(c => callIdsToUpdate.Contains(c.Id))
                .ToList();
        }

        // עדכון השיחות ב-DAL
        callsToUpdate.ForEach(call =>
        {
            lock (AdminManager.BlMutex)
            {
                _dal.Call.Update(call); // עדכון השיחה בשכבת הנתונים
            }
            Observers.NotifyItemUpdated(call.Id); // הודעה למשקיפים מחוץ ל-lock
        });

        // הודעה שהרשימה עודכנה
        Observers.NotifyListUpdated(); // הודעה מחוץ ל-lock
    }


}































