using System;
using System.Runtime.Serialization;

namespace SuppliersLibrary.Exceptions
{
    public class BadFileException : Exception
    {
        public BadFileException() : base("Something is wrong with the file.")
        {
        }

        public BadFileException(string message) : base(message)
        {
        }

        public BadFileException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BadFileException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
