namespace BO;
public enum Role
{
    Admin,
    Volunteer
}

public enum TimeUnit
{
    Minute,
    Hour,
    Day,
    Month,
    Year
}
public enum DistanceType
{
    AirDistance,
    WalkingDistance,
    DrivingDistance
}

public enum CallType
{
    Emergency,
    NonEmergency,
    Maintenance,
    None
}

public enum CompletionType
{
    SuccessfullyCompleted,
    SelfCancelled,
    Expired
}
public enum CallStatus
{
    InProgress,         // Currently handled by the volunteer
    AtRiskInProgress    // Close to its maximum completion time
}
public enum CompletionStatus
{
    Handled,    // The call was successfully handled.
    Cancelled,  // The call was cancelled.
    Expired     // The call expired without being handled.
}

public enum CallinlistEnum
{

}
public enum VolInList
{

}

public enum ClosedCallInListEnum
{

}
public enum OpenCallInListEnum
{

}