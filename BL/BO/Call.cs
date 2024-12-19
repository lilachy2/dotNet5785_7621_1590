
using Helpers;

namespace BO;

/// <param name="Id"> // The unique identifier for the call entity
/// <param name="CallType"> // The type of the call
/// <param name="Description"> // A textual description of the call
/// <param name="FullAddress"> // The full address of the call
/// <param name="Latitude"> // Latitude coordinate of the address
/// <param name="Longitude"> // Longitude coordinate of the address
/// <param name="OpenTime"> // The time when the call was opened
/// <param name="MaxCompletionTime"> // The maximum time allowed to complete the call
/// <param name="Status"> // The current status of the call (open, in progress, closed, expired, etc.)
/// <param name="CallAssignments"> // A list of call assignments for this call (if any)

public class Call
{
    public int Id { get; init; }
    public Calltype Calltype { get; set; }  
    public string? Description { get; set; } 
    public string FullAddress { get; set; } 
    public double Latitude { get; set; }  
    public double Longitude { get; set; } 
    public DateTime OpenTime { get; set; }  
    public DateTime? MaxEndTime { get; set; } 
    public CallStatus Status { get; set; } 
    public List<BO.CallAssignInList>? CallAssignments { get; set; }  // list!!
    public override string ToString() => this.ToStringProperty();
}