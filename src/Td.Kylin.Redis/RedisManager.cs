using StackExchange.Redis;

namespace Td.Kylin.Redis
{
    /// <summary>
    /// Redis管理器
    /// </summary>
    public sealed class RedisManager
    {
        private volatile static ConnectionMultiplexer _redis;

        /// <summary>
        /// 对象锁
        /// </summary>
        private static readonly object _locker = new object();

        /// <summary>
        /// ConnectionMultiplexer
        /// </summary>
        public static ConnectionMultiplexer Redis
        {
            get
            {
                if (_redis == null)
                {
                    lock (_locker)
                    {
                        if (_redis == null)
                        {
                            _redis = GetManager();
                        }
                        return _redis;
                    }
                }

                return _redis;
            }
        }

        private static ConnectionMultiplexer GetManager()
        {
            try
            {
                return ConnectionMultiplexer.Connect(RedisConfiguration.ConfigurationOptions);
            }
            catch
            {
                return null;
            }
        }
    }
}
