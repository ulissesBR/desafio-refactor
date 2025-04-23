using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedidorTCP.Entities
{
    class TCPHandler : IClientHandler
    {
        private String ip;
        private int port;
        private TcpClient client;
        private NetworkStream stream;

        public TCPHandler(String ip, int port)
        {
            this.ip = ip;
            this.port = port;

            this.client = new TcpClient();
        }

        public void Connect()
        {
            this.client.Connect(this.ip, this.port);

            this.stream = this.client.GetStream();

            Console.WriteLine("Conectado ao servidor!");
        }

        public Stream Stream()
        {
            return this.client.GetStream();
        }

        public void SendMessage(Payload payload)
        {
            this.stream.Write(payload.data, 0, payload.data.Length);
        }

        public Message ReceiveMessage(int length)
        {
            byte[] buffer = new byte[length];

            int bytesRead = this.stream.Read(buffer, 0, buffer.Length);

            return new Message(buffer, bytesRead);
        }

        public void Close()
        {
            this.stream.Close();
            this.client.Close();

            Console.WriteLine("Encerrando conexão...");
        }
    }
}
