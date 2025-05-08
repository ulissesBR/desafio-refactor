using MedidorTCP.Entities.Protocol;

namespace MedidorTCP.Entities.TCP
{
    public interface IClientHandler
    {
        void Connect();
        void Close();

        void SendMessage(Payload payload);
        byte[] ReceiveMessage(int length);
    }
}
