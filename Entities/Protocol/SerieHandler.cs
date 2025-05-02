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
        public Mensagem MensagemRecebida { get; private set; } // Mensagem recebida do medidor

        public SerieHandler(IMessageHandler messageHandler)
        {
            _messageHandler = messageHandler;
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
                Console.WriteLine("ERRO [Ler Número de Série]: " + ex.Message);
            }
            return "Erro na leitura do número de série";
        }

        public string GetNumeroDeSerie()
        {
            return System.Text.Encoding.ASCII.GetString(MensagemRecebida.Buffer, 3, MensagemRecebida.BufferLength - 4);
        }
    }
}