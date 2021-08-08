using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusher.Models
{
    public struct MessageLog
    {
        /// <summary>
        /// 发送时间
        /// </summary>
        public long CreateAt;

        /// <summary>
        /// 内容
        /// </summary>
        public string Content;

        /// <summary>
        ///  接收端数量
        /// </summary>
        public int Count;

        /// <summary>
        /// 频道
        /// </summary>
        public string Channel;
    }
}
