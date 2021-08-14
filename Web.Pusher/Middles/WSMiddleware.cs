using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http;
using Pusher.Caching;
using SP.StudioCore.Utils;
using SP.StudioCore.Web.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Web.Pusher.Caching;
using Web.Pusher.Requests;
using Web.Pusher.Responses;

namespace Web.Pusher.Middles
{
    public class WSMiddleware
    {
        private readonly RequestDelegate _next;
        public WSMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// 当前连接的websocket客户端
        /// </summary>
        private WebSocketClient client;

        public async Task Invoke(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                //后台成功接收到连接请求并建立连接后，前台的webSocket.onopen = function (event){}才执行
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync().ConfigureAwait(true);
                client = new WebSocketClient(context, webSocket);
                try
                {
                    // 注册新用户
                    await PushService.Register(client);
                    // 初始化链接
                    await Init(client);
                    await Handler(client);
                }
                catch (ConnectionAbortedException ex)
                {
                    // 关闭超时
                }
                catch (Exception ex)
                {
                    ConsoleHelper.WriteLine($"[Invoke - {ex.GetType().Name}] {ex.Message}", ConsoleColor.Red);
                }
                finally
                {
                    ConsoleHelper.WriteLine($"[CLOSE] {client.ID}", ConsoleColor.Yellow);
                    await PushService.Remove();
                }
            }
            else
            {
                await _next(context).ConfigureAwait(true);
            }
        }

        private async Task Handler(WebSocketClient client)
        {
            WebSocketReceiveResult result;
            do
            {
                byte[] buffer = new byte[1024 * 4];
                result = await client.WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).ConfigureAwait(true);
                if (result.MessageType == WebSocketMessageType.Text && !result.CloseStatus.HasValue)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    if (message == "0")
                    {
                        await Ping(client);
                    }
                    else
                    {
                        RequestBase request = RequestFactory.GetRequest(message);
                        if (request != null)
                        {
                            switch (request.action)
                            {
                                case "authorize":
                                    await Anthorize(client);
                                    break;
                                case "subscribe":
                                    // 回复订阅成功
                                    await this.Subscribe(client, (subscribe)request);
                                    break;
                                default:
                                    await client.SendAsync("hello,world");
                                    break;
                            }
                        }
                    }
                }
            } while (!result.CloseStatus.HasValue);
        }

        /// <summary>
        /// 初始化分配
        /// </summary>
        /// <returns></returns>
        private async Task Init(WebSocketClient client)
        {
            await client.SendAsync(new InitResponse(client).ToString());
        }

        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private async Task Anthorize(WebSocketClient client)
        {
            await client.SendAsync(new ReconnectionResponse(client.ID).ToString());
        }

        /// <summary>
        /// 订阅成功
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private async Task Subscribe(WebSocketClient client, subscribe subscribe)
        {
            PushCaching.Instance().Subscribe(client.ID, subscribe.channel);
            await client.SendAsync(new SubscribeResponse(subscribe.channel).ToString());
        }

        /// <summary>
        /// 回应
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private async Task Ping(WebSocketClient client)
        {
            await client.SendAsync("1");
            await PushCaching.Instance().Ping(client.ID);
        }
    }
}
