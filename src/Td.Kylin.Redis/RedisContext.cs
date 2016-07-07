using StackExchange.Redis;
using System;
using System.IO;

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
        /// Redis上下文
        /// </summary>
        /// <param name="options"></param>
        public RedisContext(ConfigurationOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Redis上下文
        /// </summary>
        /// <param name="connectionString"></param>
        public RedisContext(string connectionString)
        {
            _options = ConfigurationOptions.Parse(connectionString);
        }

        /// <summary>
        /// 连接Redis服务器
        /// </summary>
        /// <param name="log"></param>
        private void Connect(TextWriter log = null)
        {
            if (!IsConnected)
            {
                Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
                {
                    return ConnectionMultiplexer.Connect(_options, log);
                });
                _multiplexer = lazyConnection.Value;
            }
        }

        /// <summary>
        /// 是否已连接
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return null != _multiplexer ? _multiplexer.IsConnected : false;
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
            if (!IsConnected) Connect();

            if (IsConnected)
            {
                return _multiplexer.GetDatabase(db, asyncState);
            }
            else
            {
                return null;
            }
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
