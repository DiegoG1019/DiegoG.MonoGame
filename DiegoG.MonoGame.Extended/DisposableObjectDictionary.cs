using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace DiegoG.MonoGame.Extended;

public sealed class DisposableObjectDictionary<T> : IReadOnlyDictionary<string, T>, IReadOnlyCollection<T>, IDisposable
    where T : IDisposable
{
    private readonly Dictionary<string, T> dict = [];
    private readonly HashSet<T> set = [];

    public void Add(string key, T value)
    {
        if (set.Add(value))
            throw new ArgumentException("This object has already been added under a different key", nameof(value));
        dict.Add(key, value);
    }

    public bool RemoveWithoutDisposing(string key)
    {
        if (dict.Remove(key, out var value))
        {
            set.Remove(value);
            return true;
        }

        return false;
    }

    public bool Remove(string key)
    {
        if (dict.Remove(key, out var value))
        {
            set.Remove(value);
            value.Dispose();
            return true;
        }

        return false;
    }

    public void ClearWithoutDisposing()
    {
        dict.Clear();
        set.Clear();
    }

    public void Clear()
    {
        var values = dict.Values.ToList();
        dict.Clear();
        set.Clear();

        List<Exception>? dispatchInfos = null;
        for (int i = 0; i < values.Count; i++)
        {
            try
            {
                values[i].Dispose();
            }
            catch (Exception e)
            {
                (dispatchInfos ??= []).Add(e);
            }
        }

        if (dispatchInfos is not null)
            throw new AggregateException("One or more exceptions were thrown whilst attempting to dispose of this collection's items during a Clear operation", dispatchInfos);
    }

    public bool ContainsKey(string key) => dict.ContainsKey(key);

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out T value) => dict.TryGetValue(key, out value);

    public T this[string key] { get => dict[key]; }

    public IEnumerable<string> Keys => dict.Keys;

    public IEnumerable<T> Values => dict.Values;

    public int Count => dict.Count;

    public IEnumerator<KeyValuePair<string, T>> GetEnumerator() => dict.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => dict.GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => dict.Values.GetEnumerator();

    public void Dispose()
    {
        Clear();
    }
}
