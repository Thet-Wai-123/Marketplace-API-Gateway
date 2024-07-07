using RabbitMQ.Client;
using System.Text;
using System.Threading.Channels;

namespace MarketPlace_API_Gateway.Messaging_Queue
{
    public class QueueMethods :IQueueMethods
    {
        private IModel _channel;

        public QueueMethods()
        {
            var factory = new ConnectionFactory
            {
                HostName = Environment.GetEnvironmentVariable("MessagingQueue")
            };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();

            _channel.QueueDeclare(
                queue: "Inventory_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
            _channel.QueueDeclare(
                queue: "ShoppingCart_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        }

        public void SendTask(string task, string toService)
        {
            var body = Encoding.UTF8.GetBytes(task);

            if (_channel == null)
            {
                throw new ArgumentNullException(
                    nameof(_channel),
                    "Channel service is not initialized."
                );
            }
            _channel.BasicPublish(
                exchange: string.Empty,
                routingKey: toService + "_queue",
                basicProperties: null,
                body: body
            );
        }
    }
}
