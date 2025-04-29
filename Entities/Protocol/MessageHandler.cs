using System;
using MedidorTCP.Entities.TCP;

namespace MedidorTCP.Entities.Protocol
{
    class MessageHandler : IMessageHandler
    {
        private IClientHandler _clientHandler;

        public MessageHandler(IClientHandler clientHandler)
        {
            this._clientHandler = clientHandler;
        }

        public byte[] ExchangeMessage(Payload payload, int readLength)
        {
            while (true)
            {
                this._clientHandler.SendMessage(payload);
                byte[] frame = this._clientHandler.ReceiveMessage(readLength);

                return frame;
            }
        }
    }
}
