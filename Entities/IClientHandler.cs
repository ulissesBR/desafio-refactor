using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedidorTCP.Entities
{
    public interface IClientHandler
    {
        void Connect();
        void Close();

        void SendMessage(Payload payload);
        Message ReceiveMessage(int length);
    }
}
