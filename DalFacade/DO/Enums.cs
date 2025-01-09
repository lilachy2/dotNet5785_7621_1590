namespace DO;
/// <param name="TreatedOnTime"> // was treated in time
/// <param name="Volunteer"> // Role of the Volunteer
/// <param name="Manager"> // Role of the Manager
/// <param name="cancellation"> // self cancellation
/// <param name="AdminCancelled"> // Canceling an administrator
/// <param name="Expired"> // Cancellation has expired
/// <param name="Aerial_distance"> // Distance type: Aerial distance
/// <param name="walking_distance"> // Distance type: Walking distance
/// <param name="driving_distance"> // Distance type: Driving distance
/// <param name="change_distance_type"> // Distance type: Change distance type
/// <param name="fainting"> // Call type: Fainting
/// <param name="birth"> // Call type: Birth
/// <param name="resuscitation"> // Call type: Resuscitation
/// <param name="allergy"> // Call type: Allergy
/// <param name="heartattack"> // Call type: Heart attack
/// <param name="broken_bone"> // Call type: Broken bone
/// <param name="security_event"> // Call type: Security event
/// <param name="TreatedOnTime"> // Assignment completion: Treated on time
/// <param name="VolunteerCancelled"> // Assignment completion: Volunteer cancelled
/// <param name="AdminCancelled"> // Assignment completion: Admin cancelled
/// <param name="Expired"> // Assignment completion: Expired
public enum Role
{
    Manager,
    Volunteer
}

public enum distance_type
{
    Aerial_distance,
    walking_distance,
    driving_distance,
    change_distance_type
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
public enum AssignmentCompletionType
{
    TreatedOnTime, 
    VolunteerCancelled, // self cancellation
    AdminCancelled, // Canceling an administrator
    Expired,
        Open
}
