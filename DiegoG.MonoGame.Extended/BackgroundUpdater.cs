using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace DiegoG.MonoGame.Extended;

public class BackgroundUpdater : GameComponent
{
    protected BackgroundUpdater(Game game, IUpdateable updateable) : base(game)
    {
        Updateable = updateable ?? throw new ArgumentNullException(nameof(updateable));
    }

    protected readonly GameTime GameTime = new();
    protected readonly Stopwatch Stopwatch = new();
    protected readonly Lock Sync = new();
    protected Task? Task;
    
    public IUpdateable Updateable { get; }

    public override void Update(GameTime gameTime)
    {
        if (Updateable.Enabled is false) return;
        
        lock (Sync)
        {
            if (Task is not null)
            {
                if (Task.IsCompleted)
                {
                    Task.ConfigureAwait(false).GetAwaiter().GetResult();
                    Task = null;
                }
                else return;
            }
            
            GameTime.TotalGameTime = gameTime.TotalGameTime;
            GameTime.ElapsedGameTime = Stopwatch.Elapsed;
            Stopwatch.Restart();
            GameTime.IsRunningSlowly = gameTime.IsRunningSlowly;
                
            Task = Task.Run(() =>
            {
                // ReSharper disable once InconsistentlySynchronizedField
                Updateable.Update(GameTime);
            });
        }
    }

    public static BackgroundUpdater Create(Game game, IUpdateable updateable)
        => new(game, updateable);

    public static BackgroundUpdater Create(Game game, IUpdateable updateable, int delayFramesBeforeBlocking)
        => new BlockIfDelayedBackgroundUpdater(game, updateable, delayFramesBeforeBlocking);
}