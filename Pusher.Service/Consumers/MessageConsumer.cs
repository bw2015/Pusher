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
using System.Diagnostics;
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
        Stopwatch sw = new Stopwatch();
        public void Consumer(string message, object sender, BasicDeliverEventArgs ea)
        {
            sw.Restart();
            try
            {
                MessageLog log = JsonConvert.DeserializeObject<MessageLog>(message);
                PushAgent.Instance().SaveMessageLog(log);
            }
            catch
            {
                this.FailureHandling(message, sender, ea);
            }
            finally
            {
                ConsoleHelper.WriteLine($"[DB] - {message} - {sw.ElapsedMilliseconds}ms", ConsoleColor.Blue);
            }
        }

        public bool FailureHandling(string message, object sender, BasicDeliverEventArgs ea)
        {
            return true;
        }
    }
}
