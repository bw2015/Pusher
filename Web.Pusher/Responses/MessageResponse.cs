using Newtonsoft.Json;
using SP.StudioCore.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Pusher.Responses
{
    /// <summary>
    /// 回复给前台的消息内容
    /// </summary>
    public class MessageResponse : ResponseBase
    {
        /// <summary>
        /// 所属频道
        /// </summary>
        [JsonProperty("channel")]
        public string Channel { get; set; }

        /// <summary>
        /// 信息内容
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }

        /// <summary>
        /// 消息的提交事件
        /// </summary>
        [JsonProperty("t")]
        public long Time { get; set; }

        /// <summary>
        /// 信息编号
        /// </summary>
        [JsonProperty("id")]
        public string ID { get; set; }


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
