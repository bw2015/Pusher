using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Pusher.Requests
{
    public class authorize : RequestBase
    {
        public authorize(string content) : base(content) { }

        public string appkey { get; set; }

        public string artifactVersion { get; set; }

        public string userId { get; set; }

        public string userData { get; set; }
    }
}
