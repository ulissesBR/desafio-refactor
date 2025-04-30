using System;
using MedidorTCP.Entities.Driver;
using MedidorTCP.Entities.Enums;
using MedidorTCP.Entities.Exceptions;

namespace MedidorTCP.Entities.Protocol
{
    public class DataHoraHandler
    {
        private readonly IMessageHandler _messageHandler;
        public string DataHora { get; private set; }

        public DataHoraHandler(IMessageHandler messageHandler)
        {
            this._messageHandler = messageHandler;
            this.DataHora = LerDataHora();
        }

        public string LerDataHora()
        {
            byte[] rawPayload = { 0x7D, 0x00, 0x04 };
            try
            {
                var messageParsed = Operations.TryExchangeMessage(_messageHandler, rawPayload, (int)FunctionLength.LerDataHoraLength);
                if (messageParsed.IsDataHora)
                {
                    this.DataHora = messageParsed.DataHora;
                    return this.DataHora;
                }
            }
            catch (Exception ex) when (ex is ChecksumMismatchException || ex is MessageNotReceivedException)
            {
                Console.WriteLine("ERRO [Data e Hora]: " + ex.Message);
            }
            return "Erro na leitura da data/hora.";
        }
    }
}