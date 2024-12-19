
using Helpers;

namespace BO;
/// <param name="Id"> // Unique identifier for the call
/// <param name="CallType"> // Type of the call
/// <param name="Description"> // Detailed description of the call
/// <param name="FullAddress"> // Full address of the call location
/// <param name="OpenTime"> // Time when the call was opened
/// <param name="MaxCompletionTime"> // Maximum allowed time to complete the call
/// <param name="DistanceFromVolunteer"> // Distance between the volunteer and the call location


public class OpenCallInList
{
    public int Id { get; init; }
    public Calltype CallType { get; init; }
    public string? Description { get; init; }
    public string FullAddress { get; init; }
    public DateTime OpenTime { get; init; }
    public DateTime? MaxEndTime { get; init; }
    public double DistanceFromVolunteer { get; init; }
    public override string ToString() => this.ToStringProperty();

}

