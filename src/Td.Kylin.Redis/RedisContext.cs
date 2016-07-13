using StackExchange.Redis;
using System;

namespace Td.Kylin.Redis
{
    /// <summary>
    /// Redis上下文
    /// </summary>
    public class RedisContext : IDisposable
    {
        #region 私有属性

        private ConfigurationOptions _options;

        private ConnectionMultiplexer _multiplexer;

        #endregion

        /// <summary>
        /// ConnectionMultiplexer
        /// </summary>
        public ConnectionMultiplexer Multiplexer
        {
            get
            {
                if (_multiplexer == null || !_multiplexer.IsConnected)
                {
                    _multiplexer = ConnectionMultiplexer.Connect(_options);
                }

                return _multiplexer;
            }
        }

        /// <summary>
        /// Redis上下文
        /// </summary>
        /// <param name="options"><seealso cref="ConfigurationOptions"/>实例对象</param>
        public RedisContext(ConfigurationOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Redis上下文
        /// </summary>
        /// <param name="connectionString">Redis连接配置字符串</param>
        public RedisContext(string connectionString)
        {
            _options = ConfigurationOptions.Parse(connectionString);
        }

        /// <summary>
        /// 是否已连接
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return null != Multiplexer ? Multiplexer.IsConnected : false;
            }
        }

        /// <summary>
        /// 当前Redis上下文中数据库索引器
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public IDatabase this[int db]
        {
            get
            {
                return GetDatabase(db, null);
            }
        }

        /// <summary>
        /// Obtain an interactive connection to a database inside redis
        /// </summary>
        /// <param name="db"></param>
        /// <param name="asyncState"></param>
        /// <returns></returns>
        public IDatabase GetDatabase(int db = -1, object asyncState = null)
        {
            return Multiplexer.GetDatabase(db, asyncState);
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            if (null != _multiplexer)
            {
                _multiplexer.Close();
                _multiplexer.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}
