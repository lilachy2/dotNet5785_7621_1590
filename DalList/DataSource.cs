namespace Dal;

internal static class DataSource
{
    /// <param name="Volunteers"> // List to store Volunteer entities </param>
    /// <param name="Calls"> // List to store Call entities </param>
    /// <param name="Assignments"> // List to store Assignment entities </param>
    internal static List<DO.Volunteer?> Volunteers { get; } = new List<DO.Volunteer?> ();
    internal static List<DO.Call> Calls { get; } = new List<DO.Call>();
    internal static List<DO.Assignment> Assignments { get; } = new List<DO.Assignment>();
}

