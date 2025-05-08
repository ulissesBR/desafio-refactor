using System;
using MedidorTCP.Entities.Driver;
using MedidorTCP.Entities.Enums;
using MedidorTCP.Entities.Exceptions;
using MedidorTCP.Entities.Logging;
using static MedidorTCP.Entities.Enums.TipoMensagem;

namespace MedidorTCP.Entities.Protocol
{
    public class EnergiaHandler
    {
        private readonly IMessageHandler _messageHandler;
        private readonly ILogger _logger;
        private readonly IOperations _operations;

        private Mensagem mensagemRecebida; // Mensagem recebida do medidor
        public byte[] Energia { get; private set; }     // Valor da energia lida


        public string ValorEnergia { get; private set; }

        public EnergiaHandler(IMessageHandler messageHandler, ILogger logger, IOperations operations)
        {
            _messageHandler = messageHandler;
            _logger = logger.WithContext(nameof(EnergiaHandler));
            _operations = operations;
            ValorEnergia = LerValorEnergia();
        }

        public string LerValorEnergia()
        {
            byte[] rawPayload = { 0x7D, 0x00, 0x05 };
            try
            {
                mensagemRecebida = _operations.TryExchangeMessage(_messageHandler, rawPayload, (int)FunctionLength.LerValorEnergiaLength);

                _logger.Info("Frame recebido: " + mensagemRecebida);

                if (mensagemRecebida.Tipo == ValorEnergiaTipo)
                {
                    var energiaBytes = GetEnergia();
                    var energia = BitConverter.ToSingle(energiaBytes, 0);
                    energia = (float)Math.Round(energia / 1e3, 2, MidpointRounding.ToEven);
                    return energia.ToString("F2").Replace('.', ',');
                }
                else
                {
                    _logger.Error("Frame inesperado.");
                }
            }
            catch (Exception ex) when (ex is ChecksumMismatchException || ex is MessageNotReceivedException)
            {
                _logger.Error(ex.Message);
            }
            return "Falha ao ler o valor de energia após múltiplas tentativas";
        }

        public byte[] GetEnergia()
        {
            byte[] energiaBytes = new byte[4];
            Array.Copy(mensagemRecebida.Buffer, 3, energiaBytes, 0, 4);

            // Se o sistema é little-endian, inverte a ordem dos bytes
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(energiaBytes);
            }

            return energiaBytes;
        }

    }
}
