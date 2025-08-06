/*

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DiegoG.MonoGame.Extended.RenderSteps;

public interface IWorldDrawable
{
    int WorldDrawOrder { get; }

    bool WorldVisible { get; }

    event Action<IWorldDrawable> WorldDrawOrderChanged;

    void DrawOnWorld(GameTime gameTime, StatefulSpriteBatch spriteBatch, RenderStepManager manager);
}

public interface IUIDrawable
{
    int UIDrawOrder { get; }

    bool UIVisible { get; }

    event Action<IUIDrawable> UIDrawOrderChanged;

    void DrawOnUI(GameTime gameTime, StatefulSpriteBatch spriteBatch, RenderStepManager manager);
}

public interface IHUDDrawable
{
    int HUDDrawOrder { get; }

    bool HUDVisible { get; }

    event Action<IHUDDrawable> HUDDrawOrderChanged;

    void DrawOnWorld(GameTime gameTime, StatefulSpriteBatch spriteBatch, RenderStepManager manager);
}

public interface IBackgroundDrawable
{
    int BackgroundDrawOrder { get; }

    bool BackgroundVisible { get; }

    event Action<IBackgroundDrawable> BackgroundDrawOrderChanged;

    void DrawOnBackground(GameTime gameTime, StatefulSpriteBatch spriteBatch, RenderStepManager manager);
}

public class RenderStepManager : DrawableGameComponent
{
    private readonly GameComponentCollection componentCollection;
    
    
    public RenderStepManager(Game game) : this(game, game.Components) { }
    public RenderStepManager(Scene scene) : this(scene.Game, scene.SceneComponents) { }

    private RenderStepManager(Game game, GameComponentCollection componentCollection) : base(game)
    {
        componentCollection.ComponentAdded += ComponentCollectionOnComponentAdded;
        componentCollection.ComponentRemoved += ComponentCollectionOnComponentRemoved;
        this.componentCollection = componentCollection;
        
        BackgroundSpriteBatch = new(GraphicsDevice);
        UISpriteBatch = new(GraphicsDevice);
        HUDSpriteBatch = new(GraphicsDevice);
        WorldSpriteBatch = new(GraphicsDevice)
        {
            SamplerState = SamplerState.PointClamp
        };
    }
    
    public StatefulSpriteBatch WorldSpriteBatch { get; }

    public StatefulSpriteBatch BackgroundSpriteBatch { get; }

    public StatefulSpriteBatch UISpriteBatch { get; }

    public StatefulSpriteBatch HUDSpriteBatch { get; }
    
    protected virtual void ExecuteRenderSteps()
    {
        
    }

    protected override void UnloadContent()
    {
        componentCollection.ComponentAdded -= ComponentCollectionOnComponentAdded;
        componentCollection.ComponentRemoved -= ComponentCollectionOnComponentRemoved;
    }
    
    private void ComponentCollectionOnComponentRemoved(object? sender, GameComponentCollectionEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void ComponentCollectionOnComponentAdded(object? sender, GameComponentCollectionEventArgs e)
    {
        throw new NotImplementedException();
    }
}

*/