using System;
using MedidorTCP.Entities.Driver;
using MedidorTCP.Entities.Enums;
using MedidorTCP.Entities.Exceptions;

namespace MedidorTCP.Entities.Protocol
{
    public class SerieHandler
    {
        private readonly IMessageHandler _messageHandler;
        public string NumeroDeSerie { get; private set; }

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
                    NumeroDeSerie = messageParsed.NumeroDeSerie;
                    Console.WriteLine("Número de série: " + NumeroDeSerie);
                    return NumeroDeSerie;
                }
            }
            catch (Exception ex) when (ex is ChecksumMismatchException || ex is MessageNotReceivedException)
            {
                Console.WriteLine("ERRO [Ler Número de Série]: " + ex.Message);
            }
            return "Erro na leitura do número de série";
        }
    }
}