using GLV.Shared.Common;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Timers;

namespace DiegoG.MonoGame.Extended.Tasks;

public class BackgroundTasksWorkerComponent(Game game) : GameComponent(game)
{
    private readonly CountdownTimer Timer = new(0.5);
    
    public override void Update(GameTime gameTime)
    {
        Timer.Update(gameTime);
        if (Timer.State != TimerState.Completed || !BackgroundTaskStore.CheckForCompletedTasks()) return;
        
        Timer.Restart();
        BackgroundTaskStore.Sweep().ConfigureAwait(false).GetAwaiter().GetResult();
    }
}