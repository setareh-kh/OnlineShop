using System.Text.Json;
using System.Text.Json.Serialization;
using StackExchange.Redis;

namespace OnlineShop.Services.Services;

public class RedisService(string connectionString, string prefix) : IRedisService
{
    private readonly JsonSerializerOptions _options = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
    };

    private readonly ConnectionMultiplexer _redis = ConnectionMultiplexer.Connect(connectionString);
    private readonly string _prefix = $"{prefix.TrimEnd(':')}:";
    private string BuildKey(string key) => $"{_prefix}{key.TrimStart(':')}";
    public async Task SetValueAsync(string key, dynamic value, TimeSpan? expiration = null)
    {
        var db = _redis.GetDatabase();
        var serializedValue = JsonSerializer.Serialize(value, _options);
        await db.StringSetAsync(BuildKey(key), serializedValue, expiration);
    }

    public void SetValue(string key, dynamic value, TimeSpan? expiration = null)
    {
        var db = _redis.GetDatabase();
        var serializedValue = JsonSerializer.Serialize(value, _options);
        db.StringSet(BuildKey(key), serializedValue, expiration);
    }

    public async Task<T?> GetValueAsync<T>(string key)
    {
        var db = _redis.GetDatabase();
        var serializedValue = await db.StringGetAsync(BuildKey(key));
        if (serializedValue.IsNullOrEmpty)
            return default;
        try
        {
            return JsonSerializer.Deserialize<T>(serializedValue.ToString(), _options);
        }
        catch (JsonException)
        {
            await db.KeyDeleteAsync(BuildKey(key));
            return default;
        }
    }
    public T? GetValue<T>(string key)
    {
        var db = _redis.GetDatabase();
        var serializedValue = db.StringGet(BuildKey(key));

        if (serializedValue.IsNullOrEmpty)
            return default;
        try
        {
            return JsonSerializer.Deserialize<T>(serializedValue.ToString(), _options);
        }
        catch (JsonException)
        {
            db.KeyDelete(BuildKey(key));
            return default;
        }
    }
    public async Task<bool> KeyExistsAsync(string key)
    {
        var db = _redis.GetDatabase();
        return await db.KeyExistsAsync(BuildKey(key));
    }

    public bool KeyExists(string key)
    {
        var db = _redis.GetDatabase();
        return db.KeyExists(BuildKey(key));
    }

    public async Task<bool> DeleteKeyAsync(string key)
    {
        var db = _redis.GetDatabase();
        return await db.KeyDeleteAsync(BuildKey(key));
    }

    public bool DeleteKey(string key)
    {
        var db = _redis.GetDatabase();
        return db.KeyDelete(BuildKey(key));
    }

    public async Task DeleteKeysAsync(string[] keys)
    {
        if (keys.Length == 0)
            return;

        var db = _redis.GetDatabase();

        var redisKeys = keys
            .Select(key => (RedisKey)BuildKey(key))
            .ToArray();

        await db.KeyDeleteAsync(redisKeys);
    }

    public void DeleteKeys(string[] keys)
    {
        if (keys.Length <= 0) return;
        var db = _redis.GetDatabase();
        var redisKeys = Array.ConvertAll(keys, key => (RedisKey)BuildKey(key));
        db.KeyDelete(redisKeys);
    }
    /*public async Task<List<string>> GetKeysByPrefixV0Async(string prefix)
    {
        var db = _redis.GetDatabase();
        var keys = new List<string>();

        var endpoints = _redis.GetEndPoints();

        foreach (var endpoint in endpoints)
        {
            var server = _redis.GetServer(endpoint);

            await foreach (var key in server.KeysAsync(database: db.Database, pattern: $"{prefix}*"))
            {
                keys.Add(key.ToString());
            }
        }

        return keys;
    }*/
    public async Task<List<string>> GetKeysByPrefixAsync(string prefix = "")
{
    var db = _redis.GetDatabase();

    var keys = new HashSet<string>();

    var pattern = $"{BuildKey(prefix)}*";

    foreach (var endpoint in _redis.GetEndPoints())
    {
        var server = _redis.GetServer(endpoint);

        await foreach (var key in server.KeysAsync(
            database: db.Database,
            pattern: pattern))
        {
            var logicalKey = key.ToString()[_prefix.Length..];

            keys.Add(logicalKey);
        }
    }

    return keys.ToList();
}

    /*public Dictionary<string, string> KeyValuesV0(string pattern)
    {
        var db = _redis.GetDatabase();
        var server = _redis.GetServer(db.Multiplexer.GetEndPoints()[0]);
        var keys = server.Keys(-1, BuildKey(pattern));

        var results = new Dictionary<string, string>();

        foreach (var key in keys)
        {
            var value = db.StringGet(key);
            results.Add(key!, value!);
        }

        return results;
    }*/
    public Dictionary<string, string> KeyValues(string pattern)
    {
        var db = _redis.GetDatabase();

        var server = _redis.GetServer(
            _redis.GetEndPoints()[0]);

        var results = new Dictionary<string, string>();

        foreach (var key in server.Keys(
            database: db.Database,
            pattern: BuildKey(pattern)))
        {
            var value = db.StringGet(key);

            if (value.HasValue)
            {
                var logicalKey = key.ToString()[_prefix.Length..];

                results[logicalKey] = value.ToString();
            }
        }

        return results;
    }
}