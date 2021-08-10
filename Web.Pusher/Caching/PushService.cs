using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pusher.Caching;
using Pusher.Models;
using Pusher.Mq;
using SP.StudioCore.MQ;
using SP.StudioCore.Utils;
using SP.StudioCore.Web;
using SP.StudioCore.Web.Sockets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Web.Pusher.Responses;

namespace Web.Pusher.Caching
{
    /// <summary>
    /// 静态缓存
    /// </summary>
    public static class PushService
    {
        static PushService()
        {
            //Thread thread = new(Consumer);
            //thread.Start();

            System.Timers.Timer timer = new System.Timers.Timer(6 * 1000);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            ConsoleHelper.WriteLine("PushService Start", ConsoleColor.Blue);
        }
        internal static void Start()
        {
        }

        /// <summary>
        /// 定时清理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            List<Task> tasks = new();
            foreach (Guid sid in PushCaching.Instance().GetExpireMember())
            {
                tasks.Add(Remove(sid));
                ConsoleHelper.WriteLine($"Remove {sid}", ConsoleColor.Yellow);
            }
            Task.WaitAll(tasks.ToArray());
        }


        private readonly static Dictionary<Guid, WebSocketClient> clients = new();

        public static async Task SendAsync(Guid sid, string message)
        {
            if (!clients.ContainsKey(sid)) return;
            await clients[sid].SendAsync(message);
        }

     

        /// <summary>
        /// 把消息发送到频道的全部订阅者
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        internal static void SendAsync(MessageModel message)
        {
            List<Guid> list = PushCaching.Instance().GetSubscribe(message.Channel);
            int count = 0;

            if (list.Any())
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                List<Task> tasks = new();
                MessageResponse response = new MessageResponse
                {
                    Channel = message.Channel,
                    Content = message.Message,
                    Time = message.Time,
                    ID = message.ID.ToString("N")
                };
                foreach (Guid sid in list)
                {
                    if (!clients.ContainsKey(sid)) continue;
                    tasks.Add(clients[sid].SendAsync(response.ToString()));
                    count++;
                }
                Task.WaitAll(tasks.ToArray());

                ConsoleHelper.WriteLine($"send message,clients:{count},{sw.ElapsedMilliseconds}ms", ConsoleColor.Green);
            }

            MqProduct.MessageLog.Send(JsonConvert.SerializeObject(new MessageLog
            {
                ID = message.ID,
                Channel = message.Channel,
                Content = message.Message,
                Count = count,
                CreateAt = message.Time
            }));

        }

        /// <summary>
        /// 批量发送
        /// </summary>
        /// <param name="sids"></param>
        /// <param name="message"></param>
        public static void SendAsync(Guid[] sids, string message)
        {
            List<Task> tasks = new();
            foreach (Guid sid in sids)
            {
                if (!clients.ContainsKey(sid)) continue;
                tasks.Add(clients[sid].SendAsync(message));
            }
            Task.WaitAll(tasks.ToArray());
        }

        public static async Task NewUser(WebSocketClient client)
        {
            if (clients.ContainsKey(client.ID))
            {
                clients[client.ID] = client;
            }
            else
            {
                clients.Add(client.ID, client);
            }
            await PushCaching.Instance().Ping(client.ID);
        }

        public static async Task Remove(Guid sid)
        {
            if (clients.ContainsKey(sid))
            {
                await clients[sid].CloseAsync();
                clients.Remove(sid);
            }
            PushCaching.Instance().Remove(sid);
        }
    }
}
