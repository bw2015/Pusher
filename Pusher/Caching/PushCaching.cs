using Newtonsoft.Json;
using Pusher.Models;
using SP.StudioCore.Cache.Redis;
using SP.StudioCore.Utils;
using SP.StudioCore.Web;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusher.Caching
{

    public class PushCaching : CacheBase<PushCaching>
    {
        /// <summary>
        /// 频道列表
        /// </summary>
        private const string CHANNEL = "CHANNEL:";

        /// <summary>
        /// 会员所订阅的频道
        /// </summary>
        private const string USER_CHANNEL = "USER:CHANNEL:";



        /// <summary>
        /// 订阅频道
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="channels"></param>
        /// <returns></returns>
        public void Subscribe(Guid sid, params string[] channels)
        {
            if (channels == null)
            {
                ConsoleHelper.WriteLine($"未指定要订阅的频道", ConsoleColor.Red);
                return;
            }

            IBatch batch = this.NewExecutor().CreateBatch();

            foreach (string channel in channels)
            {
                //#1 写入频道下订阅的会员列表
                batch.SetAddAsync($"{CHANNEL}{channel}", sid.GetRedisValue());

                //#2 写入会员所订阅的频道
                batch.SetAddAsync($"{USER_CHANNEL}{sid:N}", channel);
            }

            batch.Execute();
        }

        /// <summary>
        /// 获取频道下的全部订阅者
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public List<Guid> GetSubscribe(string channel)
        {
            RedisValue[] values = this.NewExecutor().SetMembers($"{CHANNEL}{channel}");
            List<Guid> list = new();
            foreach (RedisValue value in values)
            {
                list.Add(value.GetRedisValue<Guid>());
            }
            return list;
        }

        /// <summary>
        /// 会员离线
        /// </summary>
        /// <param name="sid"></param>
        public void Remove(Guid sid)
        {
            string key = $"{USER_CHANNEL}{sid:N}";
            // 获取会员所订阅的全部频道
            RedisValue[] channels = this.NewExecutor().SetMembers(key);

            IBatch batch = this.NewExecutor().CreateBatch();
            foreach (RedisValue channel in channels)
            {
                batch.SetRemoveAsync($"{CHANNEL}{channel}", sid.GetRedisValue());
            }
            batch.SortedSetRemoveAsync(MEMBER, sid.GetRedisValue());
            batch.KeyDeleteAsync(key);
            batch.Execute();
        }

        private const string PUBLISH = "PUBLISH";

        /// <summary>
        /// 写入要发送的消息队列
        /// </summary>
        public void Publish(string channel, string message)
        {
            if (string.IsNullOrEmpty(channel) || string.IsNullOrEmpty(message)) return;

            MessageModel model = new MessageModel
            {
                ID = Guid.NewGuid(),
                Channel = channel,
                Message = message
            };

            this.NewExecutor().ListLeftPush(PUBLISH, JsonConvert.SerializeObject(model));
        }

        public IEnumerable<MessageModel> GetMessage()
        {
            while (true)
            {
                RedisValue value = this.NewExecutor().ListRightPop(PUBLISH);
                if (value.IsNullOrEmpty) break;
                yield return JsonConvert.DeserializeObject<MessageModel>(value.GetRedisValue<string>());
            }
        }

        /// <summary>
        /// 用户的在线活动时间
        /// </summary>
        private const string MEMBER = "MEMBER";

        /// <summary>
        /// 写入在线活动时间
        /// </summary>
        /// <param name="sid"></param>
        public async Task<bool> Ping(Guid sid)
        {
            double time = WebAgent.GetTimestamp();
            return await this.NewExecutor().SortedSetAddAsync(MEMBER, sid.GetRedisValue(), time);
        }

        /// <summary>
        /// 获取超时的用户
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Guid> GetExpireMember()
        {
            long expire = WebAgent.GetTimestamp(DateTime.Now.AddSeconds(-60));
            foreach (RedisValue value in this.NewExecutor().SortedSetRangeByScore(MEMBER, 0, expire))
            {
                yield return value.GetRedisValue<Guid>();
            }
        }

        /// <summary>
        /// 消息日志（异步落库）
        /// </summary>
        private const string LOG = "LOG";

        public async Task SaveLog(MessageLog log)
        {
            await this.NewExecutor().ListLeftPushAsync(LOG, JsonConvert.SerializeObject(log));
        }

        public IEnumerable<MessageLog> GetLog()
        {
            while (true)
            {
                RedisValue value = this.NewExecutor().ListRightPop(LOG);
                if (value.IsNullOrEmpty) break;
                yield return JsonConvert.DeserializeObject<MessageLog>(value.GetRedisValue<string>());
            }
        }
    }
}
