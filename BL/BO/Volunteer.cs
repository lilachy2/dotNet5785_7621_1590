using DO;
namespace BO;
public class Volunteer
{
    public int Id { get; init; } // Volunteer ID
    public string Name { get; set; } // Full name (התאמה לשם Name)
    public string Number_phone { get; set; } // Mobile phone number (התאמה לשם Number_phone)
    public string Email { get; set; } // Email address
    public string? FullCurrentAddress { get; set; } // Current full address (התאמה לשם FullCurrentAddress)
    public string? Password { get; set; } // Password
    public double? Latitude { get; set; } // Latitude of the current address
    public double? Longitude { get; set; } // Longitude of the current address
    public Role Role { get; set; } // Role (Admin or Volunteer)
    public bool Active { get; set; } // Is the volunteer currently active (התאמה לשם Active)
    public double? Distance { get; set; } // Maximum distance to accept a call (התאמה לשם distance)
    public DistanceType DistanceType { get; set; } // Distance type preference (e.g., air, walking, driving) 
    public int TotalHandledCalls { get; init; } // Total number of handled calls
    public int TotalCancelledCalls { get; init; } // Total number of canceled calls
    public int TotalExpiredCalls { get; init; } // Total number of calls that expired
    public BO.CallInProgress? CurrentCall { get; set; } // Current call in progress handled by the volunteer
    public override string ToString() => this.ToStringProperty();
}


