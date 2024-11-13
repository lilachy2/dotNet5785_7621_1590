namespace DO;

// Volunteer
/// <param name="TreatedOnTime"> // was treated in time
/// <param name="cancellation">// self cancellation
/// <param name="AdminCancelled"> // Canceling an administrator
/// <param name="Expired"> // Cancellation has expired
/// להשלים !!!
public enum role
{
    Volunteer,
    Manager
}

public enum distance_type
{
    Aerial_distance,
    walking_distance,
    driving_distance,
    change_distance_type
}

// Call
// להחליט ביחד 

// Assignment
//internal enum AssignmentCompletionType
//{
//    TreatedOnTime, 
//    VolunteerCancelled, 
//    AdminCancelled, 
//    Expired 
//}
public enum FinishType
{
    Treated, SelfCancel, ManagerCancel, ExpiredCancel
}

internal enum AssignmentCompletionType
{
    TreatedOnTime, // was treated in time
    VolunteerCancelled, // self cancellation
    AdminCancelled, // Canceling an administrator
    Expired // Cancellation has expired
}
