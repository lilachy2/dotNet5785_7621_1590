namespace BO;
public class VolunteerInList
{

    /// <param name="int Id"> // The unique identifier of the volunteer
    /// <param name="string FullName"> // The full name of the volunteer (first and last name)
    /// <param name="bool IsActive"> // Indicates whether the volunteer is currently active
    /// <param name="int TotalCallsHandled"> // The total number of calls handled by the volunteer
    /// <param name="int TotalCallsCancelled"> // The total number of calls cancelled by the volunteer
    /// <param name="int TotalCallsExpired"> // The total number of calls that expired under the volunteer
    /// <param name="int? CurrentCallId"> // The ID of the call currently being handled by the volunteer (if any)
    /// <param name="CallType CurrentCallType"> // The type of the call currently being handled by the volunteer
    /// <param name="CallType enum"> // Types of calls (None, Emergency, Regular, Other)

        public int Id { get; init; }
        public string FullName { get; init; }
        public bool IsActive { get; init; }
        public int TotalCallsHandled { get; init; }
        public int TotalCallsCancelled { get; init; }
        public int TotalCallsExpired { get; init; }
        public int? CurrentCallId { get; init; }
        public Calltype CurrentCallType { get; init; }
    


}




