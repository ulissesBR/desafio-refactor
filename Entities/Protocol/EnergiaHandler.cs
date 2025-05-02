using System;
using MedidorTCP.Entities.Driver;
using MedidorTCP.Entities.Enums;
using MedidorTCP.Entities.Exceptions;

namespace MedidorTCP.Entities.Protocol
{
    public class EnergiaHandler
    {
        private readonly IMessageHandler _messageHandler;
        public byte[] Energia { get; private set; }     // Valor da energia lida


        public string ValorEnergia { get; private set; }

        public EnergiaHandler(IMessageHandler messageHandler)
        {
            _messageHandler = messageHandler;
            ValorEnergia = LerValorEnergia();
        }

        private string LerValorEnergia()
        {
            byte[] rawPayload = { 0x7D, 0x00, 0x05 };
            try
            {
                var messageParsed = Operations.TryExchangeMessage(_messageHandler, rawPayload, (int)FunctionLength.LerValorEnergiaLength);

                Console.WriteLine("Frame recebido: " + messageParsed);

                if (messageParsed.IsValorEnergia)
                {
                    var energiaBytes = GetEnergia();
                    var energia = BitConverter.ToSingle(energiaBytes, 0);
                    energia = (float)Math.Round(energia / 1e3, 2, MidpointRounding.ToEven);
                    return energia.ToString("F2").Replace('.', ',');
                }
                else
                {
                    Console.WriteLine("Erro: Frame inesperado.");
                }
            }
            catch (Exception ex) when (ex is ChecksumMismatchException || ex is MessageNotReceivedException)
            {
                Console.WriteLine("ERRO [Ler Valor Energia]: " + ex.Message);
            }
            return "Falha ao ler o valor de energia após múltiplas tentativas";
        }

        public byte[] GetEnergia()
        {
            byte[] energiaBytes = new byte[4];
            Array.Copy(this.Buffer, 3, energiaBytes, 0, 4);

            // Se o sistema é little-endian, inverte a ordem dos bytes
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(energiaBytes);
            }

            return energiaBytes;
        }

    }
}
