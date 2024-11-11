namespace DalApi;

/// <param name="NextCallId"> // Represents the ID for the next call, increments automatically when a new call is added </param>
/// <param name="NextAssignmentId"> // Represents the ID for the next assignment, increments automatically when a new assignment is added
/// <param name="Clock"> // Simulated system clock, can be initialized and advanced independently of the actual computer clock
/// <param name="RiskRange"> // Time range after which a call is considered at risk, as it nears its required end time
/// <param name=""> // Method to reset the configuration settings</param>
/// <param name="Reset">// Method to reset the configuration settings

//The class represents defining interfaces for the data entities under the DalApi library
public interface IConfig
{
    int NextCallId { get; set; } 
    int NextAssignmentId { get; set; }
    DateTime Clock { get; set; } 
    TimeSpan RiskRange { get; set; } 

    void Reset(); 

}
