using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pusher.Models;
using Pusher.Mq;
using SP.StudioCore.Json;
using SP.StudioCore.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Web.Publish
{
    /// <summary>
    /// 信息广播
    /// </summary>
    public class PublishController : ControllerBase
    {
        [HttpPost("/publish")]
        public async Task<ContentResult> Publish([FromForm] string appkey, [FromForm] string channel, [FromForm] string content)
        {
            MessageModel model = new MessageModel
            {
                ID = Guid.NewGuid(),
                Channel = channel,
                Message = content,
                Time = WebAgent.GetTimestamps()
            };
            await Task.Run(() => MqProduct.Message.Send(JsonConvert.SerializeObject(model)), CancellationToken.None);
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
    }
}
