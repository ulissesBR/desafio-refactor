using MedidorTCP.Entities.Enums;
using MedidorTCP.Entities.Exceptions;
using System.Threading;
using System;
using MedidorTCP.Entities.Driver;
using MedidorTCP.Entities.Logging;

namespace MedidorTCP.Entities.Protocol
{
    public class RegistroHandler
    {
        private readonly IMessageHandler _messageHandler;
        private readonly ILogger _logger;

        public bool IsRegistroDefinido { get; private set; }

        public RegistroHandler(IMessageHandler messageHandler, ILogger logger)
        {
            _messageHandler = messageHandler;
            _logger = logger.WithContext(nameof(RegistroHandler));
        }

        public bool DefinirIndiceRegistro(ushort indice)
        {
            byte[] rawPayload = { 0x7D, 0x02, 0x03, (byte)(indice >> 8), (byte)(indice & 0xFF) };
            try
            {
                var MensagemRecebida = Operations.TryExchangeMessage(_messageHandler, rawPayload, (int)FunctionLength.DefinirIndiceRegistroLength);

                _logger.Info($"Definindo índice {indice} " +
                    $"(checksum {MensagemRecebida.Checksum:X2}): {BitConverter.ToString(MensagemRecebida.Buffer, 0, MensagemRecebida.Buffer.Length)}...");

                Thread.Sleep(10);

                _logger.Info("Frame recebido: " + MensagemRecebida);

                if (MensagemRecebida.IsIndiceRegistro)
                {
                    _logger.Info($"SUCESSO ao configurar registro {indice}!");
                    IsRegistroDefinido = true;
                    return IsRegistroDefinido;
                }
                else
                {
                    _logger.Error($"ERRO ao configurar registro {indice}:" +
                        $"\n\tBytes lidos: {MensagemRecebida.BufferLength}/5; " +
                        $"\n\tResposta: {MensagemRecebida.Function:X2}/0x83; " +
                        $"\n\tStatus: {MensagemRecebida.Buffer[3]:X2}/0x00.");
                }
            }
            catch (Exception ex) when (ex is ChecksumMismatchException || ex is MessageNotReceivedException)
            {
                _logger.Error("ERRO [Definir Indice Registro]: " + ex.Message);
            }
            return false;
        }

    }
}
