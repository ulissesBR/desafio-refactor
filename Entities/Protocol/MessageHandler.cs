using System;
using MedidorTCP.Entities.Exceptions;
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
            var tentativas = 3;

            while (tentativas > 0)
            {
                this._clientHandler.SendMessage(payload);
                byte[] frame = this._clientHandler.ReceiveMessage(readLength);

                if (frame != null)
                {
                    return frame;
                }
                else
                {
                    tentativas--;
                }
            }
            throw new MessageNotReceivedException("Falha ao receber mensagem após múltiplas tentativas");
        }
    }
}
