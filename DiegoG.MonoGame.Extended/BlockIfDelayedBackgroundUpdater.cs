using Microsoft.Xna.Framework;

namespace DiegoG.MonoGame.Extended;

public class BlockIfDelayedBackgroundUpdater : BackgroundUpdater
{
    public BlockIfDelayedBackgroundUpdater(int updateDelayFramesBeforeBlocking) 
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(updateDelayFramesBeforeBlocking, 0);
        UpdateDelayFramesBeforeBlock = updateDelayFramesBeforeBlocking;
    }
    
    public int UpdateDelayFramesBeforeBlock { get; }
    public int FramesOfDelay { get; private set; }
    public event Action<BlockIfDelayedBackgroundUpdater>? WorkDelayed;

    protected override bool CheckTaskAndAwaitIfReady(Task task)
    {
        if (task.IsCompleted)
        {
            task.ConfigureAwait(false).GetAwaiter().GetResult();
            return true;
        }
        else if (FramesOfDelay++ >= UpdateDelayFramesBeforeBlock)
        {
            WorkDelayed?.Invoke(this);
            task.ConfigureAwait(false).GetAwaiter().GetResult();
            return true;
        }
        
        return false;
    }
}