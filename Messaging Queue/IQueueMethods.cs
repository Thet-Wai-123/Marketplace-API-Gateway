namespace MarketPlace_API_Gateway.Messaging_Queue
{
    public interface IQueueMethods
    {
        void SendTask(string task, string toService);
    }
}
