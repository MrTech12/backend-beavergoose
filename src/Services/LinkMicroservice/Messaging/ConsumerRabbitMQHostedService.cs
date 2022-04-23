using LinkMicroservice.DTOs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace LinkMicroservice.Messaging
{
    public class ConsumerRabbitMQHostedService : BackgroundService
    {
        private readonly ILogger<ConsumerRabbitMQHostedService> _logger;
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        private const string QueueName = "link-creation";
        private readonly IConfiguration _configuration;

        public ConsumerRabbitMQHostedService(IConfiguration configuration, ILogger<ConsumerRabbitMQHostedService> logger)
        {
            this._configuration = configuration;
            this._logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            this._connectionFactory = new ConnectionFactory
            {
                HostName = this._configuration["RabbitMQ:HostName"],
                Port = Convert.ToInt32(this._configuration["RabbitMQ:Port"])
            };
            this._connection = _connectionFactory.CreateConnection();
            this._channel = _connection.CreateModel();

            this._channel.ExchangeDeclare("link-exchange", ExchangeType.Topic);
            this._channel.QueueDeclare(queue: QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var properties = this._channel.CreateBasicProperties();
            properties.Persistent = true; // Declaring the message as persistent
            this._channel.BasicQos(0, 1, false); // Send messages to different workers based on received acknowledgment

            this._logger.LogInformation($"Queue [{QueueName}] is waiting for messages.");
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(this._channel);
            consumer.Received += async (model, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                _logger.LogInformation($"Message received: '{content}'.");
                var fileDiscoveryDto = JsonSerializer.Deserialize<FileDiscoveryDTO>(content);

                await HandleMessage();
                _channel.BasicAck(ea.DeliveryTag, false); // Letting RabbitMQ know that the message had been received.
            };


            this._channel.BasicConsume(queue: "hello", autoAck: false, consumer: consumer);
            
            await Task.CompletedTask;
        }

        public async Task HandleMessage()
        {

        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            this._connection.Close();
            this._logger.LogInformation("RabbitMQ connection is closed.");
        }
    }
}
