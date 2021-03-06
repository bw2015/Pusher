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
        /// <summary>
        /// 消息发布
        /// </summary>        
        public static IRabbitProduct Message => RabbitBoot.GetProductInstance(MessageExchangeName.MESSAGE).Product;

        /// <summary>
        /// 发送记录
        /// </summary>
        public static IRabbitProduct MessageLog => RabbitBoot.GetProductInstance(MessageExchangeName.MESSAGE_LOG).Product;
    }
}
