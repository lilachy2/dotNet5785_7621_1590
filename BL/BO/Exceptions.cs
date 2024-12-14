
namespace BO;

public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, Exception innerException)
                : base(message, innerException) { }
}public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message) : base(message) { }
    public BlAlreadyExistsException(string message, Exception innerException)
                : base(message, innerException) { }
}

// The exception will be thrown, for example, when trying to use a null attribute value in BL.
public class BlNullPropertyException : Exception
{
    public BlNullPropertyException(string? message) : base(message) { }
}
public class Incompatible_ID : Exception
{
    public Incompatible_ID(string? message) : base(message) { }
}
public class BlInvalidaddress : Exception // Invalid address
{
    public BlInvalidaddress(string? message) : base(message) { }
}
public class BlIncorrectPasswordException : Exception
{
    public BlIncorrectPasswordException(string? message) : base(message) { }
}
public class BlMaximum_time_to_finish_readingException : Exception
{
    public BlMaximum_time_to_finish_readingException(string? message) : base(message) { }
}

public class BlDeletionImpossibleException : Exception
{
    public BlDeletionImpossibleException(string? message) : base(message) { }
    public BlDeletionImpossibleException(string message, Exception innerException)
              : base(message, innerException) { }
}
//cant update
//public class BlGeneralException : Exception
//{
//    public BlGeneralException(string? message) : base(message) { }
//    public BlGeneralException(string message, Exception innerException)
//              : base(message, innerException) { }
//}
public class BlWrongItemtException : Exception
{
    public BlWrongItemtException(string? message) : base(message) { }
    public BlWrongItemtException(string message, Exception innerException)
              : base(message, innerException) { }
}

public class InvalidOperationException : Exception
{
    public InvalidOperationException(string? message) : base(message) { }
    public InvalidOperationException(string message, Exception innerException)
              : base(message, innerException) { }
}

public class BlPermissionException : Exception
{
    public BlPermissionException(string? message) : base(message) { }
    public BlPermissionException(string message, Exception innerException)
              : base(message, innerException) { }
}
public class BlNo_assignments_volunteerException : Exception
{
    public BlNo_assignments_volunteerException(string? message) : base(message) { }
    public BlNo_assignments_volunteerException(string message, Exception innerException)
              : base(message, innerException) { }
}

public class BlCallStatusNotOpenException : Exception
{
    public BlCallStatusNotOpenException(string? message) : base(message) { }
    public BlCallStatusNotOpenException(string message, Exception innerException)
              : base(message, innerException) { }
}
public class BlGetCloseCallException : Exception
{
    public BlGetCloseCallException(string? message) : base(message) { }
    public BlGetCloseCallException(string message, Exception innerException)
              : base(message, innerException) { }
}
public class BlGetOpenCallException : Exception
{
    public BlGetOpenCallException(string? message) : base(message) { }
    public BlGetOpenCallException(string message, Exception innerException)
              : base(message, innerException) { }
}


