using StackExchange.Redis;
using System;
using Microsoft.AspNetCore.Builder;

namespace Td.Kylin.Redis
{
    /// <summary>
    /// Redis连接注入扩展
    /// </summary>
    public static class RedisInjection
    {
        #region 返回原IApplicationBuilder对象

        /// <summary>
        /// 以ConfigurationOptions形式注入
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRedis(this IApplicationBuilder builder, ConfigurationOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.Use(next => new RedisMiddleware(next, options).Invoke);
        }

        /// <summary>
        /// 以连接字符串形式注入
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRedis(this IApplicationBuilder builder, string config)
        {
            return builder.Use(next => new RedisMiddleware(next, config).Invoke);
        }

        #endregion

        #region 不返回原对象

        /// <summary>
        /// 以ConfigurationOptions形式注入
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static void UseRedis(ConfigurationOptions options)
        {
            new RedisMiddleware(options).Invoke();
        }

        /// <summary>
        /// 以连接字符串形式注入
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static void UseRedis(string config)
        {
            new RedisMiddleware(config).Invoke();
        }

        #endregion
    }
}
