using System;
using System.Threading;
using MedidorTCP.Entities.Logging;
using MedidorTCP.Entities.Exceptions;
using static MedidorTCP.Entities.Enums.TipoMensagem;
using MedidorTCP.Entities.Driver.TamanhosDasMensagens;


namespace MedidorTCP.Entities.Protocol
{
    public class RegistroHandler
    {
        private readonly IMessageHandler _messageHandler;
        private readonly ILogger _logger;
        private readonly IOperations _operations;

        public bool IsRegistroDefinido { get; private set; }

        public RegistroHandler(IMessageHandler messageHandler, ILogger logger, IOperations operations)
        {
            _messageHandler = messageHandler;
            _logger = logger.WithContext(nameof(RegistroHandler));
            _operations = operations;
        }

        public bool DefinirIndiceRegistro(int indice)
        {
            byte[] rawPayload = { 0x7D, 0x02, 0x03, (byte)(indice >> 8), (byte)(indice & 0xFF) };
            try
            {
                var mensagemRecebida = _operations.TryExchangeMessage(_messageHandler, rawPayload, TamanhoMensagem.DefinirIndiceRegistro);

                _logger.Info($"Definindo índice {indice} " +
                    $"(checksum {mensagemRecebida.Frame.ChecksumCalculado:X2}): {BitConverter.ToString(mensagemRecebida.Buffer, 0, mensagemRecebida.Buffer.Length)}...");

                Thread.Sleep(10);

                _logger.Info("Frame recebido: " + mensagemRecebida);

                if (mensagemRecebida.Tipo == IndiceRegistro)
                {
                    _logger.Info($"SUCESSO ao configurar registro {indice}!");
                    IsRegistroDefinido = true;
                    return IsRegistroDefinido;
                }
                else
                {
                    _logger.Error($"ERRO ao configurar registro {indice}:" +
                        $"\n\tBytes lidos: {mensagemRecebida.BufferLength}/5; " +
                        $"\n\tResposta: {mensagemRecebida.Frame.Function:X2}/0x83; " +
                        $"\n\tStatus: {mensagemRecebida.Buffer[3]:X2}/0x00.");
                }
            }
            catch (Exception ex) when (ex is ChecksumMismatchException || ex is MessageNotReceivedException)
            {
                _logger.Error(ex.Message);
            }
            return false;
        }

    }
}
