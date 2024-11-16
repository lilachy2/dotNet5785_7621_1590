namespace DalApi;
using DO;

/// <param name="Create"> //Creates new entity object in DAL
/// <param name="Read"> //Reads entity object by its ID 
/// <param name="ReadAll"> //stage 1 only, Reads all entity objects
/// <param name="Update"> //Updates entity object
/// <param name="Delete"> //Deletes an object by its Id
/// <param name="DeleteAll"> //Delete all entity objects
public interface IVolunteer
{
    void Create(Volunteer item); 
    Volunteer? Read(int id); 
    List<Volunteer?> ReadAll(); 
    void Update(Volunteer item); 
    void Delete(int id); 
    void DeleteAll();

    //void Reset();

}
