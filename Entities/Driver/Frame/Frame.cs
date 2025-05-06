using MedidorTCP.Entities.Extensions;

namespace MedidorTCP.Entities.Driver.Frame
{
    public class Frame
    {
        // Descrição dos bytes do frame:
        public byte Header { get; }                 // Header
        public byte FrameLength { get; }            // Tamanho do frame
        public byte Function { get; }               // Id da função
        public byte IndiceSucessoOuErro { get; }    // Indica sucesso ou erro ao definir registro a ser lido
        public byte ChecksumRecebido { get; }               // Byte de Checksum
        public byte ChecksumCalculado { get; }               // Checksum

        public Frame(byte[] buffer)
        {
            this.Header = buffer[0];
            this.FrameLength = buffer[1];
            this.Function = buffer[2];
            this.IndiceSucessoOuErro = buffer[3];
            this.ChecksumRecebido = buffer[buffer.Length - 1];
            this.ChecksumCalculado = buffer.ConferirChecksum();
        }

    }
}
