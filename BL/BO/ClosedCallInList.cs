
using Helpers;

namespace BO;

/// <param name="Id"> // The unique identifier of the call
/// <param name="CallType"> // The type of the call
/// <param name="FullAddress"> // The full address of the call
/// <param name="OpenTime"> // The time when the call was opened
/// <param name="AssignedTime"> // The time when the call was assigned
/// <param name="CompletionTime"> // The actual completion time of the call
/// <param name="CompletionStatus"> // The type of completion status of the call


public class ClosedCallInList
{
    public int Id { get; init; }
    public Calltype CallType { get; init; }
    public string FullAddress { get; init; }
    public DateTime OpenTime { get; init; }
    public DateTime EnterTime { get; init; }
    public DateTime? EndTime { get; init; }
    public /*CompletionType?*/  CallAssignmentEnum? CompletionStatus { get; init; }
    public override string ToString() => this.ToStringProperty();

}