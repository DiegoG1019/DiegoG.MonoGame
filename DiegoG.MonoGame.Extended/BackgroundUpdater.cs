using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace DiegoG.MonoGame.Extended;

/// <summary>
/// When updated, the assigned <see cref="IUpdateable"/> or <see cref="IAsyncUpdateable"/> will be executed through <see cref="Task.Run(Action)"/>; 
/// when this happens, the component will wait until the update finishes before running a new one. If <see cref="Update"/> is called again before a previous task finishes, it will simply ignore the call; the updateable will only see the GameTime it was launched with; which is a copy of the original one it was launched with 
/// </summary>
public class BackgroundUpdater : IUpdateable
{
    public BackgroundUpdater() { }

    private readonly GameTime gameTime = new();
    protected Task? Task;
    private object? upd;

    public void SetUpdatable(IUpdateable updatable) => upd = updatable;
    public void SetUpdatable(IAsyncUpdateable updatable) => upd = updatable;

    /// <summary>
    /// Gets the assigned updatable to this BackgroundUpdater. If it's a regular IUpdatable, updatable will be populated;
    /// while asyncUpdatable will be populated if it's an IAsyncUpdatable instead.
    /// Both will be null if there's no updatable currently assigned
    /// </summary>
    public void GetUpdatable(out IUpdateable? updateable, out IAsyncUpdateable? asyncUpdatable)
    {
        switch (upd)
        {
            case IUpdateable syncup:
                updateable = syncup;
                asyncUpdatable = null;
                break;
            case IAsyncUpdateable asyncup:
                asyncUpdatable = asyncup;
                updateable = null;
                break;
            default:
                asyncUpdatable = null;
                updateable = null;
                break;
        }
    }

    public void Update(GameTime gt)
    {
        if (Task is Task t)
            if (CheckTaskAndAwaitIfReady(Task) is false)
                return;
            else
                Task = null;

        var ta = LaunchUpdate(gt);
        if (ta is null) return;
        Task = ta;
    }

    public bool Enabled
    {
        get;
        set
        {
            if (value == field) return;
            field = value;
            EnabledChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    
    public int UpdateOrder 
    {
        get;
        set
        {
            if (value == field) return;
            field = value;
            UpdateOrderChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    
    public event EventHandler<EventArgs>? EnabledChanged;
    
    public event EventHandler<EventArgs>? UpdateOrderChanged;

    protected virtual bool CheckTaskAndAwaitIfReady(Task task)
    {
        if (task.IsCompleted)
        {
            Task = null;
            task.ConfigureAwait(false).GetAwaiter().GetResult();
            return true;
        }
        else return false;
    }
    
    protected virtual Task? LaunchUpdate(GameTime gt)
    {
        if (upd is null) return null;

        gameTime.TotalGameTime = gt.TotalGameTime;
        gameTime.ElapsedGameTime = gt.ElapsedGameTime;
        gameTime.IsRunningSlowly = gt.IsRunningSlowly;
        return upd switch
        {
            IUpdateable su => Task.Run(() => su.Update(gameTime)),
            IAsyncUpdateable au => Task.Run(() => au.UpdateAsync(gameTime)),
            _ => Task
        };
    }
}