namespace BO;
//public enum Role
//{
//    Admin,
//    Volunteer
//}

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

public enum Calltype
{
    fainting,
    birth,
    resuscitation,
    allergy,
    heartattack,
    broken_bone,
    security_event
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
    Id,                // Volunteer ID
    Name,              // Full name
    Number_phone,      // Mobile phone number
    Email,             // Email address
    FullCurrentAddress, // Current full address
    Password,          // Password
    Latitude,          // Latitude of the current address
    Longitude,         // Longitude of the current address
    Role,              // Role (Admin or Volunteer)
    Active,            // Is the volunteer currently active
    Distance,          // Maximum distance to accept a call
    DistanceType,      // Distance type preference
    TotalHandledCalls, // Total number of handled calls
    TotalCancelledCalls, // Total number of canceled calls
    TotalExpiredCalls, // Total number of calls that expired
    CurrentCall        // Current call in progress handled by the volunteer

}

public enum ClosedCallInListEnum
{

}
public enum OpenCallInListEnum
{

}