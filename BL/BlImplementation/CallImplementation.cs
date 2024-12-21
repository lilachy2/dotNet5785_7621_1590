
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

    public IEnumerable<BO.CallInList> GetCallsList(BO.CallInListField? filter, object? obj, BO.CallInListField? sortBy)
    {
        IEnumerable<DO.Call> calls = _dal.Call.ReadAll() ?? throw new BO.BlNullPropertyException("There are not calls int database");


        IEnumerable<BO.CallInList> boCallsInList = _dal.Call.ReadAll().Select(call => CallManager.GetCallInList(call));
        if (filter != null && obj != null)
        {
            switch (filter)
            {
                case BO.CallInListField.Id:
                    boCallsInList.Where(item => item.Id == (int)obj).Select(item => item);
                    break;

                case BO.CallInListField.CallId:
                    boCallsInList.Where(item => item.CallId == (int)obj).Select(item => item);
                    break;

                case BO.CallInListField.CallType:
                    boCallsInList.Where(item => item.CallType == (BO.Calltype)obj).Select(item => item);
                    break;

                case BO.CallInListField.OpenTime:
                    boCallsInList.Where(item => item.OpenTime == (DateTime)obj).Select(item => item);
                    break;

                case BO.CallInListField.TimeRemaining:
                    boCallsInList.Where(item => item.TimeRemaining == (TimeSpan)obj).Select(item => item);
                    break;

                case BO.CallInListField.VolunteerName:
                    boCallsInList.Where(item => item.VolunteerName == (string)obj).Select(item => item);
                    break;

                case BO.CallInListField.CompletionTime:
                    boCallsInList.Where(item => item.CompletionTime == (TimeSpan)obj).Select(item => item);
                    break;

                case BO.CallInListField.Status:
                    boCallsInList.Where(item => item.Status == (BO.CallStatus)obj).Select(item => item);
                    break;

                case BO.CallInListField.TotalAssignments:
                    boCallsInList.Where(item => item.TotalAssignments == (int)obj).Select(item => item);
                    break;
            }
        }
        if (sortBy == null)
            sortBy = BO.CallInListField.CallId;
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
        boCall.Latitude = Tools.GetLatitudeAsync(boCall.FullAddress).Result;
        boCall.Longitude = Tools.GetLongitudeAsync(boCall.FullAddress).Result;

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
            BOCall.Latitude = Tools.GetLatitudeAsync(BOCall.FullAddress).Result;
            BOCall.Longitude = Tools.GetLongitudeAsync(BOCall.FullAddress).Result;
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
            // Step 1: Fetch and convert all calls
            var allCalls = _dal.Call.ReadAll().ToList();
            var boCalls = allCalls
                .Select(c => CallManager.GetCallInList(c)) // Convert to CallInList
                .ToList();

            // Step 2: Filter only closed calls
            var filteredCalls = boCalls
                .Where(c => c.Status == BO.CallStatus.Closed) // Filter closed calls
                .ToList();

            // Step 3: Convert to ClosedCallInList
            var boClosedCalls = filteredCalls
                .Select(c => VolunteerManager.GetClosedCallInList(volunteerId)) // Convert to ClosedCallInList
                .ToList();

            // Step 4: Filter by call type if specified
            if (callType.HasValue)
            {
                boClosedCalls = boClosedCalls
                    .Where(c => c.CallType == callType.Value)
                    .ToList();
            }

            // Step 5: Sort the list if needed
            if (closedCallInListEnum.HasValue)
            {
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
            }
            else
            {
                // If no sorting value is provided, default sorting is by Id
                boClosedCalls = boClosedCalls.OrderBy(c => c.Id).ToList();
            }

            // Return the list of closed calls
            return boClosedCalls;
        }
        catch (Exception ex)
        {
            // Throw a custom exception if an error occurs
            throw new BlGetCloseCallException($"Error retrieving closed calls: {ex.Message}");
        }
    }

    //public List<BO.OpenCallInList> GetOpenCall1(int volunteerId, BO.Calltype? callType, OpenCallInListEnum? openCallInListEnum)
    //{
    //    try
    //    {
    //        var allCalls = _dal.Call.ReadAll().ToList();

    //        DO.Volunteer volunteer = _dal.Volunteer.Read(volunteerId);

    //        var boCalls = allCalls
    //       .Where(c => _dal.Assignment.ReadAll()
    //        .Any(a => a.CallId == c.Id && a.time_end_treatment == null)).Select(c =>
    //            {
    //                var boCall = CallManager.GetCallInList(c/*.Id*/); // Conversion function provided
    //                return boCall;
    //            })
    //            .ToList();




    //        var filteredCalls = boCalls
    //            .Where(c => c.Status == BO.CallStatus.Open || c.Status == BO.CallStatus.OpenAtRisk)
    //            .ToList();


    //        var boOpenCalls = filteredCalls
    //            .Select(c =>
    //            {
    //                var boCall = VolunteerManager.GetOpenCallInList(volunteerId); // Conversion function provided
    //                return boCall;
    //            })
    //            .ToList();

    //        if (callType.HasValue)
    //        {
    //            boOpenCalls = boOpenCalls.Where(c => c.CallType == callType.Value).ToList();
    //        }

    //        if (openCallInListEnum.HasValue)
    //        {
    //            boOpenCalls = openCallInListEnum switch
    //            {
    //                OpenCallInListEnum.Id => boOpenCalls.OrderBy(c => c.Id).ToList(),
    //                OpenCallInListEnum.DistanceFromVolunteer => boOpenCalls.OrderBy(c => c.DistanceFromVolunteer).ToList(),
    //                OpenCallInListEnum.OpenTime => boOpenCalls.OrderBy(c => c.OpenTime).ToList(),
    //                _ => boOpenCalls.OrderBy(c => c.Id).ToList() // Default sorting by Id
    //            };
    //        }
    //        else
    //        {
    //            boOpenCalls = boOpenCalls.OrderBy(c => c.Id).ToList();
    //        }

    //        return boOpenCalls;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new BlGetOpenCallException($"Error retrieving open calls: {ex.Message}");
    //    }
    //}

    //public List<BO.OpenCallInList> GetOpenCall1(int volunteerId, BO.Calltype? callType, OpenCallInListEnum? openCallInListEnum)
    //{
    //    try
    //    {
    //        // שלב 1: קריאת כל הקריאות מהמאגר והמרתן ל-BO.CallInList
    //        var allCalls = _dal.Call.ReadAll().ToList();
    //        var boCalls = allCalls
    //            .Select(c =>
    //            {
    //                var boCall = CallManager.GetCallInList(c); // המרה ל-CallInList
    //                return boCall;
    //            })
    //            .ToList();

    //        // שלב 2: סינון הקריאות הפתוחות
    //        var filteredCalls = boCalls
    //            .Where(c => c.Status == BO.CallStatus.Open || c.Status == BO.CallStatus.OpenAtRisk)
    //            .ToList();

    //        var boOpenCalls = filteredCalls
    // .Select(c =>
    // {
    //     var boCall = VolunteerManager.GetOpenCallInList(c.Id.GetValueOrDefault(), volunteerId);
    //     return boCall;
    // })
    // .ToList();


    //        // שלב 4: סינון לפי סוג הקריאה, אם הועבר
    //        if (callType.HasValue)
    //        {
    //            boOpenCalls = boOpenCalls.Where(c => c.CallType == callType.Value).ToList();
    //        }

    //        // שלב 5: מיון לפי הפרמטר שהועבר, אם קיים
    //        if (openCallInListEnum.HasValue)
    //        {
    //            boOpenCalls = openCallInListEnum switch
    //            {
    //                OpenCallInListEnum.Id => boOpenCalls.OrderBy(c => c.Id).ToList(),
    //                OpenCallInListEnum.DistanceFromVolunteer => boOpenCalls.OrderBy(c => c.DistanceFromVolunteer).ToList(),
    //                OpenCallInListEnum.OpenTime => boOpenCalls.OrderBy(c => c.OpenTime).ToList(),
    //                _ => boOpenCalls.OrderBy(c => c.Id).ToList() // ברירת מחדל: מיון לפי Id
    //            };
    //        }
    //        else
    //        {
    //            boOpenCalls = boOpenCalls.OrderBy(c => c.Id).ToList();
    //        }

    //        return boOpenCalls;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new BlGetOpenCallException($"Error retrieving open calls: {ex.Message}");
    //    }
    //}


    public IEnumerable<BO.OpenCallInList> GetOpenCall(int id, BO.Calltype? type, BO.OpenCallInListEnum? sortBy)
    {
        DO.Volunteer doVolunteer = _dal.Volunteer.Read(id);
        if (doVolunteer == null)
            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist");

        // Retrieve all calls from the BO
         var doallcall = _dal.Call.ReadAll();
        var allCalls = doallcall.Select(call => CallManager.GetCallInList(call)).ToList();

        BO.Volunteer BOvolunteer = VolunteerManager.GetVolunteer(id); //// לקרוא לולנטיר של עדכון שצריך ליצור
        var BOvolunteer1 =   new BO.Volunteer
        {
            Id = doVolunteer.Id,
            Name = doVolunteer.Name,
            Number_phone = doVolunteer.Number_phone,
            Email = doVolunteer.Email,
            FullCurrentAddress = doVolunteer.FullCurrentAddress,
            Password = doVolunteer.Password,
            Latitude = Tools.GetLatitudeAsync(doVolunteer.FullCurrentAddress).Result,
            Longitude = Tools.GetLongitudeAsync(doVolunteer.FullCurrentAddress).Result,
            Role = (BO.Role)doVolunteer.Role, // המרה ישירה בין ה-Enums
            Active = doVolunteer.Active,
            Distance = doVolunteer.distance,
            DistanceType = (BO.DistanceType)doVolunteer.Distance_Type, // המרה ישירה בין ה-Enums
            TotalHandledCalls = _dal.Assignment.ReadAll().Count(a => a.VolunteerId == doVolunteer.Id && a.EndOfTime == AssignmentCompletionType.TreatedOnTime),
            TotalCancelledCalls = _dal.Assignment.ReadAll().Count(a => a.VolunteerId == doVolunteer.Id &&
                (a.EndOfTime == AssignmentCompletionType.AdminCancelled || a.EndOfTime == AssignmentCompletionType.VolunteerCancelled)), // ביטול עצמי או מהנל
            TotalExpiredCalls = _dal.Assignment.ReadAll().Count(a => a.VolunteerId == doVolunteer.Id && a.EndOfTime == AssignmentCompletionType.Expired),
            CurrentCall = null



        };

        // Retrieve all assignments from the DAL
        var allAssignments = _dal.Assignment.ReadAll();
        double? lonVol = BOvolunteer1.Longitude;
        double? latVol = BOvolunteer1.Latitude;

        // Filter for only "Open" or "Risk Open" status
        IEnumerable<BO.OpenCallInList> filteredCalls = from call in allCalls
                                                       join assignment in allAssignments on call.Id equals assignment.CallId into callAssignments
                                                       from assignment in callAssignments.DefaultIfEmpty()
                                                       where (call.Status == BO.CallStatus.Open || call.Status == BO.CallStatus.OpenAtRisk)
                                                       let boCall = Read(call.CallId)
                                                       select new BO.OpenCallInList
                                                       {
                                                           Id = call.CallId,
                                                           CallType = call.CallType,
                                                           Description = boCall.Description,
                                                           FullAddress = boCall.FullAddress,
                                                           OpenTime = call.OpenTime,
                                                           MaxEndTime = boCall.MaxEndTime,
                                                           DistanceFromVolunteer = BOvolunteer1?.FullCurrentAddress != null ? CallManager.Air_distance_between_2_addresses
                                                           (latVol, lonVol, boCall.Latitude, boCall.Longitude) : 0  // Calculate the distance between the volunteer and the call
                                                       };

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

    //public void UpdateCancelTreatment1(int volunteerId, int assignmentId)
    //{
    //    try
    //    {
    //        var assignment = _dal.Assignment.Read(assignmentId);
    //        var volunteer = _dal.Volunteer.Read(volunteerId);
    //        //var call = _dal.Call.Read(assignment.CallId)
    //        //  ?? throw new BO.Incompatible_ID($"Call with ID {assignment.CallId} was not found.");
    //        var call = CallManager.GetAdd_update_Call(volunteerId);
    //        // Step 2: Check permission to cancel the treatment - Is the volunteer either the one assigned to the call or an admin?
    //        if (assignment.VolunteerId != volunteerId && volunteer.Role != DO.Role.Manager)
    //        {
    //            throw new BO.Bl_Volunteer_Cant_UpdateCancelTreatmentException("The volunteer is not authorized to cancel this treatment.");
    //        }


    //        if (assignment.time_end_treatment != null /*&& assignment.EndOfTime == DO.AssignmentCompletionType.TreatedOnTime*/)
    //        {
    //            throw new BO.BlMaximum_time_to_finish_readingException("The treatment has already been completed and cannot be canceled.");
    //        }
    //        if (call.Status == BO.CallStatus.Expired || call.Status == BO.CallStatus.InProgress)
    //        {
    //            throw new BO.BlCallStatusNotOKException("The call is already closed, expired, or in progress and cannot be updated.");

    //        }


    //        var updatedAssignment = new DO.Assignment(
    //            Id: assignment.Id,  // Keep the existing assignment ID
    //            CallId: assignment.CallId,  // Use the existing call ID
    //            VolunteerId: assignment.VolunteerId,  // Keep the original volunteer ID
    //            time_entry_treatment: assignment.time_entry_treatment,  // Keep the original entry time
    //            time_end_treatment: ClockManager.Now,  // Set the current time as the cancellation time
    //            EndOfTime: (assignment.VolunteerId == volunteerId)
    //                       ? DO.AssignmentCompletionType.VolunteerCancelled  // If the volunteer requested the cancel, set the "Cancelled by Volunteer" status
    //                       : DO.AssignmentCompletionType.AdminCancelled  // If an admin requested the cancel, set the "Cancelled by Admin" status
    //        );

    //        _dal.Assignment.Update(updatedAssignment);
    //    }
    //    catch (DO.DalDoesNotExistException ex)
    //    {
    //        // Exception thrown if the assignment with the specified ID was not found in the DAL
    //        throw new BO.BlDoesNotExistException($"Assignment with ID {assignmentId} was not found.", ex);
    //    }
    //    catch (BO.BlPermissionException ex)
    //    {
    //        // Exception thrown if the volunteer does not have permission to cancel the treatment
    //        throw new BO.BlPermissionException($"Permission denied: {ex.Message}", ex);
    //    }
    //    catch (BO.BlMaximum_time_to_finish_readingException ex)
    //    {
    //        // Exception thrown if the treatment has already been completed and can't be canceled
    //        throw new BO.BlMaximum_time_to_finish_readingException($"Treatment already completed: {ex.Message}", ex);
    //    }
    //    catch (BO.BlCallStatusNotOKException ex)
    //    {
    //        // Exception thrown if the assignment is no longer open and cannot be canceled
    //        throw new BO.BlCallStatusNotOKException($"Assignment cannot be canceled: {ex.Message}", ex);
    //    }
    //    catch (Exception ex)
    //    {
    //        // Catch any other unexpected exceptions
    //        throw new BO.BlGeneralException($"An error occurred while canceling the assignment treatment: {ex.Message}", ex);
    //    }
    //}

    //public void CancelTreat(int idVol, int idAssig)
    //{

    //    DO.Assignment assigmnetToCancel = _dal.Assignment.Read(idAssig) ?? throw new BO.BlDeleteNotPossibleException("there is no assigment with this ID");
    //    bool ismanager = false;
    //    if (assigmnetToCancel.VolunteerId != idVol)
    //    {
    //        if (_dal.Volunteer.Read(idVol).Job == DO.Role.Boss)
    //            ismanager = true;
    //        else throw new BO.BlDeleteNotPossibleException("the volunteer is not manager or not in this call");
    //    }
    //    if (assigmnetToCancel.TypeEndTreat != null || (_dal.Call.Read(assigmnetToCancel.CallId).MaxTimeToClose > ClockManager.Now) | assigmnetToCancel.TimeEnd != null)
    //        throw new BO.BlDeleteNotPossibleException("The assigmnet not open or exspaired");

    //    DO.Assignment assigmnetToUP = new DO.Assignment
    //    {
    //        Id = assigmnetToCancel.Id,
    //        CallId = assigmnetToCancel.CallId,
    //        VolunteerId = assigmnetToCancel.VolunteerId,
    //        TimeStart = assigmnetToCancel.TimeStart,
    //        TimeEnd = ClockManager.Now,
    //        TypeEndTreat = ismanager ? DO.TypeEnd.ManagerCancel : DO.TypeEnd.SelfCancel,
    //    };
    //    try
    //    {
    //        _dal.Assignment.Update(assigmnetToUP);
    //    }
    //    catch (DO.DalExsitException ex)
    //    {
    //        throw new BO.BlDeleteNotPossibleException("canot delete in DO");
    //    }

    //}

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
        if (assigmnetToCancel.time_end_treatment != null || /*(_dal.Call.Read(assigmnetToCancel.CallId).MaxEndTime > ClockManager.Now)*/  assigmnetToCancel.EndOfTime != null)
            throw new BO.BlDeletionImpossibleException("The assigmnet not open or exspaired");

        DO.Assignment assigmnetToUP = new DO.Assignment
        {
            Id = assigmnetToCancel.Id,
            CallId = assigmnetToCancel.CallId,
            VolunteerId = assigmnetToCancel.VolunteerId,
            time_entry_treatment = assigmnetToCancel.time_entry_treatment,
            time_end_treatment = ClockManager.Now,
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



    public void UpdateEndTreatment1(int volunteerId, int assignmentId)

    {
        try
        {
            // Step 1: Retrieve the assignment details from the DAL
            var assignment = _dal.Assignment.Read(assignmentId);//read trow ex.

            // Step 2: Check permission - Is the volunteer the one assigned to this call?
            if (assignment.VolunteerId != volunteerId)
            {
                throw new BO.BlPermissionException("The volunteer is not authorized to update this assignment.");
            }

            // Step 3: Retrieve the call associated with the assignment
            var call = CallManager.GetViewingCall(volunteerId);

            // If the call doesn't exist, throw an exception
            if (call == null)
            {
                throw new BO.BlDoesNotExistException($"Call with ID {assignment.CallId} was not found.");
            }

            // Step 4: Check if the call is open (not closed, expired, or in progress)
            if (call.Status == BO.CallStatus.Closed || call.Status == BO.CallStatus.Expired || call.Status == BO.CallStatus.InProgress)
            {
                throw new BO.BlCallStatusNotOKException("The call is already closed, expired, or in progress and cannot be updated.");
            }

            // Step 5: Check if the treatment end time is already set (i.e., treatment has been completed)
            if (assignment.time_end_treatment != null)
            {
                throw new BO.BlMaximum_time_to_finish_readingException("The treatment has already been completed.");
            }

            // Step 6: Create an updated assignment object with the new values
            var updatedAssignment = new DO.Assignment(
                Id: assignment.Id,  // Use the existing assignment ID
                CallId: assignment.CallId,  // Use the existing call ID
                VolunteerId: assignment.VolunteerId,  // Use the existing volunteer ID
                time_entry_treatment: assignment.time_entry_treatment,  // Keep the original entry time
                time_end_treatment: DateTime.Now,  // Set the current time as the treatment end time
                EndOfTime: DO.AssignmentCompletionType.TreatedOnTime  // Set the completion status to "Treated On Time"
            );

            // Step 7: Update the assignment in the DAL
            _dal.Assignment.Update(updatedAssignment);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            // Exception thrown if assignment or call with the specified ID was not found in the DAL
            throw new BO.BlDoesNotExistException($"Assignment or Call with specified ID was not found.", ex);
        }
        catch (BO.BlPermissionException ex)
        {
            // Exception thrown if the volunteer is not authorized to update the assignment
            throw new BO.BlPermissionException($"Permission denied: {ex.Message}", ex);
        }
        catch (BO.BlCallStatusNotOKException ex)
        {
            // Exception thrown if the call status does not allow treatment completion
            throw new BO.BlCallStatusNotOKException($"Call status issue: {ex.Message}", ex);
        }
        catch (BO.BlMaximum_time_to_finish_readingException ex)
        {
            // Exception thrown if treatment completion has already been reported
            throw new BO.BlMaximum_time_to_finish_readingException($"Treatment already completed: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            // Catch any other unexpected exceptions
            throw new BO.BlGeneralException($"An error occurred while updating the assignment treatment end: {ex.Message}", ex);
        }
    }

    public void UpdateEndTreatment2(int idVol, int idCall)
    {
        DO.Volunteer vol = _dal.Volunteer.Read(idVol) ?? throw new BO.BlNullPropertyException($"there is no volunterr with this ID {idVol}");
        BO.Call bocall = Read(idCall) ?? throw new BO.BlNullPropertyException($"there is no call with this ID {idCall}");
        if (bocall.Status != BO.CallStatus.Open || bocall.Status == BO.CallStatus.OpenAtRisk)

            throw new BO.BlAlreadyExistsException($"the call is open or expired Idcall is={idCall}");


        DO.Assignment assigmnetToCreat = new DO.Assignment
        {
            Id = 0,
            CallId = idCall,
            VolunteerId = idVol,
            time_entry_treatment = ClockManager.Now,
            time_end_treatment = null,
            EndOfTime = null
        };
        try
        {
            _dal.Assignment.Create(assigmnetToCreat);
        }
        catch (Exception)
        { throw new BO.BlAlreadyExistsException("impossible to creat"); }

    }

    public void UpdateEndTreatment(int idVol, int idCall)
    {
        DO.Volunteer vol = _dal.Volunteer.Read(idVol) ?? throw new BO.BlNullPropertyException($"there is no volunterr with this ID {idVol}");
        BO.Call bocall = Read(idCall) ?? throw new BO.BlNullPropertyException($"there is no call with this ID {idCall}");
        if (bocall.Status != BO.CallStatus.Open || bocall.Status == BO.CallStatus.OpenAtRisk)

            throw new BO.BlAlreadyExistsException($"the call is open or expired Idcall is={idCall}");


        DO.Assignment assigmnetToCreat = new DO.Assignment
        {
            Id = 0,
            CallId = idCall,
            VolunteerId = idVol,
            time_entry_treatment = ClockManager.Now,
            time_end_treatment = /*null*/ ClockManager.Now,
            EndOfTime = /*null*/  AssignmentCompletionType.TreatedOnTime
        };
        try
        {
            _dal.Assignment.Create(assigmnetToCreat);
        }
        catch (Exception)
        { throw new BO.BlAlreadyExistsException("impossible to creat"); }

    }


    public void ChooseCall(int VolunteerId, int CallId)
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
                time_entry_treatment = ClockManager.Now, // Time of entry into treatment
                time_end_treatment = null, // Time of actual treatment completion
                EndOfTime = null // End of treatment type
            };

            _dal.Assignment.Create(NewAssignment);

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











    }
