using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Pusher.Responses
{
    /// <summary>
    /// 初始化
    /// </summary>
    public class InitResponse : ResponseBase
    {
        public InitResponse(Guid sid)
        {
            this.sid = sid.ToString();
        }

        /// <summary>
        /// 初始化时候分配的客户端ID
        /// </summary>
        [JsonProperty(Order = 1)]
        public string sid { get; set; }

        [JsonProperty(Order = 2)]
        public string[] upgrades = new[] { "websocket" };

        [JsonProperty(Order = 3)]
        public int pingInterval = 8000;

        [JsonProperty(Order = 4)]
        public int pingTimeout = 3000;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
