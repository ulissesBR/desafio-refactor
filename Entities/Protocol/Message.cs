using System;
using MedidorTCP.Entities.Extensions;

namespace MedidorTCP.Entities.Protocol
{
    public class Message
    {
        public int BufferLength { get; private set; }
        public byte[] Buffer { get; private set; }

        public Message(byte[] buffer, int length)
        {
            this.Buffer = buffer;
            this.BufferLength = length;
        }
    }
}
