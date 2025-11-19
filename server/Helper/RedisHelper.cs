using StackExchange.Redis;
using System.Text.Json;

namespace server.Helper
{
    public class RedisHelper
    {
        // Singleton instance
        private static readonly Lazy<RedisHelper> lazyInstance = new Lazy<RedisHelper>(() => new RedisHelper());

        public static RedisHelper Instance => lazyInstance.Value;

        private readonly IDatabase _db;

        // Private constructor to prevent instantiation
        private RedisHelper()
        {
            var connectionString = "localhost:6789"; // có thể inject IConfiguration nếu cần
            var redis = ConnectionMultiplexer.Connect(connectionString);
            _db = redis.GetDatabase();
        }

        #region Set Value
        public bool Set<T> (string key, T value, TimeSpan? expiry = null)
        {
            var json = JsonSerializer.Serialize(value);
            return _db.StringSet(key, json, expiry);
        }
        #endregion

        #region Get Value
        public T? Get<T>(string key)
        {
            var value = _db.StringGet(key);
            if (value.IsNullOrEmpty)
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(value);
        }
        #endregion

        #region Remove Key
        public bool Remove(string key)
        {
            return _db.KeyDelete(key);
        }
        #endregion

        #region Check key exists
        public bool Exists(string key)
        {
            return _db.KeyExists(key);
        }
        #endregion
    }
}
