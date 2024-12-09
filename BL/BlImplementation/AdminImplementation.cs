
namespace BlImplementation;
using BlApi;
using BO;
using DalApi;
using Helpers;
using System;

internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void UpdateClock(TimeUnit unit)
    {
        DateTime newTime = ClockManager.Now;

        switch (unit)
        {
            case TimeUnit.Minute:
                newTime = ClockManager.Now.AddMinutes(1);
                break;
            case TimeUnit.Hour:
                newTime = ClockManager.Now.AddHours(1);
                break;
            case TimeUnit.Day:
                newTime = ClockManager.Now.AddDays(1);
                break;
            case TimeUnit.Month:
                newTime = ClockManager.Now.AddMonths(1);
                break;
            case TimeUnit.Year:
                newTime = ClockManager.Now.AddYears(1);
                break;
            default:
                throw new ArgumentException("Invalid TimeUnit value");
        }

        ClockManager.UpdateClock(newTime);
    }


    public DateTime GetClock()
    {
        return ClockManager.Now;
    }

    public TimeSpan GetMaxRange()
    {
        return _dal.Config.RiskRange;
    }


    public void InitializeDB()
    {
        ResetDB();
        DalTest.Initialization.Do();
        ClockManager.UpdateClock(ClockManager.Now);
    }

    public void ResetDB()
    {
        _dal.ResetDB();

    }

    public void SetMaxRange(TimeSpan maxRange)
    {
        _dal.Config.RiskRange = maxRange;
    }
}

