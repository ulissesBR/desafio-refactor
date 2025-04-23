using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedidorTCP.Entities
{
    public static class Extensions
    {
        public static byte checksum(this byte[] buf)
        {
            byte checksum = 0x00;

            for (int i = 1; i < buf.Length; i++)
            {
                checksum ^= buf[i];
            }

            return checksum;
        }
    }
}
