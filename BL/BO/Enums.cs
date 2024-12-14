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
//public enum CallStatus
//{
//    InProgress,         // Currently handled by the volunteer
//    AtRiskInProgress    // Close to its maximum completion time
//}
public enum CallStatus
{
    Open,            // פתוחה - לא בטיפול של אף מתנדב כרגע
    // לעבור על הסיגמנט עם שאילתה אם הוא קיים .ה . אם כן לבדוק שFINISHING לא סגור וגם 
    InProgress,      // בטיפול - בטיפול כרגע על ידי מתנדב
    Closed,          // סגורה - מתנדב סיים לטפל בה
    Expired,         // פג תוקף - לא נבחרה לטיפול או לא הסתיימה בזמן
    OpenAtRisk,      // פתוחה בסיכון - קריאה פתוחה שמתקרבת לזמן הסיום הדרוש לה
    InProgressAtRisk // בטיפול בסיכון - קריאה בטיפול שמתקרבת לזמן הסיום הדרוש לה
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
    Role
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

