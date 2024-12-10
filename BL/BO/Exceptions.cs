
namespace BO;

public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, Exception innerException)
                : base(message, innerException) { }
}

// The exception will be thrown, for example, when trying to use a null attribute value in BL.
public class BlNullPropertyException : Exception
{
    public BlNullPropertyException(string? message) : base(message) { }
}

public class BlDeletionImpossibleException : Exception
{
    public BlDeletionImpossibleException(string? message) : base(message) { }
    public BlDeletionImpossibleException(string message, Exception innerException)
              : base(message, innerException) { }
}

