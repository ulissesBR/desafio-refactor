using NUnit.Framework;
using MedidorTCP.Entities.Driver.Frame;
using MedidorTCP.Entities.Extensions;

namespace MedidorTCP.Tests {
    public class FrameTests {
        [Test]
        public void ClasseFrame_AoReceberUmaMensagemParaTratamento_DeveExtrairHeaderCorretamente() {
            var buffer = new byte[] { 0x7D, 0x05, 0x84, 0x7D, 0xE1, 0xBC, 0x59, 0x2B, 0xD3 };
            var frame = new Frame(buffer);

            Assert.AreEqual(0x7D, frame.Header);
        }

        [Test]
        public void ClasseFrame_AoReceberUmaMensagemParaTratamento_DeveExtrairFrameLengthCorretamente() {
            var buffer = new byte[] { 0x7D, 0x05, 0x84, 0x7D, 0xE1, 0xBC, 0x59, 0x2B, 0xD3 };
            var frame = new Frame(buffer);

            Assert.AreEqual(0x05, frame.FrameLength);
        }

        [Test]
        public void ClasseFrame_AoReceberUmaMensagemParaTratamento_DeveExtrairFunctionCorretamente() {
            var buffer = new byte[] { 0x7D, 0x05, 0x84, 0x7D, 0xE1, 0xBC, 0x59, 0x2B, 0xD3 };
            var frame = new Frame(buffer);

            Assert.AreEqual(0x84, frame.Function);
        }

        [Test]
        public void ClasseFrame_AoReceberUmaMensagemParaTratamento_DeveExtrairIndiceSucessoOuErroCorretamente() {
            var buffer = new byte[] { 0x7D, 0x05, 0x84, 0x7D, 0xE1, 0xBC, 0x59, 0x2B, 0xD3 };
            var frame = new Frame(buffer);

            Assert.AreEqual(0x7D, frame.IndiceSucessoOuErro);
        }

        [Test]
        public void ClasseFrame_AoReceberUmaMensagemParaTratamento_DeveExtrairChecksumRecebidoCorretamente() {
            var buffer = new byte[] { 0x7D, 0x05, 0x84, 0x7D, 0xE1, 0xBC, 0x59, 0x2B, 0xD3 };
            var frame = new Frame(buffer);

            Assert.AreEqual(0xD3, frame.ChecksumRecebido);
        }

        [Test]
        public void ClasseFrame_AoReceberUmaMensagemParaTratamento_DeveCalcularChecksumCorretamente() {
            var buffer = new byte[] { 0x7D, 0x05, 0x84, 0x7D, 0xE1, 0xBC, 0x59, 0x2B, 0xD3 };
            var frame = new Frame(buffer);

            // Verifica se o ChecksumCalculado bate com o esperado
            Assert.AreEqual(buffer.ConferirChecksum(), frame.ChecksumCalculado);
        }
    }
}
