namespace DalApi;
using DO;
/// <param name="void Create"> //Creates new entity object in DAL
/// <param name=" Assignment? Read"> //Reads entity object by its ID 
/// <param name="List<Assignment> ReadAll()> //stage 1 only, Reads all entity objects
/// <param name="void Update(Assignment item)"> //Updates entity object
/// <param name="void Delete(int id"> //Deletes an object by its Id
/// <param name="void DeleteAll()"> //Delete all entity objects
/// <param name="">
/// //The class represents defining interfaces for the data entities under the DalApi library

public interface ICall
{
    void Create(Call item);
    Call? Read(int id); 
    List<Call> ReadAll(); 
    void Update(Call item); 
    void Delete(int id);
    void DeleteAll(); 
}
