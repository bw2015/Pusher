using SP.StudioCore.Configuration;
using SP.StudioCore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusher.Agent
{
    public abstract class AgentBase<T> : DbAgent<T> where T : class, new()
    {
        public AgentBase() : base(Config.GetConnectionString("DbConnection")) { }
    }
}
