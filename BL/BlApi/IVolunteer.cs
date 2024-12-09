

namespace BlApi;

/// <param name="BO.Role PasswordEntered(string Name, string password)"> 
/// // Authenticates user and returns role; throws if invalid credentials.
///
/// <param name="BO.VolunteerInList GetAskForListVal(BO.VolInList volInList, bool active)"> 
/// // Returns a filtered/sorted volunteer list based on parameters.
///
/// <param name="BO.Volunteer? Read(int id)"> 
/// // Gets volunteer details by ID; throws if not found.
///
/// <param name="void Update(BO.Volunteer boVolunteer)"> 
/// // Updates volunteer details; validates data and permissions; throws if invalid.
///
/// <param name="void Delete(int id)"> 
/// // Deletes a volunteer by ID; ensures no active/incomplete calls; throws if not allowed.
///
/// <param name="void Create(BO.Volunteer boVolunteer)"> 
/// // Adds a new volunteer; validates data; throws if already exists
public interface IVolunteer
{
    List<BO.VolunteerInList> GetAskForListVal(BO.VolInList volInList, bool active);

    DO.Role PasswordEntered(/*string*/int  Name, string password);
    //BO.Role PasswordEntered(/*string*/int  Name, string password);

    BO.Volunteer Volunteer_details(int id);
    BO.Volunteer? Read(int id);
    //IEnumerable<BO.VolunteerInList> ReadAll(BO.VolunteerFieldSort? sort = null, BO.VolunteerFieldFilter? filter = null, object? value = null);

    void Update(BO.Volunteer boVolunteer , int id);

    void Delete(int id);

    void Create(BO.Volunteer boVolunteer);

}
