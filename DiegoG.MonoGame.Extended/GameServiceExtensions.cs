using Microsoft.Xna.Framework;

namespace DiegoG.MonoGame.Extended;

public static class GameServiceExtensions
{
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
