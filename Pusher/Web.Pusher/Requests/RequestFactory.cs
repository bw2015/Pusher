using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Web.Pusher.Requests
{
    public static class RequestFactory
    {
        public static RequestBase GetRequest(string content)
        {
            JObject info = JObject.Parse(content);
            if (!info.ContainsKey("action")) return null;
            string action = info["action"].Value<string>();
            Type type = typeof(RequestFactory).Assembly.GetType($"Web.Pusher.Requests.{action}");
            if (type == null) return null;
            return (RequestBase)Activator.CreateInstance(type, new object[] { content });
        }
    }
}
