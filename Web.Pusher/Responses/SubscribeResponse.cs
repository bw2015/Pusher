using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Pusher.Responses
{
    /// <summary>
    /// 订阅成功
    /// </summary>
    public class SubscribeResponse : ResponseBase
    {
        public SubscribeResponse(string channel)
        {
            this.Channel = channel;
        }

        public string Channel { get; set; }

        public string content = "Ok";

        public int resultCode = 200;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
