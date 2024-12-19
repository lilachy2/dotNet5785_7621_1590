
using Helpers;

namespace BO;

/// <param name="Id"> // The unique identifier for the assignment entity (not visible in the UI)
/// <param name="CallId"> // The unique identifier for the call entity
/// <param name="CallType"> // The type of the call
/// <param name="OpenTime"> // The time when the call was opened
/// <param name="TimeRemaining"> // The remaining time until the call's maximum completion time
/// <param name="VolunteerName"> // The name of the volunteer assigned to the call (last name)
/// <param name="CompletionTime"> // The total time taken to complete the call (only for completed calls)
/// <param name="Status"> // The current status of the call (open, in progress, closed, expired, etc.)
/// <param name="TotalAssignments"> // The total number of assignments for the call (how many times it was taken, canceled, etc.)

public class CallInList
{
    public int? Id { get; init; }
    public int CallId { get; init; }
    public Calltype CallType { get; init; }
    public DateTime OpenTime { get; init; }
    public TimeSpan? TimeRemaining { get; init; }
    public string? VolunteerName { get; init; }
    public TimeSpan? CompletionTime { get; init; }
    public CallStatus Status { get; init; }
    public int TotalAssignments { get; init; }

    public override string ToString() => this.ToStringProperty();

    //public override string ToString()
    //{
    //    return $"Call ID: {CallId}\n" +
    //           $"Call Type: {CallType}\n" +
    //           $"Opened: {OpenTime:yyyy-MM-dd HH:mm:ss}\n" +
    //           $"Volunteer: {VolunteerName ?? ""}\n" +
    //           $"Status: {Status}\n" +
    //           $"Time Remaining: {(TimeRemaining.HasValue ? TimeRemaining.Value.ToString(@"hh\:mm\:ss") : "")}\n" +
    //           $"Completion Time: {(CompletionTime.HasValue ? CompletionTime.Value.ToString(@"hh\:mm\:ss") : "")}\n" +
    //           $"Total Assignments: {TotalAssignments}";
    //}

}