using System;
using MedidorTCP.Entities.Exceptions;
using MedidorTCP.Entities.Enums;
using MedidorTCP.Entities.Driver;

namespace MedidorTCP.Entities.Protocol
{
    public class SerieHandler
    {
        private readonly IMessageHandler _messageHandler;

        public SerieHandler(IMessageHandler messageHandler)
        {
            _messageHandler = messageHandler;
        }

        public string LerNumeroDeSerie()
        {
            byte[] rawPayload = { 0x7D, 0x00, 0x01 };
            try
            {
                var messageParsed = Operations.TryExchangeMessage(_messageHandler, rawPayload, (int)FunctionLength.LerNumeroDeSerieLength);
                if (messageParsed.IsNumeroDeSerie)
                {
                    return messageParsed.NumeroDeSerie;
                }
            }
            catch (ChecksumMismatchException ex)
            {
                Console.WriteLine("ERRO [Ler Número de Série]: " + ex.Message);
            }

            return null;
        }
    }
}