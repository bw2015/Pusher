using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pusher.Caching;
using Pusher.Models;
using Pusher.Mq;
using SP.StudioCore.Json;
using SP.StudioCore.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Web.Pusher.Services;
using Web.Pusher.Responses;

namespace Web.Pusher
{
    /// <summary>
    /// 服务器发送消息
    /// </summary>
    public class PublishController : ControllerBase
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <returns></returns>
        [HttpPost("/send")]
        public async Task<ContentResult> Send([FromBody] MessageModel message)
        {
            MessageLog log = await PushService.SendAsync(message);
            return new ContentResult()
            {
                StatusCode = 200,
                ContentType = "application/json",
                Content = JsonConvert.SerializeObject(log)
            };
        }

        /// <summary>
        /// 获取所有在线的客户端信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("/online")]
        public async Task<ContentResult> Online()
        {
            string data = PushService.clients.Values.ToList().Select(t => new
            {
                t.ID,
                t.Query,
                t.IpAddress,
                t.Join,
                t.WebSocket.State
            }).ToJson();
            return await Task.Run(() =>
            {
                return new ContentResult()
                {
                    StatusCode = 200,
                    ContentType = "application/json",
                    Content = data
                };
            });
        }

        /// <summary>
        /// 手动调用清除超时客户端
        /// </summary>
        /// <returns></returns>
        [HttpGet("/remove")]
        public async Task<ContentResult> Remove()
        {
            Stopwatch sw = new Stopwatch();
            sw.Restart();
            int count = await PushService.RemoveAll();
            return new ContentResult()
            {
                StatusCode = 200,
                ContentType = "application/json",
                Content = new
                {
                    Count = count,
                    Time = sw.ElapsedMilliseconds + "ms"
                }.ToJson()
            };
        }
    }
}
