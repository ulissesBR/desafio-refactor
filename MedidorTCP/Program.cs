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
            //ArgumentData argumentData = new ArgumentData("20.232.142.24", "12167", "46036", "46244");
            //Run(argumentData);

            ArrayList arguments = handleArguments(args);

            foreach (ArgumentData argument in arguments)
            {
                Run(argument);
            }
        }

        private static void Run(ArgumentData arguments)
        {
            IClientHandler clientHandler;
            IOutputHandler outputHandler;
            IMessageHandler messageHandler;
            IOperations operations;
            ILogger logger = new ConsoleLogger().WithContext(nameof(Program));

            LeituraDeMemoriaHandler leituraDeMemoriaHandler;
            Console.WriteLine("Conectando-se ao servidor ({0} - porta {1})... ", arguments.Ip, arguments.Port);

            try
            {
                //int FirstIndex = 0, LastIndex = 0;

                clientHandler = new TCPHandler(arguments.Ip, arguments.Port);

                outputHandler = new CSVHandler("OutputEnergia");
                messageHandler = new MessageHandler(clientHandler);
                operations = new Operations(messageHandler, outputHandler);
                leituraDeMemoriaHandler = new LeituraDeMemoriaHandler(messageHandler, logger, operations);

                SerieHandler serieHandler = new SerieHandler(messageHandler, operations);

                clientHandler.Connect();
                //var registroStatusHandler = RegistroStatusHandler.LerRegistroStatus(messageHandler, operations);
                var numeroDeSerie = serieHandler.LerNumeroDeSerie(); // Lemos o número de série primeiro
                Console.WriteLine("Número de série: " + numeroDeSerie);

                //if (registroStatusHandler.IndiceAntigo > 0 && registroStatusHandler.IndiceNovo > 0)
                if (arguments.FirstIndex > 0 && arguments.LastIndex > 0)
                {
                    //leituraDeMemoriaHandler.LerMemoriaDeMassa(registroStatusHandler.IndiceAntigo, registroStatusHandler.IndiceNovo);
                    leituraDeMemoriaHandler.LerMemoriaDeMassa(arguments.FirstIndex, arguments.LastIndex);

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
            ArrayList arguments = new ArrayList();

            if (args.Length == 1)
            {
                return readFile(args[0]);
            }
            else if (args.Length == 4)
            {
                ArgumentData argument = new ArgumentData(args[0], args[1], args[2], args[3]);
                arguments.Add(argument);
                return arguments;
            }
            else
            {
                Console.WriteLine("Usage: ");
                Console.WriteLine(" MedidorTCP.exe nome_do_arquivo.txt");
                Console.WriteLine(" MedidorTCP.exe ip porta indice-inicial indice-final (separados por espaço).");
                Environment.Exit(1);
                return null;
            }
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
