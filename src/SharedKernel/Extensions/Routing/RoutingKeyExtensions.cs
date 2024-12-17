namespace SharedKernel.Extensions.Routing
{
    public static class RoutingKeyExtensions
    {
        public static string ToRoutingKey(this PublicationRoutingKeys routingKeys)
        {
            return routingKeys.ToString().ToLower().Replace('_', '.');
        }

        public static string ToExchangeName(this PublicationExchangeNames exchange)
        {
            return $"{exchange.ToString().ToLower()}_exchange";
        }
    }
}
