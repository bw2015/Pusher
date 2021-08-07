using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SP.StudioCore.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Web.Pusher.Requests
{
    public abstract class RequestBase
    {
        public RequestBase(string content)
        {
            JsonConvert.PopulateObject(content, this);
        }

        /// <summary>
        /// 动作名称
        /// </summary>
        public string action { get; set; }
    }
}
