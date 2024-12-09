using DO;
using Helpers;
namespace BO;

public class Volunteer
{
    public int Id { get; init; } // Volunteer ID
    public string FullName { get; set; } // Full name
    public string PhoneNumber { get; set; } // Mobile phone number
    public string Email { get; set; } // Email address
    public string? CurrentAddress { get; set; } // Current full address
    public string? Password { get; set; } // Current full address
    public double? Latitude { get; set; } // Latitude of the current address
    public double? Longitude { get; set; } // Longitude of the current address
    public Role Role { get; set; } // Role (Admin or Volunteer)
    public bool IsActive { get; set; } // Is the volunteer currently active
    public double? MaxDistanceToCall { get; set; } // Maximum distance to accept a call
    public DistanceType DistancePreference { get; set; } // Distance type preference (e.g., air, walking, driving)
    public int TotalHandledCalls { get; init; } // Total number of handled calls
    public int TotalCancelledCalls { get; init; } // Total number of canceled calls
    public int TotalExpiredCalls { get; init; } // Total number of calls that expired
    public BO.CallInProgress? CurrentCall { get; set; } // Current call in progress handled by the volunteer
    public override string ToString() => this.ToStringProperty();

}