namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

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

    public void DeleteAll()
    {
        var item = DataSource.Volunteers;
            item.Clear();
        
    }

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
