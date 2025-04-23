using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using MedidorTCP.Entities.Driver;
using Buffer = MedidorTCP.Entities.Protocol.Buffer;

namespace MedidorTCP.Entities.TCP
{
    class TcpClientHandler
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private string _numeroDeSerie;

        public TcpClientHandler(string ip, int port)
        {
            _client = new TcpClient();
            _client.Connect(ip, port);
            _stream = _client.GetStream();

            Console.WriteLine("Conectado ao servidor!");
        }


        private Buffer ExchangeMessage(byte[] payload, ushort readLength)
        {
            Buffer buffer;

            while (true)
            {
                WriteBuffer(payload);
                buffer = ReadBuffer(readLength);

                if (buffer.IsError())
                {
                    Console.WriteLine("FRAME INCORRETO");
                }
                else
                {
                    return buffer;
                }
            }
        }

        public void LerNumeroDeSerie()
        {
            byte[] payload = { 0x7D, 0x00, 0x01 };
            Buffer buffer = ExchangeMessage(payload, 256);

            byte checksum = buffer.Checksum();
            byte checksumRecebido = buffer.LastByte();

            if (checksum == checksumRecebido)
            {
                if (buffer.IsNumeroDeSerie())
                {
                    _numeroDeSerie = buffer.NumeroDeSerie();
                    Console.WriteLine("Número de série: " + _numeroDeSerie);
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
                byte[] payload = { 0x7D, 0x00, 0x02 };
                Buffer buffer = ExchangeMessage(payload, 8);

                byte checksum = buffer.Checksum();
                byte checksumRecebido = buffer.LastByte();

                if (checksum == checksumRecebido)
                {
                    if (buffer.IsRegistroStatus())
                    {
                        Console.WriteLine("Frame recebido: " + buffer);

                        ushort indiceAntigo = (ushort)((buffer.BufferData[3] << 8) | buffer.BufferData[4]);
                        ushort indiceNovo = (ushort)((buffer.BufferData[5] << 8) | buffer.BufferData[6]);

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
                SalvarCSV(registros);
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
                byte[] payload = { 0x7D, 0x00, 0x04 };
                Buffer buffer = ExchangeMessage(payload, 9);

                byte checksum = buffer.Checksum();
                byte checksumRecebido = buffer.LastByte();

                if (checksum == checksumRecebido)
                {
                    if (buffer.IsDataHora())
                    {
                        return buffer.ExtrairDataHora();
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

        private void WriteBuffer(byte[] buffer)
        {
            byte checksum = CalcularChecksum(buffer);

            byte[] requestWithChecksum = new byte[buffer.Length + 1];
            Array.Copy(buffer, requestWithChecksum, buffer.Length);
            requestWithChecksum[buffer.Length] = checksum;

            _stream.Write(requestWithChecksum, 0, requestWithChecksum.Length);
        }

        private Buffer ReadBuffer(ushort bytesToRead)
        {
            byte[] buffer = new byte[bytesToRead];

            int bytesRead = _stream.Read(buffer, 0, buffer.Length);

            return new Buffer(buffer, bytesRead);
        }

        private string LerValorEnergia()
        {
            int tentativas = 3;
            while (tentativas > 0)
            {
                byte[] payload = { 0x7D, 0x00, 0x05 };
                Buffer buffer = ExchangeMessage(payload, 8);

                byte checksum = buffer.Checksum();
                byte checksumRecebido = buffer.LastByte();

                if (checksum == checksumRecebido)
                {
                    Console.WriteLine("Frame recebido: " + buffer);
                    if (buffer.IsValorEnergia())
                    {
                        byte[] energiaBytes = buffer.Energia();

                        float energia = BitConverter.ToSingle(energiaBytes, 0);
                        energia = (float)Math.Round(energia, 2, MidpointRounding.ToEven);
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

        private void SalvarCSV(List<string> registros)
        {
            string fileName = string.Format("{0}.csv", _numeroDeSerie);
            fileName = string.Concat(fileName.Split(Path.GetInvalidFileNameChars()));

            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine(_numeroDeSerie);
                foreach (var registro in registros)
                {
                    writer.WriteLine(registro);
                }
            }
            Console.WriteLine(string.Format("Dados salvos em {0}", fileName));
        }

        private bool DefinirIndiceRegistro(ushort indice)
        {
            int tentativas = 3;
            while (tentativas > 0)
            {
                byte[] payload = { 0x7D, 0x02, 0x03, (byte)(indice >> 8), (byte)(indice & 0xFF) };
                Buffer buffer = ExchangeMessage(payload, 5);

                byte checksum = buffer.Checksum();
                byte checksumRecebido = buffer.LastByte();

                Console.WriteLine("Definindo índice {0} (checksum {1:X2}): {2}...", indice, checksum, BitConverter.ToString(buffer.BufferData, 0, buffer.BufferData.Length));

                Thread.Sleep(10);

                Console.WriteLine("Frame recebido: " + buffer);

                if (buffer.IsIndiceRegistro())
                {
                    // Ou seja, tá bonito:
                    Console.WriteLine(string.Format("SUCESSO ao configurar registro {0}!", indice));
                    return true;
                }
                else
                {
                    // Não tá bonito:
                    Console.WriteLine(string.Format("ERRO ao configurar registro {0}:\n\tBytes lidos: {1}/5; Resposta: {2}/0x83; Status: {3}/0x00.\n\tTentativas Restantes: {4}", indice, buffer.BytesRead, buffer.Function().ToString("X2"), buffer.BufferData[3].ToString("X2"), tentativas - 1));
                }
                tentativas--;
                Thread.Sleep(50);
            }
            return false;
        }

        private byte CalcularChecksum(byte[] frame)
        {
            byte checksum = 0x00;

            for (int i = 1; i < frame.Length; i++)
            {
                checksum ^= frame[i];
            }

            return checksum;
        }

        public void FecharConexao()
        {
            _stream.Close();
            _client.Close();
            Console.WriteLine("Encerrando conexão...");
        }
    }
}
