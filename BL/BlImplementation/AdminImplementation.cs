
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
    // הרשמה או ביטול הרשמה לאירוע של שעון
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
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
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

        //DalTest.Initialization.Do();
        //AdminManager.UpdateClock(AdminManager.Now);
        //AdminManager.MaxRange = AdminManager.MaxRange;

        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.InitializeDB(); //stage 7

    }

    public void ResetDB()
    {
        //_dal.ResetDB();
        //AdminManager.UpdateClock(AdminManager.Now);
        //AdminManager.MaxRange = AdminManager.MaxRange;

        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.ResetDB(); //stage 7

    }


    public TimeSpan GetMaxRange() => AdminManager.MaxRange;
    public void SetMaxRange(TimeSpan maxRange)
    {
        if (maxRange <= TimeSpan.FromHours(0))
        {
            throw new BlRiskRangException("Risk Range must be positive"); 
        }

        AdminManager.MaxRange = maxRange;

    }


    public void StartSimulator(int interval)  //stage 7
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.Start(interval); //stage 7
    }
public void StopSimulator()
    => AdminManager.Stop(); //stage 7
}

