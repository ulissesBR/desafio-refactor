namespace MedidorTCP.Entities.Extensions
{
    public static class Extensions
    {
        public static byte Checksum(this byte[] buf)
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
