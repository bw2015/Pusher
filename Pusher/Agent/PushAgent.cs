using Pusher.Models;
using SP.StudioCore.Data;
using SP.StudioCore.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusher.Agent
{
    public class PushAgent : AgentBase<PushAgent>
    {
        public void SaveMessageLog(PushMessage log)
        {
            if (log.Count == 0) return;
            if (log.Content.Length > 4000) log.Content = log.Content.Substring(0, 4000);
            try
            {
                using (DbExecutor db = NewExecutor(IsolationLevel.ReadUncommitted))
                {
                    if (db.Exists<PushMessage>(t => t.LogID == log.LogID))
                    {
                        if (log.Count != 0)
                        {
                            db.ExecuteNonQuery($"UPDATE [{typeof(PushMessage).GetTableName()}] SET [Count] = [Count] + {log.Count} WHERE LogID = '{log.LogID}'");
                        }
                    }
                    else
                    {
                        db.Insert(log);
                    }
                    db.Commit();
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteLine(ex.Message, ConsoleColor.Red);
            }
        }
    }
}
