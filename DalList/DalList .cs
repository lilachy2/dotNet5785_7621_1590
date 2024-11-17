namespace Dal;
using DalApi;
using DO;

sealed public class DalList : IDal
{
    public IConfig Config { get; } = new ConfigImplementation();
    public ICall Call { get; } = new CallImplementation();
    public IAssignment Assignment { get; } = new AssignmentImplementation();
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();


    public void ResetDB()
    {
        Volunteer.DeleteAll();
        Assignment.DeleteAll();
        Call.DeleteAll();
        Config.Reset(); 
    }
}