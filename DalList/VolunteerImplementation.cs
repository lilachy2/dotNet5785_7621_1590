namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

/// <param name="Create">// create/add method
/// <param name="Delete">Delete method of an existing object
/// <param name="DeleteAll">// Method for deleting all objects of a certain type DeleteAll
/// <param name="Read">//A request/receive method of a single object
/// <param name="ReadAll">// A request/receive method for all objects of a certain type
/// <param name="Update"> // Update method of an existing object
/// 
internal class VolunteerImplementation : IVolunteer
{
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void Create(Volunteer item)
    {
        //if (Read(item.id)!=null)//stage1
        if (Read(v => v.Id == item.Id) != null)
        {
            //throw new Exception($"Volunteer with ID={item.id} does exist"); // stage1
            throw new DO.Incompatible_ID($"Volunteer with ID={item.Id} does exist"); // stage 2

        }
        else
        {
            DataSource.Volunteers.Add(item);
            /// return item.id; ??void...
        }
    }

    /// <param name="item">// Searches for the Assignment by ID
    /// <param name="Remove">/ Removes the found Assignment from the DataSource
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void Delete(int id)
    {

        var item = DataSource.Volunteers.Find(x => x?.Id == id);

        if (item == null)
        {
            //throw new Exception($"Volunteer with ID={id} does not exist"); // stag 1
            throw new DO.Incompatible_ID($"Volunteer with ID={id} does exist"); // stage 2
        }
        else
            DataSource.Volunteers.Remove(item);
    }

    /// <param name="item">  // Retrieves the list of all Assignments
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void DeleteAll()
    {
        var item = DataSource.Volunteers;
        item.Clear();

    }

    /// <param name="item">// Searches for the Assignment by ID
    [MethodImpl(MethodImplOptions.Synchronized)]

    public Volunteer? Read(int id)//stage1
    {

        //var item = DataSource.Volunteers.Find(x => x?.id == id);  // stage1
        var item = DataSource.Volunteers.FirstOrDefault(item => item.Id == id);
        if (item == null)
            return null;

        else
            return item;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(Func<Volunteer, bool> filter)  //stage 2
    {
        return DataSource.Volunteers.FirstOrDefault(filter);
    }



    [MethodImpl(MethodImplOptions.Synchronized)]
    // IEnumerable
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null) //stage 2
    => filter == null
        ? DataSource.Volunteers.Select(item => item)
        : DataSource.Volunteers.Where(filter);

    // linq

   // public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null) //stage 2
   //=> filter == null
   //    ? from item in DataSource.Volunteers
   //      select item :
   //    from item in DataSource.Volunteers
   //    where filter(item)
   //    select item; 
    

    /// <param name="old">// Searches for the old Assignment
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Volunteer item)
    {
        Volunteer? old = DataSource.Volunteers.Find(x => x?.Id == item.Id);

        if (old == null)
        {
            //throw new Exception($"Volunteer with ID={item.id} does not exist"); // // stag 1
            throw new DO.Incompatible_ID($"Volunteer with ID={item.Id} does exist"); // stage 2
        }
        else
        {
            DataSource.Volunteers.Remove(old);
            DataSource.Volunteers.Add(item);
        }
    }
}