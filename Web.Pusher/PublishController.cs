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
        [HttpPost("/publish")]
        public ContentResult Publish([FromForm] string appkey, [FromForm] string channel, [FromForm] string content)
        {
            MessageModel model = new MessageModel
            {
                ID = Guid.NewGuid(),
                Channel = channel,
                Message = content,
                Time = WebAgent.GetTimestamps()
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

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <returns></returns>
        [HttpPost("/send")]
        public ContentResult Send([FromBody] MessageModel message)
        {
            PushService.SendAsync(message);
            return new ContentResult()
            {
                StatusCode = 200,
                Content = "OK"
            };
        }
    }
}
