
namespace BlImplementation;
using BlApi;
using BO;
using DO;
using Helpers;
using System;
using System.Collections.Generic;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    // OK
    public int[] GetCallStatusesCounts()
    {
        try
        {
            // Fetching calls from the data layer
            var doCalls = _dal.Call.ReadAll();

            // Converting the calls from DO to BO using your function
            var boCalls = doCalls.Select(doCall => CallManager.GetViewingCall(doCall.Id)).ToList();

            // Grouping the calls by status
            var groupedCalls = boCalls
                .GroupBy(call => call.Status)
                .ToDictionary(group => group.Key, group => group.Count());

            // Creating the result array
            int maxStatusIndex = Enum.GetValues(typeof(CallStatus)).Cast<int>().Max();
            var quantities = new int[maxStatusIndex + 1];

            // Filling the array based on the statuses
            foreach (var group in groupedCalls)
            {
                quantities[(int)group.Key] = group.Value;
            }

            return quantities;
        }
        catch (Exception ex)
        {
            throw new BlDoesNotExistException("Failed to retrieve call quantities by status.", ex);
        }
    }
    public IEnumerable<BO.CallInList> GetCallsList(BO.CallInListField? filterField = null,  object? filterValue = null,BO.CallInListField? sortField = null)
    // Enum value for filtering
    // Filtering value
    // Enum value for sorting
    {
        try
        {
            // Retrieve all calls from the data layer
            var doCalls = _dal.Call.ReadAll();

            // Convert calls to B.O objects using the GetCallInList method
            var callInList = doCalls.Select(call =>
            {
                var lastAssignment = _dal.Assignment
                    .ReadAll()
                    .Where(a => a.CallId == call.Id)
                    .OrderByDescending(a => a.time_entry_treatment)
                    .FirstOrDefault();

                // Ignore the call if there are no assignments
                if (lastAssignment == null) return null;

                return CallManager.GetCallInList(lastAssignment.VolunteerId);
            }).Where(call => call != null).ToList(); // Filter out null records

            // Filter the list by a specific field if filtering parameters are provided
            if (filterField.HasValue && filterValue != null)
            {
                callInList = callInList.Where(call =>
                {
                    switch (filterField)
                    {
                        case BO.CallInListField.Status:
                            return call.Status.Equals((BO.CallStatus)filterValue);
                        case BO.CallInListField.CallType:
                            return call.CallType.Equals((BO.Calltype)filterValue);
                        case BO.CallInListField.VolunteerName:
                            return call.VolunteerName == (string)filterValue;
                        default:
                            return true; // No filtering if the field does not match
                    }
                }).ToList();
            }

            // Sort the list if a sorting parameter is provided
            if (sortField.HasValue)
            {
                callInList = callInList
                    .OrderBy(call =>
                        sortField switch
                        {
                            BO.CallInListField.CallId => (object)call.CallId,
                            BO.CallInListField.OpenTime => (object)call.OpenTime,
                            BO.CallInListField.Status => (object)call.Status,
                            BO.CallInListField.VolunteerName => (object)call.VolunteerName,
                            _ => (object)call.CallId // Default
                        }
                    )
                    .ToList();
            }
            else
            {
                // Default sorting by CallId
                callInList = callInList.OrderBy(call => call.CallId).ToList();
            }

            return callInList;
        }
        catch (Exception ex)
        {
            throw new BO.BlDoesNotExistException("Failed to retrieve calls list.", ex); // לבחור חריגה 
        }
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
        CallManager.IsValideCall(boCall);
        CallManager.IsLogicCall(boCall);
        
        var doCall = CallManager.BOConvertDO_Call(boCall.Id);

        try
        {
            _dal.Call.Create(doCall);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Call with ID={boCall.Id} already exists", ex);
        }

    }
    public void Update(BO.Call boCall)
    {
        try
        {
            CallManager.IsValideCall(boCall);
        CallManager.IsLogicCall(boCall);
        Tools.IsAddressValid(boCall.FullAddress);
        boCall.Latitude = Tools.GetLatitudeAsync(boCall.FullAddress).Result;
        boCall.Longitude = Tools.GetLongitudeAsync(boCall.FullAddress).Result;
       var doCall= CallManager.BOConvertDO_Call(boCall.Id);
            _dal.Call.Update(doCall);
        }
        catch (Exception ex) 
        {
            throw new BO.Incompatible_ID($" There is no call with the number identifying ={boCall.Id}");

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
            var allCalls = _dal.Call.ReadAll().ToList();

            DO.Volunteer volunteer = _dal.Volunteer.Read(volunteerId);

            var boCalls = allCalls
                .Select(c =>
                {
                    var boCall = CallManager.GetCallInList(c.Id); // Conversion function provided
                    return boCall;
                })
                .ToList();

            var filteredCalls = boCalls
                .Where(c => c.Status == BO.CallStatus.Closed)
                .ToList();

            // Filter by volunteer ID - only calls provided by the specific volunteer
            var boClosedCalls = filteredCalls
                 .Where(c => c.Id == volunteerId)
                .Select(c =>
                {
                    var boCall = VolunteerManager.GetClosedCallInList(volunteerId); // Conversion function provided
                    return boCall;
                })
                .ToList();


            // Step 5: Filter by call type if a value is provided for callType
            if (callType.HasValue)
            {
                boClosedCalls = boClosedCalls.Where(c => c.CallType == callType.Value).ToList();
            }

            // Step 6: Sort the list according to the value in closedCallInListEnum
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
                    _ => boClosedCalls.OrderBy(c => c.Id).ToList() // Default if no matching value is found
                };
            }
            else
            {
                // If closedCallInListEnum is not provided, sort by Id
                boClosedCalls = boClosedCalls.OrderBy(c => c.Id).ToList();
            }

            // Step 7: Return the result
            return boClosedCalls;
        }
        catch (Exception ex)
        {
            throw new BlGetCloseCallException($"An error occurred while retrieving closed calls: {ex.Message}");
        }
    }

    public List<BO.OpenCallInList> GetOpenCall(int volunteerId, BO.Calltype? callType, OpenCallInListEnum? openCallInListEnum)
    {
        try
        {
            // Step 1: Retrieve all calls from the DAL
            var allCalls = _dal.Call.ReadAll().ToList();

            // Step 2: Retrieve volunteer details
            DO.Volunteer volunteer = _dal.Volunteer.Read(volunteerId);

            // Step 3: Convert each call to a Business Object (BO) using the provided conversion function
            var boCalls = allCalls
                .Select(c =>
                {
                    var boCall = CallManager.GetCallInList(c.Id); // Conversion function provided
                    return boCall;
                })
                .ToList();

            // Step 4: Filter by status - only open calls or open at risk calls
            var filteredCalls = boCalls
                .Where(c => c.Status == BO.CallStatus.Open || c.Status == BO.CallStatus.OpenAtRisk)
                .ToList();

            var boOpenCalls = filteredCalls
                .Select(c =>
                {
                    var boCall = VolunteerManager.GetOpenCallInList(volunteerId); // Conversion function provided
                    return boCall;
                })
                .ToList();

            // Step 5: Filter by call type if a value is provided for callType
            if (callType.HasValue)
            {
                boOpenCalls = boOpenCalls.Where(c => c.CallType == callType.Value).ToList();
            }

            // Step 6: Sort the list according to the third parameter
            if (openCallInListEnum.HasValue)
            {
                boOpenCalls = openCallInListEnum switch
                {
                    OpenCallInListEnum.Id => boOpenCalls.OrderBy(c => c.Id).ToList(),
                    OpenCallInListEnum.DistanceFromVolunteer => boOpenCalls.OrderBy(c => c.DistanceFromVolunteer).ToList(),
                    OpenCallInListEnum.OpenTime => boOpenCalls.OrderBy(c => c.OpenTime).ToList(),
                    _ => boOpenCalls.OrderBy(c => c.Id).ToList() // Default sorting by Id
                };
            }
            else
            {
                boOpenCalls = boOpenCalls.OrderBy(c => c.Id).ToList();
            }

            // Step 7: Return the result
            return boOpenCalls;
        }
        catch (Exception ex)
        {
            throw new BlGetOpenCallException($"Error retrieving open calls: {ex.Message}");
        }
    }
    public void UpdateCancelTreatment(int volunteerId, int assignmentId)
    {
        try
        {
            // Step 1: Retrieve the assignment details from the DAL
            var assignment = _dal.Assignment.Read(assignmentId);
            var volunteer = _dal.Volunteer.Read(volunteerId);
            //var call = _dal.Call.Read(assignment.CallId)
            //  ?? throw new BO.Incompatible_ID($"Call with ID {assignment.CallId} was not found.");
            var call = CallManager.GetAdd_update_Call(volunteerId);
            // Step 2: Check permission to cancel the treatment - Is the volunteer either the one assigned to the call or an admin?
            if (assignment.VolunteerId != volunteerId && volunteer.Role != DO.Role.Manager)
            {
                throw new BO.Bl_Volunteer_Cant_UpdateCancelTreatmentException("The volunteer is not authorized to cancel this treatment.");
            }

            // Step 3: Check if the treatment is already completed or expired

            if (assignment.time_end_treatment != null /*&& assignment.EndOfTime == DO.AssignmentCompletionType.TreatedOnTime*/)
            {
                throw new BO.BlMaximum_time_to_finish_readingException("The treatment has already been completed and cannot be canceled.");
            }
            if (call.Status == BO.CallStatus.Expired || call.Status == BO.CallStatus.InProgress)
            {
                throw new BO.BlCallStatusNotOKException("The call is already closed, expired, or in progress and cannot be updated.");

            }


            // Step 5: Create the updated assignment with the cancellation time and reason
            var updatedAssignment = new DO.Assignment(
                Id: assignment.Id,  // Keep the existing assignment ID
                CallId: assignment.CallId,  // Use the existing call ID
                VolunteerId: assignment.VolunteerId,  // Keep the original volunteer ID
                time_entry_treatment: assignment.time_entry_treatment,  // Keep the original entry time
                time_end_treatment: ClockManager.Now,  // Set the current time as the cancellation time
                EndOfTime: (assignment.VolunteerId == volunteerId)
                           ? DO.AssignmentCompletionType.VolunteerCancelled  // If the volunteer requested the cancel, set the "Cancelled by Volunteer" status
                           : DO.AssignmentCompletionType.AdminCancelled  // If an admin requested the cancel, set the "Cancelled by Admin" status
            );

            // Step 6: Update the assignment in the DAL
            _dal.Assignment.Update(updatedAssignment);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            // Exception thrown if the assignment with the specified ID was not found in the DAL
            throw new BO.BlDoesNotExistException($"Assignment with ID {assignmentId} was not found.", ex);
        }
        catch (BO.BlPermissionException ex)
        {
            // Exception thrown if the volunteer does not have permission to cancel the treatment
            throw new BO.BlPermissionException($"Permission denied: {ex.Message}", ex);
        }
        catch (BO.BlMaximum_time_to_finish_readingException ex)
        {
            // Exception thrown if the treatment has already been completed and can't be canceled
            throw new BO.BlMaximum_time_to_finish_readingException($"Treatment already completed: {ex.Message}", ex);
        }
        catch (BO.BlCallStatusNotOKException ex)
        {
            // Exception thrown if the assignment is no longer open and cannot be canceled
            throw new BO.BlCallStatusNotOKException($"Assignment cannot be canceled: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            // Catch any other unexpected exceptions
            throw new BO.BlGeneralException($"An error occurred while canceling the assignment treatment: {ex.Message}", ex);
        }
    }

    public void UpdateEndTreatment(int volunteerId, int assignmentId)

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

    public void ChooseCall(int VolunteerId, int CallId)
    {
        try
        {
            // Step 1: Retrieve the assignment details from the DAL
            var call = _dal.Call.Read(CallId); // READ THROW EXPACTION

            var bocall= CallManager.GetViewingCall(CallId);
            if (bocall.Status == CallStatus.Closed || bocall.Status == CallStatus.InProgress || bocall.Status == CallStatus.InProgressAtRisk || bocall.Status == CallStatus.Expired)
            {
                throw new BO.BlCallStatusNotOKException("The call is already closed, expired, or in progress and cannot be choose.");
            }
            var assignment = _dal.Assignment.ReadAll()
                .Where(a => a.CallId == CallId && a.VolunteerId == VolunteerId)
                .FirstOrDefault();
             UpdateEndTreatment(VolunteerId,assignment.Id);
            var NewAssignment = new DO.Assignment
            {
                CallId = CallId, // מזהה קריאה
                VolunteerId = VolunteerId, // מזהה מתנדב
                time_entry_treatment = ClockManager.Now, // זמן כניסה לטיפול (היום לפי שעון המערכת)
                time_end_treatment = null, // זמן סיום טיפול בפועל יהיה null בהתחלה
                EndOfTime = null // סוג טיפול יהיה null בהתחלה
            };
            _dal.Assignment.Create(NewAssignment);
        }
        catch (Exception ex)
        {
          throw new BO.Incompatible_ID($"Call with ID {CallId} was not found.");

        }
    }

}
