using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedidorTCP.Entities
{
    class MessageHandler : IMessageHandler
    {
        private IClientHandler clientHandler;

        public MessageHandler(IClientHandler clientHandler)
        {
            this.clientHandler = clientHandler;
        }

        public Message ExchangeMessage(Payload payload, int readLength)
        {
            while (true)
            {
                this.clientHandler.SendMessage(payload);
                Message message = this.clientHandler.ReceiveMessage(readLength);

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
