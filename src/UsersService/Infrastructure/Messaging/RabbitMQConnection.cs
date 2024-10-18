using RabbitMQ.Client;

namespace UsersService.Infrastructure.Messaging
{
    public class RabbitMQConnection
    {

        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQConnection()
        {
            try
            {

                var factory = new ConnectionFactory()
                {
                    HostName = "localhost",
                    VirtualHost = "/",
                    UserName = "guest",
                    Password = "guest",
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection error on RabbitMQ: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        public IModel GetChannel() => _channel;

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
