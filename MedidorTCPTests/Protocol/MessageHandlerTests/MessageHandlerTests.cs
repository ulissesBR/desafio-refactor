using MedidorTCP.Entities.Exceptions;
using MedidorTCP.Entities.Protocol;
using MedidorTCP.Entities.TCP;
using Moq;
using NUnit.Framework;

namespace MedidorTCPTests.Protocol.MessageHandlerTests {
    public class MessageHandlerTests {
        private Mock<IClientHandler> clientHandlerMock;
        private MessageHandler messageHandler;

        [SetUp]
        public void Setup() {
            clientHandlerMock = new Mock<IClientHandler>();
            messageHandler = new MessageHandler(clientHandlerMock.Object);
        }

        [Test]
        public void ExchangeMessage_QuandoReceberMensagemNaPrimeiraTentativa_DeveRetornarMensagem() {
            // Arrange
            var payload = new Payload(new byte[] { 0x01, 0x02 });
            var expectedResponse = new byte[] { 0x7D, 0x05, 0x84, 0x7D, 0xE1, 0xBC, 0x59, 0x2B, 0xD3 };
            int readLength = expectedResponse.Length;

            clientHandlerMock
                .Setup(c => c.ReceiveMessage(readLength))
                .Returns(expectedResponse);

            // Act
            var result = messageHandler.ExchangeMessage(payload, readLength);

            // Assert
            Assert.AreEqual(expectedResponse, result);
            clientHandlerMock.Verify(c => c.SendMessage(payload), Times.Once);
        }

        [Test]
        public void ExchangeMessage_QuandoMensagemValidaApos2Falhas_DeveRetornarMensagem() {

            // Arrange
            var payload = new Payload(new byte[] { 0x01, 0x02 });
            var expectedResponse = new byte[] { 0x7D, 0x05, 0x84, 0x7D, 0xE1, 0xBC, 0x59, 0x2B, 0xD3 };
            int readLength = expectedResponse.Length;

            clientHandlerMock.SetupSequence(c => c.ReceiveMessage(readLength))
                .Returns((byte[])null)
                .Returns((byte[])null)
                .Returns(expectedResponse);

            // Act
            var result = messageHandler.ExchangeMessage(payload, readLength);

            // Assert
            Assert.AreEqual(expectedResponse, result);
            clientHandlerMock.Verify(c => c.SendMessage(payload), Times.Exactly(3));
        }

        [Test]
        public void ExchangeMessage_QuandoNaoRecebeMensagemApos3Tentativas_DeveLancarMessageNotReceivedException() {
            // Arrange
            var payload = new Payload(new byte[] { 0x01, 0x02 });
            int readLength = 3;

            clientHandlerMock.Setup(c => c.ReceiveMessage(readLength)).Returns((byte[])null);

            // Act & Assert
            Assert.Throws<MessageNotReceivedException>(() => {
                messageHandler.ExchangeMessage(payload, readLength);
            });

            clientHandlerMock.Verify(c => c.SendMessage(payload), Times.Exactly(3));
        }

    }
}
