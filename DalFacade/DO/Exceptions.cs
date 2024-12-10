namespace DO
{
    [Serializable]
    public class DalDoesNotExistException : Exception
    {
        public DalDoesNotExistException(string? message) : base(message) { }
    }

    [Serializable]
    public class DalAlreadyExistsException : Exception
    {
        public DalAlreadyExistsException(string? message) : base(message) { }
    }

    [Serializable]
    public class DalDeletionImpossible : Exception
    {
        public DalDeletionImpossible(string? message) : base(message) { }
    }

    [Serializable]
    public class DalXMLFileLoadCreateException : Exception
    {
        public DalXMLFileLoadCreateException(string? message) : base(message) { }
    }

    [Serializable]
    public class DalIncorrectPasswordException : Exception
    {
        public DalIncorrectPasswordException(string? message) : base(message) { }
    }

    [Serializable]
    public class Incompatible_ID : Exception
    {
        public Incompatible_ID(string? message) : base(message) { }
    }
    [Serializable]
    public class DalDeletionImpossibleException : Exception
    {
        public DalDeletionImpossibleException(string? message) : base(message) { }

        public DalDeletionImpossibleException(string? message, Exception innerException)
            : base(message, innerException) { }
    }
    [Serializable]
    public class DalException : Exception
    {
        public DalException(string? message) : base(message) { }

        public DalException(string? message, Exception innerException)
            : base(message, innerException) { }
    }
}
