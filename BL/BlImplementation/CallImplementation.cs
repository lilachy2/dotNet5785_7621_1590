
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
    public List<BO.Call> GetCloseCall(int volunteerId, BO.Calltype? callType, ClosedCallInListEnum? closedCallInListEnum)
    {
        try
        {
            // שלב 1: שליפת כל הקריאות מה-DAL
            var allCalls = _dal.Call.ReadAll().ToList();

            // שלב 2: שליפת פרטי המתנדב
            DO.Volunteer volunteer = _dal.Volunteer.Read(volunteerId)
                ?? throw new BO.Incompatible_ID("Volunteer ID does not exist.");

            // שלב 3: המרת כל קריאה ל-Business Object (BO) בעזרת הפונקציה שסיפקת
            var boCalls = allCalls
                .Select(c =>
                {
                    var boCall = VolunteerManager.GetClosedCallInList(c.Id); // פונקציית המרה שסיפקת
                    return boCall;
                })
                .ToList();

            // שלב 4: סינון לפי סטטוס - רק קריאות סגורות
            //////////////////////// לבדוק ! כי ב OpenCallInList אין שדה של ססטוס 
            var filteredCalls = boCalls
                .Where(c => c.Status == BO.CallStatus.Closed )
                .ToList();

            // שלב 5: סינון לפי סוג הקריאה אם יש ערך ב-callType
            if (callType.HasValue)
            {
                filteredCalls = filteredCalls.Where(c => c.Calltype == callType.Value).ToList();
            }

            // שלב 6: מיון הרשימה לפי הפרמטר השלישי
            if (openCallInListEnum.HasValue)
            {
                filteredCalls = openCallInListEnum switch
                {
                    OpenCallInListEnum.Id => filteredCalls.OrderBy(c => c.Id).ToList(),
                    OpenCallInListEnum.DistanceFromVolunteer => filteredCalls.OrderBy(c => c.DistanceFromVolunteer).ToList(),
                    OpenCallInListEnum.OpenTime => filteredCalls.OrderBy(c => c.OpenTime).ToList(),
                    _ => filteredCalls.OrderBy(c => c.Id).ToList()
                };
            }
            else
            {
                filteredCalls = filteredCalls.OrderBy(c => c.Id).ToList();
            }

            // שלב 7: החזרת התוצאה
            return filteredCalls;
        }
       
    
        catch (Exception ex)
        {
            throw new BlGetCloseCallException($"An error occurred while retrieving closed calls: {ex.Message}");
        }
    }


    // NOT OK



    public void ChooseCall(int id, int callid)
    {
        throw new NotImplementedException();
    }

    //public int[] GetCallStatusesCounts() // לא נכון
    //{
    //    // שליפת רשימת הקריאות משכבת הנתונים
    //    var assignments = _dal.Call.ReadAll();
    //    var assignments1 = CallManager.GetViewingCall();
    //    assignments.

    //    // שימוש ב-GroupBy כדי לקבץ את הקריאות לפי הסטטוס שלהן ולספור כמה יש בכל סטטוס
    //    var groupedassignments = assignments
    //        .GroupBy(call => call.Calltype) // קיבוץ לפי הסטטוס של הקריאה
    //        .ToDictionary(group => group.Key, group => group.Count()); // המרת הקבוצות למילון (סטטוס -> כמות)

    //    // יצירת מערך שבו כל אינדקס מייצג סטטוס מתוך ה-Enum CallStatus
    //    int[] statusCounts = new int[Enum.GetValues<CallStatus>().Length];

    //    // מילוי המערך בכמויות שהתקבלו בקבוצות
    //    foreach (var group in groupedassignments)
    //    {
    //        int statusIndex = (int)group.Key; // הסטטוס מתורגם לאינדקס (מתוך Enum)
    //        statusCounts[statusIndex] = group.Value; // עדכון הערך במערך
    //    }

    //    return statusCounts;
    //}



    public List<BO.Call> GetOpenCallsForVolunteer(int volunteerId, BO.Calltype? callType, OpenCallInListEnum? openCallInListEnum)
    {
        try
        {
            // שלב 1: שליפת כל הקריאות מה-DAL
            var allCalls = _dal.Call.ReadAll().ToList();

            // שלב 2: שליפת פרטי המתנדב
            DO.Volunteer volunteer = _dal.Volunteer.Read(volunteerId)
                ?? throw new BO.Incompatible_ID("Volunteer ID does not exist.");

            // שלב 3: המרת כל קריאה ל-Business Object (BO) בעזרת הפונקציה שסיפקת
            var boCalls = allCalls
                .Select(c =>
                {
                    var boCall = VolunteerManager.GetOpenCallInList(c.Id); // פונקציית המרה שסיפקת
                    return boCall;
                })
                .ToList();

            // שלב 4: סינון לפי סטטוס - רק קריאות פתוחות או פתוחות בסיכון
            //////////////////////// לבדוק ! כי ב OpenCallInList אין שדה של ססטוס 
            var filteredCalls = boCalls
                .Where(c => c.Status == BO.CallStatus.Open || c.Status == BO.CallStatus.OpenAtRisk)
                .ToList();

            // שלב 5: סינון לפי סוג הקריאה אם יש ערך ב-callType
            if (callType.HasValue)
            {
                filteredCalls = filteredCalls.Where(c => c.Calltype == callType.Value).ToList();
            }

            // שלב 6: מיון הרשימה לפי הפרמטר השלישי
            if (openCallInListEnum.HasValue)
            {
                filteredCalls = openCallInListEnum switch
                {
                    OpenCallInListEnum.Id => filteredCalls.OrderBy(c => c.Id).ToList(),
                    OpenCallInListEnum.DistanceFromVolunteer => filteredCalls.OrderBy(c => c.DistanceFromVolunteer).ToList(),
                    OpenCallInListEnum.OpenTime => filteredCalls.OrderBy(c => c.OpenTime).ToList(),
                    _ => filteredCalls.OrderBy(c => c.Id).ToList()
                };
            }
            else
            {
                filteredCalls = filteredCalls.OrderBy(c => c.Id).ToList();
            }

            // שלב 7: החזרת התוצאה
            return filteredCalls;
        }
        catch (Exception ex)
        {
            throw new BlGetOpenCallException($"Error retrieving open calls: {ex.Message}");
        }
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

    public void UpdateEndTreatment(int id, int callid)
    {
        throw new NotImplementedException();
    }
}
