using Newtonsoft.Json;
using Pusher.Agent;
using Pusher.Models;
using Pusher.Mq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SP.StudioCore.MQ;
using SP.StudioCore.MQ.RabbitMQ;
using SP.StudioCore.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusher.Service.Consumers
{
    /// <summary>
    /// 落库
    /// </summary>
    [Consumer(Name = "Connection", ExchangeType = SP.StudioCore.MQ.RabbitMQ.ExchangeType.fanout, ExchangeName = MessageExchangeName.MESSAGE_LOG, QueueName = MessageExchangeName.MESSAGE_LOG)]
    public class MessageConsumer : IListenerMessage
    {
        public bool Consumer(string message, object sender, BasicDeliverEventArgs ea)
        {
            ConsoleHelper.WriteLine($"{message}", ConsoleColor.Blue);
            MessageLog log = JsonConvert.DeserializeObject<MessageLog>(message);
            PushAgent.Instance().SaveMessageLog(log);
            return true;
        }

        public bool FailureHandling(string message, object sender, BasicDeliverEventArgs ea)
        {
            return true;
        }
    }
}
