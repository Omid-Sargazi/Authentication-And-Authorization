using System.Threading.Tasks;
using StackExchange.Redis;

namespace RedisDay4.Day4
{
    public class DeleteKey
    {
        public static async Task Run()
        {
            Console.WriteLine("Hello Redissss");


            var redis = await ConnectionMultiplexer.ConnectAsync("localhost");
            var db = redis.GetDatabase();

            string key = "opt:123456789";
            string value = "418752";

            await db.StringSetAsync(key, value, TimeSpan.FromSeconds(30));
            Console.WriteLine("OTP saved with time = 30s");

            await db.StringSetAsync("token:admin", "Bearer abc 123", TimeSpan.FromHours(1));

            TimeSpan? ttlBearer = await db.KeyTimeToLiveAsync("token:admin");
            Console.WriteLine($"{ttlBearer?.TotalMinutes}+ is remain to expire");

            await db.KeyPersistAsync("token:admin");

            string otp = await db.StringGetAsync(key);
            Console.WriteLine($"Current OPT:{otp}");

            TimeSpan? ttl = await db.KeyTimeToLiveAsync(key);
            Console.WriteLine($"remind time{ttl?.TotalSeconds} second");

            Console.WriteLine("⏳ waiting to delete key...");
            await Task.Delay(40000);

            var expiredOtp = await db.StringGetAsync(key);
            Console.WriteLine($"is there key {(expiredOtp.IsNullOrEmpty ? "❌ No" : "✅ yes")}");
            redis.Dispose();
           
           
        }
    }
}