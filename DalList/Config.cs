/// <param name="startCallId"> // Defines the starting ID for the Call entity
/// <param name="nextCallId"> // Defines the next available ID for the Call entity
/// <param name="NextCallId"> // Property to retrieve and increment the next available Call ID
/// <param name="startVolunteerId"> // Defines the starting ID for the Volunteer entity
/// <param name="startAssignmentId"> // Defines the starting ID for the Assignment entity
/// <param name="nextAssignmentId"> // Defines the next available ID for the Assignment entity
/// <param name="NextAssignmentId"> // Property to retrieve and increment the next available Assignment ID
/// <param name="Clock"> // System clock representing the current date and time
/// <param name="RiskRange"> // Risk range representing the time window for calls approaching their end time
/// <param name="Reset()"> // Resets configuration values to their starting values

using DO;

namespace Dal;

internal static class Config
{
    internal const int startCallId = 1000;
    private static int nextCallId = startCallId;
    internal static int NextCallId { get => nextCallId = nextCallId + 1; }

    internal const int startVolunteerId = 1000;

    internal const int startAssignmentId = 1000;
    private static int nextAssignmentId = startAssignmentId;
    internal static int NextAssignmentId { get => nextAssignmentId = nextAssignmentId + 1; }

    internal static DateTime Clock { get; set; } = DateTime.Now;

    internal static TimeSpan RiskRange { get; set; } = TimeSpan.FromHours(1);

    internal static void Reset()
    {
        nextCallId = startCallId;
        nextAssignmentId = startAssignmentId;

        Clock = DateTime.Now;
        RiskRange = TimeSpan.FromHours(1);
    }
}
