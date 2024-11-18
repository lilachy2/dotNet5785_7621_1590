namespace DO;
[Serializable]
public class DalAlreadyExistsException : Exception
{
    public DalAlreadyExistsException(string? message) : base(message) 
    {}
}

[Serializable]
public class DalDeletionImpossible : Exception
{
    public DalDeletionImpossible(string? message) : base(message)
    { }
}

