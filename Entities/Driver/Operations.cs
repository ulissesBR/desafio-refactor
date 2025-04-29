using System;
using System.Collections.Generic;
using System.Threading;
using MedidorTCP.Entities.Exceptions;
using MedidorTCP.Entities.Protocol;
using MedidorTCP.Entities.Enums;

namespace MedidorTCP.Entities.Driver
{
    class Operations
    {
        private IMessageHandler _messageHandler;
        private IOutputHandler _outputHandler;

        private String numeroDeSerie;

        public Operations(IMessageHandler messageHandler, IOutputHandler outputHandler)
        {
            this._messageHandler = messageHandler;
            this._outputHandler = outputHandler;
        }

        public void LerNumeroDeSerie()
        {
            byte[] rawPayload = { 0x7D, 0x00, 0x01 };

            try
            { 
                MessageParsed messageParsed = TryExchangeMessage(rawPayload, (int)FunctionLength.LerNumeroDeSerieLength);

                if (messageParsed.IsNumeroDeSerie)
                {
                    numeroDeSerie = messageParsed.NumeroDeSerie;
                    Console.WriteLine("Número de série: " + numeroDeSerie);
                }
            }
            catch (ChecksumMismatchException ex)
            {
                Console.WriteLine("ERRO [Ler Número de Série]: " + ex.Message);
            }
        }

        public RegistroStatus LerRegistroStatus()
        {
            int tentativas = 3;
            while (tentativas > 0)
            {
                byte[] rawPayload = { 0x7D, 0x00, 0x02 };
                try
                {

                    MessageParsed messageParsed = TryExchangeMessage(rawPayload, (int)FunctionLength.LerRegistroStatusLength);

                    if (messageParsed.IsRegistroStatus)
                    {
                        Console.WriteLine("Frame recebido: " + messageParsed);

                        ushort indiceAntigo = (ushort)((messageParsed.Buffer[3] << 8) | messageParsed.Buffer[4]);
                        ushort indiceNovo = (ushort)((messageParsed.Buffer[5] << 8) | messageParsed.Buffer[6]);

                        Console.WriteLine("Índice mais antigo: {0}", indiceAntigo);
                        Console.WriteLine("Índice mais novo: {0}", indiceNovo);

                        return new RegistroStatus(indiceAntigo, indiceNovo);
                    }
                    else
                    {
                        Console.WriteLine("Frame de resposta inesperado.");
                    }
                }
                catch (ChecksumMismatchException ex)
                {
                    Console.WriteLine("ERRO [Ler Registro Status]: " + ex.Message);
                }

                tentativas--;
                Thread.Sleep(50);
            }

            Console.WriteLine("Falha ao ler os índices do registros após múltiplas tentativas");
            return new RegistroStatus(0, 0);
        }

        public void LerRegistros(ushort indiceInicial, ushort indiceFinal)
        {
            Console.WriteLine("Iniciando leitura dos registros...");
            List<string> registros = new List<string>();

            for (ushort indice = indiceInicial; indice <= indiceFinal; indice++)
            {
                if (DefinirIndiceRegistro(indice))
                {
                    string dataHora = LerDataHora();
                    string valorEnergia = LerValorEnergia();
                    Console.WriteLine("Indice: {0}; Data: {1}; Energia: {2}", indice, dataHora, valorEnergia);
                    registros.Add(string.Format("{0};{1};{2}", indice, dataHora, valorEnergia));
                }
            }


            if (registros.Count > 0)
            {
                this._outputHandler.Save(registros, this.numeroDeSerie);
            }
            else
            {
                Console.WriteLine("Nenhum registro válido encontrado.");
            }

        }

