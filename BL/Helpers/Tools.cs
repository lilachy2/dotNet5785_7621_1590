using DalApi;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Helpers;
// for help fun
internal static class Tools
{
    private static readonly DalApi.IDal _dal = DalApi.Factory.Get;




    public static string ToStringProperty<T>(this T t)
    {
        string str = "";
        foreach (PropertyInfo item in t.GetType().GetProperties())
            str += "\n" + item.Name + ": " + item.GetValue(t, null);
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
        if (!Regex.IsMatch(boVolunteer.Number_phone, @"^0\d{9}$"))
        {
            throw new ArgumentException("PhoneNumber must be 10 digits and start with 0.");
        }

        // Validate the Email field.
        if (!Regex.IsMatch(boVolunteer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            throw new ArgumentException("Invalid Email format.");
        }

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
        var isAddressValid = await Tools.IsAddressValid(boVolunteer.FullCurrentAddress);
        if (!isAddressValid)
        {
            throw new ArgumentException("The address provided is invalid.");
        }
    }



    public static async Task<bool> IsAddressValid(string address)
    {
        string baseUrl = "https://geocode.maps.co/search";
        string query = $"{baseUrl}?q={Uri.EscapeDataString(address)}";

        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(query);
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return !string.IsNullOrWhiteSpace(result) && result.Contains("\"lat\":") && result.Contains("\"lon\":");
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return false;
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

    private static bool HelpCheckdelete(BO.Volunteer volunteer)
    {
        DO.Volunteer doVolunteer = _dal.Volunteer.Read(volunteer.Id);

        return true;


    }


    //public static IEnumerable<DO.Assignment> GetCallsByVolunteerId(int volunteerId)
    //{
    //    try
    //    {
    //        // קריאת רשימת כל הקריאות משכבת הנתונים
    //        var allCalls = _dal.Assignment.ReadAll();

    //        // סינון קריאות לפי מזהה מתנדב
    //        var callsByVolunteer = allCalls.Where(call => call.VolunteerId == volunteerId);

    //        // החזרת הקריאות או אוסף ריק אם אין תוצאות
    //        return callsByVolunteer.Any() ? callsByVolunteer : Enumerable.Empty<DO.Assignment>();
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new DO.DalException($"Failed to retrieve calls for Volunteer ID={volunteerId}.", ex);
    //    }
    //}



    //internal static void CheckLogic(BO.Volunteer boVolunteer)
    //{
    //    try
    //    {
    //        CheckId(boVolunteer.Id);
    //        CheckPhonnumber(boVolunteer.Number_phone);
    //        CheckEmail(boVolunteer.Email);
    //        CheckPassword(boVolunteer.Password);
    //        CheckAddress(boVolunteer);

    //    }
    //    catch (BO.BlWrongItemtException ex)
    //    {
    //        throw new BO.BlWrongItemtException($"the item have logic problem", ex);
    //    }

    //}

}

