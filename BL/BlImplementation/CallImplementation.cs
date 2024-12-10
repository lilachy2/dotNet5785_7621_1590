
namespace BlImplementation;
using BlApi;
using BO;
using Helpers;
using System.Collections.Generic;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public CallInList callInList(CallinlistEnum? CallFilter, object? filter, CallinlistEnum? CallOrder)
    {
        throw new NotImplementedException();
    }

    public void ChooseCall(int id, int callid)
    {
        throw new NotImplementedException();
    }

    public void Create(Call boCall)
    {
        DO.Call doCall =
        new(boCall.Latitude, boCall.Longitude, boCall.Calltype, boCall.Id, boCall.Description, boCall.FullAddress, boCall.OpenTime, boCall.MaxEndTime);
        try
        {
            _dal.Call.Create(doCall);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Call with ID={boCall.Id} already exists", ex);
        }
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public int[] GetCallStatusesCounts()
    {
        throw new NotImplementedException();
    }

    public List<Call> GetCloseCall(int id, Calltype? callType, ClosedCallInListEnum? closedCallInListEnum)
    {
        throw new NotImplementedException();
    }

    public List<Call> GetOpenCall(int id, Calltype? callType, OpenCallInListEnum? openedCallInListEnum)
    {
        throw new NotImplementedException();
    }

    public Call? Read(int id)
    {
        var doCall = _dal.Call.Read(id) ??
    throw new BO.BlDoesNotExistException($"Call with ID={id} does Not exist");
        return new()
        {
            Id = id,
           // Latitude= doCall.Latitude,
           // Longitude= doCall.Longitude,
           //Calltype= doCall.Calltype,
           //Description= doCall.,
           //FullAddress= doCall.adress,
           // OpenTime= doCall.,
           //MaxEndTime= doCall.MaxEndTime,
        };
    }

    public void Update(Call boCall)
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
