using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MedidorTCP.Entities
{
    class Operations
    {
        private IMessageHandler messageHandler;
        private IOutputHandler outputHandler;

        private String numeroDeSerie;

        public Operations(IMessageHandler messageHandler, IOutputHandler outputHandler){
            this.messageHandler = messageHandler;
            this.outputHandler = outputHandler;
        }

        public void LerNumeroDeSerie()
        {
            byte[] rawPayload = { 0x7D, 0x00, 0x01 };
            Payload payload = new Payload(rawPayload);

            Message message = messageHandler.ExchangeMessage(payload, 256);

            byte checksum = message.Checksum();
            byte checksumRecebido = message.LastByte();

            if (checksum == checksumRecebido)
            {
                if (message.IsNumeroDeSerie())
                {
                    numeroDeSerie = message.NumeroDeSerie();
                    Console.WriteLine("Número de série: " + numeroDeSerie);
                }
            }
            else
            {
                Console.WriteLine("Erro de checksum.");
            }
        }

        public RegistroStatus LerRegistroStatus()
        {
            int tentativas = 3;
            while (tentativas > 0)
            {
                byte[] rawPayload = { 0x7D, 0x00, 0x02 };
                Payload payload = new Payload(rawPayload);

                Message message = messageHandler.ExchangeMessage(payload, 8);

                byte checksum = message.Checksum();
                byte checksumRecebido = message.LastByte();

                if (checksum == checksumRecebido)
                {
                    if (message.IsRegistroStatus())
                    {
                        Console.WriteLine("Frame recebido: " + message);

                        ushort indiceAntigo = (ushort)((message.buffer[3] << 8) | message.buffer[4]);
                        ushort indiceNovo = (ushort)((message.buffer[5] << 8) | message.buffer[6]);

                        Console.WriteLine("Índice mais antigo: {0}", indiceAntigo);
                        Console.WriteLine("Índice mais novo: {0}", indiceNovo);

                        return new RegistroStatus(indiceAntigo, indiceNovo);
                    }
                    else
                    {
                        Console.WriteLine("Frame de resposta inesperado.");
                    }
                }
                else
                {
                    Console.WriteLine("Erro de checksum ao ler status.");
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
                this.outputHandler.save(registros, this.numeroDeSerie);
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
                Payload payload = new Payload(rawPayload);

                Message message = messageHandler.ExchangeMessage(payload, 9);

                byte checksum = message.Checksum();
                byte checksumRecebido = message.LastByte();

                if (checksum == checksumRecebido)
                {
                    if (message.IsDataHora())
                    {
                        return message.ExtrairDataHora();
                    }
                    else
                    {
                        Console.WriteLine("Erro na leitura da data/hora.");
                    }
                }

                tentativas--;
                Thread.Sleep(50);
                Console.WriteLine("Erro de checksum ao ler data e hora");
            }

            return "Falha ao ler a data e hora após múltiplas tentativas";
        }

        private string LerValorEnergia()
        {
            int tentativas = 3;
            while (tentativas > 0)
            {
                byte[] rawPayload = { 0x7D, 0x00, 0x05 };
                Payload payload = new Payload(rawPayload);

                Message message = messageHandler.ExchangeMessage(payload, 8);

                byte checksum = message.Checksum();
                byte checksumRecebido = message.LastByte();

                if (checksum == checksumRecebido)
                {
                    Console.WriteLine("Frame recebido: " + message);
                    if (message.IsValorEnergia())
                    {
                        byte[] energiaBytes = message.Energia();

                        float energia = BitConverter.ToSingle(energiaBytes, 0);
                        energia = (float)Math.Round(energia / 1e3, 2, MidpointRounding.ToEven);
                        return energia.ToString("F2").Replace('.', ',');
                    }
                    else
                    {
                        Console.WriteLine("Erro: Frame inesperado.");
                    }
                }
                else
                {
                    Console.WriteLine("Erro de checksum ao ler energia.");
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
                Payload payload = new Payload(rawPayload);

                Message message = messageHandler.ExchangeMessage(payload, 5);

                byte checksum = message.Checksum();
                byte checksumRecebido = message.LastByte();

                Console.WriteLine("Definindo índice {0} (checksum {1:X2}): {2}...", indice, checksum, BitConverter.ToString(message.buffer, 0, message.buffer.Length));

                Thread.Sleep(10);

                Console.WriteLine("Frame recebido: " + message);

                if (message.IsIndiceRegistro())
                {
                    // Ou seja, tá bonito:
                    Console.WriteLine(string.Format("SUCESSO ao configurar registro {0}!", indice));
                    return true;
                }
                else
                {
                    // Não tá bonito:
                    Console.WriteLine(string.Format("ERRO ao configurar registro {0}:\n\tBytes lidos: {1}/5; Resposta: {2}/0x83; Status: {3}/0x00.\n\tTentativas Restantes: {4}", indice, message.length, message.Function().ToString("X2"), message.buffer[3].ToString("X2"), tentativas - 1));
                }
                tentativas--;
                Thread.Sleep(50);
            }
            return false;
        }
    }
}
