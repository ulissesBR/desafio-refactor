using MedidorTCP.Entities.Driver;
using MedidorTCP.Entities.Exceptions;
using MedidorTCP.Entities.Protocol;
using Moq;
using NUnit.Framework;

namespace MedidorTCPTests {
    public class OperationsTest {
        
        protected Mock<IMessageHandler> MessageHandlerMock;
        protected Mock<IOutputHandler> OutputHandlerMock;
        protected Operations operations;

        [SetUp]
        public void Setup() {
            MessageHandlerMock = new Mock<IMessageHandler>();
            OutputHandlerMock = new Mock<IOutputHandler>();
            operations = new Operations(MessageHandlerMock.Object, OutputHandlerMock.Object);
        }

        [Test]
        public void TryExchangeMessage_QuandoRecebeMensagemComChecksumCorreto_DevePassarNaValidacaoDeCalculoDoChecksum() {

            // Arrange
            var buffer = new byte[] { 0x7D, 0x05, 0x84, 0x7D, 0xE1, 0xBC, 0x59, 0x2B, 0xD3 };
            int payloadSize = buffer.Length;

            var payload = new Payload(buffer);
            var mensagemEsperada = Mensagem.Parse(buffer);

            MessageHandlerMock
               .Setup(m => m.ExchangeMessage(It.IsAny<Payload>(), It.IsAny<int>()))
               .Returns(buffer);

            // Act
            var resultado = operations.TryExchangeMessage(MessageHandlerMock.Object, buffer, payloadSize);

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(mensagemEsperada.Frame.ChecksumCalculado, resultado.Frame.ChecksumRecebido);
        }

        [Test]
        public void TryExchangeMessage_QuandoRecebeMensagemComChecksumInvalido_DeveLancarExcecaoChecksumMismatchException() {
            // Arrange
            var buffer = new byte[] { 0x7D, 0x05, 0x84, 0x7D, 0xE1, 0xBC, 0x59, 0x2B, 0x00 };
            int payloadSize = buffer.Length;

            MessageHandlerMock
                .Setup(m => m.ExchangeMessage(It.IsAny<Payload>(), It.IsAny<int>()))
                .Returns(buffer);

            // Act & Assert
            Assert.Throws<ChecksumMismatchException>(() => {
                operations.TryExchangeMessage(MessageHandlerMock.Object, buffer, payloadSize);
            });
        }
    }
}
