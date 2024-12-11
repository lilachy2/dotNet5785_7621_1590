
using DalApi;

namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4
    internal static void IsLogicCall(BO.Call boCall)
    {
        // Ensure MaxEndTime is greater than OpenTime.
        if (boCall.MaxEndTime.HasValue && boCall.MaxEndTime.Value <= boCall.OpenTime)
        {
            throw new ArgumentException("MaxEndTime must be greater than OpenTime.");
        }

        // Check that CallType is valid (assuming enums start at 0).
        if (!Enum.IsDefined(typeof(BO.Calltype), boCall.Calltype))
        {
            throw new ArgumentException("Invalid call type.");
        }

        // Validate the status (assuming statuses start at 0).
        if (!Enum.IsDefined(typeof(BO.CallStatus), boCall.Status))
        {
            throw new ArgumentException("Invalid call status.");
        }

    }
    internal static void IsValideCall(BO.Call boCall)
    {
        // Validate that the ID is positive.
        if (boCall.Id <= 0)
        {
            throw new ArgumentException("Call ID must be a positive integer.");
        }

        // Validate that the description is not null or empty.
        if (string.IsNullOrWhiteSpace(boCall.Description))
        {
            throw new ArgumentException("Description cannot be null or empty.");
        }

        // Validate that the latitude is within valid range (-90 to 90).
        if (boCall.Latitude.HasValue && (boCall.Latitude.Value < -90 || boCall.Latitude.Value > 90))
        {
            throw new ArgumentException("Latitude must be between -90 and 90.");
        }

        // Validate that the longitude is within valid range (-180 to 180).
        if (boCall.Longitude.HasValue && (boCall.Longitude.Value < -180 || boCall.Longitude.Value > 180))
        {
            throw new ArgumentException("Longitude must be between -180 and 180.");
        }

        // Validate that the address is not null or empty.
        if (string.IsNullOrWhiteSpace(boCall.FullAddress))
        {
            throw new ArgumentException("Address cannot be null or empty.");
        }

        // Validate the address format using an external API.
        if (!Tools.IsAddressValid(boCall.FullAddress).Result)
        {
            throw new ArgumentException("The address is invalid.");
        }

    }


}
