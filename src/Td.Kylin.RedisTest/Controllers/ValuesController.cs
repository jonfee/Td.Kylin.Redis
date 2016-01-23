using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Td.Kylin.Redis;
using System.Text;
using StackExchange.Redis;

namespace Td.Kylin.RedisTest.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("redis")]
        public IActionResult GetRedis()
        {
            var redis = RedisManager.Redis;

            var db = redis.GetDatabase();

            db.StringSet("jonfee", 10000);

            var value = db.StringGet("jonfee");

            List<UserModel> users = new List<UserModel>
            {
                new UserModel {UserID=1,Name="jonfee",Sex="男" },
                new UserModel {UserID=2,Name="yangjj",Sex="女" },
                new UserModel {UserID=3,Name="lwb",Sex="男" },
                new UserModel {UserID=4,Name="yuanke",Sex="女" },
            };

            db.StringSet("Users", users);

            var redisUsers = db.StringGet<List<UserModel>>("Users");

            RedisKey key = this.Me();
            db.KeyDelete(key);

            Dictionary<RedisValue, UserModel> userDic = new Dictionary<RedisValue, UserModel>();
            users.ForEach((item) =>
            {
                userDic.Add(item.UserID.ToString(), item);
            });

            db.HashSet(key, userDic);

           var vals = db.HashGetAll(key);

            var vals2 = db.HashGetAll<UserModel>(key);

            return Ok(value);
        }

        public class UserModel
        {
            public int UserID { get; set; }

            public string Name { get; set; }

            public string Sex { get; set; }
        }
    }
}
