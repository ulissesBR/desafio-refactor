namespace MedidorTCP.Entities.Protocol
{
    interface IMessageHandler
    {
        Message ExchangeMessage(Payload payload, int readLength);
    }
}
