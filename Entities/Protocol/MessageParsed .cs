using System;
using MedidorTCP.Entities.Extensions;

namespace MedidorTCP.Entities.Protocol
{
    public class MessageParsed
    {
        private const int FrameOverhead = 4;            // Tamanho do cabeçalho do frame (Header + Length + Function + Checksum => 1 byte cada)

        public byte[] Buffer { get; private set; }      // Buffer com o raw data
        public int BufferLength { get; private set; }   // Tamanho do buffer

        /*public int ExpectedLength { get; private set; }*/ // Tamanho esperado do buffer

        // Descrição dos bytes do frame:
        public byte Header { get; private set; }        // Header
        public byte FrameLength { get; private set; }   // Tamanho do frame
        public byte Function { get; private set; }      // Id da função
        public byte LastByte { get; private set; }      // Byte ce Checksum
        public byte[] Energia { get; private set; }     // Valor da energia lida
        public byte Checksum { get; private set; }      // Checksum

        // Booleanos 
        public bool IsError { get; private set; }           // Indica que ocorreu um erro
        public bool IsValorEnergia { get; private set; }    // Indica se é um valor de energia válido
        public bool IsDataHora { get; private set; }        // Indica se é uma resposta válida de data/hora
        public bool IsRegistroStatus { get; private set; }  // Indica se é uma resposta válida de registro de status
        public bool IsNumeroDeSerie { get; private set; }   // Indica se é uma resposta válida de número de série
        public bool IsIndiceRegistro { get; private set; }  // Indica se é uma resposta válida de índice de registro

        // Grandezas
        public String NumeroDeSerie { get; private set; }   // Número de série do medidor
        public String DataHora { get; private set; }        // Data e hora da leitura
                

        private MessageParsed(byte[] rawData)
        {
            this.Buffer = rawData;
            //this.ExpectedLength = expectedLength;
            this.BufferLength = GetBufferLength();

            // Dados comuns para todos os frames:
            this.Header = this.GetHeader();
            this.FrameLength = this.GetMessageLength();
            this.Function = this.GetFunction();
            this.LastByte = this.GetLastByte();
            this.Checksum = this.GetChecksum();

            // Dados de funções (mensagens) específicas:
            this.Energia = this.GetEnergia();
            this.NumeroDeSerie = this.GetNumeroDeSerie();
            this.DataHora = this.GetDataHora();

            // Booleanos para validação do frame:
            this.IsError = this.GetIsError();
            this.IsValorEnergia = this.GetIsValorEnergia();
            this.IsDataHora = this.GetIsDataHora();
            this.IsRegistroStatus = this.GetIsRegistroStatus();
            this.IsNumeroDeSerie = this.GetIsNumeroDeSerie();
            this.IsIndiceRegistro = this.GetIsIndiceRegistro();


        }

        public static MessageParsed Parse(byte[] buffer)
        {
            return new MessageParsed(buffer);
        }

        public int GetBufferLength()
        {
            return this.GetMessageLength() + FrameOverhead;
        }

        public byte GetHeader()
        {
            return this.Buffer[0];
        }

        public byte GetMessageLength()
        {
            return this.Buffer[1];
        }

        public byte GetFunction()
        {
            return this.Buffer[2];
        }

        public byte GetLastByte()
        {
            return this.Buffer[this.Buffer.Length - 1];
        }

        public byte[] GetEnergia()
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

        public byte GetChecksum()
        {
            return this.Buffer.Checksum();
        }

        public bool GetIsError()
        {
            return this.GetFunction() == 0xFF && this.GetMessageLength() == 0;
        }

        public bool GetIsValorEnergia()
        {
            return this.BufferLength == 8 && GetFunction() == 0x85;
        }

        public bool GetIsDataHora()
        {
            return this.BufferLength == 9 && this.GetFunction() == 0x84;
        }

        public bool GetIsRegistroStatus()
        {
            return this.BufferLength == 8 && this.GetFunction() == 0x82;
        }

        public bool GetIsNumeroDeSerie()
        {
            return this.BufferLength > 3 && this.GetFunction() == 0x81;
        }

        public bool GetIsIndiceRegistro()
        {
            return this.BufferLength == 5 && GetFunction() == 0x83 && this.Buffer[3] == 0x00;
        }

        public String GetNumeroDeSerie()
        {
            return System.Text.Encoding.ASCII.GetString(this.Buffer, 3, this.BufferLength - 4);
        }

        public String GetDataHora()
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

        public override String ToString()
        {
            return BitConverter.ToString(this.Buffer);
        }

    }
}
