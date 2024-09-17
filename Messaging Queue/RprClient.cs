using System.Text;
using System.Threading.Channels;
using MarketPlace_API_Gateway.DTOs.Inventory_Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MarketPlace_API_Gateway.Messaging_Queue
{
    public class RprClient : IRpcClient
    {
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly string replyQueueName;

        public RprClient()
        {
            var factory = new ConnectionFactory
            {
                HostName = Environment.GetEnvironmentVariable("MessagingQueueHostName"),
                UserName = Environment.GetEnvironmentVariable("MessagingQueueUserName"),
                Password = Environment.GetEnvironmentVariable("MessagingQueuePassword")
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            replyQueueName = channel.QueueDeclare().QueueName;

            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        }

        public async Task<GetProductDTO> ReceiveRPCResponse(string correlationId)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($" [x] Received {message}");
                //Here is where I would deserialize the message and return it
            };

            channel.BasicConsume(queue: replyQueueName, autoAck: false, consumer: consumer);
        }
    }
}
