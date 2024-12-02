using System.Data;

namespace DO;

//ENUM types references are therefore not written

/// <param name="">  </param>
/// <param name="Id"> ---- Running ID number---- Represents a number that uniquely identifies the call. </param>
/// <param name="CallId">   ---- Running ID number---Represents a number that identifies the call that the volunteer chose to handle</param>
/// <param name="VolunteerId"> represents the ID of the volunteer who chose to take care of the reading </param>
/// <param name="time_entry_treatment">  Time (date and time) when the current call was processed. The time when for the first time the current volunteer chose to take care of her.</param>
/// <param name="time_end_treatment"> Time (date and time) when the current volunteer finished handling the current call. </param>
/// <param name="EndOfTime">  The manner in which it ended in the present reading by the present volunteer.</param>
/// <param name="">  </param>
///The department represents an entity that connects a "call" with a "volunteer" who chose to take care of it. The call has
///been assigned for the volunteer. Includes a running and unique ID number of the link entity. 
///as well as the identification number of the call and the volunteer's social security number.

public record Assignment
(
    int Id,
    int CallId,
    int VolunteerId,
    DateTime time_entry_treatment,
    DateTime? time_end_treatment = null,
    AssignmentCompletionType? EndOfTime = default
)
{
    // בנאי ברירת מחדל
    public Assignment() : this(0, 0, 0, default(DateTime)) { }
}

