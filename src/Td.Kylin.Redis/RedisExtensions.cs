using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Td.Kylin.Redis
{
    /// <summary>
    /// Redis扩展
    /// </summary>
    public static class RedisExtensions
    {
        #region Redis 公共方法

        /// <summary>
        /// 获取该方法的调用者方法或属性名
        /// </summary>
        /// <param name="current"></param>
        /// <param name="caller"></param>
        /// <returns></returns>
        public static string Me(this object current, [CallerMemberName]string caller = null)
        {
            return caller;
        }

        /// <summary>
        /// RedisValue集合转换为指定类型的列表集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static List<T> RedisValueFactory<T>(this RedisValue[] values)
        {
            var result = new List<T>();

            if (null != values && values.Length > 0)
            {
                foreach (var v in values)
                {
                    T item = v.DeserializeObject<T>();

                    result.Add(item);
                }
            }

            return result;
        }

        /// <summary>
        /// 将泛型集合对象转换为RedisValues[]数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static RedisValue[] RedisValueFactory<T>(this IEnumerable<T> values)
        {
            var result = new List<RedisValue>();

            if (null != values && values.Count() > 0)
            {
                foreach (var v in values)
                {
                    RedisValue val = v.SerializeObject();

                    result.Add(val);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        ///  将对象序列为Json格式字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SerializeObject<T>(this T value)
        {
            return JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// Json字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(this RedisValue value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        #endregion

        #region Redis String字符串

        /// <summary>
        /// 自定义扩展StringGet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T StringGet<T>(this IDatabase db, RedisKey key)
        {
            var r = db.StringGet(key);
            return r.DeserializeObject<T>();
        }

        /// <summary>
        /// 自定义扩展StringGetList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<T> StringGetList<T>(this IDatabase db, RedisKey key)
        {
            return db.StringGet(key).DeserializeObject<List<T>>();
        }

        /// <summary>
        /// 自定义扩展StringSetList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="list"></param>
        public static void StringSetList<T>(this IDatabase db, RedisKey key, List<T> list)
        {
            db.StringSet(key, list);
        }

        /// <summary>
        /// 自定义扩展StringSet
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void StringSet<T>(this IDatabase db, RedisKey key, T value)
        {
            db.StringSet(key, value.SerializeObject());
        }

        #endregion

        #region Redis Hash哈希表

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        public static void HashSet<T>(this IDatabase db, RedisKey key, Dictionary<RedisValue, T> fieldValues, CommandFlags flags = CommandFlags.None)
        {
            if (null != fieldValues && fieldValues.Count > 0)
            {
                var data = new List<HashEntry>();

                foreach (var kv in fieldValues)
                {
                    var jsonValue = kv.Value.SerializeObject();

                    var entry = new HashEntry(kv.Key, jsonValue);

                    data.Add(entry);
                }

                db.HashSet(key, data.ToArray(), flags);
            }
        }

        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        public static Dictionary<RedisValue, T> HashGetAll<T>(this IDatabase db, RedisKey key)
        {
            var values = db.HashGetAll(key);

            var result = new Dictionary<RedisValue, T>();

            var vals = values.OrderBy(x => (double)x.Name).ToList();

            foreach (var v in vals)
            {
                var name = v.Name;
                T item = v.Value.DeserializeObject<T>();

                result.Add(name, item);
            }

            return result;
        }

        #endregion

        #region Redis Set集合

        /// <summary>
        /// 存储数据到Set集合
        /// </summary>
        public static void SetAdd<T>(this IDatabase db, RedisKey key, T member, CommandFlags flags = CommandFlags.None)
        {
            var value = member.SerializeObject();

            db.SetAdd(key, new RedisValue[] { value }, flags);
        }

        /// <summary>
        /// 从Set集合中获取数据
        /// </summary>
        public static List<T> SetMembers<T>(this IDatabase db, RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            var values = db.SetMembers(key, flags);

            return values.RedisValueFactory<T>();
        }

        #endregion

        #region Redis SortedSet有序集合

        /// <summary>
        /// 存储数据到SortedSet有序集合
        /// </summary>
        public static void SortedSetAdd<T>(this IDatabase db, RedisKey key, T member, double score, CommandFlags flags = CommandFlags.None)
        {
            var value = member.SerializeObject();

            db.SortedSetAdd(key, value, score, flags);
        }

        /// <summary>
        /// 存储数据到SortedSet有序集合
        /// </summary>
        public static void SortedSetAdd<T>(this IDatabase db, RedisKey key, Dictionary<T, double> values, CommandFlags flags = CommandFlags.None)
        {
            if (null != values && values.Count > 0)
            {
                var data = new List<SortedSetEntry>();

                foreach (var kv in values)
                {
                    var element = kv.Key.SerializeObject();

                    var entry = new SortedSetEntry(element, kv.Value);

                    data.Add(entry);
                }

                db.SortedSetAdd(key, data.ToArray(), flags);
            }
        }

        /// <summary>
        /// 从SortedSet有序集合中获取数据
        /// </summary>
        /// <returns></returns>
        public static Dictionary<T, double> SortedSetRangeByRankWithScores<T>(this IDatabase db, RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            var values = db.SortedSetRangeByRankWithScores(key, start, stop, order, flags);

            return values.SortedSetEntryFactory<T>();
        }

        /// <summary>
        /// 从SortedSet有序集合中获取数据
        /// </summary>
        public static Dictionary<T, double> SortedSetRangeByScoreWithScores<T>(this IDatabase db, RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            var values = db.SortedSetRangeByScoreWithScores(key, start, stop, exclude, order, skip, take, flags);

            return values.SortedSetEntryFactory<T>();
        }

        /// <summary>
        /// 从SortedSet有序集合中获取数据
        /// </summary>
        /// <returns></returns>
        public static List<T> SortedSetRangeByValue<T>(this IDatabase db, RedisKey key, RedisValue min = default(RedisValue), RedisValue max = default(RedisValue), Exclude exclude = Exclude.None, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            var values = db.SortedSetRangeByValue(key, min, max, exclude, skip, take, flags);

            return values.RedisValueFactory<T>();
        }

        /// <summary>
        /// 从SortedSet有序集合中获取数据
        /// </summary>
        /// <returns></returns>
        public static List<T> SortedSetRangeByScore<T>(this IDatabase db, RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            var values = db.SortedSetRangeByScore(key, start, stop, exclude, order, skip, take, flags);

            return values.RedisValueFactory<T>();
        }

        /// <summary>
        /// 从SortedSet有序集合中获取数据
        /// </summary>
        /// <returns></returns>
        public static List<T> SortedSetRangeByRank<T>(this IDatabase db, RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            var values = db.SortedSetRangeByRank(key, start, stop, order, flags);

            return values.RedisValueFactory<T>();
        }

        /// <summary>
        /// SortedSetEntry集合转换为指定类型的键值集合
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        public static Dictionary<T, double> SortedSetEntryFactory<T>(this SortedSetEntry[] entries)
        {
            var result = new Dictionary<T, double>();

            if (null != entries && entries.Length > 0)
            {
                foreach (var v in entries)
                {
                    var score = v.Score;

                    T item = v.Element.DeserializeObject<T>();

                    result.Add(item, score);
                }
            }
            return result;
        }

        #endregion

        #region Redis List列表

        /// <summary>
        /// 获取列表指定索引位置的数据对象 
        /// </summary>
        /// <returns></returns>
        public static T ListGetByIndex<T>(this IDatabase db, RedisKey key, long index, CommandFlags flags = CommandFlags.None)
        {
            var value = db.ListGetByIndex(key, index, flags);

            return value.DeserializeObject<T>();
        }

        /// <summary>
        /// 在列表指定的对象后面插入数据
        /// </summary>
        /// <returns></returns>
        public static long ListInsertAfter<T>(this IDatabase db, RedisKey key, T pivot, T value, CommandFlags flags = CommandFlags.None)
        {
            return db.ListInsertAfter(key, pivot.SerializeObject(), value.SerializeObject(), flags);
        }

        /// <summary>
        /// 在列表指定的对象后面插入数据
        /// </summary>
        /// <returns></returns>
        public static long ListInsertAfter<T>(this IDatabase db, RedisKey key, RedisValue pivot, T value, CommandFlags flags = CommandFlags.None)
        {
            return db.ListInsertAfter(key, pivot, value.SerializeObject(), flags);
        }

        /// <summary>
        /// 在列表指定对象前面插入数据
        /// </summary>
        /// <returns></returns>
        public static long ListInsertBefore<T>(this IDatabase db, RedisKey key, T pivot, T value, CommandFlags flags = CommandFlags.None)
        {
            return db.ListInsertBefore(key, pivot.SerializeObject(), value.SerializeObject(), flags);
        }

        /// <summary>
        /// 在列表指定对象前面插入数据
        /// </summary>
        /// <returns></returns>
        public static long ListInsertBefore<T>(this IDatabase db, RedisKey key, RedisValue pivot, T value, CommandFlags flags = CommandFlags.None)
        {
            return db.ListInsertBefore(key, pivot, value.SerializeObject(), flags);
        }

        /// <summary>
        /// Removes and returns the first element of the list stored at key.
        /// </summary>
        /// <returns>the value of the first element, or nil when key does not exist.</returns>
        public static T ListLeftPop<T>(this IDatabase db, RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return db.ListLeftPop(key, flags).DeserializeObject<T>();
        }

        /// <summary>
        /// Insert all the specified values at the head of the list stored at key.
        /// If key does not exist, it is created as empty list before performing the push operations.
        /// Elements are inserted one after the other to the head of the list, from the leftmost element to the rightmost element.
        /// So for instance the command LPUSH mylist a  b c will result into a list containing c as first element, b as second element and a as third element.
        /// </summary>
        /// <returns></returns>
        public static long ListLeftPush<T>(this IDatabase db, RedisKey key, IEnumerable<T> values, CommandFlags flags = CommandFlags.None)
        {
            var rvArr = values.RedisValueFactory();

            return db.ListLeftPush(key, rvArr, flags);
        }

        /// <summary>
        /// Insert the specified value at the head of the list stored at key. 
        /// If key does not exist, it is created as empty list before performing the push operations.
        /// </summary>
        /// <returns></returns>
        public static long ListLeftPush<T>(this IDatabase db, RedisKey key, T value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            var rv = value.SerializeObject();

            return db.ListLeftPush(key, rv, when, flags);
        }

        /// <summary>
        /// 获取列表中指定位置范围内的数据集合
        /// </summary>
        /// <returns></returns>
        public static List<T> ListRange<T>(this IDatabase db, RedisKey key, long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None)
        {
            var values = db.ListRange(key, start, stop, flags);

            return values.RedisValueFactory<T>();
        }

        /// <summary>
        /// 从列表中移除指定数据开始起后面指定数量的数据元素，count=0时，移除后面所有
        /// </summary>
        /// <returns></returns>
        public static long ListRemove<T>(this IDatabase db, RedisKey key, T value, long count = 0, CommandFlags flags = CommandFlags.None)
        {
            var rv = value.SerializeObject();

            return db.ListRemove(key, rv, count, flags);
        }

        /// <summary>
        /// Removes and returns the last element of the list stored at key.
        /// </summary>
        /// <returns></returns>
        public static T ListRightPop<T>(this IDatabase db, RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return db.ListRightPop(key, flags).DeserializeObject<T>();
        }

        /// <summary>
        /// Atomically returns and removes the last element (tail) of the list stored at source, and pushes the element at the first element (head) of the list stored at destination.
        /// </summary>
        /// <returns></returns>
        public static T ListRightPopLeftPush<T>(this IDatabase db, RedisKey source, RedisKey destination, CommandFlags flags = CommandFlags.None)
        {
            return db.ListRightPopLeftPush(source, destination, flags).DeserializeObject<T>();
        }

        /// <summary>
        /// Insert all the specified values at the tail of the list stored at key.
        /// </summary>
        /// <returns></returns>
        public static long ListRightPush<T>(this IDatabase db, RedisKey key, IEnumerable<T> values, CommandFlags flags = CommandFlags.None)
        {
            var rvList = values.RedisValueFactory();

            return db.ListRightPush(key, rvList, flags);
        }

        /// <summary>
        /// Insert the specified value at the tail of the list stored at key
        /// If key does not exist, it is created as empty list before performing the push operation.
        /// </summary>
        /// <returns></returns>
        public static long ListRightPush<T>(this IDatabase db, RedisKey key, T value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            return db.ListRightPush(key, value.SerializeObject(), when, flags);
        }

        /// <summary>
        /// Sets the list element at index to value.
        /// For more information on the index argument,see ListGetByIndex. An error is returned for out of range indexes.
        /// </summary>
        public static void ListSetByIndex<T>(this IDatabase db, RedisKey key, long index, T value, CommandFlags flags = CommandFlags.None)
        {
            db.ListSetByIndex(key, index, value.SerializeObject(), flags);
        }

        #endregion
    }
}
