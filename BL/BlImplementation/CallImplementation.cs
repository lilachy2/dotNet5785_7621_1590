
namespace BlImplementation;
using BlApi;
using BO;

using Helpers;
using System;
using System.Collections.Generic;

internal class CallImplementation : BlApi.ICall
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
            throw new BO./*BlDoesNotExistException*/("Failed to retrieve calls list.", ex); // לבחור חריגה 
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
                throw new BlCallStatusNotOpenException("A call that is not in open status cannot be deleted.");
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

            DO.Volunteer volunteer = _dal.Volunteer.Read(volunteerId)
                ?? throw new BO.Incompatible_ID("Volunteer ID does not exist.");

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
            DO.Volunteer volunteer = _dal.Volunteer.Read(volunteerId)
                ?? throw new BO.Incompatible_ID("Volunteer ID does not exist.");

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


    // NOT OK


    //public void UpdateEndTreatment(int volunteerId, int assignmentId)
    //{
    //    try
    //    {
    //        // שלב 1: שליפת פרטי ההקצאה מה-DAL
    //        var assignment = _dal.Assignment.Read(assignmentId)
    //            ?? throw new BO.Incompatible_ID("Assignment ID does not exist.");

    //        // שלב 2: בדיקת הרשאה - האם המתנדב הוא זה שהוקצה לקריאה
    //        if (assignment.VolunteerId != volunteerId)
    //        {
    //            throw new BO.Incompatible_ID("Volunteer ID does not exist.");
    //        }
    //        //המרה bocall
    //        var boCall = CallManager.GetViewingCall(volunteerId);
    //        // שלב 5: עדכון סיום טיפול
    //        if(boCall.Status != CallStatus.Closed|| boCall.Status != CallStatus.Expired || boCall.Status != CallStatus.InProgress)
    //        {
    //            var CallAssignInListParameter = CallManager.GetCallAssignInList(assignment.CallId);
    //           if( CallAssignInListParameter.CompletionTime != null)
    //            {
    //                CallAssignInListParameter.CompletionStatus = CallAssignmentEnum.TreatedOnTime;  // שינוי סטטוס הקריאה ל-"טופלה"
    //                CallAssignInListParameter.CompletionTime = DateTime.Now;    // עדכון זמן סיום טיפול בפועל"
    //                // שלב 6: ביצוע העדכון ב-DAL
    //                var NewAssignment = CallManager.GetViewingAssignment(assignmentId);
    //                _dal.Assignment.Update(NewAssignment);

    //            }
    //        }
    //    }
    //    catch (DO.AssignmentNotFoundException ex)
    //    {
    //        // חריגה אם לא נמצאה הקצאה עם מזהה כזה
    //        throw new BO.AssignmentNotFoundException($"Assignment with ID {assignmentId} was not found.", ex);
    //    }
    //    catch (Exception ex)
    //    {
    //        // כל חריגה אחרת
    //        throw new BO.UpdateEndTreatmentException($"An error occurred while updating the assignment treatment end: {ex.Message}", ex);
    //    }
    //}



    public void UpdateEndTreatment(int volunteerId, int assignmentId)
    {
        try
        {
            // שלב 1: שליפת פרטי ההקצאה מה-DAL
            var assignment = _dal.Assignment.Read(assignmentId)
                ?? throw new BO.BlDoesNotExistException($"Assignment with ID {assignmentId} was not found.");

            // שלב 2: בדיקת הרשאה - האם המתנדב הוא זה שהוקצה לקריאה
            if (assignment.VolunteerId != volunteerId)
            {
                throw new BO.BlPermissionException("The volunteer is not authorized to update this assignment.");
            }


           
            var call = CallManager.GetViewingCall(volunteerId);

            if (call == null)
            {
                throw new BO.BlDoesNotExistException($"Call with ID {assignment.CallId} was not found.");
            }
           
            // שלב 4: בדיקת סטטוס הקריאה - האם היא לא סגורה או פג תוקף
            if (call.Status == BO.CallStatus.Closed || call.Status == BO.CallStatus.Expired || call.Status == BO.CallStatus.InProgress)
            {
                throw new BO.BlCallStatusNotOpenException("The call is already closed, expired, or in progress and cannot be updated.");
            }

            // שלב 5: בדיקת אם זמן סיום טיפול בפועל כבר הוזן
            if (assignment.time_end_treatment != null)
            {
                throw new BO.InvalidOperationException("The treatment has already been completed.");
            }

            // שלב 6: עדכון סיום טיפול
            assignment.EndOfTime = DO.AssignmentCompletionType.TreatedOnTime;  // שינוי סטטוס הקריאה ל-"טופלה"
            assignment.time_end_treatment = DateTime.Now;  // עדכון זמן סיום טיפול בפועל

            // שלב 7: ביצוע העדכון ב-DAL
            _dal.Assignment.Update(assignment);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            // חריגה אם לא נמצאה הקצאה או קריאה עם מזהה כזה
            throw new BO.BlDoesNotExistException($"Assignment or Call with specified ID was not found.", ex);
        }
        catch (DO.DalDeletionImpossibleException ex)
        {
            // חריגה במקרה שלא ניתן למחוק/לעדכן את ההקצאה
            throw new BO.BlDeletionImpossibleException($"Deletion or update of the assignment is impossible: {ex.Message}", ex);
        }
        catch (BO.BlPermissionException ex)
        {
            // חריגה במקרה של הרשאה שגויה
            throw new BO.BlPermissionException($"Permission denied: {ex.Message}", ex);
        }
        catch (BO.BlCallStatusNotOpenException ex)
        {
            // חריגה אם סטטוס הקריאה לא מאפשר סיום טיפול
            throw new BO.BlCallStatusNotOpenException($"Call status issue: {ex.Message}", ex);
        }
        catch (BO.BlMaximum_time_to_finish_readingException ex)
        {
            // חריגה אם זמן סיום טיפול כבר הוזן
            throw new BO.BlMaximum_time_to_finish_readingException($"Treatment already completed: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            // כל חריגה אחרת
            throw new BO.BlGeneralException($"An error occurred while updating the assignment treatment end: {ex.Message}", ex);
        }
    }










    public void ChooseCall(int id, int callid)
    {
        throw new NotImplementedException();
    }





    public void CompleteAssignment(int volunteerId, int assignmentId) { }

    public void UpdateCancelTreatment(int id, int callid)
    {
        try
        {
            // שליפת ישות ההקצאה משכבת הנתונים לפי מזהה ההקצאה
            var assignment = _dal.Assignment.Read(callid);

            if (assignment == null)
            {
                // אם לא נמצאה הקצאה עם מזהה כזה, נזרקת חריגה מתאימה
                throw new Exception($"Assignment with ID {callid} does not exist.");
            }

            // בדיקת הרשאה: האם המתנדב שמנסה לעדכן הוא המתנדב שההקצאה שייכת אליו
            if (assignment.VolunteerId != id)
            {
                throw new Exception("Authorization failed: You are not assigned to this task.");
            }
            
            DO.Assignment assignment1 =new DO.Assignment();
            // בדיקה שההקצאה פתוחה
            //if (assignment.time_end_treatment != null || assignment.EndOfTime != CallAssignmentEnum.TreatedOnTime)
            if ((BO.CallAssignmentEnum)assignment.EndOfTime == BO.CallAssignmentEnum.TreatedOnTime)
            {
                throw new Exception("The assignment is not open or has already been completed.");
            }

            // עדכון שדות ההקצאה
            assignment.EndOfTime = CallAssignmentEnum.TreatedOnTime; // עדכון סוג הסיום ל-"טופלה"
            assignment.EndTime = DateTime.Now; // עדכון זמן הסיום לשעון המערכת

            // ניסיון לעדכן את ההקצאה בשכבת הנתונים
            _dal.Assignment.Update(assignment);
        }
        catch (Exception ex)
        {
            // זריקת החריגה מחדש לשכבת התצוגה עם הודעה מתאימה
            throw new Exception($"Error completing assignment: {ex.Message}", ex);
        }
    }

}
