using System;
using MedidorTCP.Entities.Driver;
using MedidorTCP.Entities.Enums;
using MedidorTCP.Entities.Exceptions;
using MedidorTCP.Entities.Logging;

namespace MedidorTCP.Entities.Protocol
{
    public class DataHoraHandler
    {
        private readonly IMessageHandler _messageHandler;
        private readonly ILogger _logger;
        private readonly IOperations _operations;
        private Mensagem _mensagemRecebida;

        public string DataHora { get; private set; }

        public DataHoraHandler(IMessageHandler messageHandler, ILogger logger, IOperations operations)
        {
            this._messageHandler = messageHandler;
            this._logger = logger.WithContext(nameof(DataHoraHandler));
            this._operations = operations;
            this.DataHora = LerDataHora();
        }

        public string LerDataHora()
        {
            byte[] rawPayload = { 0x7D, 0x00, 0x04 };
            try
            {
                _mensagemRecebida = _operations.TryExchangeMessage(_messageHandler, rawPayload, (int)FunctionLength.LerDataHoraLength);
                if (_mensagemRecebida.IsDataHora)
                {
                    this.DataHora = GetDataHora();
                    return this.DataHora;
                }
            }
            catch (Exception ex) when (ex is ChecksumMismatchException || ex is MessageNotReceivedException)
            {
                _logger.Error(ex.Message);
            }

            return "Erro na leitura da data/hora.";
        }

        public String GetDataHora()
        {
            int ano = (((_mensagemRecebida.Buffer[3] << 8) | _mensagemRecebida.Buffer[4]) >> 4);
            int mes = _mensagemRecebida.Buffer[4] & 0x0F;
            int dia = _mensagemRecebida.Buffer[5] >> 3;
            int hora = ((_mensagemRecebida.Buffer[5] & 0x07) << 2) | (_mensagemRecebida.Buffer[6] >> 6);
            int minuto = _mensagemRecebida.Buffer[6] & 0x3F;
            int segundo = _mensagemRecebida.Buffer[7] >> 2;

            _logger.Info ($"Data/Hora: {ano:D4}-{mes:D2}-{dia:D2} {hora:D2}:{minuto:D2}:{segundo:D2}");

            return ano.ToString("D4") + "-" +
                mes.ToString("D2") + "-" +
                dia.ToString("D2") + " " +
                hora.ToString("D2") + ":" +
                minuto.ToString("D2") + ":" +
                segundo.ToString("D2");
        }
    }
}