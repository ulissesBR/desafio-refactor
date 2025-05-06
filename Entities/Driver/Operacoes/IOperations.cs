namespace MedidorTCP.Entities.Protocol
{
    public interface IOperations
    {
        Mensagem TryExchangeMessage(IMessageHandler handler, byte[] rawPayload, int payloadSize);
    }
}
