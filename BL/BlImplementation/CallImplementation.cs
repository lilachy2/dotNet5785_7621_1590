
namespace BlImplementation;
using BlApi;
using BO;
using Helpers;
using System.Collections.Generic;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void Create(Call boCall)
    {
        CallManager.IsValideCall(boCall);
        CallManager.IsLogicCall(boCall);
        DO.Call doCall =
        new(
            boCall.Latitude,
            boCall.Longitude,
            (DO.Calltype)boCall.Calltype,
            boCall.Id,
            boCall.Description,
            boCall.FullAddress,
            boCall.OpenTime,
            boCall.MaxEndTime);
        try
        {
            _dal.Call.Create(doCall);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Call with ID={boCall.Id} already exists", ex);
        }
    }
   


    public BO.Call Read(int callId)
    {
        try
        {
            var doCall = _dal.Call.Read(callId)
                       ?? throw new BO.BlDoesNotExistException($"Call with ID={callId} does not exist.");

            var boCall = new BO.Call
            {
                Id = doCall.Id,
                Calltype = (BO.Calltype)doCall.Calltype,
                Description = doCall.VerbalDescription,
                FullAddress = doCall.ReadAddress,
                Latitude = doCall.Latitude,
                Longitude = doCall.Longitude,
                OpenTime = doCall.OpeningTime,
                MaxEndTime = doCall.MaxEndTime,
                Status = (BO.CallStatus)doCall.Calltype,
                CallAssignments = new List<BO.CallAssignInList>() // Placeholder for assignments
            };

            // Request the list of assignments for the call from the data layer
            //var doAssignments = _dal.CallAssignment.GetAssignmentsByCallId(callId);

            //// Build the CallAssignments list within the BO.Call object
            //if (doAssignments != null)
            //{
            //    foreach (var doAssignment in doAssignments)
            //    {
            //        boCall.CallAssignments.Add(new BO.CallAssignInList
            //        {
            //            AssignmentId = doAssignment.Id,
            //            VolunteerId = doAssignment.VolunteerId,
            //            VolunteerName = doAssignment.VolunteerName,
            //            AssignmentTime = doAssignment.AssignmentTime,
            //            Status = (BO.AssignmentStatus)doAssignment.Status
            //        });
            //    }
            //}

            return boCall;
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID={callId} does not exist.", ex);
        }
    }
    public CallInList callInList(CallinlistEnum? CallFilter, object? filter, CallinlistEnum? CallOrder)
    {
        throw new NotImplementedException();
    }

    public void ChooseCall(int id, int callid)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public int[] GetCallStatusesCounts()
    {
        throw new NotImplementedException();
    }

    public List<BO.Call> GetCloseCall(int id, BO.Calltype? callType, ClosedCallInListEnum? closedCallInListEnum)
    {
        throw new NotImplementedException();
    }

    public List<BO.Call> GetOpenCall(int id, BO.Calltype? callType, OpenCallInListEnum? openedCallInListEnum)
    {
        throw new NotImplementedException();
    }

    public void Update(BO.Call boCall)
    {
        throw new NotImplementedException();
    }

    public void UpdateCancelTreatment(int id, int callid)
    {
        throw new NotImplementedException();
    }

    public void UpdateEndTreatment(int id, int callid)
    {
        throw new NotImplementedException();
    }
}
