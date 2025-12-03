using System.Text;
using GLV.Shared.Common.Collections;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;

namespace DiegoG.MonoGame.Extended;

public static class GameServiceExtensions
{
    extension(Game game)
    {
        public ObjectPool<T>.PooledHelper GetPooledSafe<T>(out T obj) where T : class
            => game.Services.GetPooledSafe<T>(out obj);

        public ObjectPool<T>.PooledHelper GetPooledSafe<T>() where T : class
            => game.Services.GetPooledSafe<T>();

        public T GetPooled<T>() where T : class
            => game.Services.GetPooled<T>();
    }

    extension(GameServiceContainer services)
    {
        public ObjectPool<T>.PooledHelper GetPooledSafe<T>() where T : class
        {
            var pool = services.GetService<ObjectPool<T>>();
            return pool.ObtainSafe();
        }

        public void AddPool<T>(ObjectPool<T> pool
        ) where T : class
        {
            services.AddService(pool);
        }

        public void AddStringBuilderPool()
            => services.AddService(StringBuilderPool.Shared);

        public ObjectPool<T>.PooledHelper GetPooledSafe<T>(out T obj) where T : class
        {
            var pool = services.GetService<ObjectPool<T>>();
            return pool.ObtainSafe(out obj);
        }

        public T GetPooled<T>() where T : class
        {
            var pool = services.GetService<ObjectPool<T>>();
            return pool.Obtain();
        }
    }
}
