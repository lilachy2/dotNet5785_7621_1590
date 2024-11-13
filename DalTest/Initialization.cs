namespace DalTest;
using DalApi;
using DO;

/// <param name="Random"> A field that all entities will use, 
/// to generate random numbers while filling in the values ​​of the objects.
/// <param name=""> 
/// <param name=""> 
public static class Initialization
{
    /// <param name=""> fields of the appropriate interface type.
    private static IVolunteer? s_dalVolunteer; 
    private static ICall? s_dalCall; 
    private static IAssignment? s_dalAssignment; 
    private static IConfig? s_dalConfig;

    private static readonly Random s_rand = new();



}
