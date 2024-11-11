using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Common.Extensions
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
