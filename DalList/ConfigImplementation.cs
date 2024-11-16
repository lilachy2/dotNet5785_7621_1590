namespace Dal;
using DalApi;
using System;

/// <param name="NextCallId"> // Property to get the starting Call ID from Config </param>
/// <param name="NextAssignmentId"> // Property to get the starting Assignment ID from Config </param>
/// <param name="RiskRange"> // Property to get and set the RiskRange from Config </param>
/// <param name="Clock"> // Property to get and set the system Clock from Config </param>
/// <param name="Reset()"> // Method to reset configuration values in Config </param>>

public class ConfigImplementation : IConfig
{
     public int NextCallId
        {
            get => Config.startCallId;
        }

        public int NextAssignmentId
        {
            get => Config.startAssignmentId;
        }
         public TimeSpan RiskRange
         {
        get => Config.RiskRange;
        set => Config.RiskRange = value;
         }


    public DateTime Clock
        {
            get => Config.Clock;
            set => Config.Clock = value;
        }

       

        public void Reset()
        {
            Config.Reset();
        }
 }

