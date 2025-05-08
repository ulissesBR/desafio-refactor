namespace MedidorTCP.Entities.Extensions
{
    public static class Extensions
    {
        // Calcula o chacksum do payload que será enviado para o medidor.
        public static byte CalcularChecksum(this byte[] buf)
        {
            byte checksum = 0x00;

            for (int i = 1; i < buf.Length; i++)
            {
                checksum ^= buf[i];
            }

            return checksum;
        }

        // Confere o CS do payload da mensagem recebida, ignorando o último byte do frame, que é o CS recebido.
        public static byte ConferirChecksum(this byte[] buf)
        {
            byte checksum = 0x00;

            for (int i = 1; i < buf.Length-1; i++)
            {
                checksum ^= buf[i];
            }

            return checksum;
        }

    }
}
