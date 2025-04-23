using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Collections;
using MedidorTCP.Entities;

namespace MedidorTCP
{
    class Program
    {
        static void Main(string[] args)
        {
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

            Operations operations;

            Console.WriteLine("Conectando-se ao servidor ({0} - porta {1})... ", arguments.ip, arguments.port);

            try
            {
                clientHandler = new TCPHandler(arguments.ip, arguments.port);

                outputHandler = new CSVHandler("OutputEnergia");
                messageHandler = new MessageHandler(clientHandler);
                operations = new Operations(messageHandler, outputHandler);

                clientHandler.Connect();

                operations.LerNumeroDeSerie(); // Lemos o número de série primeiro

                // TODO: CONFERIR ISSO! E se os indices forem, efetivamente, 0?
                if (arguments.firstIndex > 0 && arguments.lastIndex > 0)
                {
                    operations.LerRegistros((ushort)arguments.firstIndex, (ushort)arguments.lastIndex);
                }

                clientHandler.Close();     // Lidos os dados, fechamos a conexão.
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro: {0}", ex.Message);
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
