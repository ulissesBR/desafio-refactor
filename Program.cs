using System;
using System.Collections;
using System.IO;
using MedidorTCP.Entities.Driver;
using MedidorTCP.Entities.FileUtilities;
using MedidorTCP.Entities.Logging;
using MedidorTCP.Entities.Protocol;
using MedidorTCP.Entities.TCP;
using MedidorTCP.Entities.UserInputHandle;
using Microsoft.Win32;

namespace MedidorTCP
{
    class Program
    {
        static void Main(string[] args)
        {
            /* Teste */
            ArgumentData argumentData = new ArgumentData("20.232.142.24", "12167", "46036", "46244");
            Run(argumentData);

            //ArrayList arguments = handleArguments(args);

            //foreach (ArgumentData argument in arguments)
            //{
                //Run(argument);
            //}
        }

        private static void Run(ArgumentData arguments)
        {
            IClientHandler clientHandler;
            IOutputHandler outputHandler;
            IMessageHandler messageHandler;
            ILogger logger = new ConsoleLogger();

            Operations operations;
            LeituraDeMemoriaHandler leituraDeMemoriaHandler;
            Console.WriteLine("Conectando-se ao servidor ({0} - porta {1})... ", arguments.Ip, arguments.Port);

            try
            {
                clientHandler = new TCPHandler(arguments.Ip, arguments.Port);

                outputHandler = new CSVHandler("OutputEnergia");
                messageHandler = new MessageHandler(clientHandler);
                operations = new Operations(messageHandler, outputHandler);
                leituraDeMemoriaHandler = new LeituraDeMemoriaHandler(messageHandler, logger);

                SerieHandler serieHandler = new SerieHandler(messageHandler);

                clientHandler.Connect();

                var numeroDeSerie = serieHandler.LerNumeroDeSerie(); // Lemos o número de série primeiro
                //operations.LerNumeroDeSerie(); 
                Console.WriteLine("Número de série: " + numeroDeSerie);

                if (arguments.FirstIndex > 0 && arguments.LastIndex > 0)
                {
                    leituraDeMemoriaHandler.LerMemoriaDeMassa((ushort)arguments.FirstIndex, (ushort)arguments.LastIndex);
                    
                    if (leituraDeMemoriaHandler.Registros.Count > 0)
                    {
                        outputHandler.Save(leituraDeMemoriaHandler.Registros, numeroDeSerie);
                    }
                    else
                    {
                        Console.WriteLine("Nenhum registro válido encontrado.");
                    }
                }

                clientHandler.Close();     // Lidos os dados, fechamos a conexão.
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro [Programa principal]: {0}", ex.Message);
            }

            Console.WriteLine("Conexão encerrada.");
            //Console.ReadKey(); // Para o cli não fechar ao finalizar o programa.
        }

        private static ArrayList handleArguments(String[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: MedidorTCP.exe nomeDoArquivo");
                Environment.Exit(1);
            }

            return readFile(args[0]);

        }

        private static ArrayList readFile(String fileName)
        {
            ArrayList lines = new ArrayList();
            using (StreamReader reader = new StreamReader(fileName))
            {
                String line;
                while (true)
                {
                    line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }

                    string[] fields = line.Split(' ');

                    ArgumentData argument = new ArgumentData(fields[0], fields[1], fields[2], fields[3]);
                    lines.Add(argument);
                }
            }

            return lines;
        }
    }
}
