﻿namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

/// <param name="Create">// create/add method
/// <param name="Delete">Delete method of an existing object
/// <param name="DeleteAll">// Method for deleting all objects of a certain type DeleteAll
/// <param name="Read">//A request/receive method of a single object
/// <param name="ReadAll">// A request/receive method for all objects of a certain type
/// <param name="Update"> // Update method of an existing object
/// 
public class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {
            if (Read(item.id)!=null)
            {
                throw new Exception($"Volunteer with ID={item.id} does exist");
            }
            else
            {
            DataSource.Volunteers.Add(item);
           /// return item.id; ??void...
        }
    }

    /// <param name="item">// Searches for the Assignment by ID
    /// <param name="Remove">/ Removes the found Assignment from the DataSource
    public void Delete(int id)
    {

        var item = DataSource.Volunteers.Find(x => x?.id == id);

        if (item == null)
        {
            throw new Exception($"Volunteer with ID={id} does not exist");
        }
        else
        DataSource.Volunteers.Remove(item);
    }

    /// <param name="item">  // Retrieves the list of all Assignments
   public void DeleteAll()
    {
        var item = DataSource.Volunteers;
            item.Clear();
        
    }

    /// <param name="item">// Searches for the Assignment by ID

    public Volunteer? Read(int id)
    {

        var item = DataSource.Volunteers.Find(x => x?.id == id);

        if (item == null)
            return null;
        
        else
            return item;
    }

    public List<Volunteer?> ReadAll()
    {
        return new List<Volunteer?>(DataSource.Volunteers) ;///ask about ?
    }

    /// <param name="old">// Searches for the old Assignment

    public void Update(Volunteer item)
    {
        Volunteer? old = DataSource.Volunteers.Find(x => x?.id == item.id);

        if (old == null)
        {
            throw new Exception($"Volunteer with ID={item.id} does not exist");
        }
        else
        {
            DataSource.Volunteers.Remove(old);
            DataSource.Volunteers.Add(item);
        }
    }
}
