﻿using BlApi;
using BO;
using DalApi;
using DO;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Helpers;
internal static class Tools
{
    private static readonly DalApi.IDal _dal = DalApi.Factory.Get; 
   public static string ToStringProperty<T>(this T t)
    {
        if (t == null)
            return string.Empty; // Return an empty string if the object is null.

        string str = "";

        // Iterate through each property of the object
        foreach (PropertyInfo item in t.GetType().GetProperties())
        {
            var value = item.GetValue(t, null);

            // If the property value is null, print the property name with an empty value
            if (value == null)
            {
                str += $"\n{item.Name}: "; // Empty value for null properties
                continue;
            }

            // Check if the value is a collection (IEnumerable), but not a string
            if (value is System.Collections.IEnumerable enumerable && value.GetType() != typeof(string))
            {
                // Format collections by joining their items with a comma
                var items = string.Join(", ", enumerable.Cast<object>());
                str += $"\n{item.Name}: [{items}]"; // Display the items of the collection
            }
            else
            {
                // For other values, print the value directly
                str += $"\n{item.Name}: {value}";
            }
        }
        return str;
    }

    public static string ToStringPropertyArray<T>(this T[] t)
    {
        string str = "";
        foreach (var elem in t)
        {
            foreach (PropertyInfo item in t.GetType().GetProperties())
                str += "\n" + item.Name + ": " + item.GetValue(t, null);
        }
        return str;
    }


    /// <summary>
    /// Checks if an address is valid using the Geocode API.
    /// </summary>
    /// <param name="address">The address to validate.</param>
    /// <returns>True if the address is valid, otherwise false.</returns>

    internal static async Task ValidateVolunteerData(BO.Volunteer boVolunteer)
    {
        // Validate the ID of the volunteer.
        if (boVolunteer.Id <= 0 || boVolunteer.Id.ToString().Length < 8 || boVolunteer.Id.ToString().Length > 9)
        {
            throw new ArgumentException("Invalid ID. It must be 8-9 digits.");
        }

        // Validate the FullName field.
        if (string.IsNullOrWhiteSpace(boVolunteer.Name))
        {
            throw new ArgumentException("Name cannot be null or empty.");
        }

        // Validate the PhoneNumber field.
        VolunteerManager.CheckPhonnumber(boVolunteer.Number_phone);


        // Validate the Email field.
        VolunteerManager.CheckEmail(boVolunteer.Email);

        // Validate the Latitude field.
        if (boVolunteer.Latitude.HasValue && (boVolunteer.Latitude.Value < -90 || boVolunteer.Latitude.Value > 90))
        {
            throw new ArgumentException("Latitude must be between -90 and 90.");
        }

        // Validate the Longitude field.
        if (boVolunteer.Longitude.HasValue && (boVolunteer.Longitude.Value < -180 || boVolunteer.Longitude.Value > 180))
        {
            throw new ArgumentException("Longitude must be between -180 and 180.");
        }

        // Validate the address
        var isAddressValid = /*await*/ Tools.IsAddressValid(boVolunteer.FullCurrentAddress);
        if (!isAddressValid)
        {
            throw new ArgumentException("The address provided is invalid.");
        }
    }

    private const string ApiKey = "67589f7ea5000746604541qlg6b8a20"; // המפתח API שלך
    private const string BaseUrl = "https://geocode.maps.co/search";

    public static bool IsAddressValid(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address cannot be null or empty.");

        // בניית ה-URL לבקשה
        string query = $"{BaseUrl}?q={Uri.EscapeDataString(address)}&api_key={ApiKey}";

        bool isValid = false; // משתנה בוליאני

        using (HttpClient client = new HttpClient())
        {
            try
            {
                // שליחת הבקשה באופן סינכרוני
                HttpResponseMessage response = client.GetAsync(query).Result;

                Console.WriteLine($"Response Status Code: {response.StatusCode}");

                // בדיקה אם הסטטוס הצליח
                isValid = response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                isValid = false;
            }
        }

        return isValid; // החזרת התוצאה
    }

    /// <summary>
    /// Gets the latitude of a given address.
    /// </summary>
    /// <param name="address">The address to process.</param>
    /// <returns>The latitude, or null if the address is invalid or not found.</returns>
    public static async Task<double> GetLatitudeAsync(string address)
    {
        var coordinates = await GetCoordinatesAsync(address);
        return coordinates?.Latitude?? 0;
    }

