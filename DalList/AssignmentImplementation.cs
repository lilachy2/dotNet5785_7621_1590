﻿namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <param name="Create">// create/add method
/// <param name="Delete">Delete method of an existing object
/// <param name="DeleteAll">// Method for deleting all objects of a certain type DeleteAll
/// <param name="Read">//A request/receive method of a single object
/// <param name="ReadAll">// A request/receive method for all objects of a certain type
/// <param name="Update"> // Update method of an existing object
/// 
/// <param name="newId1 ">// Generates the next available ID for the Assignment
/// <param name="newItem "> // Creates a new Assignment object with the generated IDs
/// <param name="">


internal class AssignmentImplementation : IAssignment
{
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void Create(Assignment item)
    {
        int newId1 = Config.NextAssignmentId;
        // with - במקום לשנות את item ישירות, נוצר עותק חדש של item DPS 
        Assignment newItem = item with { Id = newId1 };
        DataSource.Assignments.Add(newItem);
        ///return newItem.Id;
    }

    /// <param name="item">// Searches for the Assignment by ID
    /// <param name="Remove">/ Removes the found Assignment from the DataSource
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void Delete(int id)
    {
        var item = DataSource.Assignments.Find(x => x?.Id == id);

        if (item == null)
        {
            //throw new Exception($"Volunteer with ID={id} does not exist"); // stag 1
            throw new DO.Incompatible_ID($"Assignment with ID={id} does not exist"); // stag 2
        }
        else
            DataSource.Assignments.Remove(item);
    }

    /// <param name="item">  // Retrieves the list of all Assignments
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void DeleteAll()
    {
        var item = DataSource.Assignments;
        item.Clear();
    }

    /// <param name="item">// Searches for the Assignment by ID
    [MethodImpl(MethodImplOptions.Synchronized)]

    public Assignment? Read(int id)  // // stage1
    {

        //var item = DataSource.Assignments.Find(x => x?.Id == id); // stage 1
        var item = DataSource.Assignments.FirstOrDefault(item => item.Id == id)?? throw new DO.Incompatible_ID($"Assignment with ID {id} was not found."); //stage 2


        if (item == null)
            return null;

        else
            return item;
    }
    [MethodImpl(MethodImplOptions.Synchronized)]

    public Assignment? Read(Func<Assignment, bool> filter)  //stage 2
    {
        return DataSource.Assignments.FirstOrDefault(filter);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]

    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
     => filter == null
         ? DataSource.Assignments
         : DataSource.Assignments.Where(filter);

    /// <param name="old">// Searches for the old Assignment
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void Update(Assignment item)
    {
        Assignment? old = DataSource.Assignments.Find(x => x?.Id == item.Id);

        if (old == null)
        {

            //throw new Exception($"Volunteer with ID={id} does not exist"); // stag 1
            throw new DO.Incompatible_ID($"Assignment with ID={item.Id} does not exist"); // stag 2

        }
        else
        {
            DataSource.Assignments.Remove(old);
            DataSource.Assignments.Add(item);
        }
    }
}