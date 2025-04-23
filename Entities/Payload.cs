using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedidorTCP.Entities
{
    public class Payload
    {
        public byte[] data { get; private set; }
        public byte checksum { get; private set; }

        public Payload(byte[] payload)
        {
            this.data = new byte[payload.Length + 1];
            Array.Copy(payload, this.data, payload.Length);

            this.checksum = payload.checksum();
            this.data[this.data.Length - 1] = this.checksum;
        }
    }
}
