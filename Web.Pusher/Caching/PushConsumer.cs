using Newtonsoft.Json;
using Pusher;
using Pusher.Models;
using Pusher.Mq;
using RabbitMQ.Client.Events;
using SP.StudioCore.Configuration;
using SP.StudioCore.MQ;
using SP.StudioCore.MQ.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Pusher.Caching
{
    [Consumer(Name = "Connection", ExchangeType = ExchangeType.fanout, ExchangeName = MessageExchangeName.MESSAGE, QueueName = "Local")]
    public class PushConsumer : IListenerMessage
    {
        public bool Consumer(string message, object sender, BasicDeliverEventArgs ea)
        {
            MessageModel model = JsonConvert.DeserializeObject<MessageModel>(message);
            PushService.SendAsync(model).Wait();
            return true;
        }

        public bool FailureHandling(string message, object sender, BasicDeliverEventArgs ea)
        {
            return true;
        }
    }
}
