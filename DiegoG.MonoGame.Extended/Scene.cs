using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace DiegoG.MonoGame.Extended;

public class Scene : DrawableGameComponent
{
    public GameComponentCollection SceneComponents { get; } = [];
    private readonly List<IDrawable> drawables = [];
    private readonly List<IUpdateable> updateables = [];

    public Scene(Game game) : base(game)
    {
        SceneComponents.ComponentAdded += SceneComponentsOnComponentAdded;
        SceneComponents.ComponentRemoved += SceneComponentsOnComponentRemoved;
    }
    
    protected override void LoadContent()
    {
        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        lock (updateables)
        {
            for (var index = 0; index < updateables.Count; index++)
            {
                var dr = updateables[index];
                if (dr.Enabled)
                    dr.Update(gameTime);
            }  
        }
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        lock (drawables)
        {
            for (var index = 0; index < drawables.Count; index++)
            {
                var dr = drawables[index];
                if (dr.Visible)
                    dr.Draw(gameTime);
            }  
        }
    }

    protected override void Dispose(bool disposing)
    {
        SceneComponents.ComponentAdded -= SceneComponentsOnComponentAdded;
        SceneComponents.ComponentRemoved -= SceneComponentsOnComponentRemoved;
        
        for (int index = 0; index < SceneComponents.Count; index++)
        {
            if (this.SceneComponents[index] is IDisposable component)
                component.Dispose();
        }
        
        SceneComponents.Clear();
        // ReSharper disable InconsistentlySynchronizedField
        drawables.Clear();
        updateables.Clear();
        
        base.Dispose(disposing);
    }

    #region Sorting and event handling
    
    private void SceneComponentsOnComponentRemoved(object? sender, GameComponentCollectionEventArgs e)
    {
        var comp = e.GameComponent;
        lock (drawables)
            if (comp is IDrawable dr)
            {
                drawables.Remove(dr);
                dr.DrawOrderChanged -= OnDrawOrderChanged;
            }

        lock (updateables)
            if (comp is IUpdateable up)
            {
                updateables.Remove(up);
                up.UpdateOrderChanged -= UpOnUpdateOrderChanged;
            }
        
        if (comp is IDisposable disp)
            disp.Dispose();
    }
    
    private void SceneComponentsOnComponentAdded(object? sender, GameComponentCollectionEventArgs e)
    {
        var comp = e.GameComponent;
        lock (drawables)
            if (comp is IDrawable dr)
            {
                AddSortedDrawable(dr);
                dr.DrawOrderChanged += DrOnDrawOrderChanged; 
            }

        lock (updateables)
            if (comp is IUpdateable up)
            {
                AddSortedUpdateable(up);
                up.UpdateOrderChanged += UpOnUpdateOrderChanged;
            }
        
        comp.Initialize();
    }

    private void DrOnDrawOrderChanged(object? sender, EventArgs e)
    {
        Debug.Assert(sender is IDrawable);
        MoveSortedDrawable((IDrawable)sender);
    }

    private void UpOnUpdateOrderChanged(object? sender, EventArgs e)
    {
        Debug.Assert(sender is IUpdateable);
        MoveSortedUpdateable((IUpdateable)sender);
    }

    private void MoveSortedDrawable(IDrawable item)
    {
        lock (drawables)
        {
            drawables.Remove(item);
            AddSortedDrawable(item);
        }
    }
    
    private void AddSortedDrawable(IDrawable item)
    {
        lock (drawables)
        {
            if (drawables.Count == 0 || drawables[^1].DrawOrder.CompareTo(item.DrawOrder) <= 0)
            {
                drawables.Add(item);
                return;
            }

            if (drawables[0].DrawOrder.CompareTo(item.DrawOrder) >= 0)
            {
                drawables.Insert(0, item);
                return;
            }
            
            var index = drawables.BinarySearch(item);
            if (index < 0) 
                index = ~index;
            
            drawables.Insert(index, item);
        }
    }
    
    private void MoveSortedUpdateable(IUpdateable item)
    {
        lock (updateables)
        {
            updateables.Remove(item);
            AddSortedUpdateable(item);
        }
    }
    
    private void AddSortedUpdateable(IUpdateable item)
    {
        lock (updateables)
        {
            if (updateables.Count == 0 || updateables[^1].UpdateOrder.CompareTo(item.UpdateOrder) <= 0)
            {
                updateables.Add(item);
                return;
            }

            if (updateables[0].UpdateOrder.CompareTo(item.UpdateOrder) >= 0)
            {
                updateables.Insert(0, item);
                return;
            }
            
            var index = updateables.BinarySearch(item, GameComponentOrderComparer.Instance);
            if (index < 0) 
                index = ~index;
            
            updateables.Insert(index, item);
        }
    }
    
    #endregion
}