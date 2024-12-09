
namespace BlImplementation;
using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;
using System.Collections.Generic;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public DO.Role PasswordEntered(/*string */int Name, string password)
    {
        /// <summary>
        /// Verifies the user's credentials and returns their role if valid.
        /// Throws exceptions if the user does not exist or the password is incorrect.
        /// </summary>

        var volunteer = _dal.Volunteer.Read(Name);
        if (volunteer == null)

            throw new BO.BlDoesNotExistException("The user does not exist");

        if (volunteer.Password != password)

            throw new BO.BlIncorrectPasswordException("The password is incorrect");

        return volunteer.Role;
    }


    public BO.Volunteer Volunteer_details(int id)
    {
        try
        {
            var doVolunteer = _dal.Volunteer.Read(id);

            var boVolunteer = new BO.Volunteer
            {
                Id = doVolunteer.Id,
                Name = doVolunteer.Name,
                Number_phone = doVolunteer.Number_phone,
                Email = doVolunteer.Email,
                Password = doVolunteer.Password,
                FullCurrentAddress = doVolunteer.FullCurrentAddress,
                Latitude = doVolunteer.Latitude,
                Longitude = doVolunteer.Longitude,
                Role = doVolunteer.Role,
                Active = doVolunteer.Active,
                //distance = doVolunteer.distance,
                //DistancePreference = (BO.DistanceType)doVolunteer.Distance_Type,

                //TotalHandledCalls = 0,
                //TotalCancelledCalls = 0,
                //TotalExpiredCalls = 0,

                //CurrentCall = null
            };
            // Optionally fetch and set the current call in progress (if exists)

            var doCall = _dal.Call.Read(id);

            boVolunteer.CurrentCall = new BO.CallInProgress
            {
                CallId = doCall.Id,
                Description = doCall.VerbalDescription,
                //Status = (BO.CallStatus)doCall.,
                CallType = (BO.CallType)doCall.Calltype,
                //StartTime = doCall.OpeningTime,
                //EndTime = doCall.MaxEndTime
            };
            return boVolunteer;
        }

        // למטה לא זורק חריגה ב DAL לבדוק איזה טיפוס האם DO זורק
        catch (DO.DalDeletionImpossible ex)
        {
            throw new DO.DalDoesNotExistException("\"Student with ID={id} already exists\"");
        }
    }

    public void Create(BO.Volunteer boVolunteer)
    {

        DO.Volunteer doVolunteer = new DO.Volunteer(
      boVolunteer.Id,
      boVolunteer.Name,
      boVolunteer.Number_phone,
      boVolunteer.Email,
      boVolunteer.Password,
      boVolunteer.Role,
      boVolunteer.DistanceType,
      boVolunteer.Active,
      boVolunteer.FullCurrentAddress ?? string.Empty, // אם FullCurrentAddress הוא null, נשתמש במיתר ריק
      boVolunteer.Latitude ?? 0, // אם Latitude הוא null, נשתמש ב-0
      boVolunteer.Longitude ?? 0, // אם Longitude הוא null, נשתמש ב-0
      boVolunteer.Distance ?? 0 // אם Distance הוא null, נשתמש ב-0
  );



        try
        {
            _dal.Volunteer.Create(doVolunteer);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException DalAlreadyExistsException($"Student with ID={boVolunteer.Id} already exists", ex);
        }


    }

    public void Delete(int id)
    {
        try
        { // בשביל אם נזרק חריגה מ READ
            var volunteers = _dal.Volunteer.Read(id);
            if (// לא יכול למחוק  )
                throw //

            var volunteers1 = _dal.Volunteer.Delete(id);
        }
        catch { }




    }




    public BO.Volunteer? Read(int id)
    {
        throw new NotImplementedException();
    }

    public void Update(BO.Volunteer boVolunteer, int Id)
    {
        try
        {
            if (boVolunteer.Id == Id || _dal.Volunteer.Read(Id).Role == Role.Manager)
                throw new Incompatible_ID("ID number does not match the requested one.");

            //if (BO.HelpCheck(BO.Volunteer volunteer))
            // תקינות הערכים בפורמט מתודת עזר


            var doVolunteer = new DO.Volunteer
            {
                Id = boVolunteer.Id,
                Name = boVolunteer.Name,
                Number_phone = boVolunteer.Number_phone,
                Email = boVolunteer.Email,
                Password = boVolunteer.Password,
                FullCurrentAddress = boVolunteer.FullCurrentAddress,
                Latitude = boVolunteer.Latitude,
                Longitude = boVolunteer.Longitude,
                Role = boVolunteer.Role,
                Active = boVolunteer.Active,
                distance = boVolunteer.Distance,
                Distance_Type = (DO.distance_type)boVolunteer.DistanceType
            };

            // Update in the data layer
            _dal.Volunteer.Update(doVolunteer);


        }

        catch (DO.DalDeletionImpossible ex)
        {
            throw new DO.DalDoesNotExistException("\"Student with ID={id} already exists\"");
        }




    } // מתודות עזר

    public List<BO.VolunteerInList> GetAskForListVal(BO.VolInList volInList, bool active)
    {
        var volunteers = _dal.Volunteer.ReadAll();
        

        if (active == null)
        {
            // doing convert
            List<BO.VolunteerInList> list = volunteers
                .Select(v => new BO.VolunteerInList
                {
                    Id = v.Id,
                }).ToList();
            return list;
        }

        if (active!=null)
        {
            volunteers = volunteers.Where(v => v.Active == active).ToList();
            return volunteers;

        }

        if (volInList==null)
        {
            volunteers = volunteers.Where(v => v.Id == id).ToList();
            return volunteers;
        }






    }

}
