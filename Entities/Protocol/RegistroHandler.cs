using MedidorTCP.Entities.Enums;
using MedidorTCP.Entities.Exceptions;
using System.Threading;
using System;
using MedidorTCP.Entities.Driver;

namespace MedidorTCP.Entities.Protocol
{
    public class RegistroHandler
    {
        private readonly IMessageHandler _messageHandler;

        public bool RegistroDefinido { get; private set; }

        public RegistroHandler(IMessageHandler messageHandler)
        {
            _messageHandler = messageHandler;
        }

        public bool DefinirIndiceRegistro(ushort indice)
        {
            byte[] rawPayload = { 0x7D, 0x02, 0x03, (byte)(indice >> 8), (byte)(indice & 0xFF) };
            try
            {
                var messageParsed = Operations.TryExchangeMessage(_messageHandler, rawPayload, (int)FunctionLength.DefinirIndiceRegistroLength);

                Console.WriteLine("Definindo índice {0} (checksum {1:X2}): {2}...", 
                    indice, 
                    messageParsed.Checksum, 
                    BitConverter.ToString(messageParsed.Buffer, 0, messageParsed.Buffer.Length)
                    );

                Thread.Sleep(10);

                Console.WriteLine("Frame recebido: " + messageParsed);

                if (messageParsed.IsIndiceRegistro)
                {
                    Console.WriteLine(string.Format("SUCESSO ao configurar registro {0}!", indice));
                    RegistroDefinido = true;
                    return RegistroDefinido;
                }
                else
                {
                    // Não tá bonito:
                    Console.WriteLine(
                        string.Format(
                            "ERRO ao configurar registro {0}:" +
                        "\n\tBytes lidos: {1}/5; Resposta: {2}/0x83; Status: {3}/0x00." +
                        indice, 
                        messageParsed.BufferLength, 
                        messageParsed.Function.ToString("X2"), 
                        messageParsed.Buffer[3].ToString("X2")
                        )
                        );
                }
            }
            catch (Exception ex) when (ex is ChecksumMismatchException || ex is MessageNotReceivedException)
            {
                Console.WriteLine("ERRO [Definir Indice Registro]: " + ex.Message);
            }
            return false;
        }

    }
}
