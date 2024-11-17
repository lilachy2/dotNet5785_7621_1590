namespace Dal;
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
/// <param name="newId1 ">// Generates the next available ID for the Assignment
/// <param name="newItem "> // Creates a new Assignment object with the generated IDs
/// <param name="">


public class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        int newId1 = Config.NextAssignmentId;
        //int newId2 = Config.NextAssignmentId;
        Assignment newItem = new Assignment() { Id = newId1/*, CallId= newId2*/};
        DataSource.Assignments.Add(newItem);
        ///return newItem.Id;
    }

    /// <param name="item">// Searches for the Assignment by ID
    /// <param name="Remove">/ Removes the found Assignment from the DataSource
    public void Delete(int id)
    {
        var item = DataSource.Assignments.Find(x => x?.Id == id);

        if (item == null)
        {
            throw new Exception($"Volunteer with ID={id} does not exist");
        }
        else
            DataSource.Assignments.Remove(item);
    }

    /// <param name="item">  // Retrieves the list of all Assignments

    public void DeleteAll()
    {
        var item = DataSource.Assignments;
        item.Clear();
    }

    /// <param name="item">// Searches for the Assignment by ID
    public Assignment? Read(int id)
    {
        var item = DataSource.Assignments.Find(x => x?.Id == id);

        if (item == null)
            return null;

        else
            return item;
    }

    public List<Assignment> ReadAll()
    {
        List<Assignment?> Assignment = new List<Assignment?>(DataSource.Assignments);

        // print 
        foreach (var Assignment1 in Assignment)
        {
            if (Assignment1 != null)
            {
                Console.WriteLine(Assignment1);
            }
            else
            {
                Console.WriteLine("Null Volunteer");
            }
        }

        return new List<Assignment>(DataSource.Assignments);///ask about ?
    }

    /// <param name="old">// Searches for the old Assignment

    public void Update(Assignment item)
    {
        Assignment? old = DataSource.Assignments.Find(x => x?.Id == item.Id);

        if (old == null)
        {
            throw new Exception($"Volunteer with ID={item.Id} does not exist");
        }
        else
        {
            DataSource.Assignments.Remove(old);
            DataSource.Assignments.Add(item);
        }
    }
}
