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
using Web.Pusher.Services;
using Web.Pusher.Requests;
using Web.Pusher.Responses;
using System.Diagnostics;

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
            Stopwatch sw = new Stopwatch();
            sw.Restart();
            if (context.WebSockets.IsWebSocketRequest)
            {
                //后台成功接收到连接请求并建立连接后，前台的webSocket.onopen = function (event){}才执行
                using (WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync())
                {
                    client = new WebSocketClient(context, webSocket);
                    ConsoleHelper.WriteLine($"{client.ID}    -   创建Client对象  -   {sw.ElapsedMilliseconds}ms", ConsoleColor.Cyan);
                    // 注册新用户
                    await PushService.Register(client);
                    ConsoleHelper.WriteLine($"{client.ID}    -   注册客户端  -   {sw.ElapsedMilliseconds}ms", ConsoleColor.Cyan);
                    try
                    {
                        // 初始化链接
                        await Init(client);
                        ConsoleHelper.WriteLine($"{client.ID}    -   初始化协议  -   {sw.ElapsedMilliseconds}ms", ConsoleColor.Cyan);

                        await Handler(client);
                    }
                    catch (ConnectionAbortedException ex)
                    {
                        // 关闭超时
                        ConsoleHelper.WriteLine($"[Invoke - {ex.GetType().Name}] {ex.Message}", ConsoleColor.Red);
                    }
                    catch (WebSocketException ex)
                    {
                        ConsoleHelper.WriteLine($"[Invoke - {ex.GetType().Name}] {ex.Message}", ConsoleColor.Red);
                    }
                    catch (Exception ex)
                    {
                        ConsoleHelper.WriteLine($"[Invoke - {ex.GetType().Name}] {ex.Message}", ConsoleColor.Red);
                    }
                    finally
                    {
                        ConsoleHelper.WriteLine($"[CLOSE] {client.ID}", ConsoleColor.Yellow);
                    }
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
                if (!result.CloseStatus.HasValue && result.MessageType == WebSocketMessageType.Text)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    if (message == "0")
                    {
                        await Online(client);
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
        private async Task Online(WebSocketClient client)
        {
            await client.SendAsync("1");
            await PushCaching.Instance().Online(client.ID);
        }
    }
}
