using System;
using MedidorTCP.Entities.TCP;

namespace MedidorTCP.Entities.Protocol
{
    class MessageHandler : IMessageHandler
    {
        private IClientHandler _clientHandler;
        private IClientHandler clientHandler;

        public MessageHandler(IClientHandler clientHandler)
        {
            this._clientHandler = clientHandler;
        }

        //public MessageHandler(IClientHandler clientHandler)
        //{
            //this.clientHandler = clientHandler;
        //}

        public Message ExchangeMessage(Payload payload, int readLength)
        {
            while (true)
            {
                this._clientHandler.SendMessage(payload);
                Message message = this._clientHandler.ReceiveMessage(readLength);

                if (message.IsError())
                {
                    Console.WriteLine("FRAME INCORRETO... Reenviando");
                }
                else
                {
                    return message;
                }
            }
        }
    }
}
