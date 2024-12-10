﻿namespace BO;
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
    IsActive,
    Role
}

public enum ClosedCallInListEnum
{

}
public enum OpenCallInListEnum
{

}
public enum DistanceType
{
    Aerial_distance,
    walking_distance,
    driving_distance,
    change_distance_type
}