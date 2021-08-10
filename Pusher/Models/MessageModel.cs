using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusher.Models
{
    public class MessageModel
    {
        /// <summary>
        /// 消息ID
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 频道
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// 消息的发送事件
        /// </summary>
        public long Time { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }
    }
}
