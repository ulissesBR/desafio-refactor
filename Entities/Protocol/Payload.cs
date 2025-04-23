using System;
using MedidorTCP.Entities.Extensions;

namespace MedidorTCP.Entities.Protocol
{
    public class Payload
    {
        public byte[] Data { get; private set; }
        public byte Checksum { get; private set; }

        public Payload(byte[] payload)
        {
            this.Data = new byte[payload.Length + 1];
            Array.Copy(payload, this.Data, payload.Length);

            this.Checksum = payload.Checksum();
            this.Data[this.Data.Length - 1] = this.Checksum;
        }
    }
}
