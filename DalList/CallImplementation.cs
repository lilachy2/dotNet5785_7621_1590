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

internal  class CallImplementation : ICall
{
    public void Create(Call item)
    {
        int newId = Config.NextCallId;
            //NextCall;
        Call newItem = new Call() { Id = newId };   
        DataSource.Calls.Add(newItem);
        ///return newItem.Id;
    }

    /// <param name="item">// Searches for the Assignment by ID
    /// <param name="Remove">/ Removes the found Assignment from the DataSource
    public void Delete(int id)
    {
        var item = DataSource.Calls.Find(x => x?.Id == id);

        if (item == null)
        {
            throw new Exception($"Volunteer with ID={id} does not exist");
        }
        else
            DataSource.Calls.Remove(item);
    }

    /// <param name="item">  // Retrieves the list of all Assignments
    public void DeleteAll()
    {
        var item = DataSource.Calls;
        item.Clear();
    }

    /// <param name="item">// Searches for the Assignment by ID

    public Call? Read(int id)
    {
        var item = DataSource.Calls.Find(x => x?.Id == id);

        if (item == null)
            return null;

        else
            return item;
    }

    public List<Call> ReadAll()
    {
        List<Call> calls = new List<Call>(DataSource.Calls);

        // print 
        foreach (var call in calls)
        {
            if (call != null)
            {
                Console.WriteLine(call);
            }
            else
            {
                Console.WriteLine("Null Volunteer");
            }
        }

        return new List<Call>(DataSource.Calls);///ask about ?
    }

    /// <param name="old">// Searches for the old Assignment
    public void Update(Call item)
    {
        Call? old = DataSource.Calls.Find(x => x?.Id == item.Id);

        if (old == null)
        {
            throw new Exception($"Volunteer with ID={item.Id} does not exist");
        }
        else
        {
            DataSource.Calls.Remove(old);
            DataSource.Calls.Add(item);
        }
    }
}
