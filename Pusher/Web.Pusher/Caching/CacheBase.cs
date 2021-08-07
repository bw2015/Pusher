using SP.StudioCore.Cache.Redis;
using SP.StudioCore.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Pusher.Caching
{
    public abstract class CacheBase<T> : RedisCacheBase where T : class, new()
    {
        protected override int DB_INDEX => 10;

        private static T _intance;
        /// <summary>
        /// 单例模式
        /// </summary>
        /// <returns></returns>
        public static T Instance()
        {
            if (_intance == null) _intance = new T();
            return _intance;
        }

        protected CacheBase() : base(Config.GetConnectionString("redis"))
        {
        }
    }
}
