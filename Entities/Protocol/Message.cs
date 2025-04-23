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

        public int ExpectedLength()
        {
            return this.Buffer.Length;
        }

        public int Length()
        {
            return this.BufferLength;
        }

        public override String ToString()
        {
            return BitConverter.ToString(this.Buffer);
        }

        public byte Header()
        {
            return this.Buffer[0];
        }

        public byte MessageLength()
        {
            return this.Buffer[1];
        }

        public byte Function()
        {
            return this.Buffer[2];
        }

        public byte LastByte()
        {
            return this.Buffer[this.Buffer.Length - 1];
        }

        public byte[] Energia()
        {
            byte[] energiaBytes = new byte[4];
            Array.Copy(this.Buffer, 3, energiaBytes, 0, 4);

            // Se o sistema é little-endian, inverte a ordem dos bytes
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(energiaBytes);
            }

            return energiaBytes;
        }

        public byte Checksum()
        {
            return this.Buffer.Checksum();
        }

        public bool IsError()
        {
            return this.Function() == 0xFF && this.MessageLength() == 0;
        }

        public bool IsValorEnergia()
        {
            return this.BufferLength == 8 && Function() == 0x85;
        }

        public bool IsDataHora()
        {
            return this.BufferLength == 9 && this.Function() == 0x84;
        }

        public bool IsRegistroStatus()
        {
            return this.BufferLength == 8 && this.Function() == 0x82;
        }

        public bool IsNumeroDeSerie()
        {
            return this.BufferLength > 3 && this.Function() == 0x81;
        }

        public bool IsIndiceRegistro()
        {
            return this.BufferLength == 5 && Function() == 0x83 && this.Buffer[3] == 0x00;
        }

        public String NumeroDeSerie()
        {
            return System.Text.Encoding.ASCII.GetString(this.Buffer, 3, this.BufferLength - 4);
        }

        public String ExtrairDataHora()
        {
            int ano = (((this.Buffer[3] << 8) | this.Buffer[4]) >> 4);
            int mes = this.Buffer[4] & 0x0F;
            int dia = this.Buffer[5] >> 3;
            int hora = ((this.Buffer[5] & 0x07) << 2) | (this.Buffer[6] >> 6);
            int minuto = this.Buffer[6] & 0x3F;
            int segundo = this.Buffer[7] >> 2;

            Console.WriteLine("Data/Hora: {0:D4}-{1:D2}-{2:D2} {3:D2}:{4:D2}:{5:D2}",
                                    ano, mes, dia, hora, minuto, segundo);

            return ano.ToString("D4") + "-" +
                mes.ToString("D2") + "-" +
                dia.ToString("D2") + " " +
                hora.ToString("D2") + ":" +
                minuto.ToString("D2") + ":" +
                segundo.ToString("D2");
        }
    }
}
