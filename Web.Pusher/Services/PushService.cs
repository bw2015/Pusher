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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Web.Pusher.Responses;

namespace Web.Pusher.Services
{
    /// <summary>
    /// 静态缓存
    /// </summary>
    public static class PushService
    {

        private static DateTime _removeTime = DateTime.Now;

        /// <summary>
        /// 定时清理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal static async Task<int> Remove()
        {
            if (_removeTime > DateTime.Now) return 0;
            int count = 0;
            try
            {
                _removeTime = DateTime.Now.AddSeconds(10);
                foreach (Guid sid in PushCaching.Instance().GetExpireMember())
                {
                    count++;
                    await Remove(sid);
                }
            }
            finally
            {
                _removeTime = DateTime.Now.AddSeconds(10);
            }
            return count;
        }

        /// <summary>
        /// 执行彻底的清理方法
        /// </summary>
        /// <returns></returns>
        internal static async Task<int> RemoveAll()
        {
            int count = 0;
            List<Guid> users = clients.Keys.ToList();
            foreach (Guid userId in users)
            {
                if (!PushCaching.Instance().ExistsMember(userId))
                {
                    count++;
                    await Remove(userId);
                }
            }
            return count;
        }

        internal readonly static ConcurrentDictionary<Guid, WebSocketClient> clients = new();


        /// <summary>
        /// 把消息发送到频道的全部订阅者
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        internal static async Task<MessageLog> SendAsync(MessageModel message)
        {
            Stopwatch sw = Stopwatch.StartNew();
            List<Guid> list = PushCaching.Instance().GetSubscribe(message.Channel);
            int count = 0;
            if (list.Any())
            {
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
                    await clients[sid].SendAsync(response.ToString());
                    count++;
                }
            }

            ConsoleHelper.WriteLine($"[SendAsync]   -   {count} -   {sw.ElapsedMilliseconds}ms", ConsoleColor.Green);
            return new MessageLog
            {
                ID = message.ID,
                Channel = message.Channel,
                Content = message.Message,
                Count = count,
                CreateAt = message.Time
            };
        }

        /// <summary>
        /// 注册会员，并且写入本地内存
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task Register(WebSocketClient client)
        {
            if (clients.ContainsKey(client.ID))
            {
                clients[client.ID] = client;
            }
            else
            {
                clients.TryAdd(client.ID, client);
            }
            await PushCaching.Instance().Online(client.ID);
        }

        public static async Task Remove(Guid sid)
        {
            try
            {
                if (clients.ContainsKey(sid))
                {
                    await clients[sid].CloseAsync();
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteLine($"[Remove - {ex.GetType().Name}] - {ex.Message} -   PushService.Remove.Exception", ConsoleColor.Red);
            }
            finally
            {
                PushCaching.Instance().Remove(sid);
                if (clients.TryRemove(sid, out WebSocketClient client))
                {
                    ConsoleHelper.WriteLine($"[REMOVE]  -   {sid}   -   {client?.IpAddress} -   PushService.Remove.Finish", ConsoleColor.Yellow);
                    client?.Dispose();
                }
            }
        }
    }
}
