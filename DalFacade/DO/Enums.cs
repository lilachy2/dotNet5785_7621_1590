namespace DO;

// Volunteer
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
internal enum AssignmentCompletionType
{
    TreatedOnTime, // was treated in time
    VolunteerCancelled, // self cancellation
    AdminCancelled, // Canceling an administrator
    Expired // Cancellation has expired
}

