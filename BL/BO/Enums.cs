namespace BO;
public enum Role
{
    Manager,
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

public enum Calltype
{
    fainting,
    birth,
    resuscitation,
    allergy,
    heartattack,
    broken_bone,
    security_event,
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
    Open,            // Open - Not currently being handled by any volunteer
    InProgress,      // In Progress - Currently being handled by a volunteer
    Closed,          // Closed - The volunteer has finished handling the call
    Expired,         // Expired - Not selected for handling or did not complete in time
    OpenAtRisk,      // Open At Risk - The call is open and approaching its required completion time
    InProgressAtRisk // In Progress At Risk - The call is being handled but is approaching its required completion time
}


public enum CompletionStatus
{
    Handled,    // The call was successfully handled.
    Cancelled,  // The call was cancelled.
    Expired     // The call expired without being handled.
}

public enum CallAssignmentEnum
{
    // like assigment 
    TreatedOnTime,
    VolunteerCancelled,
    AdminCancelled,
    Expired

}
public enum VolInList
{
    Id,                // Volunteer ID
    Name,              // Full name
    IsActive,
    None,
}

public enum ClosedCallInListEnum
{
    Id,              // The unique identifier of the call
    CallType,        // The type of the call
    FullAddress,     // The full address of the call
    OpenTime,        // The time when the call was opened
    EnterTime,       // The time when the call was assigned
    EndTime,         // The actual completion time of the call
    CompletionStatus // The type of completion status of the call
}

public enum OpenCallInListEnum
{
    Id,
    CallType,
    Description,
    FullAddress,
    OpenTime,
    MaxEndTime,
    DistanceFromVolunteer
}
public enum DistanceType
{
    Aerial_distance,
    walking_distance,
    driving_distance,
    change_distance_type
}

public enum CallInListField
{
    Id,                 // The unique identifier for the assignment entity
    CallId,             // The unique identifier for the call entity
    CallType,           // The type of the call
    OpenTime,           // The time when the call was opened
    TimeRemaining,      // The remaining time until the call's maximum completion time
    VolunteerName,      // The name of the volunteer assigned to the call
    CompletionTime,     // The total time taken to complete the call
    Status,             // The current status of the call
    TotalAssignments    // The total number of assignments for the call
}

