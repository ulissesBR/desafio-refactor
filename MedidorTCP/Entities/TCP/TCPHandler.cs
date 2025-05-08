using System;
using System.IO;
using System.Net.Sockets;
using MedidorTCP.Entities.Protocol;

namespace MedidorTCP.Entities.TCP
{
    class TCPHandler : IClientHandler
    {
        private String _ip;
        private int _port;
        private TcpClient _client;
        private NetworkStream _stream;

        public TCPHandler(String ip, int port)
        {
            this._ip = ip;
            this._port = port;

            this._client = new TcpClient();
        }

        public void Connect()
        {
            this._client.Connect(this._ip, this._port);

            this._stream = this._client.GetStream();

            Console.WriteLine("Conectado ao servidor!");
        }

        public Stream Stream()
        {
            return this._client.GetStream();
        }

        public void SendMessage(Payload payload)
        {
            this._stream.Write(payload.Data, 0, payload.Data.Length);
        }

        public byte[] ReceiveMessage(int length)
        {
            byte[] buffer = new byte[length];

            int bytesRead = this._stream.Read(buffer, 0, buffer.Length);

            return buffer;
        }

        public void Close()
        {
            this._stream.Close();
            this._client.Close();

            Console.WriteLine("Encerrando conexão...");
        }
    }
}
