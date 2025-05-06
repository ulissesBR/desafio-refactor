using System;

namespace MedidorTCP.Entities.Exceptions
{
    public class MessageNotReceivedException : Exception
    {
        public MessageNotReceivedException()
        {

        }

        public MessageNotReceivedException(string message)
            :base(message)
        {

        }

        public MessageNotReceivedException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
