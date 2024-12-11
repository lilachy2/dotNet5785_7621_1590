namespace DO;



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
/// 
/// <param name=""> The class representing the call entity contains details for a "call", including a unique runner ID number. </param>


public record Call
(
    double? Latitude,
    double? Longitude,
    Calltype Calltype,
    int Id ,
    string? VerbalDescription = null,
    string ReadAddress = " ",
    DateTime OpeningTime = default,
    DateTime? MaxEndTime = null
)
{
    public Call() : this(default, default, default, default) { }


    //private global::BO.Calltype calltype;
    //private string? description;
    //private string? fullAddress;
    //private DateTime openTime;

    //public Call(double? latitude, double? longitude, global::BO.Calltype calltype, int id, string? description, string? fullAddress, DateTime openTime, DateTime? maxEndTime)
    //{
    //    Latitude = latitude;
    //    Longitude = longitude;
    //    this.calltype = calltype;
    //    Id = id;
    //    this.description = description;
    //    this.fullAddress = fullAddress;
    //    this.openTime = openTime;
    //    MaxEndTime = maxEndTime;
    //}
}

