namespace OnlineShop.Services;

public interface IRedisService
{
    Task SetValueAsync(string key, dynamic value, TimeSpan? expiration = null);
    void SetValue(string key, dynamic value, TimeSpan? expiration = null);
    Task<T?> GetValueAsync<T>(string key);
    T? GetValue<T>(string key);
    Task<bool> KeyExistsAsync(string key);
    bool KeyExists(string key);
    Task<bool> DeleteKeyAsync(string key);
    bool DeleteKey(string key);
    Task DeleteKeysAsync(string[] keys);
    void DeleteKeys(string[] keys);
    Task<List<string>> GetKeysByPrefixAsync(string prefix);
    Dictionary<string, string> KeyValues(string pattern);
}