using System.Text;
using RabbitMQ.Client;
using System.Text.Json;

namespace OrchestrationService.AsyncDataServices;

public class OrchestrationPublisher : IDisposable, IOrchestrationPublisher
{
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly ILogger<OrchestrationPublisher> _logger;
        public OrchestrationPublisher(IConnection connection, ILogger<OrchestrationPublisher> logger)
        {
                _connection = connection;
                _logger = logger; 
                _channel = _connection.CreateChannelAsync().Result;

                _channel.QueueDeclareAsync(
                        queue: "orchestration.jobs",
                        durable: true,
                        exclusive: false,
                        autoDelete: false);
        }

        public Task PublishAsync(object message)
        {
                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

                var properties = new BasicProperties();
                properties.ContentType = "application/json";
                properties.DeliveryMode = DeliveryModes.Persistent; 

                _channel.BasicPublishAsync(
                        exchange: "", 
                        routingKey: "orchestration.jobs",
                        mandatory: true,
                        basicProperties: properties,
                        body: body);

                return Task.CompletedTask;
        }

        public void Dispose()
        {
                try
                {
                        if (_channel.IsOpen)
                        {
                                _channel.CloseAsync().Wait();
                                _connection.CloseAsync().Wait();
                        }

                        _channel.Dispose();
                        _connection.Dispose();
                }
                catch { _logger.LogError("Failed to dispose channel"); }
                finally
                {
                        GC.SuppressFinalize(this);
                }
        }
}