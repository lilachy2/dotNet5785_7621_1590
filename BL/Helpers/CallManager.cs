
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
            throw new BlInvalidaddress("Address cannot be null or empty.");
        }

        // Validate the address format using an external API.
        if (!Tools.IsAddressValidAsync(boCall.FullAddress).Result)
        {
            throw new BlInvalidaddress("The address is invalid.");
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
        //_ = UpdateCoordinatesForCallAsync(doVolunteer);
//        _ = VolunteerManager.UpdateCoordinatesForVolunteerAsync( doVolunteer.FullCurrentAddress, null, doVolunteer);

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
    public static async Task UpdateCoordinatesForCallAsync1(DO.Volunteer doVolunteer)
    {
        if (doVolunteer.FullCurrentAddress is not null)
        {
            // בדוק אם הכתובת תקינה בעולם לפני שמבצע את החישוב
            if (!await Tools.IsAddressValidAsync(doVolunteer.FullCurrentAddress).ConfigureAwait(false))
            //if (!await Task.Run(() => Tools.IsAddressValidAsync(doVolunteer.FullCurrentAddress)).ConfigureAwait(false))
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

    public static async Task UpdateCoordinatesForCallAsync(DO.Volunteer doVolunteer)
    {
        if (doVolunteer.FullCurrentAddress is not null)
        {
            Console.WriteLine($"Checking address validity: {doVolunteer.FullCurrentAddress}");


            try
            {
                // קריאה לפונקציה אסינכרונית עם CancellationToken
                if (!await Tools.IsAddressValidAsync(doVolunteer.FullCurrentAddress).ConfigureAwait(false))
                {
                    Console.WriteLine("The address is not valid in the real world.");
                    throw new BlInvalidaddress("The address is not valid in the real world.");
                }

                // חישוב הקואורדינטות (אסינכרונית)
                var coordinates = await Tools.GetCoordinatesAsync(doVolunteer.FullCurrentAddress).ConfigureAwait(false);

                if (coordinates.HasValue)
                {
                    Console.WriteLine($"Coordinates calculated: Latitude={coordinates.Value.Latitude}, Longitude={coordinates.Value.Longitude}");

                    // עדכון האובייקט עם הקואורדינטות החדשות
                    var updatedVolunteer = doVolunteer with
                    {
                        Latitude = coordinates.Value.Latitude,
                        Longitude = coordinates.Value.Longitude
                    };

                    lock (AdminManager.BlMutex)
                    {
                        _dal.Volunteer.Update(updatedVolunteer);
                        Console.WriteLine($"Volunteer updated in the database: {updatedVolunteer}");
                    }

                    // שליחת התראות על עדכון
                    Observers.NotifyListUpdated();
                    Observers.NotifyItemUpdated(updatedVolunteer.Id);
                    Console.WriteLine("Observers notified about the update.");
                }
                else
                {
                    Console.WriteLine($"Failed to calculate coordinates for address: {doVolunteer.FullCurrentAddress}");
                    throw new BlInvalidaddress("Failed to calculate coordinates for the address.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Address is null, no update performed.");
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
                Latitude = doCall.Latitude ??0, // Latitude coordinate of the address
                Longitude = doCall.Longitude??0, // Longitude coordinate of the address
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
        ? (doCall.MaxEndTime - _dal.Config.Clock).HasValue/* && (doCall.MaxEndTime - _dal.Config.Clock).Value.TotalMilliseconds >= 0*/
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




    ///////implapotiion
    ///
    public static void ChooseCall(int idVol, int idCall)
    {

        DO.Volunteer vol = null;
        BO.Call boCall = null;
        IEnumerable<Assignment> existingAssignments = null;

        lock (AdminManager.BlMutex) //stage 7

        // Retrieve volunteer and call; throw exception if not found.

        {
            vol = _dal.Volunteer.Read(idVol) ??
                         throw new BO.BlNullPropertyException($"There is no volunteer with this ID {idVol}");
            boCall = Read(idCall) ??
                            throw new BO.BlNullPropertyException($"There is no call with this ID {idCall}");
        }

        // Check if the call is open.
        if (boCall.Status != BO.CallStatus.Open && boCall.Status != BO.CallStatus.OpenAtRisk)
            throw new BO.BlAlreadyExistsException($"The call is not open or is already being handled. IdCall = {idCall}");

        lock (AdminManager.BlMutex)
            // Check if the call already has an open assignment.
            existingAssignments = _dal.Assignment.ReadAll()
                                    .Where(a => a.CallId == idCall && a.time_end_treatment == null)
                                    .ToList();

        if (existingAssignments.Any())
            throw new BO.BlAlreadyExistsException($"The call is already assigned to another volunteer. IdCall = {idCall}");

        // Check if the call has expired.
        if (boCall.Status == (BO.CallStatus.Expired))
            throw new BO.BlAlreadyExistsException($"The call has expired. IdCall = {idCall}");

        DO.Assignment assigmnetToCreat = null;
        lock (AdminManager.BlMutex)
    {        // Create a new assignment for the volunteer and the call.
            assigmnetToCreat = new DO.Assignment
            {
                Id = 0, // ID will be generated automatically
                CallId = idCall,
                VolunteerId = idVol,
                time_entry_treatment = AdminManager.Now,
                time_end_treatment = null,
                EndOfTime = null
            };
}
        try
        {
            lock (AdminManager.BlMutex)
                // Try to create the assignment in the database.
                _dal.Assignment.Create(assigmnetToCreat);
            //CallManager.Observers.NotifyItemUpdated(assigmnetToCreat.Id);  //stage 5
            // CallManager.Observers.NotifyListUpdated();  //stage 5

        }
        catch (Exception e)
        {
            // Handle error if creation fails.
            throw new BO.BlAlreadyExistsException("Impossible to create the assignment.");
        }


        CallManager.Observers.NotifyItemUpdated(idCall);  //stage 5
        CallManager.Observers.NotifyListUpdated();  //stage 5

        VolunteerManager.Observers.NotifyItemUpdated(idVol);  //stage 5
        VolunteerManager.Observers.NotifyListUpdated();  //stage 5


    }
    public static BO.Call Read(int callId)
    {
        try
        {
            lock (AdminManager.BlMutex) //stage 7
            {
                var call = _dal.Call.Read(callId);
                return CallManager.GetViewingCall(call.Id);
            }

        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID={callId} does not exist.", ex);
        }
    }

    public static void UpdateEndTreatment(int idVol, int idAssig)

    {

        DO.Assignment assignmentToClose = null;
        lock (AdminManager.BlMutex) //stage 7
            // Retrieve the assignment by its ID; throw an exception if not found.
            assignmentToClose = _dal.Assignment.Read(idAssig) ?? throw new BO.BlNullPropertyException("There is no assignment with this ID");

        // Check if the volunteer matches the one in the assignment; throw an exception if not.
        if (assignmentToClose.VolunteerId != idVol)
        {
            throw new BO.BlNullPropertyException("The volunteer is not treating in this assignment");
        }

        // Ensure the assignment is still open (not already closed); throw an exception if it is.
        if (assignmentToClose.EndOfTime != null || assignmentToClose.time_end_treatment != null)
            throw new BO.BlNullPropertyException("The assignment is not open");

        // Update the assignment to mark it as closed, setting end time and status.
        DO.Assignment assignmentToUP = new DO.Assignment
        {
            Id = assignmentToClose.Id,
            CallId = assignmentToClose.CallId,
            VolunteerId = assignmentToClose.VolunteerId,
            time_entry_treatment = assignmentToClose.time_entry_treatment,
            time_end_treatment = AdminManager.Now,
            EndOfTime = DO.AssignmentCompletionType.TreatedOnTime,
        };

        try
        {
            // Attempt to update the assignment in the database.
            lock (AdminManager.BlMutex) //stage 7
                _dal.Assignment.Update(assignmentToUP);

        }
        catch (DO.Incompatible_ID ex)
        {
            // Handle error if updating the assignment fails.
            throw new DO.Incompatible_ID("Cannot update in DO");
        }


        VolunteerManager.Observers.NotifyListUpdated();
        VolunteerManager.Observers.NotifyItemUpdated(idVol);

        //CallManager.Observers.NotifyItemUpdated(assignmentToClose.CallId);  //stage 5
        CallManager.Observers.NotifyItemUpdated(idAssig);  //stage 5
        CallManager.Observers.NotifyListUpdated();  //stage 5


    }

    public  static void UpdateCancelTreatment(int idVol, int idAssig)
    {

        DO.Assignment assigmnetToCancel = null;

        lock (AdminManager.BlMutex) //stage 7
            assigmnetToCancel = _dal.Assignment.Read(idAssig) ?? throw new BO.BlDeletionImpossibleException("there is no assigment with this ID");

        bool ismanager = false;

        if (assigmnetToCancel.VolunteerId != idVol)
        {
            lock (AdminManager.BlMutex) //stage 7
                if (_dal.Volunteer.Read(idVol).Role == DO.Role.Manager)
                    ismanager = true;
                else throw new BO.BlDeletionImpossibleException("the volunteer is not manager or not in this call");
        }
        if (assigmnetToCancel.time_end_treatment != null)//// לבדוק
            throw new BO.BlDeletionImpossibleException("The assigmnet not open or exspaired");

        DO.Assignment assigmnetToUP = new DO.Assignment
        {
            Id = assigmnetToCancel.Id,
            CallId = assigmnetToCancel.CallId,
            VolunteerId = assigmnetToCancel.VolunteerId,
            time_entry_treatment = assigmnetToCancel.time_entry_treatment,
            time_end_treatment = AdminManager.Now,
            EndOfTime = ismanager ? DO.AssignmentCompletionType.AdminCancelled : DO.AssignmentCompletionType.VolunteerCancelled,
        };

        try
        {
            lock (AdminManager.BlMutex) //stage 7
                _dal.Assignment.Update(assigmnetToUP);
        }

        catch (Exception ex)
        {
            throw new BO.BlDeletionImpossibleException("canot delete in DO");
        }



        VolunteerManager.Observers.NotifyListUpdated();
        VolunteerManager.Observers.NotifyItemUpdated(idVol);

        //CallManager.Observers.NotifyItemUpdated(assigmnetToCancel.CallId);  //stage 5
        CallManager.Observers.NotifyItemUpdated(idAssig);  //stage 5
        CallManager.Observers.NotifyListUpdated();  //stage 5

    }

    public static IEnumerable<BO.OpenCallInList> GetOpenCall(int id, BO.Calltype? type, BO.OpenCallInListEnum? sortBy)
    //public async Task<List<BO.OpenCallInList>> GetOpenCallAsync(int id, BO.Calltype? type, BO.OpenCallInListEnum? sortBy)

    {
        DO.Volunteer volunteer = null;
        IEnumerable<BO.CallInList> allCalls = null;
        IEnumerable<Assignment> allAssignments = null;

        lock (AdminManager.BlMutex) //stage 7
            volunteer = _dal.Volunteer.Read(id);
        if (volunteer == null)
            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist");

        // Retrieve all calls from the BO
        lock (AdminManager.BlMutex) //stage 7
        {
            allCalls = GetCallsList(null, null, null, null);

            // Retrieve all assignments from the DAL
            allAssignments = _dal.Assignment.ReadAll();
        }

        //double? lonVol = 
        //    UpdateCoordinatesForCallLON(volunteer.FullCurrentAddress).Result;
        //double? latVol =
        //    UpdateCoordinatesForCallLAN(volunteer.FullCurrentAddress).Result;

        //double? lonVol = Task.Run(() => UpdateCoordinatesForCallLON(volunteer.FullCurrentAddress)).Result;
        //double? latVol = Task.Run(() => UpdateCoordinatesForCallLAN(volunteer.FullCurrentAddress)).Result;

        // Filter for only "Open" or "Risk Open" status
        IEnumerable<BO.OpenCallInList> filteredCalls = allCalls
                .Where(call => call.Status == BO.CallStatus.Open || call.Status == BO.CallStatus.OpenAtRisk)  // Filter by Open or OpenAtRisk status
                .Select(call =>
                {
                    var boCall = Read(call.CallId);  // Get full details of the call
                    var assignment = allAssignments.FirstOrDefault(a => a.CallId == call.Id);  // Find the assignment if any

                    return new BO.OpenCallInList
                    {
                        Id = call.CallId,
                        CallType = call.CallType,
                        Description = boCall.Description,
                        FullAddress = boCall.FullAddress,
                        OpenTime = call.OpenTime,
                        MaxEndTime = boCall.MaxEndTime,
                        DistanceFromVolunteer = volunteer?.FullCurrentAddress != null
                       //     ? CallManager.Air_distance_between_2_addresses(latVol, lonVol, boCall.Latitude, boCall.Longitude)
                            ? CallManager.Air_distance_between_2_addresses(volunteer.Latitude, volunteer.Longitude, boCall.Latitude, boCall.Longitude)
                            : 0  // Calculate the distance between the volunteer and the call
                    };
                });

        // Filter by call type if provided
        if (type.HasValue && type != BO.Calltype.None)
        {
            filteredCalls = filteredCalls.Where(c => c.CallType == type.Value);
        }

        // Sort by the requested field or by default (call ID)
        if (sortBy.HasValue)
        {
            filteredCalls = sortBy.Value switch
            {
                BO.OpenCallInListEnum.Id => filteredCalls.OrderBy(c => c.Id),
                BO.OpenCallInListEnum.CallType => filteredCalls.OrderBy(c => c.CallType),
                BO.OpenCallInListEnum.Description => filteredCalls.OrderBy(c => c.Description),
                BO.OpenCallInListEnum.FullAddress => filteredCalls.OrderBy(c => c.FullAddress),
                BO.OpenCallInListEnum.OpenTime => filteredCalls.OrderBy(c => c.OpenTime),
                BO.OpenCallInListEnum.MaxEndTime => filteredCalls.OrderBy(c => c.MaxEndTime),
                BO.OpenCallInListEnum.DistanceFromVolunteer => filteredCalls.OrderBy(c => c.DistanceFromVolunteer),
                _ => filteredCalls.OrderBy(c => c.Id)
            };
        }
        else
        {
            filteredCalls = filteredCalls.OrderBy(c => c.Id);
        }

        return filteredCalls;

    }
    public static IEnumerable<BO.CallInList> GetCallsList(BO.Calltype? filter, object? obj, BO.CallInListField? sortBy, BO.CallStatus? statusFilter)
    {
        IEnumerable<DO.Call> calls = null;
        IEnumerable<BO.CallInList> boCallsInList = null;
        lock (AdminManager.BlMutex) //stage 7

        {
            calls = _dal.Call.ReadAll() ?? throw new BO.BlNullPropertyException("There are no calls in the database");
            //IEnumerable<BO.CallInList> boCallsInList = calls.Select(call => CallManager.GetCallInList(call));
            boCallsInList = _dal.Call.ReadAll().Select(call => CallManager.GetCallInList(call)).ToList();
        }

        // סינון לפי Calltype
        if (filter != null && obj != null)
        {
            switch (filter)
            {
                case BO.Calltype.allergy:
                    boCallsInList = boCallsInList.Where(item => item.CallType == (BO.Calltype)obj);
                    break;
                case BO.Calltype.birth:
                    boCallsInList = boCallsInList.Where(item => item.CallType == (BO.Calltype)obj);
                    break;
                case BO.Calltype.broken_bone:
                    boCallsInList = boCallsInList.Where(item => item.CallType == (BO.Calltype)obj);
                    break;
                case BO.Calltype.heartattack:
                    boCallsInList = boCallsInList.Where(item => item.CallType == (BO.Calltype)obj);
                    break;
                case BO.Calltype.resuscitation:
                    boCallsInList = boCallsInList.Where(item => item.CallType == (BO.Calltype)obj);
                    break;
                case BO.Calltype.security_event:
                    boCallsInList = boCallsInList.Where(item => item.CallType == (BO.Calltype)obj);
                    break;
                case BO.Calltype.None:
                    break;
            }
        }

        // סינון לפי CallStatus, אם מתקבל NoneToFilter לא מבצעים סינון
        if (statusFilter != BO.CallStatus.NoneToFilter && statusFilter != null)
        {
            boCallsInList = boCallsInList.Where(item => item.Status == statusFilter);
        }

        // אם לא הוגדר סינון לפי שדה, יש לסנן לפי CallId כברירת מחדל
        if (sortBy == null)
            sortBy = BO.CallInListField.CallId;

        // סינון לפי שדות
        switch (sortBy)
        {
            case BO.CallInListField.Id:
                boCallsInList = boCallsInList.OrderBy(item => item.Id.HasValue ? 0 : 1)
                                             .ThenBy(item => item.Id)
                                             .ToList();
                break;
            case BO.CallInListField.CallId:
                boCallsInList = boCallsInList.OrderBy(item => item.CallId).ToList();
                break;
            case BO.CallInListField.CallType:
                boCallsInList = boCallsInList.OrderBy(item => item.CallType).ToList();
                break;
            case BO.CallInListField.OpenTime:
                boCallsInList = boCallsInList.OrderBy(item => item.OpenTime).ToList();
                break;
            case BO.CallInListField.TimeRemaining:
                boCallsInList = boCallsInList.OrderBy(item => item.TimeRemaining).ToList();
                break;
            case BO.CallInListField.VolunteerName:
                boCallsInList = boCallsInList.OrderBy(item => item.VolunteerName).ToList();
                break;
            case BO.CallInListField.CompletionTime:
                boCallsInList = boCallsInList.OrderBy(item => item.CompletionTime).ToList();
                break;
            case BO.CallInListField.Status:
                boCallsInList = boCallsInList.OrderBy(item => item.Status).ToList();
                break;
            case BO.CallInListField.TotalAssignments:
                boCallsInList = boCallsInList.OrderBy(item => item.TotalAssignments).ToList();
                break;
        }

        return boCallsInList;

    }

    public static async Task<double> UpdateCoordinatesForCallLON(string adress)
    {
        try
        {
            // Update the call in the data layer with the new coordinates
            double lon = await Tools.GetLongitudeAsync(adress);
            VolunteerManager.Observers.NotifyListUpdated();
            return lon;
        }
        catch (Exception ex)
        {
            // Handle exceptions related to coordinate updates
            throw new BO.BlGeneralException("Failed to update coordinates for the call.", ex);
        }
    }

    public static async Task<double> UpdateCoordinatesForCallLAN(string adress)
    {
        try
        {
            // Update the call in the data layer with the new coordinates
            double lan = await Tools.GetLatitudeAsync(adress);
            VolunteerManager.Observers.NotifyListUpdated();
            return lan;
        }
        catch (Exception ex)
        {
            // Handle exceptions related to coordinate updates
            throw new BO.BlGeneralException("Failed to update coordinates for the call.", ex);
        }
    }



}































