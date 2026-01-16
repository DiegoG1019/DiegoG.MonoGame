using GLV.Shared.Common;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Timers;

namespace DiegoG.MonoGame.Extended.Tasks;

public class BackgroundTasksWorkerComponent(Game game, CancellationToken ct) : GameComponent(game)
{
    private readonly CountdownTimer timer = new(0.5);
    private Task? sweep;
    
    public override void Update(GameTime gameTime)
    {
        if (ct.IsCancellationRequested)
            BackgroundTaskStore.Disable();

        if (sweep is not null)
        {
            if (sweep.IsCompleted)
            {
#pragma warning disable VSTHRD002 // The task is already completed
                sweep.ConfigureAwait(false).GetAwaiter().GetResult();
                sweep = null;
            }
            else return;
        }
        
        timer.Update(gameTime);
        
        if (timer.State != TimerState.Completed) return;
        timer.Restart();
        
        if (BackgroundTaskStore.CompletedSingleFireTasks == 0) return;
        sweep = BackgroundTaskStore.SweepAsync();
    }
}