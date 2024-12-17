namespace SharedKernel.Common.Messaging
{
    public class RabbitMQSettings
    {
        public string HostName { get; set; } = "localhost";
        public string VirtualHost { get; set; } = "/";
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
    }
}
