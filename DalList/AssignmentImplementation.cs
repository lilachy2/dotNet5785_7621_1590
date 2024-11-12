namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        int newId1 = Config.Next_Assignment_CallId;
        int newId2 = Config.Next_Assignment_Id;
        Assignment newItem = new Assignment() { Id = newId1, CallId= newId2};
        DataSource.Assignments.Add(newItem);
        ///return newItem.Id;
    }

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

    public void DeleteAll()
    {
        var item = DataSource.Assignments;
        item.Clear();
    }

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
        return new List<Assignment>(DataSource.Assignments);///ask about ?
    }

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
