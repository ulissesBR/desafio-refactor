using NUnit.Framework;
using MedidorTCP.Entities.Protocol;
using MedidorTCP.Entities.Enums;

namespace MedidorTCP.Tests.Entities.Protocol
{
    [TestFixture]
    public class MensagemTests
    {
        [Test]
        public void Parse_AoVerificarTipoDeMensagemRecebida_DeveDetectarErro()
        {
            // Arrange
            var buffer = new byte[] { 0x7D, 0x00, 0xFF, 0x00 };

            // Act
            var mensagem = Mensagem.Parse(buffer);

            // Assert
            Assert.AreEqual(TipoMensagem.Erro, mensagem.Tipo);
        }

        [Test]
        public void Parse_AoVerificarTipoDeMensagemDataHoraId84_DeveRetornarTipoDataHora()
        {
            // Arrange
            var buffer = new byte[] { 0x7D, 0x05, 0x84, 0x7D, 0xE1, 0xBC, 0x59, 0x2B, 0xD3 };

            // Act
            var mensagem = Mensagem.Parse(buffer);

            // Assert
            Assert.AreEqual(TipoMensagem.DataHoraTipo, mensagem.Tipo);
        }

        [Test]
        public void Parse_AoReceberMensagemDeTipoDesconhecido_DeveRetornarTipoDeMensagemDesconhecido()
        {
            // Arrange
            var buffer = new byte[] { 0x7D, 0x02, 0xAB, 0x00, 0x00, 0x00 };

            // Act
            var mensagem = Mensagem.Parse(buffer);

            // Assert
            Assert.AreEqual(TipoMensagem.Desconhecida, mensagem.Tipo);
        }

        [Test]
        public void GetBufferLength_AoCalcularTamanhoDoBuffer_DeveRetornarTamanhoCorreto()
        {
            // Arrange
            var buffer = new byte[] { 0x7D, 0x05, 0x84, 0x7D, 0xE1, 0xBC, 0x59, 0x2B, 0xD3 }; 
                                    
            var mensagem = Mensagem.Parse(buffer);

            // Act
            var length = mensagem.GetBufferLength();

            // Assert
            Assert.AreEqual(9, length);
        }
    }
}
