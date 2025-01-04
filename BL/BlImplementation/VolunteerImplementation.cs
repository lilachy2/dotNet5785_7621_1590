﻿namespace BlImplementation;
using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;
using System.Collections.Generic;

internal class VolunteerImplementation : BlApi.IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    #region Stage 5
    public void AddObserver(Action listObserver) =>
    VolunteerManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
    VolunteerManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
    VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
    VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5

    //public IEnumerable<VolunteerInList> ReadAll(bool? Active, BO.VolInList? sortBy)
    //{
    //    var volunteers = _dal.Volunteer.ReadAll();

    //    if (Active.HasValue)
    //    {
    //        volunteers = volunteers.Where(volunteer => volunteer.Active == Active.Value);
    //    }

    //    switch (sortBy)
    //    {
    //        case BO.VolInList.Name:
    //            volunteers = volunteers.OrderBy(volunteer => volunteer.Name); // מיין לפי שם
    //            break;
    //        case BO.VolInList.Role:
    //            volunteers = volunteers.OrderBy(volunteer => volunteer.Role); // מיין לפי תפקיד
    //            break;
    //        case BO.VolInList.IsActive:
    //            volunteers = volunteers.OrderBy(volunteer => volunteer.Active); // מיין לפי מצב פעילות (פעיל/לא פעיל)
    //            break;
    //        default:
    //            volunteers = volunteers.OrderBy(volunteer => volunteer.Id); // מיון ברירת מחדל לפי מזהה
    //            break;
    //    }

    //    return volunteers
    //        .Select(volunteer => VolunteerManager.GetVolunteerInList(volunteer.Id)) // ממירים לכל מתנדב ברשימה לפי המזהה
    //        .ToList();
    //}
    public IEnumerable<VolunteerInList> ReadAll(bool? Active, BO.VolInList? sortBy)
    {
        var volunteers = _dal.Volunteer.ReadAll();

        // Filter by activity status
        if (Active.HasValue)
        {
            volunteers = volunteers.Where(volunteer => volunteer.Active == Active.Value);
        }
        // Sort by the selected parameter
        switch (sortBy)
        {
            case BO.VolInList.Name:
                volunteers = volunteers.OrderBy(volunteer => volunteer.Name); // Sort by name
                break;
           
            case BO.VolInList.IsActive:
                volunteers = volunteers.OrderBy(volunteer => volunteer.Active); // Sort by activity status (active/inactive)
                break;
            default:
                volunteers = volunteers.OrderBy(volunteer => volunteer.Id); // Default sorting by ID
                break;
        }
        // Convert the list to a list of volunteers by their ID
        var volunteerList = volunteers
            .Select(volunteer => VolunteerManager.GetVolunteerInList(volunteer.Id))
            .ToList();

        // Filter by call type after the conversion

        return volunteerList;
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
            var fordebug = VolunteerManager.GetVolunteer(id);
            return (/*VolunteerManager.GetVolunteer(id)*/ fordebug);

        }
        catch (Exception ex)
        {
            // Log the exception (optional, for debugging purposes)
            Console.WriteLine($"Error while reading volunteer with ID {id}: {ex.Message}");
            return null;
        }


    }

   
    public void Update(BO.Volunteer boVolunteer, int requesterId)
    {
        try
        {
            var requester = _dal.Volunteer.Read(requesterId);

            if (Tools.IsAddressValid(requester.FullCurrentAddress)/*.Result*/== true)
            {
                boVolunteer.Latitude = Tools.GetLatitudeAsync(requester.FullCurrentAddress).Result;
                boVolunteer.Longitude = Tools.GetLongitudeAsync(requester.FullCurrentAddress).Result;
            }

            var DOVolunteer = VolunteerManager.BOconvertDO(boVolunteer); // convert

            if (boVolunteer.Id != requesterId && DOVolunteer.Role != DO.Role.Manager)
            {
                throw new BO.Incompatible_ID("Requester is not authorized to update this volunteer.");
            }

            // check format
            //Tools.ValidateVolunteerData(boVolunteer);
            VolunteerManager.CheckFormat(boVolunteer);
            // check logic
            //If the address format is correct, enter the latitude and longitude.

            BO.Volunteer boVolunteerForLogic = VolunteerManager.GetVolunteer(DOVolunteer.Id);
            VolunteerManager.CheckLogic(boVolunteer, boVolunteerForLogic, requesterId, requester.Role);

            _dal.Volunteer.Update(DOVolunteer);
            VolunteerManager.Observers.NotifyItemUpdated(boVolunteer.Id);  // stage 5
            VolunteerManager.Observers.NotifyListUpdated(); // stage 5   

        }
        catch (DO.DalDoesNotExistException ex)
        {
            Console.WriteLine($"Error: Volunteer with ID={boVolunteer.Id} does not exist. Exception: {ex.Message}");
            throw new BO.BlDoesNotExistException($"Volunteer with ID={boVolunteer.Id} does not exist.", ex);
        }
        catch (BO.Incompatible_ID ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }
        catch (BlCheckPhonnumberException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }
        catch (BlIncorrectPasswordException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: Failed to update volunteer details. Exception: {ex.Message}");
            throw new BO.BlGeneralException("Failed to update volunteer details.", ex);
        }
    }


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
            //VolunteerManager.Observers.NotifyItemUpdated(volunteerId);
            VolunteerManager.Observers.NotifyListUpdated(); //stage 5   


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

            // we need add 
            boVolunteer.Latitude = Tools.GetLatitudeAsync(boVolunteer.FullCurrentAddress).Result;
            boVolunteer.Longitude = Tools.GetLongitudeAsync(boVolunteer.FullCurrentAddress).Result;

            // 5. Add the volunteer to the data layer
            _dal.Volunteer.Create(DOVolunteer); // Add the volunteer to the data layer (DAL)
            VolunteerManager.Observers.NotifyListUpdated(); //stage 5   

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


