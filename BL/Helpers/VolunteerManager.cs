
using DalApi;
using System.Text.RegularExpressions;

namespace Helpers;

internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4
    internal static void CheckLogic(BO.Volunteer boVolunteer)
    {
        try
        {
            Tools.CheckId(boVolunteer.Id);
            CheckPhonnumber(boVolunteer.Number_phone);
            CheckEmail(boVolunteer.Email);
            Tools.IsAddressValid(boVolunteer.FullCurrentAddress);

        }
        catch (BO.BlWrongItemtException ex)
        {
            throw new BO.BlWrongItemtException($"the item have logic problem", ex);
        }

    }

    internal static void CheckPhonnumber(string Number_phone)
    {
        if (string.IsNullOrWhiteSpace(Number_phone) || !Regex.IsMatch(Number_phone, @"^0\d{9}$"))
        {
            throw new ArgumentException("PhoneNumber must be a 10-digit number starting with 0.");
        }
    }
    
    internal static void CheckEmail(string Email)
    {
        if (!Regex.IsMatch(Email, @"^(?("")(""[^""]+?""@)|(([0-9a-zA-Z](([\.\-]?)(?![\.\-])))*[0-9a-zA-Z]@))([0-9a-zA-Z][\-0-9a-zA-Z]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,}$"))
        {
            throw new ArgumentException("Invalid Email format.");
        }

    }



}
