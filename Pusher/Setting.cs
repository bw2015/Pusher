using SP.StudioCore.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusher
{
    public static class Setting
    {
        /// <summary>
        /// 当前的服务器标识
        /// </summary>

        public static string Server { get; set; }

        /// <summary>
        /// 推送消息通知的Url
        /// </summary>
        public static string Send { get; set; }

    }
}
