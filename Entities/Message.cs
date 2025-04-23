using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedidorTCP.Entities
{
    public class Message
    {
        public int length { get; private set; }
        public byte[] buffer { get; private set; }

        public Message(byte[] buffer, int length)
        {
            this.buffer = buffer;
            this.length = length;
        }

        public int ExpectedLength()
        {
            return this.buffer.Length;
        }

        public int Length()
        {
            return this.length;
        }

        public override String ToString()
        {
            return BitConverter.ToString(this.buffer);
        }

        public byte Header()
        {
            return this.buffer[0];
        }

        public byte MessageLength()
        {
            return this.buffer[1];
        }

        public byte Function()
        {
            return this.buffer[2];
        }

        public byte LastByte()
        {
            return this.buffer[this.buffer.Length - 1];
        }

        public byte[] Energia()
        {
            byte[] energiaBytes = new byte[4];
            Array.Copy(this.buffer, 3, energiaBytes, 0, 4);

            // Se o sistema é little-endian, inverte a ordem dos bytes
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(energiaBytes);
            }

            return energiaBytes;
        }

        public byte Checksum()
        {
            return this.buffer[this.buffer.Length - 1];//this.buffer.checksum();
        }

        public bool IsError()
        {
            return this.Function() == 0xFF && this.MessageLength() == 0;
        }

        public bool IsValorEnergia()
        {
            return this.length == 8 && Function() == 0x85;
        }

        public bool IsDataHora()
        {
            return this.length == 9 && this.Function() == 0x84;
        }

        public bool IsRegistroStatus()
        {
            return this.length == 8 && this.Function() == 0x82;
        }

        public bool IsNumeroDeSerie()
        {
            return this.length > 3 && this.Function() == 0x81;
        }

        public bool IsIndiceRegistro()
        {
            return this.length == 5 && Function() == 0x83 && this.buffer[3] == 0x00;
        }

        public String NumeroDeSerie()
        {
            return System.Text.Encoding.ASCII.GetString(this.buffer, 3, this.length - 4);
        }

        public String ExtrairDataHora()
        {
            int ano = (((this.buffer[3] << 8) | this.buffer[4]) >> 4);
            int mes = this.buffer[4] & 0x0F;
            int dia = this.buffer[5] >> 3;
            int hora = ((this.buffer[5] & 0x07) << 2) | (this.buffer[6] >> 6);
            int minuto = this.buffer[6] & 0x3F;
            int segundo = this.buffer[7] >> 2;

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
