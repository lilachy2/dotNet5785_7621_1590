namespace DO;

// Volunteer
/// <param name="TreatedOnTime"> // was treated in time
/// <param name="cancellation">// self cancellation
/// <param name="AdminCancelled"> // Canceling an administrator
/// <param name="Expired"> // Cancellation has expired
/// להשלים !!!
public enum Role
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
public enum AssignmentCompletionType
{
    TreatedOnTime,
    VolunteerCancelled,
    AdminCancelled,
    Expired
}
