namespace BlImplementation;
using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;
using System.Collections.Generic;

internal class VolunteerImplementation : BlApi.IVolunteer
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

    public BO.Volunteer Read(int id)
    {
        try
        {
        return (VolunteerManager.GetVolunteer(id));

        } 
        catch (Exception ex)
        {
            // Log the exception (optional, for debugging purposes)
            Console.WriteLine($"Error while reading volunteer with ID {id}: {ex.Message}");
            return null;
        }


    }

    //public BO.Volunteer VolunteerDetails(int id)
    //{
    //    try
    //    {
    //        var doVolunteer = _dal.Volunteer.Read(id)
    //                          ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist.");

    //        var boVolunteer = new BO.Volunteer
    //        {
    //            Id = doVolunteer.Id,
    //            Name = doVolunteer.Name,
    //            Number_phone = doVolunteer.Number_phone,
    //            Email = doVolunteer.Email,
    //            Password = doVolunteer.Password,
    //            FullCurrentAddress = doVolunteer.FullCurrentAddress,
    //            Latitude = doVolunteer.Latitude,
    //            Longitude = doVolunteer.Longitude,
    //            Role = (BO.Role)doVolunteer.Role,
    //            Active = doVolunteer.Active,
    //            Distance = doVolunteer.distance,
    //            DistanceType = (BO.DistanceType)doVolunteer.Distance_Type,
    //            TotalHandledCalls = 0,
    //            TotalCancelledCalls = 0,
    //            TotalExpiredCalls = 0,
    //            CurrentCall = null
    //        };

    //        // חיפוש קריאה פעילה לפי מזהה מתנדב
    //        var calls = _dal.Call.ReadAll(); // הנחה שמחזיר את כל הקריאות
    //        var callInProgress = calls.FirstOrDefault(call => call.VolunteerId == id && call.Status == DO.CallStatus.InProgress);

    //        if (callInProgress != null)
    //        {
    //            boVolunteer.CurrentCall = new BO.CallInProgress
    //            {
    //                Id = callInProgress.Id,
    //                CallId = callInProgress.CallId,
    //                CallType = (BO.Calltype)callInProgress.Calltype,
    //                Description = callInProgress.VerbalDescription,
    //                FullAddress = callInProgress.Address,
    //                OpenTime = callInProgress.StartTime,
    //                MaxCompletionTime = callInProgress.MaxCompletionTime,
    //                EnterTime = callInProgress.VolunteerEnterTime,
    //                DistanceFromVolunteer = callInProgress.DistanceFromVolunteer,
    //                Status = (BO.CallStatus)callInProgress.Status
    //            };
    //        }

    //        return boVolunteer;
    //    }
    //    catch (DO.DalDoesNotExistException ex)
    //    {
    //        throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist.", ex);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new BO.BlGeneralException("Failed to retrieve volunteer details.", ex);
    //    }
    //}

   
    public void Update(BO.Volunteer boVolunteer, int requesterId)
    {

        var DOVolunteer= VolunteerManager.BOconvertDO(boVolunteer); //convert 

        try
        {
            var requester = _dal.Volunteer.Read(requesterId);

            if (boVolunteer.Id != requesterId && DOVolunteer.Role != DO.Role.Manager)
            {
                throw new BO.Incompatible_ID("Requester is not authorized to update this volunteer.");
            }


            // check format
           //Tools.ValidateVolunteerData(boVolunteer);
            VolunteerManager.CheckFormat(boVolunteer);
            // check logic
            //If the address format is correct, enter the latitude and longitude.
            if ( Tools.IsAddressValid(requester.FullCurrentAddress).Result== true)
            {
                boVolunteer.Latitude= Tools.GetLatitudeAsync(requester.FullCurrentAddress).Result; 
                boVolunteer.Longitude= Tools.GetLongitudeAsync(requester.FullCurrentAddress).Result; 
            }


            // מימוש של
            BO.Volunteer boVolunteerForLogic = VolunteerManager.GetVolunteer(DOVolunteer.Id);
            VolunteerManager.CheckLogic(boVolunteer, boVolunteerForLogic, requesterId, requester.Role);

            _dal.Volunteer.Update(DOVolunteer);
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

    //public void Delete(int id)
    //{
    //    try
    //    {
    //        var volunteer = _dal.Volunteer.Read(id);

    //        if (volunteer == null)
    //        {
    //            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist.");
    //        }

    //        // קריאת היסטוריית הקריאות של המתנדב (ניתן להוסיף פונקציה מתאימה בשכבת הנתונים)
    //        //var handledCalls = GetCallsByVolunteerId(id);//add
    //        //var handledCalls = Tools.GetCallsByVolunteerId(id);
    //        var handledCalls = volunteer.TotalHandledCalls;

    //        if (handledCalls.Any(call => call.Status == DO.AssignmentCompletionType.TreatedOnTime ||
    //                          call.Status == DO.AssignmentCompletionType.VolunteerCancelled ||
    //                          call.Status == DO.AssignmentCompletionType.AdminCancelled ||
    //                          call.Status == DO.AssignmentCompletionType.Expired))
    //        {
    //            throw new BO.BlDeletionImpossibleException($"Cannot delete volunteer with ID={id}. The volunteer has handled or is currently handling calls.");
    //        }


    //        // מחיקת המתנדב
    //        _dal.Volunteer.Delete(id);
    //    }
    //    catch (DO.DalDoesNotExistException ex)
    //    {
    //        // חריגה אם המתנדב לא נמצא בשכבת הנתונים
    //        throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist.", ex);
    //    }
    //    catch (BO.BlDeletionImpossibleException ex)
    //    {
    //        // חריגה אם המחיקה אינה אפשרית (חריגת מחיקה מה-DAL)
    //        throw new BO.BlDeletionImpossibleException($"Unable to delete volunteer with ID={id}.", ex);
    //    }
    //}

    public void Delete(int volunteerId)
    {
        // Retrieve all assignments associated with the volunteer
        var assignments = _dal.Assignment.ReadAll()
            .Where(a => a.VolunteerId == volunteerId)
            .ToList();

        // Check if there is a call that the volunteer is currently handling or has handled in the past
        if (assignments.Any(a => a.time_end_treatment == null || a.time_entry_treatment != default) || assignments.Any())

        {
            throw new BlDeletionImpossibleException($"Cannot delete the volunteer with ID {volunteerId}. The volunteer is currently handling or has handled a call in the past.");
        }

        // Attempt to delete the volunteer from the data layer
        try
        {
            _dal.Volunteer.Delete(volunteerId);
        }
        catch (DalDoesNotExistException ex) // If the volunteer does not exist in the system
        {
            throw new BlDoesNotExistException($"The volunteer with ID {volunteerId} was not found in the system.", ex);
        }
    }


    public void Create(BO.Volunteer boVolunteer)
    {
        try
        {
            // 1. Format checks
            VolunteerManager.CheckFormat(boVolunteer); // Validate the format of the volunteer's data (phone number, email, address)

            // 2. Check if the volunteer already exists in the data layer
            var existingVolunteer = _dal.Volunteer.Read(boVolunteer.Id); // Check if a volunteer with the same ID already exists
            if (existingVolunteer != null)
            {
                throw new BO.BlDoesNotExistException($"Volunteer with ID {boVolunteer.Id} already exists."); // If exists, throw an exception
            }

            // 3. Convert BO.Volunteer to DO.Volunteer
            var DOVolunteer = VolunteerManager.BOconvertDO(boVolunteer); // Convert BO object to DO object

            // 4. Logic checks
            // If the volunteer already exists, send the existing volunteer for comparison, otherwise send null for a new volunteer
            if (existingVolunteer != null)
            {
                // If the volunteer already exists in DAL, send the existing volunteer for logic validation
                var BOexistingVolunteer = VolunteerManager.GetVolunteer(existingVolunteer.Id); // Retrieve existing volunteer as BO object
                VolunteerManager.CheckLogic(boVolunteer, BOexistingVolunteer, false); // false means no manager is performing the action
            }
            else
            {
                // If the volunteer is new, perform logic check without comparison to an existing volunteer
                VolunteerManager.CheckLogic(boVolunteer, null, false); // Pass null for a new volunteer
            }

            // 5. Add the volunteer to the data layer
            _dal.Volunteer.Create(DOVolunteer); // Add the volunteer to the data layer (DAL)
        }
        catch (DO.DalDoesNotExistException ex) // Handle existing volunteer exception
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID {boVolunteer.Id} already exists.", ex); // Re-throw the exception with the appropriate message
        }
        catch (BO.BlWrongItemtException ex) // Handle format or logic issue exception
        {
            throw new BO.BlWrongItemtException("There was an issue with the format or logic of the data.", ex); // Re-throw the exception with a message indicating format or logic issues
        }
        catch (BO.BlPermissionException ex) // Handle permission error when adding a volunteer
        {
            throw new BO.BlPermissionException("Permission error during volunteer addition.", ex); // Re-throw the permission exception with a relevant message
        }
        catch (Exception ex) // Catch any other general exceptions
        {
            throw new BO.BlGeneralException("Failed to add volunteer.", ex); // Re-throw the general exception with a message indicating the failure
        }
    }






}


