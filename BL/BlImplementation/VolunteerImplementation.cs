namespace BlImplementation;
using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;
using System;
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


    public IEnumerable<VolunteerInList> ReadAll(bool? Active, BO.VolInList? sortBy)
    {

        return VolunteerManager.ReadAll(Active, sortBy);
    }
    public DO.Role PasswordEntered(int Id, string password)
    {
        /// <summary>
        /// Verifies the user's credentials and returns their role if valid.
        /// Throws exceptions if the user does not exist or the password is incorrect.
        /// </summary>
        /// 

         DO.Volunteer volunteer = null;
        lock (AdminManager.BlMutex) //stage 7

            volunteer = _dal.Volunteer.Read(Id);
        if (volunteer == null)

            throw new BO.BlDoesNotExistException("The user does not exist");

        if (volunteer.Password != password)

            throw new BO.BlIncorrectPasswordException("The password is incorrect");

        return volunteer.Role;
    }
    

    public BO.Volunteer Read(int id)
    {

       return VolunteerManager.Read(id);

    }
    
    public void Update(BO.Volunteer boVolunteer, int requesterId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

        DO.Volunteer? requester = null;

        try
        {
            lock (AdminManager.BlMutex) //stage 7
                requester = _dal.Volunteer.Read(requesterId);

          

            var DOVolunteer = VolunteerManager.BOconvertDO(boVolunteer); // convert

            if (boVolunteer.Id != requesterId && DOVolunteer.Role != DO.Role.Manager)
            {
                throw new BO.Incompatible_ID("Requester is not authorized to update this volunteer.");
            }

            VolunteerManager.CheckFormat(boVolunteer);

            BO.Volunteer boVolunteerForLogic = VolunteerManager.GetVolunteer(DOVolunteer.Id);
            VolunteerManager.CheckLogic(boVolunteer, boVolunteerForLogic, requesterId, requester.Role);

            lock (AdminManager.BlMutex) //stage 7
                _dal.Volunteer.Update(DOVolunteer); // עדכון סינכרוני מתבצע כאן

              // אם כתובת תקינה, התחל חישוב קואורדינטות אסינכרוני
            if (Tools.IsAddressValidAsync(boVolunteer.FullCurrentAddress).Result)
            {
                // התחלת החישוב של הקואורדינטות ברקע, לא מחכים לו
                _ = VolunteerManager.UpdateCoordinatesForVolunteerAsync(DOVolunteer.FullCurrentAddress, boVolunteer, null); // תחילת חישוב אסינכרוני
            }
            else
            {
                throw new BlInvalidaddress("The address is not valid");
            }

        }
        catch (DO.DalDoesNotExistException ex)
        {
            Console.WriteLine($"Error: Volunteer with ID={boVolunteer.Id} does not exist. Exception: {ex.Message}");
            throw new BO.BlDoesNotExistException($"Volunteer with ID={boVolunteer.Id} does not exist.", ex);
        }
        catch (BO.Incompatible_ID ex)
        {
            throw new BO.Incompatible_ID($"Volunteer with ID {boVolunteer.Id} has an incompatible ID.", ex);
        }
        catch (BlInvalidaddress ex)
        {
            throw new BlInvalidaddress($"Invalid address provided for volunteer with ID {boVolunteer.Id}.", ex);
        }
        catch (BO.InvalidOperationException ex)
        {
            throw new BO.InvalidOperationException("", ex);
        }
        catch (BO.BlCheckPhonnumberException ex)
        {
            throw new BO.BlCheckPhonnumberException("", ex);
        }
        catch (BO.BlEmailException ex)
        {
            throw new BO.BlEmailException("", ex);
        }
        catch (BO.BlIncorrectPasswordException ex)
        {
            throw new BO.BlIncorrectPasswordException("", ex);
        }
        catch (BO.BlPermissionException ex)
        {
            throw new BO.BlPermissionException("", ex);
        }   
        catch (BO.BlCan_chang_to_NotActivException ex)
        {
            throw new BO.BlCan_chang_to_NotActivException("", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralException("An unexpected error occurred while updating volunteer details.", ex);
        }

        VolunteerManager.Observers.NotifyItemUpdated(boVolunteer.Id);  // stage 5
        VolunteerManager.Observers.NotifyListUpdated(); // stage 5   
    }

    // פונקציה אסינכרונית לחישוב הקואורדינאטות
    private async Task UpdateCoordinatesForVolunteerAsyncHELP(BO.Volunteer boVolunteer, string address)
    {
      await VolunteerManager. UpdateCoordinatesForVolunteerAsync(address,boVolunteer, null);
    }


    public void Delete(int volunteerId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
      IEnumerable < DO.Assignment> assignments = null;

        lock (AdminManager.BlMutex) //stage 7

         // Retrieve all assignments associated with the volunteer
             assignments = _dal.Assignment.ReadAll()
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
            lock (AdminManager.BlMutex) //stage 7
                _dal.Volunteer.Delete(volunteerId);
                //VolunteerManager.Observers.NotifyItemUpdated(volunteerId);


            }
            catch (DalDoesNotExistException ex) // If the volunteer does not exist in the system
            {
                throw new BlDoesNotExistException($"The volunteer with ID {volunteerId} was not found in the system.", ex);
            }
        
            VolunteerManager.Observers.NotifyListUpdated(); //stage 5   
    }
    public void Create(BO.Volunteer boVolunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        DO.Volunteer existingVolunteer = null;

        try
        {
            // 1. Format checks
            VolunteerManager.CheckFormat(boVolunteer); // Validate the format of the volunteer's data (phone number, email, address)

            // 2. Check if the volunteer already exists in the data layer
            lock (AdminManager.BlMutex) //stage 7
                existingVolunteer = _dal.Volunteer.Read(boVolunteer.Id); // Check if a volunteer with the same ID already exists
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
                var BOexistingVolunteer = VolunteerManager.GetVolunteer(existingVolunteer.Id); // Retrieve existing volunteer as BO object
                VolunteerManager.CheckLogic(boVolunteer, BOexistingVolunteer, false); // false means no manager is performing the action
            }
            else
            {
                VolunteerManager.CheckLogic(boVolunteer, null, false); // Logic check for new volunteer
            }

            // 5. Asynchronously calculate latitude and longitude
            _ = VolunteerManager.UpdateCoordinatesForVolunteerAsync( boVolunteer.FullCurrentAddress,  boVolunteer,null);

            // 6. Add the volunteer to the data layer
            lock (AdminManager.BlMutex) //stage 7
                _dal.Volunteer.Create(DOVolunteer); // Add the volunteer to the data layer (DAL)

        }
        catch (DO.DalDoesNotExistException ex) // Handle existing volunteer exception
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID {boVolunteer.Id} already exists.", ex);
        }
     
        catch (BO.InvalidOperationException ex)
        {
            throw new BO.InvalidOperationException("", ex);
        }
        catch (BO.BlCheckIdException ex)
        {
            throw new BO.BlCheckIdException("", ex);
        }
        catch (BO.BlCheckPhonnumberException ex)
        {
            throw new BO.BlCheckPhonnumberException("", ex);
        }
        catch (BO.BlEmailException ex)
        {
            throw new BO.BlEmailException("", ex);
        }
        catch (BO.BlIncorrectPasswordException ex)
        {
            throw new BO.BlIncorrectPasswordException("", ex);
        }
       
        catch (BO.BlCan_chang_to_NotActivException ex)
        {
            throw new BO.BlCan_chang_to_NotActivException("", ex);
        }
        catch (BO.BlWrongItemtException ex) // Handle format or logic issue exception
        {
            throw new BO.BlWrongItemtException("There was an issue with the format or logic of the data.", ex);
        }
        catch (BO.BlPermissionException ex) // Handle permission error when adding a volunteer
        {
            throw new BO.BlPermissionException("Permission error during volunteer addition.", ex);
        }
        catch (Exception ex) // Catch any other general exceptions
        {
            throw new BO.BlGeneralException("Failed to add volunteer.", ex);
        }

        VolunteerManager.Observers.NotifyListUpdated(); // stage 5
    }

    // פונקציה אסינכרונית לחישוב הקואורדינאטות
    public  void SimulationVolunteerActivity()
    {
        VolunteerManager.SimulateVolunteerActivity();
    }


}


