namespace DO;

/// <summary>
/// Config Entity represents static system-wide configuration settings
/// </summary>
/// <param name="NextCallId"> ---- Running ID number----Represents the next unique ID for a new call. Increments automatically by 1 each time it’s accessed when a new call is added</param>
/// <param name="NextAssignmentId"> ---- Running ID number----Represents the next unique ID for a new assignment entity between a volunteer and a call. Increments automatically by 1 each time it’s accessed when a new assignment entity is added</param>
/// <param name="Clock">Represents the system clock which can be managed independently from the real system clock. Allows the system administrator to initialize and advance the system time</param>
/// <param name="RiskRange">Defines the time range beyond which a call is considered at risk, indicating it’s close to its required end time</param>
public record Config // לעזות אותה סטטית ?
(
    int NextCallId,
    int NextAssignmentId,
    DateTime Clock,
    TimeSpan RiskRange
)
{
    /// <summary>
    /// Default constructor for future stages
    /// </summary>
    public Config() : this(0, 0, DateTime.Now, TimeSpan.Zero) { }
}
