using BO;

/// <param name="DateTime GetClock()"> // Returns the current value of the system clock.
/// <param name="void ForwardClock(BO.TimeUnit unit)"> // Advances the system clock by the specified time unit (minute, hour, day, month, year).
/// <param name="int GetMaxRange()"> // Returns the current value of the "risk time range" configuration variable.
/// <param name="void ResetDB()"> // Resets the database: clears all configuration and data entity lists.
/// <param name="void InitializeDB()"> // Initializes the database: resets it and populates all entities with default initial values.
/// <param name="void SetMaxRange(int maxRange)"> // Sets the "risk time range" configuration variable to a new value.

/// // The interface defines management methods for system administration tasks under the BlApi library.
public interface IAdmin
{
    #region Stage 5
    void AddConfigObserver(Action configObserver);
    void RemoveConfigObserver(Action configObserver);
    void AddClockObserver(Action clockObserver);
    void RemoveClockObserver(Action clockObserver);
    #endregion Stage 5

    DateTime GetClock();
    void ForwardClock(BO.TimeUnit unit);
    TimeSpan GetMaxRange();
    void ResetDB();
    void InitializeDB();
    void SetMaxRange(TimeSpan maxRange);
}