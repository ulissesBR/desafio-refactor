using MedidorTCP.Entities.Protocol;

namespace MedidorTCP.Entities.TCP
{
    public interface IClientHandler
    {
        void Connect();
        void Close();

        void SendMessage(Payload payload);
        Message ReceiveMessage(int length);
    }
}
