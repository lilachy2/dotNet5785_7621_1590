namespace DO;
[Serializable]
public class DalDoesNotExistException : Exception
{
    public DalDoesNotExistException(string? message) : base(message) { }


}
public class DalAlreadyExistsException : Exception
{
    public DalAlreadyExistsException(string? message) : base(message) { }

}

public class DalDeletionImpossible : Exception
{
    public DalDeletionImpossible(string? message) : base(message) { }
}
public class DalXMLFileLoadCreateException : Exception // Stage3
{
    public DalXMLFileLoadCreateException(string? message) : base(message) { }
}
public class DalIncorrectPasswordException : Exception // Password
{
    public DalIncorrectPasswordException(string? message) : base(message) { }
}
public class Incompatible_ID : Exception // Incompatible_ID In BO.VolunteerImplementation
{
    public Incompatible_ID(string? message) : base(message) { }
}