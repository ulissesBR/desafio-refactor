using System;

namespace MedidorTCP.Entities.Protocol
{
    public class MensagemEnergia
    {
        public byte[] EnergiaBruta { get; private set; }

        public MensagemEnergia(byte[] buffer)
        {
            EnergiaBruta = new byte[4];
            Array.Copy(buffer, 3, EnergiaBruta, 0, 4);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(EnergiaBruta);
            }
        }

        public int EnergiaComoInteiro()
        {
            return BitConverter.ToInt32(EnergiaBruta, 0);
        }

        public override string ToString()
        {
            return $"Energia (int): {EnergiaComoInteiro()}";
        }
    }
}
