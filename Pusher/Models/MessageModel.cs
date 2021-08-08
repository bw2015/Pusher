using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusher.Models
{
    public struct MessageModel
    {
        /// <summary>
        /// 消息ID
        /// </summary>
        public Guid ID;

        /// <summary>
        /// 频道
        /// </summary>
        public string Channel;

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message;
    }
}
