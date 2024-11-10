namespace DO;

//ENUM types references are therefore not written

/// <param name="">  </param>
/// <param name="id"> ---- Running ID number---- Represents a number that uniquely identifies the call. </param>
/// <param name="CallId">  Represents a number that identifies the call that the volunteer chose to handle</param>
/// <param name="VolunteerId"> represents the ID of the volunteer who chose to take care of the reading </param>
/// <param name="time_entry_treatment">  Time (date and time) when the current call was processed. The time when for the first time the current volunteer chose to take care of her.</param>
/// <param name="time_end_treatment"> Time (date and time) when the current volunteer finished handling the current call. </param>
/// <param name="">  </param>
/// <param name="">  </param>

public record Assignment
 (
      DateTime time_entry_treatment,
        DateTime? time_end_treatment,
        int Id = 0,
        int CallId = 0,
        int VolunteerId = 0
      
 )
{
    public Assignment() : this(default, default) { }
}
