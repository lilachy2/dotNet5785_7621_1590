
namespace BlImplementation;
using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;
using System;
using System.Collections.Generic;

internal class CallImplementation : BlApi.ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;


    #region Stage 5
    public void AddObserver(Action listObserver) =>
    CallManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
     CallManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
      CallManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
      CallManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5

    public int[] GetCallStatusesCounts()
    {
        try
        {
            // Fetching calls from the data layer
            var doCalls = _dal.Call.ReadAll();

            // Converting the calls from DO to BO using your function
            var boCalls = doCalls.Select(doCall => CallManager.GetViewingCall(doCall.Id)).ToList();

            // Grouping the calls by status and counting occurrences
            var groupedCalls = boCalls
                .GroupBy(call => call.Status)
                .ToDictionary(group => (int)group.Key, group => group.Count());

            // Creating the result array with specific order and summing at the last position
            var quantities = new int[7];
            quantities[0] = groupedCalls.ContainsKey((int)CallStatus.Open) ? groupedCalls[(int)CallStatus.Open] : 0;
            quantities[1] = groupedCalls.ContainsKey((int)CallStatus.Closed) ? groupedCalls[(int)CallStatus.Closed] : 0;
            quantities[2] = groupedCalls.ContainsKey((int)CallStatus.InProgress) ? groupedCalls[(int)CallStatus.InProgress] : 0;
            quantities[3] = groupedCalls.ContainsKey((int)CallStatus.Expired) ? groupedCalls[(int)CallStatus.Expired] : 0;
            quantities[4] = groupedCalls.ContainsKey((int)CallStatus.InProgressAtRisk) ? groupedCalls[(int)CallStatus.InProgressAtRisk] : 0;
            quantities[5] = groupedCalls.ContainsKey((int)CallStatus.OpenAtRisk) ? groupedCalls[(int)CallStatus.OpenAtRisk] : 0;

            // Summing up all values into the last position
            quantities[6] = quantities.Take(6).Sum();

            return quantities;
        }
        catch (Exception ex)
        {
            throw new BlDoesNotExistException("Failed to retrieve call quantities by status.", ex);
        }
    }

    //public IEnumerable<BO.CallInList> GetCallsList(BO.Calltype? filter, object? obj, BO.CallInListField? sortBy)
    //{
    //    IEnumerable<DO.Call> calls = _dal.Call.ReadAll() ?? throw new BO.BlNullPropertyException("There are no calls in the database");

    //    IEnumerable<BO.CallInList> boCallsInList = calls.Select(call => CallManager.GetCallInList(call));

    //    if (filter != null && obj != null)
    //    {
    //        switch (filter)
    //        {
    //            case BO.Calltype.allergy:
    //                boCallsInList = boCallsInList.Where(item => item.CallType == (BO.Calltype)obj);
    //                break;  
    //            case BO.Calltype.birth:
    //                boCallsInList = boCallsInList.Where(item => item.CallType == (BO.Calltype)obj);
    //                break;       
    //            case BO.Calltype.broken_bone:
    //                boCallsInList = boCallsInList.Where(item => item.CallType == (BO.Calltype)obj);
    //                break;           
    //            case BO.Calltype.heartattack:
    //                boCallsInList = boCallsInList.Where(item => item.CallType == (BO.Calltype)obj);
    //                break;
    //            case BO.Calltype.resuscitation:
    //                boCallsInList = boCallsInList.Where(item => item.CallType == (BO.Calltype)obj);
    //                break;
    //            case BO.Calltype.security_event:
    //                boCallsInList = boCallsInList.Where(item => item.CallType == (BO.Calltype)obj);
    //                break; 
    //            case BO.Calltype.None:

    //                break;

    //        }
    //    }

    //    if (sortBy == null)
    //        sortBy = BO.CallInListField.CallId;

    //    switch (sortBy)
    //    {
    //        case BO.CallInListField.Id:
    //            boCallsInList = boCallsInList.OrderBy(item => item.Id.HasValue ? 0 : 1)
    //                                         .ThenBy(item => item.Id)
    //                                         .ToList();
    //            break;

    //        case BO.CallInListField.CallId:
    //            boCallsInList = boCallsInList.OrderBy(item => item.CallId).ToList();
    //            break;

    //        case BO.CallInListField.CallType:
    //            boCallsInList = boCallsInList.OrderBy(item => item.CallType).ToList();
    //            break;

    //        case BO.CallInListField.OpenTime:
    //            boCallsInList = boCallsInList.OrderBy(item => item.OpenTime).ToList();
    //            break;

    //        case BO.CallInListField.TimeRemaining:
    //            boCallsInList = boCallsInList.OrderBy(item => item.TimeRemaining).ToList();
    //            break;

    //        case BO.CallInListField.VolunteerName:
    //            boCallsInList = boCallsInList.OrderBy(item => item.VolunteerName).ToList();
    //            break;

    //        case BO.CallInListField.CompletionTime:
    //            boCallsInList = boCallsInList.OrderBy(item => item.CompletionTime).ToList();
    //            break;

    //        case BO.CallInListField.Status:
    //            boCallsInList = boCallsInList.OrderBy(item => item.Status).ToList();
    //            break;

    //        case BO.CallInListField.TotalAssignments:
    //            boCallsInList = boCallsInList.OrderBy(item => item.TotalAssignments).ToList();
    //            break;
    //    }

    //    return boCallsInList;
    //}

    public IEnumerable<BO.CallInList> GetCallsList(BO.Calltype? filter, object? obj, BO.CallInListField? sortBy, BO.CallStatus? statusFilter = null)
    {
        IEnumerable<DO.Call> calls = _dal.Call.ReadAll() ?? throw new BO.BlNullPropertyException("There are no calls in the database");

        IEnumerable<BO.CallInList> boCallsInList = calls.Select(call => CallManager.GetCallInList(call));

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

    public BO.Call Read(int callId)
    {
        try
        {
            var call = _dal.Call.Read(callId);
            return CallManager.GetViewingCall(call.Id);

        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID={callId} does not exist.", ex);
        }
    }
    public void Create(BO.Call boCall)
    {
        // we need add 
        boCall.Latitude = Tools.GetLatitude(boCall.FullAddress);
        boCall.Longitude = Tools.GetLongitude(boCall.FullAddress);

        //boCall.Status = CallManager.CalculateCallStatus();
        //boCall.CallAssignments = null; // for first time not have CallAssignments

        CallManager.IsValideCall(boCall);
        CallManager.IsLogicCall(boCall);

        //var doCall = CallManager.BOConvertDO_Call(boCall.Id);

        try
        {
            DO.Call doCall = new DO.Call(
       boCall.Latitude,            // Latitude
       boCall.Longitude,           // Longitude
       (DO.Calltype)boCall.Calltype,            // Calltype
       boCall.Id,                  // Id
       boCall.Description,         // VerbalDescription
       boCall.FullAddress,         // ReadAddress
       boCall.OpenTime,            // OpeningTime
       boCall.MaxEndTime           // MaxEndTime
   );


            _dal.Call.Create(doCall);
            CallManager.Observers.NotifyListUpdated(); //stage 5   

        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Call with ID={boCall.Id} already exists", ex);
        }

    }
    public void Update(BO.Call BOCall)
    {
        try
        {
            CallManager.IsValideCall(BOCall);
            CallManager.IsLogicCall(BOCall);
            Tools.IsAddressValid(BOCall.FullAddress);
            //BOCall.Latitude = Tools.GetLatitude(BOCall.FullAddress).Result;
            //BOCall.Longitude = Tools.GetLongitudeAsync(BOCall.FullAddress).Result;  
            BOCall.Latitude = Tools.GetLatitude(BOCall.FullAddress);
            BOCall.Longitude = Tools.GetLongitude(BOCall.FullAddress);
            //var doCall= CallManager.BOConvertDO_Call(BOCall.Id);

            var doCall = new DO.Call
            {
                Id = BOCall.Id,
                Calltype = (DO.Calltype)BOCall.Calltype, // Explicit cast to BO.Calltype enum
                VerbalDescription = BOCall.Description,
                ReadAddress = BOCall.FullAddress,
                Latitude = BOCall.Latitude, // Convert nullable to non-nullable
                Longitude = BOCall.Longitude, // Convert nullable to non-nullable
                OpeningTime = BOCall.OpenTime,
                MaxEndTime = BOCall.MaxEndTime
            };


            _dal.Call.Update(doCall);
            CallManager.Observers.NotifyListUpdated(); //stage 5   
            CallManager.Observers.NotifyItemUpdated(doCall.Id);  //stage 5
        }
        catch (BlIsLogicCallException ex)
        {
            throw new BO.BlIsLogicCallException($"Error: {ex.Message}", ex);
        }

        catch (Exception ex)
        {
            throw new BO.Incompatible_ID($" There is no call with the number identifying ={BOCall.Id}");

        }

    }
    public void Delete(int callId)
    {
        try
        {
            // Retrieve the call and convert it to a BO.Call object using the GetAdd_update_Call function
            BO.Call call = CallManager.GetViewingCall(callId);

            // Check the call's status (must be Open)
            if (call.Status != BO.CallStatus.Open)
            {
                throw new BlCallStatusNotOKException("A call that is not in open status cannot be deleted.");
            }

            // Check if the call has been assigned to a volunteer (it should not have assignments)
            if (call.CallAssignments != null && call.CallAssignments.Any())
            {
                throw new BlNo_assignments_volunteerException("A call previously assigned to a volunteer cannot be deleted.");
            }

            // Attempt to delete the call from the data layer
            _dal.Call.Delete(callId);
            CallManager.Observers.NotifyListUpdated(); //stage 5   

        }
        catch (DO.Incompatible_ID ex) // Exception thrown by the data layer
        {
            throw new BO.Incompatible_ID($"There is no call with the received ID = {callId}");
        }
    }

    //public List<BO.ClosedCallInList> GetCloseCall1(int volunteerId, BO.Calltype? callType, ClosedCallInListEnum? closedCallInListEnum)
    //{
    //    try
    //    {
    //        // Step 1: Fetch and convert all calls
    //        var allCalls = _dal.Call.ReadAll().ToList();
    //        var boCalls = allCalls
    //            .Select(c => CallManager.GetCallInList(c)) // Convert to CallInList
    //            .ToList();

    //        // Step 2: Filter only closed calls
    //        var filteredCalls = boCalls
    //            .Where(c => c.Status == BO.CallStatus.Closed) // Filter closed calls
    //            .ToList();

    //        // Step 3: Convert to ClosedCallInList
    //        var boClosedCalls = filteredCalls
    //            .Select(c => VolunteerManager.GetClosedCallInList(volunteerId)) // Convert to ClosedCallInList
    //            .ToList();

    //        // Step 4: Filter by call type if specified
    //        if (callType.HasValue)
    //        {
    //            boClosedCalls = boClosedCalls
    //                .Where(c => c.CallType == callType.Value)
    //                .ToList();
    //        }

    //        // Step 5: Sort the list if needed
    //        if (closedCallInListEnum.HasValue)
    //        {
    //            boClosedCalls = closedCallInListEnum switch
    //            {
    //                ClosedCallInListEnum.Id => boClosedCalls.OrderBy(c => c.Id).ToList(),
    //                ClosedCallInListEnum.CallType => boClosedCalls.OrderBy(c => c.CallType).ToList(),
    //                ClosedCallInListEnum.FullAddress => boClosedCalls.OrderBy(c => c.FullAddress).ToList(),
    //                ClosedCallInListEnum.OpenTime => boClosedCalls.OrderBy(c => c.OpenTime).ToList(),
    //                ClosedCallInListEnum.EnterTime => boClosedCalls.OrderBy(c => c.EnterTime).ToList(),
    //                ClosedCallInListEnum.EndTime => boClosedCalls.OrderBy(c => c.EndTime).ToList(),
    //                ClosedCallInListEnum.CompletionStatus => boClosedCalls.OrderBy(c => c.CompletionStatus).ToList(),
    //                _ => boClosedCalls.OrderBy(c => c.Id).ToList() // Default sorting by Id
    //            };
    //        }
    //        else
    //        {
    //            // If no sorting value is provided, default sorting is by Id
    //            boClosedCalls = boClosedCalls.OrderBy(c => c.Id).ToList();
    //        }

    //        // Return the list of closed calls
    //        return boClosedCalls;
    //    }
    //    catch (Exception ex)
    //    {
    //        // Throw a custom exception if an error occurs
    //        throw new BlGetCloseCallException($"Error retrieving closed calls: {ex.Message}");
    //    }
    //}

    public List<BO.ClosedCallInList> GetCloseCall(int volunteerId, BO.Calltype? callType, ClosedCallInListEnum? closedCallInListEnum)
    {
        try
        {
            // Step 1: Get all assignments for the specific volunteer that have been completed (EndOfTime is not null)
            var volunteerAssignments = _dal.Assignment.ReadAll()
                .Where(a => a.VolunteerId == volunteerId && a.EndOfTime != null)
                .ToList();

            // Step 2: Create list of ClosedCallInList objects using Select and LINQ
            var boClosedCalls = volunteerAssignments
                .Select(assignment =>
                {
                    try
                    {
                        var call = _dal.Call.Read(assignment.CallId);
                        if (call == null) return null; // Skip if the call is not found

                        return new BO.ClosedCallInList
                        {
                            Id = assignment.CallId,
                            CallType = (BO.Calltype)call.Calltype,
                            FullAddress = call.ReadAddress,
                            OpenTime = call.OpeningTime,
                            EnterTime = assignment.time_entry_treatment,
                            EndTime = assignment.time_end_treatment,
                            CompletionStatus = (BO.CallAssignmentEnum?)assignment.EndOfTime
                        };
                    }
                    catch (Exception)
                    {
                        return null; // Return null if there's an error with a particular call
                    }
                })
                .Where(closedCall => closedCall != null) // Filter out null values (calls that failed to process)
                .ToList();

            // Step 3: Filter by call type if specified
            if (callType.HasValue)
            {
                boClosedCalls = boClosedCalls
                    .Where(c => c.CallType == callType.Value)
                    .ToList();
            }

            // Step 4: Sort based on the specified enum
            boClosedCalls = closedCallInListEnum switch
            {
                ClosedCallInListEnum.Id => boClosedCalls.OrderBy(c => c.Id).ToList(),
                ClosedCallInListEnum.CallType => boClosedCalls.OrderBy(c => c.CallType).ToList(),
                ClosedCallInListEnum.FullAddress => boClosedCalls.OrderBy(c => c.FullAddress).ToList(),
                ClosedCallInListEnum.OpenTime => boClosedCalls.OrderBy(c => c.OpenTime).ToList(),
                ClosedCallInListEnum.EnterTime => boClosedCalls.OrderBy(c => c.EnterTime).ToList(),
                ClosedCallInListEnum.EndTime => boClosedCalls.OrderBy(c => c.EndTime).ToList(),
                ClosedCallInListEnum.CompletionStatus => boClosedCalls.OrderBy(c => c.CompletionStatus).ToList(),
                _ => boClosedCalls.OrderBy(c => c.Id).ToList() // Default sorting by Id
            };

            return boClosedCalls;
        }
        catch (Exception ex)
        {
            throw new BlGetCloseCallException($"Error retrieving closed calls: {ex.Message}", ex);
        }
    }


    //public IEnumerable<BO.OpenCallInList> GetOpenCall1(int id, BO.Calltype? type, BO.OpenCallInListEnum? sortBy)
    //{
    //    DO.Volunteer doVolunteer = _dal.Volunteer.Read(id);
    //    if (doVolunteer == null)
    //        throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist");

    //    // Retrieve all calls from the BO
    //    var doallcall = _dal.Call.ReadAll();
    //   // var allCalls = doallcall.Select(call => CallManager.GetCallInList(call)).ToList();
    //    var allCalls = doallcall.Select(call => CallManager.GetAdd_update_Call(id)).ToList();

    //    //BO.Volunteer BOvolunteer = VolunteerManager.GetVolunteer(id); //// לקרוא לולנטיר של עדכון שצריך ליצור
    //    var BOvolunteer1 = new BO.Volunteer
    //    {
    //        Id = doVolunteer.Id,
    //        Name = doVolunteer.Name,
    //        Number_phone = doVolunteer.Number_phone,
    //        Email = doVolunteer.Email,
    //        FullCurrentAddress = doVolunteer.FullCurrentAddress,
    //        Password = doVolunteer.Password,
    //        //Latitude = Tools.GetLatitude(doVolunteer.FullCurrentAddress).Result,
    //        //Longitude = Tools.GetLongitudeAsync(doVolunteer.FullCurrentAddress).Result,
    //        Latitude = Tools.GetLatitude(doVolunteer.FullCurrentAddress),
    //        Longitude = Tools.GetLongitude(doVolunteer.FullCurrentAddress),
    //        Role = (BO.Role)doVolunteer.Role, // המרה ישירה בין ה-Enums
    //        Active = doVolunteer.Active,
    //        Distance = doVolunteer.distance,
    //        DistanceType = (BO.DistanceType)doVolunteer.Distance_Type, // המרה ישירה בין ה-Enums
    //        TotalHandledCalls = _dal.Assignment.ReadAll().Count(a => a.VolunteerId == doVolunteer.Id && a.EndOfTime == AssignmentCompletionType.TreatedOnTime),
    //        TotalCancelledCalls = _dal.Assignment.ReadAll().Count(a => a.VolunteerId == doVolunteer.Id &&
    //            (a.EndOfTime == AssignmentCompletionType.AdminCancelled || a.EndOfTime == AssignmentCompletionType.VolunteerCancelled)), // ביטול עצמי או מהנל
    //        TotalExpiredCalls = _dal.Assignment.ReadAll().Count(a => a.VolunteerId == doVolunteer.Id && a.EndOfTime == AssignmentCompletionType.Expired),
    //        CurrentCall = null



    //    };

    //    // Retrieve all assignments from the DAL
    //    var allAssignments = _dal.Assignment.ReadAll();
    //    double? lonVol = BOvolunteer1.Longitude;
    //    double? latVol = BOvolunteer1.Latitude;

    //    //// Filter for only "Open" or "Risk Open" status
    //    //IEnumerable<BO.OpenCallInList> filteredCalls = from call in allCalls
    //    //                                               join assignment in allAssignments on call.Id equals assignment.CallId into callAssignments
    //    //                                               from assignment in callAssignments.DefaultIfEmpty()
    //    //                                               where (call.Status == BO.CallStatus.Open || call.Status == BO.CallStatus.OpenAtRisk)
    //    //                                               let boCall = Read(call.CallId)
    //    //                                               select new BO.OpenCallInList
    //    //                                               {
    //    //                                                   Id = call.CallId,
    //    //                                                   CallType = call.CallType,
    //    //                                                   Description = boCall.Description,
    //    //                                                   FullAddress = boCall.FullAddress,
    //    //                                                   OpenTime = call.OpenTime,
    //    //                                                   MaxEndTime = boCall.MaxEndTime,
    //    //                                                   DistanceFromVolunteer = BOvolunteer1?.FullCurrentAddress != null ? CallManager.Air_distance_between_2_addresses
    //    //                                                   (latVol, lonVol, boCall.Latitude, boCall.Longitude) : 0  // Calculate the distance between the volunteer and the call

    //    //                                               };



    //    IEnumerable<BO.OpenCallInList> filteredCalls = allCalls
    //.Where(call => (call.Status == BO.CallStatus.Open || call.Status == BO.CallStatus.OpenAtRisk) &&
    //               allAssignments.Any(a => a.CallId == call.Id)) // מסנן רק את הקריאות שיש להן משימות
    //.Select(call =>
    //{
    //    var callAssignments = allAssignments.Where(a => a.CallId == call.Id).ToList(); // מוצא את המשימות המתאימות
    //    BO.Call boCall = Read(call.Id);
    //    double? distance = 0;

    //    if (BOvolunteer1?.FullCurrentAddress != null)
    //    {
    //        var assignment = callAssignments.FirstOrDefault(); // מקבל את המשימה הראשונה
    //        if (assignment != null)
    //        {
    //            distance = CallManager.Air_distance_between_2_addresses(
    //                latVol, lonVol, boCall.Latitude, boCall.Longitude);
    //        }
    //    }

    //    return new BO.OpenCallInList
    //    {
    //        Id = call.Id,
    //        CallType = call.Calltype,
    //        Description = boCall.Description,
    //        FullAddress = boCall.FullAddress,
    //        OpenTime = call.OpenTime,
    //        MaxEndTime = boCall.MaxEndTime,
    //        DistanceFromVolunteer = distance.Value
    //    };
    //});


    //    // Filter by call type if provided
    //    if (type.HasValue)
    //    {
    //        filteredCalls = filteredCalls.Where(c => c.CallType == type.Value);
    //    }

    //    // Sort by the requested field or by default (call ID)
    //    if (sortBy.HasValue)
    //    {
    //        filteredCalls = sortBy.Value switch
    //        {
    //            BO.OpenCallInListEnum.Id => filteredCalls.OrderBy(c => c.Id),
    //            BO.OpenCallInListEnum.CallType => filteredCalls.OrderBy(c => c.CallType),
    //            BO.OpenCallInListEnum.Description => filteredCalls.OrderBy(c => c.Description),
    //            BO.OpenCallInListEnum.FullAddress => filteredCalls.OrderBy(c => c.FullAddress),
    //            BO.OpenCallInListEnum.OpenTime => filteredCalls.OrderBy(c => c.OpenTime),
    //            BO.OpenCallInListEnum.MaxEndTime => filteredCalls.OrderBy(c => c.MaxEndTime),
    //            BO.OpenCallInListEnum.DistanceFromVolunteer => filteredCalls.OrderBy(c => c.DistanceFromVolunteer),
    //            _ => filteredCalls.OrderBy(c => c.Id)
    //        };
    //    }
    //    else
    //    {
    //        filteredCalls = filteredCalls.OrderBy(c => c.Id);
    //    }

    //    return filteredCalls;
    //}

    public IEnumerable<BO.OpenCallInList> GetOpenCall(int id, BO.Calltype? type, BO.OpenCallInListEnum? sortBy)
    {
        DO.Volunteer volunteer = _dal.Volunteer.Read(id);
        if (volunteer == null)
            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist");

        // Retrieve all calls from the BO
        IEnumerable<BO.CallInList> allCalls = GetCallsList(null, null, null);

        // Retrieve all assignments from the DAL
        var allAssignments = _dal.Assignment.ReadAll();
        double? lonVol = Tools.GetLongitude(volunteer.FullCurrentAddress);
        double? latVol = Tools.GetLatitude(volunteer.FullCurrentAddress);

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
                        ? CallManager.Air_distance_between_2_addresses(latVol, lonVol, boCall.Latitude, boCall.Longitude)
                        : 0  // Calculate the distance between the volunteer and the call
                };
            });

        // Filter by call type if provided
        if (type.HasValue)
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


    public void UpdateCancelTreatment(int idVol, int idAssig)
    {

        DO.Assignment assigmnetToCancel = _dal.Assignment.Read(idAssig) ?? throw new BO.BlDeletionImpossibleException("there is no assigment with this ID");
        bool ismanager = false;
        if (assigmnetToCancel.VolunteerId != idVol)
        {
            if (_dal.Volunteer.Read(idVol).Role == DO.Role.Manager)
                ismanager = true;
            else throw new BO.BlDeletionImpossibleException("the volunteer is not manager or not in this call");
        }
        if (assigmnetToCancel.time_end_treatment != null )//// לבדוק
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
            _dal.Assignment.Update(assigmnetToUP);
        }
        catch (Exception ex)
        {
            throw new BO.BlDeletionImpossibleException("canot delete in DO");
        }

    }

    public void UpdateEndTreatment(int idVol, int idCall)
    {
        // קורא את המתנדב לפי מזהה המתנדב
        DO.Volunteer vol = _dal.Volunteer.Read(idVol) ?? throw new BO.BlNullPropertyException($"There is no volunteer with ID {idVol}");
        var assignment = _dal.Assignment.ReadAll()
               .FirstOrDefault(a => a.CallId == idCall && a.VolunteerId == idVol);
        // קורא את הקריאה לפי מזהה הקריאה
        BO.Call bocall = Read(idCall) ?? throw new BO.BlNullPropertyException($"There is no call with ID {idCall}");

        // בדוק אם הקריאה פתוחה או אם היא פג תוקף
        if (( (bocall.Status != BO.CallStatus.Open) && (bocall.Status != BO.CallStatus.OpenAtRisk)) || bocall.Status == BO.CallStatus.Expired )
        {
            throw new BO.BlAlreadyExistsException($"The call with ID {idCall} is not open or has expired/cancelled.");
        }

        // בדוק שהמתנדב הוא זה שהוקצה לקריאה
        //if (bocall.Assignment == null || bocall.Assignment.VolunteerId != idVol)
        //{
        //    throw new BO.BlPermissionException($"Volunteer with ID {idVol} is not assigned to this call.");
        //}

        // יצירת ישות Assignment לעדכון
        DO.Assignment assignmentToUpdate = new DO.Assignment
        {
            Id = assignment.Id, // זהה את מזהה ההקצאה
            CallId = idCall,
            VolunteerId = idVol,
            time_entry_treatment = assignment.time_entry_treatment, // שמור את זמן הכניסה לטיפול
            time_end_treatment = AdminManager.Now, // עדכן את זמן סיום הטיפול בפועל
            EndOfTime = AssignmentCompletionType.TreatedOnTime // עדכון סוג סיום הטיפול ל"טופלה"
        };

        try
        {
            // ניסיון לעדכן את ההקצאה בשכבת הנתונים
            _dal.Assignment.Update(assignmentToUpdate);
        }
        catch (Exception ex)
        {
            throw new BO.BlDoesNotExistException("Failed to update assignment.", ex);
        }
    }


    public void ChooseCall1(int VolunteerId, int CallId)
    {
        try
        {
            // Step 1: Retrieve the call details from the DAL
            var call = _dal.Call.Read(CallId); // Throws exception if not found

            var bocall = CallManager.GetViewingCall(CallId);
            if (bocall.Status == CallStatus.Closed || bocall.Status == CallStatus.InProgress || bocall.Status == CallStatus.InProgressAtRisk || bocall.Status == CallStatus.Expired)
            {
                throw new BO.BlCallStatusNotOKException("The call is already closed, expired, or in progress and cannot be chosen.");
            }

            // Step 2: Retrieve the assignment for this call and volunteer
            var assignment = _dal.Assignment.ReadAll()
                .FirstOrDefault(a => a.CallId == CallId && a.VolunteerId == VolunteerId);

            // If there is an existing assignment, update its treatment end
            if (assignment != null)
            {
                UpdateEndTreatment(VolunteerId, assignment.Id);
            }

            // Step 3: Create a new assignment only if status allows
            var NewAssignment = new DO.Assignment
            {
                CallId = CallId, // Call identifier
                VolunteerId = VolunteerId, // Volunteer identifier
                time_entry_treatment = AdminManager.Now, // Time of entry into treatment
                time_end_treatment = null, // Time of actual treatment completion
                EndOfTime = null // End of treatment type
            };

            _dal.Assignment.Create(NewAssignment);
            //CallManager.Observers.NotifyListUpdated(); //stage 5   


            // Optionally, make sure to check and keep the status open after assignment creation
            bocall = CallManager.GetViewingCall(CallId); // Fetch the updated call
            if (bocall.Status == CallStatus.InProgress || bocall.Status == CallStatus.Open || bocall.Status == CallStatus.OpenAtRisk)
            {
                Console.WriteLine("Call remains open.");
            }

        }
        catch (Exception ex)
        {
            throw new BO.Incompatible_ID($"Call with ID {CallId} was not found.");
        }
    }
    //public void ChooseCall(int idVol, int idCall)
    //{
    //    // Retrieve volunteer and call; throw exception if not found.
    //    DO.Volunteer vol = _dal.Volunteer.Read(idVol) ?? throw new BO.BlNullPropertyException($"There is no volunteer with this ID {idVol}");
    //    BO.Call boCall = Read(idCall) ?? throw new BO.BlNullPropertyException($"There is no call with this ID {idCall}");

    //    // Check if the call is open; throw exception if not.
    //    if (boCall.Status != BO.CallStatus.Open || boCall.Status == BO.CallStatus.OpenAtRisk)
    //        throw new BO.BlAlreadyExistsException($"The call is open or expired. IdCall is = {idCall}");

    //    // Create a new assignment for the volunteer and the call.
    //    DO.Assignment assigmnetToCreat = new DO.Assignment
    //    {
    //        Id = 0, // ID will be generated automatically
    //        CallId = idCall,
    //        VolunteerId = idVol,
    //        time_entry_treatment = AdminManager.Now,
    //        time_end_treatment = null,
    //        EndOfTime = null
    //    };

    //    try
    //    {
    //        // Try to create the assignment in the database.
    //        _dal.Assignment.Create(assigmnetToCreat);
    //    }
    //    catch (Exception e)
    //    {
    //        // Handle error if creation fails.
    //        throw new BO.BlAlreadyExistsException("Impossible to create the assignment.");
    //    }
    //}

    public void ChooseCall(int idVol, int idCall)
    {
        // Retrieve volunteer and call; throw exception if not found.
        DO.Volunteer vol = _dal.Volunteer.Read(idVol) ??
                           throw new BO.BlNullPropertyException($"There is no volunteer with this ID {idVol}");
        BO.Call boCall = Read(idCall) ??
                         throw new BO.BlNullPropertyException($"There is no call with this ID {idCall}");

        // Check if the call is open.
        if (boCall.Status != BO.CallStatus.Open)
            throw new BO.BlAlreadyExistsException($"The call is not open or is already being handled. IdCall = {idCall}");

        // Check if the call already has an open assignment.
        var existingAssignments = _dal.Assignment.ReadAll()
                                .Where(a => a.CallId == idCall && a.time_end_treatment == null)
                                .ToList();

        if (existingAssignments.Any())
            throw new BO.BlAlreadyExistsException($"The call is already assigned to another volunteer. IdCall = {idCall}");

        //// Check if the call has expired.
        //if (boCall. < AdminManager.Now)
        //    throw new BO.BlAlreadyExistsException($"The call has expired. IdCall = {idCall}");

        // Create a new assignment for the volunteer and the call.
        DO.Assignment assigmnetToCreat = new DO.Assignment
        {
            Id = 0, // ID will be generated automatically
            CallId = idCall,
            VolunteerId = idVol,
            time_entry_treatment = AdminManager.Now,
            time_end_treatment = null,
            EndOfTime = null
        };

        try
        {
            // Try to create the assignment in the database.
            _dal.Assignment.Create(assigmnetToCreat);
            CallManager.Observers.NotifyItemUpdated(assigmnetToCreat.Id);  //stage 5
            CallManager.Observers.NotifyListUpdated();  //stage 5

        }
        catch (Exception e)
        {
            // Handle error if creation fails.
            throw new BO.BlAlreadyExistsException("Impossible to create the assignment.");
        }
    }


}
