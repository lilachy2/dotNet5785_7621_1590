namespace Dal;
using DalApi;

/// <param name=" get => Config.start_NextCallId;"> // fun that get the value </param>
/// <param name="">  </param>
/// <param name="">  </param>
/// <param name="">  </param>
/// <param name="">  </param>

internal class ConfigImplementation : IConfig
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

