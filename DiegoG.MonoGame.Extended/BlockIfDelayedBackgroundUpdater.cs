using Microsoft.Xna.Framework;

namespace DiegoG.MonoGame.Extended;

public class BlockIfDelayedBackgroundUpdater : BackgroundUpdater
{
    public BlockIfDelayedBackgroundUpdater(Game game, IUpdateable updateable, int delayFramesBeforeBlocking) : base(game, updateable)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(delayFramesBeforeBlocking, 0);
        DelayFramesBeforeBlock = delayFramesBeforeBlocking;
    }
    
    public int DelayFramesBeforeBlock { get; }
    public int FramesOfDelay { get; private set; }
    public event Action<BlockIfDelayedBackgroundUpdater>? WorkDelayed;

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
                else if (FramesOfDelay++ >= DelayFramesBeforeBlock)
                {
                    WorkDelayed?.Invoke(this);
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
}