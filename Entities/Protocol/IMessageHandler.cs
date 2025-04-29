namespace MedidorTCP.Entities.Protocol
{
    interface IMessageHandler
    {
        byte[] ExchangeMessage(Payload payload, int readLength);
    }
}
