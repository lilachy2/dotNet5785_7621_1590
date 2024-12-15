
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


public class BlDeletionImpossibleException : Exception
{
    public BlDeletionImpossibleException(string? message) : base(message) { }
    public BlDeletionImpossibleException(string message, Exception innerException)
              : base(message, innerException) { }
}
//cant update
public class BlGeneralException : Exception
{
    public BlGeneralException(string? message) : base(message) { }
    public BlGeneralException(string message, Exception innerException)
              : base(message, innerException) { }
}
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

public class BlCallStatusNotOKException : Exception
{
    public BlCallStatusNotOKException(string? message) : base(message) { }
    public BlCallStatusNotOKException(string message, Exception innerException)
              : base(message, innerException) { }
}
public class BlMaximum_time_to_finish_readingException : Exception
{
    public BlMaximum_time_to_finish_readingException(string? message) : base(message) { }
    public BlMaximum_time_to_finish_readingException(string message, Exception innerException)
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


public class Bl_Volunteer_Cant_UpdateCancelTreatmentException : Exception
{
    public Bl_Volunteer_Cant_UpdateCancelTreatmentException(string? message) : base(message) { }
    public Bl_Volunteer_Cant_UpdateCancelTreatmentException(string message, Exception innerException)
              : base(message, innerException) { }
}


