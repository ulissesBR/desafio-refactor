using System;
using MedidorTCP.Entities.Protocol;

namespace MedidorTCP.Entities.Logging
{
    public class ConsoleLogger : ILogger
    {
        private string _contexto;

        public ConsoleLogger(string contexto = "")
        {
            _contexto = contexto;
        }
        public ILogger WithContext(string novoContexto)
        {
            return new ConsoleLogger(novoContexto);
        }

        public void Info(string mensagem)
        {
            Console.WriteLine($"[INFO]  [{_contexto}] {mensagem}");
        }

        public void Warning(string mensagem)
        {
            Console.WriteLine($"[WARNING]  [{_contexto}] {mensagem}");
        }

        public void Error(string mensagem)
        {
            Console.WriteLine($"[ERROR]:  [{_contexto}] {mensagem}");
        }


    }
}
