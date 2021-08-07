using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Pusher.Responses
{
    /// <summary>
    /// 重连成功的通知信息
    /// </summary>
    public class ReconnectionResponse
    {
        public ReconnectionResponse(Guid sid)
        {
            this.sid = sid;
        }

        public string content  = "Ok";

        public Guid sid { get; set; }

        public bool enableSubscribe = true;

        public bool enablePublish  = false;

        public int resultCode  = 200;

        public override string ToString()
        {
            return string.Concat("430", JsonConvert.SerializeObject(new[] { this }));
        }
    }
}
