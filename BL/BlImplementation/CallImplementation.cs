
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
            IEnumerable<DO.Call> doCalls = null;
            lock (AdminManager.BlMutex) //stage 7
                // Fetching calls from the data layer
                 doCalls = _dal.Call.ReadAll();

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

    public IEnumerable<BO.CallInList> GetCallsList(BO.Calltype? filter, object? obj, BO.CallInListField? sortBy, BO.CallStatus? statusFilter = null)
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

    public BO.Call Read(int callId)
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
    // public void Create(BO.Call boCall)
    // {
    //     AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
    //     // we need add 
    //     boCall.Latitude = Tools.GetLatitudeAsync(boCall.FullAddress).Result;
    //     boCall.Longitude = Tools.GetLongitudeAsync(boCall.FullAddress).Result;

    //     //boCall.Status = CallManager.CalculateCallStatus();
    //     //boCall.CallAssignments = null; // for first time not have CallAssignments

    //     CallManager.IsValideCall(boCall);
    //     CallManager.IsLogicCall(boCall);

    //     //var doCall = CallManager.BOConvertDO_Call(boCall.Id);

    //     try
    //     {
    //         DO.Call doCall = new DO.Call(
    //    boCall.Latitude,            // Latitude
    //    boCall.Longitude,           // Longitude
    //    (DO.Calltype)boCall.Calltype,            // Calltype
    //    boCall.Id,                  // Id
    //    boCall.Description,         // VerbalDescription
    //    boCall.FullAddress,         // ReadAddress
    //    boCall.OpenTime,            // OpeningTime
    //    boCall.MaxEndTime           // MaxEndTime
    //);

    //         lock (AdminManager.BlMutex) //stage 7
    //             _dal.Call.Create(doCall);

    //         CallManager.Observers.NotifyListUpdated(); //stage 5   

    //     }
    //     catch (DO.DalAlreadyExistsException ex)
    //     {
    //         throw new BO.BlAlreadyExistsException($"Call with ID={boCall.Id} already exists", ex);
    //     }

    // }

    public void Create(BO.Call boCall)
    {
        AdminManager.ThrowOnSimulatorIsRunning(); // stage 7

        // 1. Validate the call format and logic
        CallManager.IsValideCall(boCall);
        CallManager.IsLogicCall(boCall);
        // 4. Asynchronously calculate and update the coordinates
        _ = UpdateCoordinatesForCallAsync(boCall);

        try
        {
            // 2. Convert BO.Call to DO.Call (without Latitude and Longitude initially)
            DO.Call doCall = new DO.Call(
                boCall.Latitude,                      // Latitude (placeholder)
                boCall.Longitude,                      // Longitude (placeholder)
                (DO.Calltype)boCall.Calltype,  // Calltype
                boCall.Id,                // Id
                boCall.Description,       // VerbalDescription
                boCall.FullAddress,       // ReadAddress
                boCall.OpenTime,          // OpeningTime
                boCall.MaxEndTime         // MaxEndTime
            );


            // 3. Add the call to the data layer
            lock (AdminManager.BlMutex) // stage 7
                _dal.Call.Create(doCall);

       

            // 5. Notify observers about the update
            CallManager.Observers.NotifyListUpdated(); // stage 5
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Call with ID={boCall.Id} already exists", ex);
        }
    }

    //public void Update(BO.Call BOCall)
    //{
    //    AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

    //    try
    //    {
    //        CallManager.IsValideCall(BOCall);
    //        CallManager.IsLogicCall(BOCall);
    //        Tools.IsAddressValidAsync(BOCall.FullAddress);
    //        //BOCall.Latitude = Tools.GetLatitude(BOCall.FullAddress).Result;
    //        //BOCall.Longitude = Tools.GetLongitudeAsync(BOCall.FullAddress).Result;
    //        //
    //        BOCall.Latitude = Tools.GetLatitudeAsync(BOCall.FullAddress).Result;
    //        BOCall.Longitude = Tools.GetLongitudeAsync(BOCall.FullAddress).Result;
    //        //var doCall= CallManager.BOConvertDO_Call(BOCall.Id);

    //        var doCall = new DO.Call
    //        {
    //            Id = BOCall.Id,
    //            Calltype = (DO.Calltype)BOCall.Calltype, // Explicit cast to BO.Calltype enum
    //            VerbalDescription = BOCall.Description,
    //            ReadAddress = BOCall.FullAddress,
    //            Latitude = BOCall.Latitude, // Convert nullable to non-nullable
    //            Longitude = BOCall.Longitude, // Convert nullable to non-nullable
    //            OpeningTime = BOCall.OpenTime,
    //            MaxEndTime = BOCall.MaxEndTime
    //        };

    //        lock (AdminManager.BlMutex) //stage 7
    //            _dal.Call.Update(doCall);

    //        CallManager.Observers.NotifyListUpdated(); //stage 5   
    //        CallManager.Observers.NotifyItemUpdated(doCall.Id);  //stage 5
    //    }
    //    catch (BlIsLogicCallException ex)
    //    {
    //        throw new BO.BlIsLogicCallException($"Error: {ex.Message}", ex);
    //    }
    //    catch(BlInvalidaddress ex)
    //    {
    //        throw new BO.BlInvalidaddress($"Error: {ex.Message}", ex);
    //    }
    //    catch (DO.Incompatible_ID ex) // Exception thrown by the data layer
    //    {
    //        throw new BO.Incompatible_ID($" There is no call with the number identifying ={BOCall.Id}");
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new BO.Incompatible_ID($" There is no call with the number identifying ={BOCall.Id}");

    //    }

    //}

    public void Update(BO.Call BOCall)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

        try
        {
            CallManager.IsValideCall(BOCall);
            CallManager.IsLogicCall(BOCall);

            // קודם כל, עדכון המידע ב-DAL בלי הקואורדינטות
            var doCall = new DO.Call
            {
                Id = BOCall.Id,
                Calltype = (DO.Calltype)BOCall.Calltype, // Explicit cast to BO.Calltype enum
                VerbalDescription = BOCall.Description,
                ReadAddress = BOCall.FullAddress,
                Latitude = null, // נבצע עדכון בלי קואורדינאטות
                Longitude = null, // נבצע עדכון בלי קואורדינאטות
                OpeningTime = BOCall.OpenTime,
                MaxEndTime = BOCall.MaxEndTime
            };


            // שלב שני - חישוב הקואורדינאטות בצורה אסינכרונית
            _ = UpdateCoordinatesForCallAsync(BOCall); // שליחה של הבקשה בצורה אסינכרונית לחישוב הקואורדינאטות


            lock (AdminManager.BlMutex) //stage 7
                _dal.Call.Update(doCall); // עדכון ראשוני ללא קואורדינאטות
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
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

        try
        {
            BO.Call call = null;
            // Retrieve the call and convert it to a BO.Call object using the GetAdd_update_Call function
            lock (AdminManager.BlMutex) //stage 7
            {
              call = CallManager.GetViewingCall(callId);

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
            }

            CallManager.Observers.NotifyListUpdated(); //stage 5   

        }
        catch (DO.Incompatible_ID ex) // Exception thrown by the data layer
        {
            throw new BO.Incompatible_ID($"There is no call with the received ID = {callId}");
        }
    }


    public List<BO.ClosedCallInList> GetCloseCall(int volunteerId, BO.Calltype? callType, ClosedCallInListEnum? closedCallInListEnum)
    {
        try
        {
            IEnumerable<Assignment> volunteerAssignments = null;
            // Step 1: Get all assignments for the specific volunteer that have been completed (EndOfTime is not null)
            lock (AdminManager.BlMutex) //stage 7
                volunteerAssignments = _dal.Assignment.ReadAll()
               .Where(a => a.VolunteerId == volunteerId && a.EndOfTime != null)
               .ToList();

            // Step 2: Create list of ClosedCallInList objects using Select and LINQ
            var boClosedCalls = volunteerAssignments
                .Select(assignment =>
                {
                    try
                    {
                        DO.Call call = null;
                        lock (AdminManager.BlMutex)
                            call = _dal.Call.Read(assignment.CallId);
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
            if (callType != BO.Calltype.None)
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

    /// <summary>
    /// ///////////?????
    /// </summary>
    /// <param name="id"></param>
    /// <param name="type"></param>
    /// <param name="sortBy"></param>
    /// <returns></returns>
    /// <exception cref="BO.BlDoesNotExistException"></exception>
    public IEnumerable<BO.OpenCallInList> GetOpenCall(int id, BO.Calltype? type, BO.OpenCallInListEnum? sortBy)
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
            allCalls = GetCallsList(null, null, null);

            // Retrieve all assignments from the DAL
             allAssignments = _dal.Assignment.ReadAll();
        }

        //double? lonVol = 
        //    UpdateCoordinatesForCallLON(volunteer.FullCurrentAddress).Result;
        //double? latVol =
        //    UpdateCoordinatesForCallLAN(volunteer.FullCurrentAddress).Result;

        double? lonVol = Task.Run(() => UpdateCoordinatesForCallLON(volunteer.FullCurrentAddress)).Result;
        double? latVol = Task.Run(() => UpdateCoordinatesForCallLAN(volunteer.FullCurrentAddress)).Result;

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


    public void ChooseCall(int idVol, int idCall)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

        DO.Volunteer vol = null;
        BO.Call boCall = null;
        IEnumerable<Assignment> existingAssignments = null;

        lock (AdminManager.BlMutex) //stage 7

        // Retrieve volunteer and call; throw exception if not found.
        
          {  vol = _dal.Volunteer.Read(idVol) ??
                           throw new BO.BlNullPropertyException($"There is no volunteer with this ID {idVol}");
            boCall = Read(idCall) ??
                            throw new BO.BlNullPropertyException($"There is no call with this ID {idCall}");
}

            // Check if the call is open.
            if (boCall.Status != BO.CallStatus.Open)
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

    public void UpdateCancelTreatment(int idVol, int idAssig)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

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

    public void UpdateEndTreatment(int idVol, int idAssig)

    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

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



    // Function to asynchronously update the coordinates of a call
    private async Task UpdateCoordinatesForCallAsync(BO.Call boCall)
    {
        try
        {
            double lat = await Tools.GetLatitudeAsync(boCall.FullAddress);
            double loc = await Tools.GetLongitudeAsync(boCall.FullAddress);
            DO.Call doCall = CallManager.BOConvertDO_Call(boCall.Id);

            doCall = doCall with { Latitude = lat, Longitude = loc };
            lock (AdminManager.BlMutex)
                _dal.Call.Update(doCall);
            VolunteerManager.Observers.NotifyListUpdated();
            VolunteerManager.Observers.NotifyItemUpdated(doCall.Id);
            // Update the call in the data layer with the new coordinates

        }
        catch (Exception ex)
        {
            // Handle exceptions related to coordinate updates
            throw new BO.BlGeneralException("Failed to update coordinates for the call.", ex);
        }
    }
    public async Task<double> UpdateCoordinatesForCallLON(string adress)
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

        public async Task<double> UpdateCoordinatesForCallLAN(string adress)
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