    /// <summary>
    /// Gets the longitude of a given address.
    /// </summary>
    /// <param name="address">The address to process.</param>
    /// <returns>The longitude, or null if the address is invalid or not found.</returns>
    public static async Task<double> GetLongitudeAsync(string address)
    {
        var coordinates = await GetCoordinatesAsync(address);
        return coordinates?.Longitude ?? 0 ;
    }

    /// <summary>
    /// Computes the latitude and longitude of a given address using the Geocode API.
    /// </summary>
    /// <param name="address">The address to process.</param>
    /// <returns>A tuple of latitude and longitude, or null if the address is invalid or not found.</returns>
    private static async Task<(double Latitude, double Longitude)?> GetCoordinatesAsync(string address)
    {
        string query = $"{BaseUrl}?q={Uri.EscapeDataString(address)}";

        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(query);
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();

                    JsonDocument jsonDocument = JsonDocument.Parse(result);
                    JsonElement results = jsonDocument.RootElement;

                    if (results.GetArrayLength() > 0)
                    {
                        JsonElement firstResult = results[0];
                        double latitude = double.Parse(firstResult.GetProperty("lat").GetString());
                        double longitude = double.Parse(firstResult.GetProperty("lon").GetString());
                        return (latitude, longitude);
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
    internal static void CheckId(int id)
    {
        // Convert the integer ID to a string to process individual digits
        string idString = id.ToString();

        // Ensure the ID is exactly 9 digits long
        if (idString.Length != 9)
        {
            throw new BO.BlWrongItemtException($"this ID {id} does not posssible");
        }

        int sum = 0;

        // Iterate through each digit in the ID
        for (int i = 0; i < 9; i++)
        {
            // Convert the character to its numeric value
            int digit = idString[i] - '0';

            // Determine the multiplier: 1 for odd positions, 2 for even positions
            int multiplier = (i % 2) + 1;

            // Multiply the digit by the multiplier
            int product = digit * multiplier;

            // If the result is two digits, sum the digits (e.g., 14 -> 1 + 4)
            if (product > 9)
            {
                product = product / 10 + product % 10;
            }

            // Add the processed digit to the total sum
            sum += product;
        }

        // תעודת זהות תקינה אם סכום ספרות הביקורת מתחלק ב-10
        if (sum % 10 != 0)
        {
            throw new BO.BlWrongItemtException($"this ID {id} does not posssible");
        }
    }
    public static int TotalHandledCalls(int Id)
    {
        // Count how many were treated on time
        return _dal.Assignment.ReadAll()
            .Count(a => a.VolunteerId == Id &&
                        a.EndOfTime == AssignmentCompletionType.TreatedOnTime);

    }
    public static int TotalCallsCancelledhelp(int Id)
    {
        // Count how many were canceled
        return _dal.Assignment.ReadAll()
            .Count(a => a.VolunteerId == Id &&
                        a.EndOfTime == AssignmentCompletionType.VolunteerCancelled || a.EndOfTime == AssignmentCompletionType.AdminCancelled);

    }
    public static int TotalCallsExpiredelo(int Id)
    {
        // Count how many were Expired
        return _dal.Assignment.ReadAll()
            .Count(a => a.VolunteerId == Id &&
                        a.EndOfTime == AssignmentCompletionType.Expired);

    }
    public static int? CurrentCallIdhelp(int Id)
    {
        // check CurrentCallId
        var assignment = _dal.Assignment.ReadAll()
                .FirstOrDefault(a => a.VolunteerId == Id && a.time_end_treatment == null);
        return assignment?.CallId;

    }
    public static BO.Calltype CurrentCallType(int Id)
    {

        // בדיקת האם יש קריאה בטיפול
        var assignment = _dal.Assignment.ReadAll()
            .FirstOrDefault(a => a.VolunteerId == Id && a.time_end_treatment == null);

        // אם לא קיימת קריאה בטיפולו, מחזיר None
        if (assignment == null)
        {
            return BO.Calltype.None;
        }
        var call = _dal.Call.ReadAll()
           .FirstOrDefault(c => c.Id == assignment.CallId).Calltype;
        return (BO.Calltype)call;

    }

}

