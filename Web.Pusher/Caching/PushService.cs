using Pusher.Caching;
using Pusher.Models;
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
            Thread thread = new(() =>
            {
                SendAsync().Wait();
            });
            thread.Start();

            System.Timers.Timer timer = new System.Timers.Timer(6 * 1000);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            ConsoleHelper.WriteLine("PushService Start", ConsoleColor.Blue);
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

        private static async Task SendAsync()
        {
            ConsoleHelper.WriteLine($"start message sending", ConsoleColor.Green);
            while (true)
            {
                foreach (MessageModel message in PushCaching.Instance().GetMessage())
                {
                    await SendAsync(message);
                }
                Thread.Sleep(200);
            }
        }

        private readonly static Dictionary<Guid, WebSocketClient> clients = new();

        public static async Task SendAsync(Guid sid, string message)
        {
            if (!clients.ContainsKey(sid)) return;
            await clients[sid].SendAsync(message);
        }

        internal static void Start()
        {
        }

        /// <summary>
        /// 把消息发送到频道的全部订阅者
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task SendAsync(MessageModel message)
        {
            List<Guid> list = PushCaching.Instance().GetSubscribe(message.Channel);
            if (!list.Any()) return;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<Task> tasks = new();
            int count = 0;
            MessageResponse response = new MessageResponse
            {
                Channel = message.Channel,
                Content = message.Message,
                ID = message.ID.ToString("N")
            };
            foreach (Guid sid in list)
            {
                if (!clients.ContainsKey(sid)) continue;
                tasks.Add(clients[sid].SendAsync(response.ToString()));
                count++;
            }
            Task.WaitAll(tasks.ToArray());

            await PushCaching.Instance().SaveLog(new MessageLog
            {
                Channel = message.Channel,
                Content = message.Message,
                Count = count,
                CreateAt = WebAgent.GetTimestamps()
            });

            ConsoleHelper.WriteLine($"send message,clients:{count},{sw.ElapsedMilliseconds}ms", ConsoleColor.Green);
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
