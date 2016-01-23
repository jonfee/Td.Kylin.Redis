using StackExchange.Redis;

namespace Td.Kylin.Redis
{
    /// <summary>
    /// Redis连接配置
    /// </summary>
    public class RedisConfiguration
    {
        /// <summary>
        /// 配置项
        /// </summary>
        public static ConfigurationOptions ConfigurationOptions { get; set; }
    }
}
