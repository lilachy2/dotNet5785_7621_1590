namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;

/// <param name="Create">// create/add method
/// <param name="Delete">Delete method of an existing object
/// <param name="DeleteAll">// Method for deleting all objects of a certain type DeleteAll
/// <param name="Read">//A request/receive method of a single object
/// <param name="ReadAll">// A request/receive method for all objects of a certain type
/// <param name="Update"> // Update method of an existing object

internal class CallImplementation : ICall
{
    //public void Create(Call item)
    //{
    //    int newId = Config.NextCallId;
    //    Call newItem = item with { Id = newId };
    //    DataSource.Calls.Add(newItem);
    //}

    public void Create(Call item)
    {
        int newId = Config.NextCallId;

        Call newItem = new Call(
            Latitude: item.Latitude,          // Latitude
            Longitude: item.Longitude,        // Longitude
            Calltype: item.Calltype,          // Calltype
            Id: newId,                        // ה-ID החדש
            VerbalDescription: item.VerbalDescription, // VerbalDescription
            ReadAddress: item.ReadAddress,    // ReadAddress
            OpeningTime: item.OpeningTime,    // OpeningTime
            MaxEndTime: item.MaxEndTime      // MaxEndTime
        );

        DataSource.Calls.Add(newItem);
    }


    /// <param name="item">// Searches for the Assignment by ID
    /// <param name="Remove">/ Removes the found Assignment from the DataSource
    public void Delete(int id)
    {
        var item = DataSource.Calls.Find(x => x?.Id == id);

        if (item == null)
        {
            //throw new Exception($"Volunteer with ID={id} does not exist"); // stag 1
            throw new DO.Incompatible_ID($"Call with ID={id} does not exist"); // stag 2
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

    public Call? Read(int id)//stage1
    {
        //var item = DataSource.Calls.Find(x => x?.Id == id);  // stage1
        var item = DataSource.Calls.FirstOrDefault(item => item.Id == id) ?? throw new DO.Incompatible_ID($"Call with ID={id} does not exist"); //stage 2


        if (item == null)
            return null;

        else
            return item;
    }

    public Call? Read(Func<Call, bool> filter)  //stage 2
    {
        return DataSource.Calls.FirstOrDefault(filter);

    }

    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null) //stage 2
    => filter == null
        ? DataSource.Calls.Select(item => item)
        : DataSource.Calls.Where(filter);



    /// <param name="old">// Searches for the old Assignment
    public void Update(Call item)
    {
        Call? old = DataSource.Calls.Find(x => x?.Id == item.Id);

        if (old == null)
        {
            //throw new Exception($"Volunteer with ID={id} does not exist"); // stag 1
            throw new DO.Incompatible_ID($"Call with ID={item.Id} does not exist"); // stag 2
        }
        else
        {
            DataSource.Calls.Remove(old);
            DataSource.Calls.Add(item);
        }
    }
}