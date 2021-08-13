using Newtonsoft.Json;
using Pusher;
using Pusher.Models;
using Pusher.Mq;
using RabbitMQ.Client.Events;
using SP.StudioCore.Configuration;
using SP.StudioCore.MQ;
using SP.StudioCore.MQ.RabbitMQ;
using SP.StudioCore.Net;
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
    /// 推送
    /// </summary>
    [Consumer(Name = "Connection", ExchangeType = ExchangeType.fanout, ExchangeName = MessageExchangeName.MESSAGE)]
    public class PushConsumer : IListenerMessage
    {
        Stopwatch sw = new Stopwatch();

        private string sendAPI
        {
            get
            {
                if (!string.IsNullOrEmpty(Setting.Send)) return Setting.Send;
                return Config.GetConfig("Rabbit", "send");
            }
        }

        public bool Consumer(string message, object sender, BasicDeliverEventArgs ea)
        {
            sw.Restart();
            try
            {
                string result = NetAgent.UploadData(sendAPI, message, Encoding.UTF8, headers: new Dictionary<string, string>()
                {
                    { "Content-Type","application/json" }
                });
                return MqProduct.MessageLog.Send(result);
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteLine(ErrorHelper.GetExceptionContent(ex), ConsoleColor.Red);
                return this.FailureHandling(message, sender, ea);
            }
            finally
            {
                ConsoleHelper.WriteLine($"[Push] - {message} - {sw.ElapsedMilliseconds}ms", ConsoleColor.Yellow);
            }
        }

        public bool FailureHandling(string message, object sender, BasicDeliverEventArgs ea)
        {
            return true;
        }
    }
}
