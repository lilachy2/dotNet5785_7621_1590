namespace DO;


//ENUM types references are therefore not written

/// <summary>
/// Course Entity
/// </summary>
/// <param name="id"> ---- Running ID number---- Represents a number that uniquely identifies the call. </param>
/// <param name="Verbal_description">Description of the reading. Detailed details on the reading.  </param>
/// <param name="read_address"> Full and real address in correct format, of the reading location </param>
/// /// <param name="Latitude">// Latitude - a number indicating how far a point on the Earth is south or north of the equator. /param>
/// <param name="Longitude"> // Longitude - a number indicating how far a point on Earth is east or west of the equator.
/// <param name="DateTime"> Time (date and time) when the call was opened by the manager. </param>
/// <param name=""> Time (date and time) by which the reading should be closed. </param>
/// <param name="">  </param>


public record Call
(
    int Id = 0,
    string? VerbalDescription = null,
    string ReadAddress = " ",
    double? Latitude = null,
    double? Longitude = null,
    DateTime OpeningTime = default,
    DateTime? MaxEndTime = null
)
{
    // בנאי ריק הנדרש עבור XmlSerializer
    public Call() : this(0, null, " ", null, null, default, null) { }
}

