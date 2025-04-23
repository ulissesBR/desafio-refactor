using System;

namespace MedidorTCP.Entities.Protocol
{

    public class Buffer
    {
        public byte[] BufferData { get; set; }
        public int BytesRead { get; set; }

        public Buffer(byte[] bufferData, int bytesRead)
        {
            this.BufferData = bufferData;
            this.BytesRead = bytesRead;
        }

        public byte Header()
        {
            return this.BufferData[0];
        }

        public byte MessageLength()
        {
            return this.BufferData[1];
        }

        public byte Function()
        {
            return this.BufferData[2];
        }

        public byte LastByte()
        {
            return BufferData[BufferData.Length - 1];
        }

        public byte[] Energia()
        {
            byte[] energiaBytes = new byte[4];
            Array.Copy(BufferData, 3, energiaBytes, 0, 4);

            // Se o sistema é little-endian, inverte a ordem dos bytes
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(energiaBytes);
            }

            return energiaBytes;
        }

        public byte Checksum()
        {
            byte checksum = 0x00;

            for (int i = 1; i < BufferData.Length - 1; i++)
            {
                checksum ^= BufferData[i];
            }

            return checksum;
        }

        public bool IsError()
        {
            return BufferData[2] == 0xFF && BufferData[1] == 0;
        }

        public bool IsValorEnergia()
        {
            return BytesRead == 8 && Function() == 0x85;
        }

        public bool IsDataHora()
        {
            return BytesRead == 9 && BufferData[2] == 0x84;
        }

        public bool IsRegistroStatus()
        {
            return BytesRead == 8 && BufferData[2] == 0x82;
        }

        public bool IsNumeroDeSerie()
        {
            return BytesRead > 3 && BufferData[2] == 0x81;
        }

        public bool IsIndiceRegistro()
        {
            return BytesRead == 5 && Function() == 0x83 && BufferData[3] == 0x00;
        }

        public String NumeroDeSerie()
        {
            return System.Text.Encoding.ASCII.GetString(BufferData, 3, BytesRead - 4);
        }

        public String ExtrairDataHora()
        {
            int ano = (((BufferData[3] << 8) | BufferData[4]) >> 4);
            int mes = BufferData[4] & 0x0F;
            int dia = BufferData[5] >> 3;
            int hora = ((BufferData[5] & 0x07) << 2) | (BufferData[6] >> 6);
            int minuto = BufferData[6] & 0x3F;
            int segundo = BufferData[7] >> 2;

            Console.WriteLine("Data/Hora: {0:D4}-{1:D2}-{2:D2} {3:D2}:{4:D2}:{5:D2}",
                                    ano, mes, dia, hora, minuto, segundo);

            return ano.ToString("D4") + "-" +
                mes.ToString("D2") + "-" +
                dia.ToString("D2") + " " +
                hora.ToString("D2") + ":" +
                minuto.ToString("D2") + ":" +
                segundo.ToString("D2");
        }

        public override String ToString()
        {
            return BitConverter.ToString(BufferData);
        }
    }
}
