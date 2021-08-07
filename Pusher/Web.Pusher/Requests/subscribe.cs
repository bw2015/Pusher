using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Pusher.Requests
{
    /// <summary>
    /// 订阅
    /// </summary>
    public class subscribe : RequestBase
    {
        public subscribe(string content) : base(content)
        {
        }

        /// <summary>
        /// 订阅的频道
        /// </summary>
        public string channel { get; set; }

        /// <summary>
        /// 客户端标识
        /// </summary>
        public Guid sid { get; set; }
    }
}
