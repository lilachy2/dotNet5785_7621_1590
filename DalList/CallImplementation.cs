namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class CallImplementation : ICall
{
    public void Create(Call item)
    {
        int newId = Config.NextCall;
        Call newItem = new Call() { Id = newId };   
        DataSource.Calls.Add(newItem);
        ///return newItem.Id;
    }

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

    public void DeleteAll()
    {
        var item = DataSource.Calls;
        item.Clear();
    }

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
        return new List<Call>(DataSource.Calls);///ask about ?
    }

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
