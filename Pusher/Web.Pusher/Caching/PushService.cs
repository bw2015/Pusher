using SP.StudioCore.Utils;
using SP.StudioCore.Web;
using SP.StudioCore.Web.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Web.Pusher.Models;
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
        }

        private static async Task SendAsync()
        {
            ConsoleHelper.WriteLine($"启动发送服务", ConsoleColor.Green);
            while (true)
            {
                foreach (MessageModel message in PushCaching.Instance().GetMessage())
                {
                    await SendAsync(message);
                }
                Thread.Sleep(1000);
            }
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
        public static async Task SendAsync(MessageModel message)
        {
            List<Guid> list = PushCaching.Instance().GetSubscribe(message.Channel);
            if (!list.Any()) return;
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

        public static void Remove(Guid sid)
        {
            if (clients.ContainsKey(sid))
            {
                clients.Remove(sid);
            }
        }
    }
}
