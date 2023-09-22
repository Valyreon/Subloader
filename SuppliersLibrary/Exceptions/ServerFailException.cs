using System;
using System.Runtime.Serialization;

namespace SuppliersLibrary.Exceptions;

public class ServerFailException : Exception
{
    public ServerFailException() : base("Server failed.")
    {
    }

    public ServerFailException(string message) : base(message)
    {
    }

    public ServerFailException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected ServerFailException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
