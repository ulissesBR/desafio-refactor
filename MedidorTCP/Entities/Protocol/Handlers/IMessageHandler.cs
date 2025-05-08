namespace MedidorTCP.Entities.Protocol
{
    public interface IMessageHandler
    {
        byte[] ExchangeMessage(Payload payload, int readLength);
    }
}
