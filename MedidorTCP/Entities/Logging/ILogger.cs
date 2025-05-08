namespace MedidorTCP.Entities.Logging
{
    public interface ILogger
    {
        void Info(string message);
        void Warning(string message);
        void Error(string message);
        ILogger WithContext(string newContext);
    }
}
