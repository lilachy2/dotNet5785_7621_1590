
namespace BlImplementation;
using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

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
            // ContainsKey- האם המפתח קיים במילון   
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

    public IEnumerable<BO.CallInList> GetCallsList(BO.Calltype? filter, object? obj, BO.CallInListField? sortBy, BO.CallStatus? statusFilter)
    {
       return CallManager.GetCallsList(filter, obj, sortBy, statusFilter); 
    }

    public BO.Call Read(int callId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

        return CallManager.Read(callId);
    }


    public void Create(BO.Call boCall)
    {
        AdminManager.ThrowOnSimulatorIsRunning(); // stage 7

        // 1. Validate the call format and logic
        CallManager.IsValideCall(boCall);
        CallManager.IsLogicCall(boCall);
        // 4. Asynchronously calculate and update the coordinates
        _ = UpdateCoordinatesForCallAsync(boCall, null);

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
            //_ = UpdateCoordinatesForCallAsync(BOCall); // שליחה של הבקשה בצורה אסינכרונית לחישוב הקואורדינאטות


            lock (AdminManager.BlMutex) //stage 7
                _dal.Call.Update(doCall); // עדכון ראשוני ללא קואורדינאטות
            CallManager.Observers.NotifyListUpdated(); //stage 5   
            CallManager.Observers.NotifyItemUpdated(doCall.Id);  //stage 5



            DO.Assignment? lastAssignment = null;

            lock (AdminManager.BlMutex) //stage 7
                                        //  VolunteerManager.Observers.NotifyListUpdated(); //stage 5   
                lastAssignment = _dal.Assignment
                 .ReadAll()
                 .Where(a => a.CallId == BOCall.Id)
                 .OrderByDescending(a => a.Id) // מיון כך שהאחרון שנוסף יהיה ראשון
                 .FirstOrDefault(); // מחזיר את ההקצאה האחרונה

            //= _dal.Assignment.ReadAll().Where(a => a.CallId == BOCall.Id).FirstOrDefault();
            if (lastAssignment != null)
            {
                VolunteerManager.Observers.NotifyItemUpdated(lastAssignment.VolunteerId);  //stage 5
            }


            _ = UpdateCoordinatesForCallAsync(null, doCall); // שליחה של הבקשה בצורה אסינכרונית לחישוב הקואורדינאטות

            //  VolunteerManager.Observers.NotifyListUpdated(); //stage 5   
            //VolunteerManager.Observers.NotifyItemUpdated(assimat.VolunteerId);  //stage 5

        }
        catch (BlIsLogicCallException ex)
        {
            throw new BO.BlIsLogicCallException($"Error: {ex.Message}", ex);
        }catch (BlInvalidaddress ex)
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

            //var filteredAssignments = from item in volunteerAssignments
            //                          where item.VolunteerId == volunteerId && item.EndOfTime != null
            //                          select item;

            //// אם אתה רוצה להמיר את התוצאה לרשימה, השתמש ב-ToList()
            //var result = filteredAssignments.ToList();

            // כדי שישתנה אוטמטית ולא נצטרך לעשות FORECHE + תמיכה ב LIST 


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
    {
       return CallManager.GetOpenCall(id, type, sortBy);
    }

    public void ChooseCall(int idVol, int idCall)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        CallManager.ChooseCall(idVol, idCall);  
    }
    public void UpdateCancelTreatment(int idVol, int idAssig)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

        CallManager.UpdateCancelTreatment(idVol, idAssig);  
    }

    public void UpdateEndTreatment(int idVol, int idAssig)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

        CallManager.UpdateEndTreatment(idVol, idAssig); 
    }


    // Function to asynchronously update the coordinates of a call
    private async Task UpdateCoordinatesForCallAsync(BO.Call boCall, DO.Call? doCall)
    {
        try
        {
            string FullAddress = null;
            if (doCall == null)
            {
                doCall = CallManager.BOConvertDO_Call(boCall.Id);
                FullAddress = boCall.FullAddress;
            }
            else
            {
                FullAddress = doCall.ReadAddress;
            }
            
            double lat = await Tools.GetLatitudeAsync(FullAddress);
            double loc = await Tools.GetLongitudeAsync(FullAddress);

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
      return await CallManager.UpdateCoordinatesForCallLON(adress);
    }

    public async Task<double> UpdateCoordinatesForCallLAN(string adress)
    {
        return await CallManager.UpdateCoordinatesForCallLAN(adress);

    }





}
