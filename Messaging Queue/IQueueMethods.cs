namespace MarketPlace_API_Gateway.Messaging_Queue
{
    public interface IQueueMethods
    {
        void SendTask(string action, string requiredInfo, string toService);
    }
}
