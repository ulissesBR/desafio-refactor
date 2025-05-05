using MedidorTCP.Entities.Enums;
using MedidorTCP.Entities.Exceptions;
using MedidorTCP.Entities.Protocol;
using System.Threading;
using System;

namespace MedidorTCP.Entities.Driver
{
    public class RegistroStatusHandler
    {
        public static readonly IMessageHandler _messageHandler;
        public static IOperations _operations;

        public ushort IndiceAntigo { get; private set; }
        public ushort IndiceNovo { get; private set; }

        public RegistroStatusHandler(ushort indiceAntigo, ushort indiceNovo, IOperations operations)
        {
            IndiceAntigo = indiceAntigo;
            IndiceNovo = indiceNovo;
            _operations = operations;
        }

        public static RegistroStatusHandler LerRegistroStatus()
        {
            byte[] rawPayload = { 0x7D, 0x00, 0x02 };
            try
            {

                Mensagem messageParsed = _operations.TryExchangeMessage(_messageHandler, rawPayload, (int)FunctionLength.LerRegistroStatusLength);

                if (messageParsed.IsRegistroStatus)
                {
                    Console.WriteLine("Frame recebido: " + messageParsed);

                    var indiceAntigo = (ushort)((messageParsed.Buffer[3] << 8) | messageParsed.Buffer[4]);
                    var indiceNovo = (ushort)((messageParsed.Buffer[5] << 8) | messageParsed.Buffer[6]);

                    Console.WriteLine("Índice mais antigo: {0}", indiceAntigo);
                    Console.WriteLine("Índice mais novo: {0}", indiceNovo);

                    return new RegistroStatusHandler(indiceAntigo, indiceNovo, _operations);
                }
                else
                {
                    Console.WriteLine("Frame de resposta inesperado.");
                }
            }
            catch (Exception ex) when (ex is ChecksumMismatchException || ex is MessageNotReceivedException)
            {
                Console.WriteLine("ERRO [Ler Registro Status]: " + ex.Message);
            }

            Console.WriteLine("Falha ao ler os índices do registros após múltiplas tentativas");
            return new RegistroStatusHandler(0, 0, null);
        }
    }
}
