namespace Portfolio.Api.Services;

public class CacheService
{
    private Dictionary<string, object?> cache = new Dictionary<string, object?>();

    public void Set<T>(string set, T dat)
    {
        cache[set] = dat;
    }

    public bool TryGetValue<T>(string key, out T dat)
    {
        if (cache.TryGetValue(key, out object? o))
        {
            dat = (T)o;
            return true;
        }

        dat = default;
        return false;
    }

    public bool Remove(string key)
        => cache.Remove(key);

    public void Clear()
    {
        cache.Clear();
    }
}