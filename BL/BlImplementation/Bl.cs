namespace BlImplementation;
using BlApi;
internal class Bl : BlApi.IBl

{
    public ICall Call { get; } = new CallImplementation();
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();
    public IAdmin Admin { get; } = new AdminImplementation();


}
