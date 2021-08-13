using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pusher.Caching;
using Pusher.Models;
using Pusher.Mq;
using SP.StudioCore.Json;
using SP.StudioCore.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Pusher.Caching;
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
        public ContentResult Send([FromBody] MessageModel message)
        {
            MessageLog log = PushService.SendAsync(message);
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
        public ContentResult Online()
        {
            string data = PushService.clients.Values.ToList().Select(t => new
            {
                t.ID,
                t.Query,
                t.IpAddress,
                t.Join,
                t.WebSocket.State
            }).ToJson();
            return new ContentResult()
            {
                StatusCode = 200,
                ContentType = "application/json",
                Content = data
            };
        }
    }
}
