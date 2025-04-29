using System;

namespace MedidorTCP.Entities.Exceptions
{
    public class ChecksumMismatchException : Exception
    {
        public ChecksumMismatchException()
        {

        }

        public ChecksumMismatchException(string message)
            : base(message)
        {

        }

        public ChecksumMismatchException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }

}
