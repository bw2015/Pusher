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

        public static readonly string PushServer;

        static Setting()
        {
            PushServer = Config.GetConfig("Rabbit", "Server");
        }
    }
}
