
namespace BlImplementation;
using BlApi;
using BO;
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

    public List<Call> GetCloseCall(int id, CallType? callType, ClosedCallInListEnum? closedCallInListEnum)
    {
        throw new NotImplementedException();
    }

    public List<Call> GetOpenCall(int id, CallType? callType, OpenCallInListEnum? openedCallInListEnum)
    {
        throw new NotImplementedException();
    }

    public Call? Read(int id)
    {
        throw new NotImplementedException();
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
