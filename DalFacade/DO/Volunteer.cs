using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DO;

public record Volunteer
(
     //ENUM types references are therefore not written
     /// <summary>
     /// Course Entity
     /// </summary>
     /// <param name="Name">the name   </param>
     /// <param name="Id">//represents T.Z. and the standard that identifies the volunteer in a unique way. -   </param>
     /// <param name="Number_phone">// Stands for a standard cell phone. 10 digits only. Starts with the number 0</param>
     /// <param name="Email"> //Represents a valid e-mail address in terms of format. .</param>
     /// <param name="Password</param>
     /// <param name="FullCurrentAddress"></param>
     /// <param name="Latitude">// Latitude - a number indicating how far a point on the Earth is south or north of the equator. /param>
     /// <param name="Longitude"> // Longitude - a number indicating how far a point on Earth is east or west of the equator.
     /// <param name=" Activ>// Is the volunteer active or inactive (retired from the organization).
     /// <param name=" distance> Air distance, walking distance, car travel distance. The default is air distance.

     // The class represents the data entities under the DO directory
     int Id , 
     string Name,
     int Number_phone ,
     string Email ,
     Role Role =Role.Volunteer, // להוסיף 
     distance_type Distance_Type= distance_type.Aerial_distance, // להוסיך
     string? FullCurrentAddress=  null,
      double? Latitude   = null, 
      double? Longitude = null, 
      bool Active =true,
     double? distance = null

)
{
    //1, " ", 0, " "
    public Volunteer() : this(1, "", 0, "" ) { }

}

