
namespace BlImplementation;
using BlApi;
using BO;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void Create(Volunteer boVolunteer)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public VolunteerInList GetAskForListVal(VolInList volInList, bool active)
    {
        throw new NotImplementedException();
    }

    public Role PasswordEntered(string Name, string password)
    {
        throw new NotImplementedException();
    }

    public Volunteer? Read(int id)
    {
        throw new NotImplementedException();
    }

    public void Update(Volunteer boVolunteer)
    {
        throw new NotImplementedException();
    }
}