        private string LerDataHora()
        {
            int tentativas = 3;
            while (tentativas > 0)
            {
                byte[] rawPayload = { 0x7D, 0x00, 0x04 };
                try
                {
                    MessageParsed messageParsed = TryExchangeMessage(rawPayload, (int)FunctionLength.LerDataHoraLength);
                    
                    if (messageParsed.IsDataHora)
                    {
                        return messageParsed.DataHora;
                    }
                    else
                    {
                        Console.WriteLine("Erro na leitura da data/hora.");
                    }
                }
                catch (ChecksumMismatchException ex)
                {
                    Console.WriteLine("ERRO [Data e Hora]: " + ex.Message);
                }

                tentativas--;
                Thread.Sleep(50);
            }

            return "Falha ao ler a data e hora após múltiplas tentativas";
        }

        private string LerValorEnergia()
        {
            int tentativas = 3;
            while (tentativas > 0)
            {
                byte[] rawPayload = { 0x7D, 0x00, 0x05 };
                try
                {
                    MessageParsed messageParsed = TryExchangeMessage(rawPayload, (int)FunctionLength.LerValorEnergiaLength);

                    Console.WriteLine("Frame recebido: " + messageParsed);

                    if (messageParsed.IsValorEnergia)
                    {
                        byte[] energiaBytes = messageParsed.Energia;

                        float energia = BitConverter.ToSingle(energiaBytes, 0);
                        energia = (float)Math.Round(energia / 1e3, 2, MidpointRounding.ToEven);
                        return energia.ToString("F2").Replace('.', ',');
                    }
                    else
                    {
                        Console.WriteLine("Erro: Frame inesperado.");
                    }
                }
                catch (ChecksumMismatchException ex)
                {
                    Console.WriteLine("ERRO [Ler Valor Energia]: " + ex.Message);
                }

                tentativas--;
                Thread.Sleep(50);

            }
            return "Falha ao ler o valor de energia após múltiplas tentativas";
        }

        private bool DefinirIndiceRegistro(ushort indice)
        {
            int tentativas = 3;
            while (tentativas > 0)
            {
                byte[] rawPayload = { 0x7D, 0x02, 0x03, (byte)(indice >> 8), (byte)(indice & 0xFF) };
                try
                {
                    MessageParsed messageParsed = TryExchangeMessage(rawPayload, (int)FunctionLength.DefinirIndiceRegistroLength);

                    Console.WriteLine("Definindo índice {0} (checksum {1:X2}): {2}...", indice, messageParsed.Checksum, BitConverter.ToString(messageParsed.Buffer, 0, messageParsed.Buffer.Length));

                    Thread.Sleep(10);

                    Console.WriteLine("Frame recebido: " + messageParsed);

                    if (messageParsed.IsIndiceRegistro)
                    {
                        Console.WriteLine(string.Format("SUCESSO ao configurar registro {0}!", indice));
                        return true;
                    }
                    else
                    {
                        // Não tá bonito:
                        Console.WriteLine(string.Format("ERRO ao configurar registro {0}:\n\tBytes lidos: {1}/5; Resposta: {2}/0x83; Status: {3}/0x00.\n\tTentativas Restantes: {4}", indice, messageParsed.BufferLength, messageParsed.Function.ToString("X2"), messageParsed.Buffer[3].ToString("X2"), tentativas - 1));
                    }
                    tentativas--;
                    Thread.Sleep(50);
                }
                catch (ChecksumMismatchException ex)
                {
                    Console.WriteLine("ERRO [Definir Indice Registro]: " + ex.Message);
                }
            }
            return false;
        }

        public MessageParsed TryExchangeMessage(byte[] rawPayload, int payloadSize)
        {
            // Lógica externalizada para esse novo método "TryExchangeMessage"
            // O método envia e recebe a mensagem, calcula o checksum e o compara com o recebido. Retorna um MessageParsed, ou uma falha, lançando exepction dentro do método.
            Payload payload = new Payload(rawPayload);

            var frame = _messageHandler.ExchangeMessage(payload, payloadSize);
            MessageParsed messageParsed = MessageParsed.Parse(frame);

            byte checksum = messageParsed.Checksum;
            byte checksumRecebido = messageParsed.LastByte;

            if (checksum == checksumRecebido)
            {
                return messageParsed;
            }
            else
            {
                throw new ChecksumMismatchException("Checksum recebido é diferente do checksum calculado.");
            }
        }
    }
}
