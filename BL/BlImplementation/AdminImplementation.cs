
namespace BlImplementation;
using BlApi;
using DO;
using BO;
using DalApi;
using Helpers;
using System;

internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    #region Stage 5
    public void AddClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers += clockObserver;
    public void RemoveClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers -= clockObserver;
    public void AddConfigObserver(Action configObserver) =>
   AdminManager.ConfigUpdatedObservers += configObserver;
    public void RemoveConfigObserver(Action configObserver) =>
    AdminManager.ConfigUpdatedObservers -= configObserver;
    #endregion Stage 5

    public void ForwardClock(TimeUnit unit)
    {
        DateTime newTime = AdminManager.Now;

        switch (unit)
        {
            case TimeUnit.Minute:
                newTime = AdminManager.Now.AddMinutes(1);
                break;
            case TimeUnit.Hour:
                newTime = AdminManager.Now.AddHours(1);
                break;
            case TimeUnit.Day:
                newTime = AdminManager.Now.AddDays(1);
                break;
            case TimeUnit.Month:
                newTime = AdminManager.Now.AddMonths(1);
                break;
            case TimeUnit.Year:
                newTime = AdminManager.Now.AddYears(1);
                break;
            default:
                newTime = AdminManager.Now;
                break;

        }

        AdminManager.UpdateClock(newTime);
    }
    public DateTime GetClock()
    {
        return AdminManager.Now;
    }

    


    public void InitializeDB()
    {
        //DalTest.Initialization.Do();
        //ClockManager.UpdateClock(ClockManager.Now);

        DalTest.Initialization.Do();
        AdminManager.UpdateClock(AdminManager.Now);
        AdminManager.MaxRange = AdminManager.MaxRange;
    }

    public void ResetDB()
    {
        _dal.ResetDB();
        AdminManager.UpdateClock(AdminManager.Now);
        AdminManager.MaxRange = AdminManager.MaxRange;
    }


    public TimeSpan GetMaxRange() => AdminManager.MaxRange;
    public void SetMaxRange(TimeSpan maxRange) => AdminManager.MaxRange = maxRange;

}

