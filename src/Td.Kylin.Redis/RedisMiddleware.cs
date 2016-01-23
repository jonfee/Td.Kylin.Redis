using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Td.Kylin.Redis
{
    /// <summary>
    /// Redis连接中间件
    /// </summary>
    internal sealed class RedisMiddleware
    {
        /// <summary>
        /// Http Request
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// The options relevant to a set of redis connections
        /// </summary>
        private readonly ConfigurationOptions _options;

        #region 

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="next"></param>
        /// <param name="options">Redis Connections</param>
        public RedisMiddleware(RequestDelegate next, ConfigurationOptions options)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            _options = options;
            _next = next;
        }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="next"></param>
        /// <param name="redisConnection">Redis 连接字符串</param>
        public RedisMiddleware(RequestDelegate next, string redisConnection)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            var tempOptions = ConfigurationOptions.Parse(redisConnection);

            if (tempOptions == null)
            {
                throw new ArgumentNullException(nameof(redisConnection));
            }

            _options = tempOptions;
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            RedisConfiguration.ConfigurationOptions = _options;

            return _next(context);
        }

        #endregion

        #region 

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="next"></param>
        /// <param name="options">Redis Connections</param>
        public RedisMiddleware(ConfigurationOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            _options = options;
        }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="next"></param>
        /// <param name="redisConnection">Redis 连接字符串</param>
        public RedisMiddleware(string redisConnection)
        {
            var tempOptions = ConfigurationOptions.Parse(redisConnection);

            if (tempOptions == null)
            {
                throw new ArgumentNullException(nameof(redisConnection));
            }

            _options = tempOptions;
        }

        public void Invoke()
        {
            RedisConfiguration.ConfigurationOptions = _options;
        }

        #endregion
    }
}
