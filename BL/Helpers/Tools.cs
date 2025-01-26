using BlApi;
using BO;
using DalApi;
using DO;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Threading.Tasks;

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


    // saync - dont remove- for 7 stage


    /// <summary>
    /// Gets the latitude of a given address.
    /// </summary>
    /// <param name="address">The address to process.</param>
    /// <returns>The latitude, or null if the address is invalid or not found.</returns>
    public static async Task<double> GetLatitudeAsync(string address)
    {
        var coordinates = await GetCoordinatesAsync(address);
        return coordinates?.Latitude ?? 0;
    }

    /// <summary>
    /// Gets the longitude of a given address.
    /// </summary>
    /// <param name="address">The address to process.</param>
    /// <returns>The longitude, or null if the address is invalid or not found.</returns>
    public static async Task<double> GetLongitudeAsync(string address)
    {
        var coordinates = await GetCoordinatesAsync(address);
        return coordinates?.Longitude ?? 0;
    }

    /// <summary>
    /// Computes the latitude and longitude of a given address using the Geocode API.
    /// </summary>
    /// <param name="address">The address to process.</param>
    /// <returns>A tuple of latitude and longitude, or null if the address is invalid or not found.</returns>


    public static async Task<(double Latitude, double Longitude)?> GetCoordinatesAsync(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address cannot be empty or null.", nameof(address));

        string query = $"{BaseUrl}?q={Uri.EscapeDataString(address)}&format=json&api_key={ApiKey}";

        try
        {
            using (HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) }) // 5 second timeout
            {
                // Add "User-Agent" header (required in most APIs)
                client.DefaultRequestHeaders.Add("User-Agent", "YourAppName/1.0");
                HttpResponseMessage response = await client.GetAsync(query);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Error in request: {response.StatusCode}");
                }
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                JsonDocument jsonDocument = JsonDocument.Parse(jsonResponse);
                JsonElement results = jsonDocument.RootElement;
                if (results.ValueKind == JsonValueKind.Array && results.GetArrayLength() > 0)
                {
                    JsonElement firstResult = results[0];
                    if (firstResult.TryGetProperty("lat", out JsonElement latElement) &&
                        firstResult.TryGetProperty("lon", out JsonElement lonElement))
                    {
                        if (double.TryParse(latElement.GetString(),
                            System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out double latitude) &&
                            double.TryParse(lonElement.GetString(),
                            System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out double longitude))
                        {
                            return (latitude, longitude);
                        }
                    }
                }
                throw new Exception("No coordinates found for the given address.");
            }
        }
        catch (TimeoutException)
        {
            throw new Exception("The request timed out.");
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Request error: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new Exception($"General error: {ex.Message}");
        }
    }

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

        //Validate the address
        //var isAddressValid = Tools.IsAddressValid(boVolunteer.FullCurrentAddress);
        // if (!isAddressValid)
        // {
        //     throw new ArgumentException("The address provided is invalid.");
        // }

        //var isAddressValid
        var isAddressValid = Tools.IsAddressValidAsync(boVolunteer.FullCurrentAddress);

        if (!isAddressValid.Result)
        {
            throw new ArgumentException("The address provided is invalid.");
        }

    }
 

    private const string ApiKey = "67589f7ea5000746604541qlg6b8a20"; // המפתח API שלך
    private const string BaseUrl = "https://geocode.maps.co/search";

   
    public static async Task<bool> IsAddressValidAsync1(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address cannot be null or empty.");

        string query = $"{BaseUrl}?q={Uri.EscapeDataString(address)}&api_key={ApiKey}";

        try
        {
            using (HttpClient client = new HttpClient())
            {
                // שליחת בקשה אסינכרונית
                HttpResponseMessage response = await client.GetAsync(query);

                // בדיקה אם הסטטוס הצליח
                bool isValid = response.IsSuccessStatusCode;

                Console.WriteLine($"Response Status Code: {response.StatusCode}");
                return isValid;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false; // במקרה של שגיאה, מחזירים false
        }
    }

    private static readonly HttpClient client = new HttpClient()
    {
        Timeout = TimeSpan.FromSeconds(3) // הגדרת זמן timeout (10 שניות)
    };

    public static async Task<bool> IsAddressValidAsync(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address cannot be null or empty.");

        string query = $"{BaseUrl}?q={Uri.EscapeDataString(address)}&api_key={ApiKey}";

        try
        {
            // ביצוע בקשה אסינכרונית עם ConfigureAwait(false) כדי למנוע בעיות עם UI thread
            HttpResponseMessage response = await client.GetAsync(query).ConfigureAwait(false);

            // החזרת True אם הסטטוס הצליח (2xx)
            bool isValid = response.IsSuccessStatusCode;

            Console.WriteLine($"Response Status Code: {response.StatusCode}");
            return isValid;
        }
        catch (Exception ex)
        {
            // הדפסת השגיאה אם יש חריגה
            Console.WriteLine($"Error: {ex.Message}");
            return false; // במקרה של שגיאה מחזירים false
        }
    }


    /// <summary>
    /// Gets the latitude of a given address.
    /// </summary>
    /// <param name="address">The address to process.</param>
    /// <returns>The latitude, or null if the address is invalid or not found.</returns>
    //public static double GetLatitude(string address)
    //{
    //    var coordinates = GetCoordinates(address);
    //    return coordinates?.Latitude ?? 0;
    //}

    ///// <summary>
    ///// Gets the longitude of a given address.
    ///// </summary>
    ///// <param name="address">The address to process.</param>
    ///// <returns>The longitude, or null if the address is invalid or not found.</returns>
    //public static double GetLongitude(string address)
    //{
    //    var coordinates = GetCoordinates(address);
    //    return coordinates?.Longitude ?? 0;
    //}
    /// <summary>
    /// Computes the latitude and longitude of a given address using the Geocode API.
    /// </summary>
    /// <param name="address">The address to process.</param>
    /// <returns>A tuple of latitude and longitude, or null if the address is invalid or not found.</returns>
    //private static (double Latitude, double Longitude)? GetCoordinates(string address)
    //{
    //    if (string.IsNullOrWhiteSpace(address))
    //        throw new ArgumentException("Address cannot be empty or null.", nameof(address));

    //    string query = $"{BaseUrl}?q={Uri.EscapeDataString(address)}&format=json&api_key={ApiKey}";

    //    try
    //    {
    //        using (HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) }) // 5 second timeout
    //        {
    //            // Add "User-Agent" header (required in most APIs)
    //            client.DefaultRequestHeaders.Add("User-Agent", "YourAppName/1.0");
    //            HttpResponseMessage response = client.GetAsync(query).Result;
    //            if (response.StatusCode != HttpStatusCode.OK)
    //            {
    //                throw new Exception($"Error in request: {response.StatusCode}");
    //            }
    //            string jsonResponse = response.Content.ReadAsStringAsync().Result;
    //            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    //            JsonDocument jsonDocument = JsonDocument.Parse(jsonResponse);
    //            JsonElement results = jsonDocument.RootElement;
    //            if (results.ValueKind == JsonValueKind.Array && results.GetArrayLength() > 0)
    //            {
    //                JsonElement firstResult = results[0];
    //                if (firstResult.TryGetProperty("lat", out JsonElement latElement) &&
    //                    firstResult.TryGetProperty("lon", out JsonElement lonElement))
    //                {
    //                    if (double.TryParse(latElement.GetString(),
    //                        System.Globalization.NumberStyles.Any,
    //                        System.Globalization.CultureInfo.InvariantCulture,
    //                        out double latitude) &&
    //                        double.TryParse(lonElement.GetString(),
    //                        System.Globalization.NumberStyles.Any,
    //                        System.Globalization.CultureInfo.InvariantCulture,
    //                        out double longitude))
    //                    {
    //                        return (latitude, longitude);
    //                    }
    //                }
    //            }
    //            throw new Exception("No coordinates found for the given address.");
    //        }
    //    }
    //    catch (TimeoutException)
    //    {
    //        throw new Exception("The request timed out.");
    //    }
    //    catch (HttpRequestException ex)
    //    {
    //        throw new Exception($"Request error: {ex.Message}");
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new Exception($"General error: {ex.Message}");
    //    }
    //}



    internal static void CheckId(int id)
    {
        // Convert the integer ID to a string to process individual digits
        string idString = id.ToString();

        // Ensure the ID is exactly 9 digits long
        if (idString.Length != 9)
        {
            throw new BO.BlWrongItemtException($"This ID {id} is not valid (must be 9 digits long).");
        }

        int sum = 0;

        // Iterate through each digit in the ID
        for (int i = 0; i < 9; i++)
        {
            // Convert the character to its numeric value
            int digit = idString[i] - '0';

            // Determine the multiplier: 1 for odd positions (1, 3, 5, 7, 9), 2 for even positions (2, 4, 6, 8)
            int multiplier = (i % 2 == 0) ? 1 : 2; // Multiplier is 1 for odd positions (index 0, 2, 4...) and 2 for even

            // Multiply the digit by the multiplier
            int product = digit * multiplier;

            // If the result is two digits, sum the digits (e.g., 14 -> 1 + 4)
            if (product > 9)
            {
                product = product / 10 + product % 10; // Sum of digits if the result is two digits
            }

            // Add the processed digit to the total sum
            sum += product;
        }

        // A valid ID has a checksum that is divisible by 10
        if (sum % 10 != 0)
        {
            throw new BO.BlWrongItemtException($"This ID {id} is not valid.");
        }
    }

    public static int TotalHandledCalls(int Id)
    {
        lock (AdminManager.BlMutex) //stage 7
            // Count how many were treated on time
            return _dal.Assignment.ReadAll()
            .Count(a => a.VolunteerId == Id &&
                        a.EndOfTime == AssignmentCompletionType.TreatedOnTime);

    }
    public static int TotalCallsCancelledhelp(int Id)
    {
        lock (AdminManager.BlMutex) //stage 7

            // Count how many were canceled
            return _dal.Assignment.ReadAll()
            .Count(a => a.VolunteerId == Id &&
                        (a.EndOfTime == AssignmentCompletionType.VolunteerCancelled || a.EndOfTime == AssignmentCompletionType.AdminCancelled));

    }
    public static int TotalCallsExpiredelo(int Id)
    {
        lock (AdminManager.BlMutex) //stage 7

            // Count how many were Expired
            return _dal.Assignment.ReadAll()
            .Count(a => a.VolunteerId == Id &&
                        a.EndOfTime == AssignmentCompletionType.Expired);

    }
    public static int? CurrentCallIdhelp(int Id)
    {

        // check CurrentCallId
        lock (AdminManager.BlMutex) //stage 7
         {   var assignment = _dal.Assignment.ReadAll()
                .FirstOrDefault(a => a.VolunteerId == Id && a.time_end_treatment == null);
            return assignment?.CallId;
        }

    }
    public static BO.Calltype CurrentCallType(int Id)
    {
        lock (AdminManager.BlMutex) //stage 7

     {       // בדיקת האם יש קריאה בטיפול
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

}