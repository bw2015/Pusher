using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pusher.Caching;
using Pusher.Models;
using Pusher.Mq;
using SP.StudioCore.Json;
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
        [HttpPost("/publish")]
        public ContentResult Send([FromForm] string appkey, [FromForm] string channel, [FromForm] string content)
        {
            MessageModel model = new MessageModel
            {
                ID = Guid.NewGuid(),
                Channel = channel,
                Message = content
            };
            //  PushCaching.Instance().Publish(channel, content);
            MqProduct.Message.Send(JsonConvert.SerializeObject(model));
            return new ContentResult()
            {
                StatusCode = 200,
                ContentType = "application/json",
                Content = new
                {
                    code = 200,
                    content = "OK"
                }.ToJson()
            };
        }

        [HttpGet("/socket.io/")]
        public ContentResult transport([FromQuery] int eio, [FromForm] string transport, [FromQuery] string sid)
        {
            //___eio[1]('111:0{"sid":"13880e6d-485e-4ba9-a2a0-da08a038fad6","upgrades":["websocket"],"pingInterval":8000,"pingTimeout":3000}');
            if (string.IsNullOrEmpty(sid))
            {
                InitResponse init = new InitResponse(Guid.NewGuid());
                return new ContentResult()
                {
                    StatusCode = 200,
                    ContentType = "application/javascript",
                    Content = $"___eio[0]('111:0{init.ToJson()}');"
                };
            }
            else
            {
                return new ContentResult()
                {
                    StatusCode = 200,
                    ContentType = "application/javascript",
                    Content = $"___eio[0]('2:40');"
                };
            }
        }

        [HttpPost("/socket.io/")]
        public ContentResult transport()
        {
            return new ContentResult()
            {
                StatusCode = 200,
                ContentType = "text/html"
            };
        }
    }
}
