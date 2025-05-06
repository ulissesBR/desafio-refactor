using System;
using MedidorTCP.Entities.Extensions;

namespace MedidorTCP.Entities.Protocol
{
    public class Mensagem
    {
        private const int FrameOverhead = 4;            // Tamanho do cabeçalho do frame (Header + Length + Function + Checksum => 1 byte cada)

        public byte[] Buffer { get; private set; }      // Buffer com o raw data
        public int BufferLength { get; private set; }   // Tamanho do buffer

        /*public int ExpectedLength { get; private set; }*/ // Tamanho esperado do buffer

        // Descrição dos bytes do frame:
        public byte Header { get; private set; }        // Header
        public byte FrameLength { get; private set; }   // Tamanho do frame
        public byte Function { get; private set; }      // Id da função
        public byte LastByte { get; private set; }      // Byte de Checksum
        public byte Checksum { get; private set; }      // Checksum

        // Booleanos 
        public bool IsError { get; private set; }           // Indica que ocorreu um erro
        public bool IsValorEnergia { get; private set; }    // Indica se é um valor de energia válido
        public bool IsDataHora { get; private set; }        // Indica se é uma resposta válida de data/hora
        public bool IsRegistroStatus { get; private set; }  // Indica se é uma resposta válida de registro de status
        public bool IsNumeroDeSerie { get; private set; }   // Indica se é uma resposta válida de número de série
        public bool IsIndiceRegistro { get; private set; }  // Indica se é uma resposta válida de índice de registro
                

        private Mensagem(byte[] rawData)
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

            // Verificação de erro:
            this.IsError = this.GetIsError();
            this.IsValorEnergia = this.GetIsValorEnergia();
            this.IsDataHora = this.GetIsDataHora();
            this.IsRegistroStatus = this.GetIsRegistroStatus();
            this.IsNumeroDeSerie = this.GetIsNumeroDeSerie();
            this.IsIndiceRegistro = this.GetIsIndiceRegistro();
            // FrameIdentifier.cs
            // Passar o byte array para verificação e identificação. Retorna o objeto para decisão.
        }

        public static Mensagem Parse(byte[] buffer)
        {
            return new Mensagem(buffer);
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

        public override String ToString()
        {
            return BitConverter.ToString(this.Buffer);
        }

    }
}
