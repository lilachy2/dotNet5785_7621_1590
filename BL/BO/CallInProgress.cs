namespace BO;

public class CallInProgress
{
    public int Id { get; init; } // Unique ID of the assignment (not displayed in the UI)
    public int CallId { get; init; } // Unique ID of the call
    public Calltype CallType { get; init; } // Type of the call (ENUM)
    public string? Description { get; init; } // Detailed description of the call
    public string FullAddress { get; init; } // Full address of the call
    public DateTime OpenTime { get; init; } // Time when the call was opened
    public DateTime? MaxCompletionTime { get; init; } // Maximum allowed completion time
    public DateTime EnterTime { get; init; } // Time the volunteer started handling the call
    public double DistanceFromVolunteer { get; init; } // Distance between the volunteer and the call
    public CallStatus Status { get; init; } // Current status of the call (ENUM)

}