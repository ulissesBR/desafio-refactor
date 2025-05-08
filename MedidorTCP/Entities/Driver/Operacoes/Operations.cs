using System;
using MedidorTCP.Entities.Exceptions;
using MedidorTCP.Entities.Protocol;

namespace MedidorTCP.Entities.Driver
{
    public class Operations : IOperations
    {
        private IMessageHandler _messageHandler;
        private IOutputHandler _outputHandler;

        private String numeroDeSerie;

        public Operations(IMessageHandler messageHandler, IOutputHandler outputHandler)
        {
            this._messageHandler = messageHandler;
            this._outputHandler = outputHandler;
        }

        public Mensagem TryExchangeMessage(IMessageHandler handler, byte[] rawPayload, int payloadSize)
        {
            Payload payload = new Payload(rawPayload);
            var frame = handler.ExchangeMessage(payload, payloadSize);
            var messageParsed = Mensagem.Parse(frame);

            if (messageParsed.Frame.ChecksumCalculado == messageParsed.Frame.ChecksumRecebido)
            {
                return messageParsed;
            }
            else
            {
                throw new ChecksumMismatchException("Checksum recebido é diferente do checksum calculado.");
            }
        }
    }
}
