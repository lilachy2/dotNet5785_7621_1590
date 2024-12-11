namespace BlImplementation;
using BlApi;
using BO;
using Helpers;
using System.Collections.Generic;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public IEnumerable<VolunteerInList> ReadAll(bool? Active, BO.VolInList? sortBy)
    {
        var volunteers = _dal.Volunteer.ReadAll();

        if (Active.HasValue)
        {
            volunteers = volunteers.Where(volunteer => volunteer.Active == Active.Value);
        }

        volunteers = sortBy switch
        {
            BO.VolInList.Name => volunteers.OrderBy(volunteer => volunteer.Name), // Sort by volunteer's full name
            BO.VolInList.Role => volunteers.OrderBy(volunteer => volunteer.Role),     // Sort by volunteer's job (role)
            BO.VolInList.IsActive => volunteers.OrderBy(volunteer => volunteer.Active), // Sort by activity status (active/inactive)
            _ => volunteers.OrderBy(volunteer => volunteer.Id) // Default: sort by volunteer ID
        };

        return volunteers.Select(volunteer => new VolunteerInList
        {
            Id = volunteer.Id,
            FullName = volunteer.Name,
            IsActive = volunteer.Active
        });
    }
    public DO.Role PasswordEntered(int Id, string password)
    {
        /// <summary>
        /// Verifies the user's credentials and returns their role if valid.
        /// Throws exceptions if the user does not exist or the password is incorrect.
        /// </summary>

        var volunteer = _dal.Volunteer.Read(Id);
        if (volunteer == null)

            throw new BO.BlDoesNotExistException("The user does not exist");

        if (volunteer.Password != password)

            throw new BO.BlIncorrectPasswordException("The password is incorrect");

        return volunteer.Role;
    }
    public BO.Volunteer VolunteerDetails(int id)
    {
        try
        {
            var doVolunteer = _dal.Volunteer.Read(id)
                              ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist.");

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
                Role = (BO.Role)doVolunteer.Role,
                Active = doVolunteer.Active,
                Distance = doVolunteer.distance,
                DistanceType = (BO.DistanceType)doVolunteer.Distance_Type,
                TotalHandledCalls = 0,
                TotalCancelledCalls = 0,
                TotalExpiredCalls = 0,
                CurrentCall = null
            };

            // חיפוש קריאה פעילה לפי מזהה מתנדב
            var calls = _dal.Call.ReadAll(); // הנחה שמחזיר את כל הקריאות
            var callInProgress = calls.FirstOrDefault(call => call.VolunteerId == id && call.Status == DO.CallStatus.InProgress);

            if (callInProgress != null)
            {
                boVolunteer.CurrentCall = new BO.CallInProgress
                {
                    Id = callInProgress.Id,
                    CallId = callInProgress.CallId,
                    CallType = (BO.Calltype)callInProgress.Calltype,
                    Description = callInProgress.VerbalDescription,
                    FullAddress = callInProgress.Address,
                    OpenTime = callInProgress.StartTime,
                    MaxCompletionTime = callInProgress.MaxCompletionTime,
                    EnterTime = callInProgress.VolunteerEnterTime,
                    DistanceFromVolunteer = callInProgress.DistanceFromVolunteer,
                    Status = (BO.CallStatus)callInProgress.Status
                };
            }

            return boVolunteer;
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralException("Failed to retrieve volunteer details.", ex);
        }
    }

    public BO.Volunteer Read(int id)
    {
        try
        {
            var doVolunteer = _dal.Volunteer.Read(id)
                              ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist.");

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
                Role = (BO.Role)doVolunteer.Role,
                Active = doVolunteer.Active,
                Distance = doVolunteer.distance,
                DistanceType = (BO.DistanceType)doVolunteer.Distance_Type,
                TotalHandledCalls = 0,
                TotalCancelledCalls = 0,
                TotalExpiredCalls = 0,
                CurrentCall = null
            };

            var doCall = _dal.Call.Read(id);
            if (doCall != null)
            {
                boVolunteer.CurrentCall = new BO.CallInProgress
                {
                    CallId = doCall.Id,
                    Description = doCall.VerbalDescription,
                    CallType = (BO.Calltype)doCall.Calltype,
                    // Populate additional fields
                };
            }

            return boVolunteer;
        }
        catch (DO.DalDeletionImpossible)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist.");
        }
    }
    public void Update(BO.Volunteer boVolunteer, int requesterId)
    {
        try
        {
            var requester = _dal.Volunteer.Read(requesterId);

            if (boVolunteer.Id != requesterId && requester.Role != DO.Role.Manager)
            {
                throw new Incompatible_ID("Requester is not authorized to update this volunteer.");
            }

           Tools.ValidateVolunteerData(boVolunteer);
            VolunteerManager.CheckLogic(boVolunteer);


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
                Role = (requesterId == boVolunteer.Id) ? requester.Role : (DO.Role)boVolunteer.Role, //only the manager can change the role.
                Active = boVolunteer.Active,
                distance = boVolunteer.Distance,
                Distance_Type = (DO.distance_type)boVolunteer.DistanceType
            };

            _dal.Volunteer.Update(doVolunteer);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={boVolunteer.Id} does not exist.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralException("Failed to update volunteer details.", ex);
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
      (DO.Role)boVolunteer.Role,
      (DO.distance_type)boVolunteer.DistanceType,
      boVolunteer.Active,
      boVolunteer.FullCurrentAddress ?? string.Empty,
      boVolunteer.Latitude ?? 0,
      boVolunteer.Longitude ?? 0,
      boVolunteer.Distance ?? 0
  );



        try
        {
            _dal.Volunteer.Create(doVolunteer);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Student with ID={boVolunteer.Id} already exists", ex);
        }


    }

    public void Delete(int id)
    {
        //try
        //{
        //    var volunteer = _dal.Volunteer.Read(id);

        //    if (volunteer == null)
        //    {
        //        throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist.");
        //    }

        //    // קריאת היסטוריית הקריאות של המתנדב (ניתן להוסיף פונקציה מתאימה בשכבת הנתונים)
        //    //var handledCalls = GetCallsByVolunteerId(id);//add
        //    //var handledCalls = Tools.GetCallsByVolunteerId(id);
        //    var handledCalls = volunteer.TotalHandledCalls;

        //    if (handledCalls.Any(call => call.Status == DO.AssignmentCompletionType.TreatedOnTime ||
        //                      call.Status == DO.AssignmentCompletionType.VolunteerCancelled ||
        //                      call.Status == DO.AssignmentCompletionType.AdminCancelled ||
        //                      call.Status == DO.AssignmentCompletionType.Expired))
        //    {
        //        throw new BO.BlDeletionImpossibleException($"Cannot delete volunteer with ID={id}. The volunteer has handled or is currently handling calls.");
        //    }


        //    // מחיקת המתנדב
        //    _dal.Volunteer.Delete(id);
        //}
        //catch (DO.DalDoesNotExistException ex)
        //{
        //    // חריגה אם המתנדב לא נמצא בשכבת הנתונים
        //    throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist.", ex);
        //}
        //catch (BO.BlDeletionImpossibleException ex)
        //{
        //    // חריגה אם המחיקה אינה אפשרית (חריגת מחיקה מה-DAL)
        //    throw new BO.BlDeletionImpossibleException($"Unable to delete volunteer with ID={id}.", ex);
        //}
    }










}


