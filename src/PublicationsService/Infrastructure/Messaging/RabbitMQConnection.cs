﻿using RabbitMQ.Client;

namespace PublicationsService.Infrastructure.Messaging
{
    public class RabbitMQConnection
    {
        #region Properties
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public IModel GetChannel() => _channel;
        #endregion

        #region Constructor
        public RabbitMQConnection()
        {
            try {
                var factory = new ConnectionFactory()
                {
                    HostName = "localhost",
                    VirtualHost = "/",
                    UserName = "guest",
                    Password = "guest"
                };
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
            }
            catch(Exception ex) {
                Console.WriteLine($"Connection error on RabbitMQ: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
        #endregion

        #region Methods
        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
        #endregion
    }
}