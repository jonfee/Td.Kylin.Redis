using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Td.Kylin.Redis;

namespace ConsoleApp1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            RedisInjection.UseRedis("127.0.0.1:6379,password=kylinjonfee++");

            var db = RedisManager.Redis.GetDatabase();

            int i = 0, j = 0;
            System.Threading.Timer writeTimer = null;

            writeTimer = new System.Threading.Timer((obj) =>
              {
                  db.StringSet("jonfee" + i, i);
                  Console.WriteLine(string.Format("jonfee{0}，写入时间为 {1}", i, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fff")));

                  if (++i == 50) writeTimer.Change(Timeout.Infinite, Timeout.Infinite);
              }, null, 0, 100);

            System.Threading.Timer readTimer = null;

            readTimer = new System.Threading.Timer((obj) =>
              {
                  var val = db.StringGet("jonfee" + j);
                  Console.WriteLine(string.Format("jonfee{0}的值为{1}，读出时间为 {2}", j, val, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fff")));

                  if (++j == 50) readTimer.Change(Timeout.Infinite, Timeout.Infinite);
              }, null, 1000, 200);

            Console.ReadKey();
        }
    }
}
