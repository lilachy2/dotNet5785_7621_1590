
namespace BO;

/// <param name="int VolunteerId"> // The ID of the volunteer (nullable)
/// <param name="string VolunteerName"> // The name of the volunteer (nullable)
/// <param name="DateTime AssignedTime"> // The time the volunteer started handling the call
/// <param name="DateTime? CompletionTime"> // The time the handling of the call was completed (nullable)
/// <param name="CompletionStatus CompletionStatus"> // The type of completion status (nullable)


    public class CallAssignInList
    {
        public int? VolunteerId { get; init; }  
        public string? VolunteerName { get; set; }  
        public DateTime EnterTime { get; set; } 
        public DateTime? CompletionTime { get; set; } 
        public /*CompletionStatus?*/  CallAssignmentEnum? CompletionStatus { get; set; } 
    }

   