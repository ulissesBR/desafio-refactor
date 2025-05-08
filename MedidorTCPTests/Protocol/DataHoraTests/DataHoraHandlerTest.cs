using MedidorTCP.Entities.Enums;
using MedidorTCP.Entities.Exceptions;
using MedidorTCP.Entities.Logging;
using MedidorTCP.Entities.Protocol;
using Moq;
using NUnit.Framework;

namespace MedidorTCPTests
{
    public class DataHoraHandlerTest
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
        // Método, comportamento, resultado esperado.
        public void LerDataHora_ComErroDeChecksum_RetornaExceptionEsperada()
        {
            // Arrange
            var resultadoEsperado = "Erro na leitura da data/hora.";

            OperationsMock.Setup(x => x.TryExchangeMessage(It.IsAny<IMessageHandler>(), It.IsAny<byte[]>(), It.IsAny<int>()))
                .Throws(new ChecksumMismatchException("Erro de checksum"));

            var dataHoraHandler = new DataHoraHandler(MessageHandlerMock.Object, LoggerMock.Object, OperationsMock.Object);

            // Act
            var resultado = dataHoraHandler.LerDataHora();

            // Assert
            Assert.AreEqual(resultadoEsperado, resultado);
        }

        [Test]
        public void LerDataHora_ComErroDeMensagemNaoRecebida_RetornaExceptionEsperada()
        {
            // Arrange
            var resultadoEsperado = "Erro na leitura da data/hora.";

            OperationsMock.Setup(x => x.TryExchangeMessage(It.IsAny<IMessageHandler>(), It.IsAny<byte[]>(), It.IsAny<int>()))
                .Throws(new MessageNotReceivedException("Falha ao receber mensagem após múltiplas tentativas"));

            var dataHoraHandler = new DataHoraHandler(MessageHandlerMock.Object, LoggerMock.Object, OperationsMock.Object);

            // Act
            var resultado = dataHoraHandler.LerDataHora();

            // Assert
            Assert.AreEqual(resultadoEsperado, resultado);
        }

        [Test]
        public void LerDataHora_QuandoMensagemValida_DeveRetornarDataHoraCorreta()
        {
            // Arrange
            var buffer = new byte[] { 0x7D, 0x05, 0x84, 0x7D, 0xE1, 0xBC, 0x59, 0x2B, 0xD3 }; // Buffer com mensagem válida
            var mensagemRecebidaMock = Mensagem.Parse(buffer);

            OperationsMock
                .Setup(o => o.TryExchangeMessage(It.IsAny<IMessageHandler>(), It.IsAny<byte[]>(), It.IsAny<int>()))
                .Returns(mensagemRecebidaMock);

            var handler = new DataHoraHandler(MessageHandlerMock.Object, LoggerMock.Object, OperationsMock.Object);

            // Act
            string resultado = handler.LerDataHora();

            // Assert
            Assert.AreEqual("2014-01-23 17:25:10", resultado); // Valor válido que está contido no buffer referenciado
        }
    }
}
