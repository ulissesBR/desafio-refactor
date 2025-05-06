using MedidorTCP.Entities.Enums;
using MedidorTCP.Entities.Exceptions;
using MedidorTCP.Entities.Protocol;
using static MedidorTCP.Entities.Enums.TipoMensagem;
using System.Threading;
using System;

namespace MedidorTCP.Entities.Driver
{
    public class RegistroStatusHandler
    {
        public static IMessageHandler _messageHandler;
        public static IOperations _operations;

        public int IndiceAntigo { get; private set; }
        public int IndiceNovo { get; private set; }

        public RegistroStatusHandler(int indiceAntigo, int indiceNovo, IOperations operations)
        {
            IndiceAntigo = indiceAntigo;
            IndiceNovo = indiceNovo;
            _operations = operations;
        }

        public static RegistroStatusHandler LerRegistroStatus(IMessageHandler messageHandler, IOperations operations)
        {
            _operations = operations;
            _messageHandler = messageHandler;
            byte[] rawPayload = { 0x7D, 0x00, 0x02 };
            try
            {

                Mensagem mensagemRecebida = _operations.TryExchangeMessage(_messageHandler, rawPayload, (int)FunctionLength.LerRegistroStatusLength);

                if (mensagemRecebida.Tipo == RegistroStatus)
                {
                    Console.WriteLine("Frame recebido: " + mensagemRecebida);

                    var indiceAntigo = (ushort)((mensagemRecebida.Buffer[3] << 8) | mensagemRecebida.Buffer[4]);
                    var indiceNovo = (ushort)((mensagemRecebida.Buffer[5] << 8) | mensagemRecebida.Buffer[6]);

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
