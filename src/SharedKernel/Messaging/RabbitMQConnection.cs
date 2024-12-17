using RabbitMQ.Client;

namespace SharedKernel.Common.Messaging
{
    public class RabbitMQConnection
    {
        #region Properties
        private readonly RabbitMQSettings _settings;
        private IConnection _connection;
        private IModel _channel;

        //public IModel GetChannel() => _channel;
        #endregion

        #region Constructor
        public RabbitMQConnection(RabbitMQSettings settings)
        {
            _settings = settings;
            //try
            //{
            //    var factory = new ConnectionFactory()
            //    {
            //        HostName = "localhost",
            //        VirtualHost = "/",
            //        UserName = "guest",
            //        Password = "guest"
            //    };
            //    _connection = factory.CreateConnection();
            //    _channel = _connection.CreateModel();
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Connection error on RabbitMQ: {ex.Message}");
            //    Console.WriteLine(ex.StackTrace);
            //    throw;
            //}
        }
        #endregion

        #region Methods
        public IModel GetChannel()
        {
            if (_channel == null || !_channel.IsOpen)
            {
                try
                {
                    var factory = new ConnectionFactory()
                    {
                        HostName = _settings.HostName, //"localhost",
                        VirtualHost = _settings.VirtualHost, //"/",
                        UserName = _settings.UserName, //"guest",
                        Password = _settings.Password //"guest"
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
                return _channel;
            }
            return null;
        }
        #endregion
    }
}
