using Pusher.Models;
using SP.StudioCore.Data;
using SP.StudioCore.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusher.Agent
{
    public class PushAgent : AgentBase<PushAgent>
    {
        public void SaveMessageLog(PushMessage log)
        {
            try
            {
                using (DbExecutor db = NewExecutor())
                {
                    db.Insert(log);
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteLine(ex.Message, ConsoleColor.Red);
            }
        }
    }
}
