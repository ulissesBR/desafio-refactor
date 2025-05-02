using System;
using MedidorTCP.Entities.Driver;
using MedidorTCP.Entities.Enums;
using MedidorTCP.Entities.Exceptions;
using MedidorTCP.Entities.Logging;

namespace MedidorTCP.Entities.Protocol
{
    public class SerieHandler
    {
        private readonly IMessageHandler _messageHandler;
        private readonly ILogger _logger;

        public string NumeroDeSerie { get; private set; }
        public Mensagem MensagemRecebida { get; private set; } // Mensagem recebida do medidor

        public SerieHandler(IMessageHandler messageHandler)
        {
            _messageHandler = messageHandler;
            _logger = new ConsoleLogger(nameof(SerieHandler));
        }

        public string LerNumeroDeSerie()
        {
            byte[] rawPayload = { 0x7D, 0x00, 0x01 };
            try
            {
                MensagemRecebida = Operations.TryExchangeMessage(_messageHandler, rawPayload, (int)FunctionLength.LerNumeroDeSerieLength);
                if (MensagemRecebida.IsNumeroDeSerie)
                {
                    NumeroDeSerie = GetNumeroDeSerie();
                    return NumeroDeSerie;
                }
            }
            catch (Exception ex) when (ex is ChecksumMismatchException || ex is MessageNotReceivedException)
            {
                _logger.Error(ex.Message);
            }
            return "Erro na leitura do número de série";
        }

        public string GetNumeroDeSerie()
        {
            return System.Text.Encoding.ASCII.GetString(MensagemRecebida.Buffer, 3, MensagemRecebida.BufferLength - 4);
        }
    }
}