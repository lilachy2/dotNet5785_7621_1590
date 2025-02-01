namespace DalApi;

public interface IDal // Dallist is the only class that implements this interface
{

    IConfig Config { get; }
    ICall Call { get; }
    IAssignment Assignment { get; }
    IVolunteer Volunteer { get; }
    void ResetDB();


}
