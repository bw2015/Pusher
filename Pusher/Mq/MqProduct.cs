using SP.StudioCore.MQ.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusher.Mq
{
    public static class MqProduct
    {
        public static IRabbitProduct Message => RabbitBoot.GetProductInstance(MessageExchangeName.MESSAGE).Product;
    }
}
