using System;
using MedidorTCP.Entities.Driver.Frame;
using MedidorTCP.Entities.Enums;

namespace MedidorTCP.Entities.Protocol
{
    public class Mensagem
    {
        private const int FrameOverhead = 4;            // Tamanho do cabeçalho do frame (Header + Length + Function + Checksum => 1 byte cada)
       
        public byte[] Buffer { get; }                   // Buffer com o frame cru
        public int BufferLength { get; private set; }   // Tamanho do buffer
        public Frame Frame { get; }                     // Objeto com as informações do frame
        
        public TipoMensagem Tipo;                       // Enum com os tipos das mensagens

        private Mensagem(byte[] buffer)
        {
            this.Buffer = buffer;
            this.BufferLength = GetBufferLength();
            this.Frame = new Frame(buffer);
            this.Tipo = IdentificadorDeMensagem.Identificar(Frame, BufferLength, Buffer);
        }

        public static Mensagem Parse(byte[] buffer)
        {
            return new Mensagem(buffer);
        }

        public int GetBufferLength()
        {
            return this.Buffer[1] + FrameOverhead;
        }

        public override String ToString()
        {
            return BitConverter.ToString(this.Buffer);
        }

    }
}
