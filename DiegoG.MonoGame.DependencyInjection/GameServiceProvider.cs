using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;

namespace DiegoG.MonoGame.DependencyInjection;

public class GameServiceProvider : IServiceProvider, IKeyedServiceProvider, IDisposable, IAsyncDisposable
{
    public GameServiceProvider(Game game, Action<ServiceCollection>? configureServices)
    {
        var collection = new ServiceCollection();
        collection.AddSingleton(game);
        configureServices?.Invoke(collection);
        Services = collection.BuildServiceProvider();
        Game = game;
    }
    
    public Game Game { get; }
    public ServiceProvider Services { get; }

    public object? GetService(Type serviceType)
        => Services.GetService(serviceType) ?? Game.Services.GetService(serviceType);

    public object? GetKeyedService(Type serviceType, object? serviceKey)
        => Services.GetKeyedService(serviceType, serviceKey);

    public object GetRequiredKeyedService(Type serviceType, object? serviceKey)
        => Services.GetRequiredKeyedService(serviceType, serviceKey);

    public void Dispose()
    {
        Services.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await Services.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}