using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Pusher.Responses
{
    public abstract class ResponseBase
    {
        [JsonProperty("action")]
        public string Action => this.GetType().Name;
    }
}
