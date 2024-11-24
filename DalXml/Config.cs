namespace Dal;

/// <param name="GetAndIncreaseConfigIntVal"> // Increases the value by one and returns it
/// <param name="Config"> // Internal configuration for managing IDs, dates, and ranges
/// <param name="NextCallId"> // Gets the next call ID and increments it
/// <param name="NextAssignmentID"> // Gets the next assignment ID and increments it
/// <param name="Clock"> // Represents the system clock
/// <param name="RiskRange"> // Represents the risk range date
/// <param name="Reset"> // Resets configuration to default values

internal static class Config
{
    internal const string s_data_config_xml = "data-config";
    internal const string s_Assignments_xml = "Assignments.xml";
    internal const string s_Volunteers_xml = "Volunteers.xml";
    internal const string s_Calls_xml = "Calls.xml";

    internal static int NextCallId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCallId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallId", value);
    }

    internal static int NextAssignmentID
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCallId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallId", value);
    }

    internal static DateTime Clock
    {
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }

    internal static DateTime RiskRange
    {
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "RiskRange");
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "RiskRange", value);
    }

    internal static void Reset()
    {
        NextCallId = 1000;
        NextAssignmentID = 1000;

        Clock = DateTime.Now;
        RiskRange = DateTime.Now;
    }
}
