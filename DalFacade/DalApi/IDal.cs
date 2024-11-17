namespace DalApi;

public interface IDal
{

    IConfig Config { get; }
    ICall Call { get; }
    IAssignment Assignment { get; }
    IVolunteer Volunteer { get; }
    void ResetDB();


}
