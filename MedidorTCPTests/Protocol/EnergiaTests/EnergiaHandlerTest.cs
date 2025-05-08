using MedidorTCP.Entities.Enums;
using MedidorTCP.Entities.Exceptions;
using MedidorTCP.Entities.Logging;
using MedidorTCP.Entities.Protocol;
using Moq;
using NUnit.Framework;

namespace MedidorTCPTests
{
    public class EnergiaHandlerTest
    {
        protected Mock<IMessageHandler> MessageHandlerMock;
        protected Mock<ILogger> LoggerMock;
        protected Mock<IOperations> OperationsMock;

        [SetUp]
        public void SetUp()
        {
            MessageHandlerMock = new Mock<IMessageHandler>();
            LoggerMock = new Mock<ILogger>();
            OperationsMock = new Mock<IOperations>();

            LoggerMock
                .Setup(x => x.WithContext(It.IsAny<string>()))
                .Returns(new Mock<ILogger>().Object);
        }

        [Test]
        public void LerValorEnergia_QuandoMensagemValida_DeveRetornarValorDaEnergiaCorreto()
        {
            // Arrange
            var buffer = new byte[] { 0x7D, 0x04, 0x85, 0x46, 0x3C, 0x18, 0xB4, 0x57 };
            var mensagemRecebidaMock = Mensagem.Parse(buffer);

            OperationsMock
                .Setup(o => o.TryExchangeMessage(It.IsAny<IMessageHandler>(), It.IsAny<byte[]>(), It.IsAny<int>()))
                .Returns(mensagemRecebidaMock);

            var handler = new EnergiaHandler(MessageHandlerMock.Object, LoggerMock.Object, OperationsMock.Object);

            // Act
            string resultado = handler.LerValorEnergia();

            // Assert
            Assert.AreEqual("12,04", resultado);
        }

        [Test]
        public void LerValorEnergia_ComErroDeChecksum_RetornaExceptionEsperada()
        {
            // Arrange
            var resultadoEsperado = "Falha ao ler o valor de energia após múltiplas tentativas";

            OperationsMock.Setup(x => x.TryExchangeMessage(It.IsAny<IMessageHandler>(), It.IsAny<byte[]>(), It.IsAny<int>()))
                .Throws(new ChecksumMismatchException("Erro de checksum"));

            var valorEnergiaHandler = new EnergiaHandler(MessageHandlerMock.Object, LoggerMock.Object, OperationsMock.Object);

            // Act
            var resultado = valorEnergiaHandler.LerValorEnergia();

            // Assert
            Assert.AreEqual(resultadoEsperado, resultado);
        }
        [Test]
        public void LerValorEnergia_ComErroDeMensagemNaoRecebida_RetornaExceptionEsperada()
        {
            // Arrange
            var resultadoEsperado = "Falha ao ler o valor de energia após múltiplas tentativas";

            OperationsMock.Setup(x => x.TryExchangeMessage(It.IsAny<IMessageHandler>(), It.IsAny<byte[]>(), It.IsAny<int>()))
                .Throws(new MessageNotReceivedException("Erro ao receber mensagem."));

            var valorEnergiaHandler = new EnergiaHandler(MessageHandlerMock.Object, LoggerMock.Object, OperationsMock.Object);

            // Act
            var resultado = valorEnergiaHandler.LerValorEnergia();

            // Assert
            Assert.AreEqual(resultadoEsperado, resultado);
        }
    }
}
