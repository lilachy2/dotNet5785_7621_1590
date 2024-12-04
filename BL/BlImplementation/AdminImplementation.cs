
namespace BlImplementation;
using BlApi;
using BO;
using System;

internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void ForwardClock(TimeUnit unit)
    {
        throw new NotImplementedException();
    }

    public DateTime GetClock()
    {
        throw new NotImplementedException();
    }

    public int GetMaxRange()
    {
        throw new NotImplementedException();
    }

    public void InitializeDB()
    {
        throw new NotImplementedException();
    }

    public void ResetDB()
    {
        throw new NotImplementedException();
    }

    public void SetMaxRange(int maxRange)
    {
        throw new NotImplementedException();
    }
}

