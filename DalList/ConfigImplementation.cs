namespace Dal;
using DalApi;
using System;
using System.Runtime.CompilerServices;

/// <param name="NextCallId"> // Property to get the starting Call ID from Config </param>
/// <param name="NextAssignmentId"> // Property to get the starting Assignment ID from Config </param>
/// <param name="RiskRange"> // Property to get and set the RiskRange from Config </param>
/// <param name="Clock"> // Property to get and set the system Clock from Config </param>
/// <param name="Reset()"> // Method to reset configuration values in Config </param>>

internal class ConfigImplementation : IConfig
{
     public int NextCallId
        {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => Config.startCallId;
        }

        public int NextAssignmentId
        {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => Config.startAssignmentId;
        }
         public TimeSpan RiskRange
         {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => Config.RiskRange;
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => Config.RiskRange = value;
         }


    public DateTime Clock
        {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => Config.Clock;
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => Config.Clock = value;
        }


    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Reset()
        {
            Config.Reset();
        }
 }

