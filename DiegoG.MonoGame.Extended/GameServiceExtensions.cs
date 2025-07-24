using System.Text;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;

namespace DiegoG.MonoGame.Extended;

public readonly struct PooledHelper<T> : IDisposable where T : class
{
    private readonly T pooled;
    private readonly Pool<T> pool;
    
    internal PooledHelper(T pooled, Pool<T> pool)
    {
        this.pooled = pooled;
        this.pool = pool;
    }
    
    public void Dispose()
    {
        pool.Free(pooled);
    }
} 

public static class GameServiceExtensions
{
    public static void AddPool<T>(
        this GameServiceContainer services, 
        Func<T> createItem, 
        Action<T>? resetItem = null, 
        int capacity = 16,
        int maximum = int.MaxValue
    ) where T : class
    {
        services.AddService(typeof(T) == typeof(StringBuilder)
            ? new Pool<T>(createItem, resetItem ?? (sb => ((StringBuilder)(object)sb).Clear()), capacity, maximum)
            : new Pool<T>(createItem, resetItem ?? (_ => { }), capacity, maximum));
    }

    public static PooledHelper<T> GetPooled<T>(this Game game, out T obj) where T : class
        => game.Services.GetPooled<T>(out obj);

    public static PooledHelper<T> GetPooled<T>(this GameServiceContainer services, out T obj) where T : class
    {
        var pool = services.GetService<Pool<T>>();
        obj = pool.Obtain();
        return new PooledHelper<T>(obj, pool);
    }

    public static T GetService<T>(this Game game) where T : class
        => game.Services.GetService<T>();

    public static T GetInstanceService<T>(this Game game)
        => game.Services.GetInstanceService<T>();

    public static void AddInstanceService<T>(this Game game, Func<GameServiceContainer, T> provider)
        => game.Services.AddInstanceService(provider);

    public static T GetInstanceService<T>(this GameServiceContainer services)
        => services.GetService<Func<GameServiceContainer, T>>().Invoke(services);

    public static void AddInstanceService<T>(this GameServiceContainer services, Func<GameServiceContainer, T> provider)
    {
        ArgumentNullException.ThrowIfNull(provider);
        services.AddService(provider);
    }
}
