using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DiegoG.MonoGame.Extended;

public class MousePositionableComponent : GameComponent, ISpacePositionable
{
    public MousePositionableComponent(Game game) : base(game) { }

    public GameWindow? GameWindow { get; init; }
    private Vector2 lastReadPos;

    public static Vector2 GetMousePosition(GameWindow? gameWindow)
        => (gameWindow is null ? Mouse.GetState() : Mouse.GetState(gameWindow)).Position.ToVector2();

    public static Vector2 GetRelativePositionIn(ISpace space, GameWindow? gameWindow = null)
        => ISpacePositionable.TranslateSpace(GetMousePosition(gameWindow), space.InverseTransform);

    public static Vector2 GetRelativePositionIn(Matrix inverseTransform, GameWindow? gameWindow = null)
        => ISpacePositionable.TranslateSpace(GetMousePosition(gameWindow), inverseTransform);

    public Vector2 Position
    {
        get => lastReadPos;
        set
        {
            lastReadPos = value;
            Mouse.SetPosition((int)value.X, (int)value.Y);
        }
    }

    public Vector2 RelativePosition
    {
        get => ISpacePositionable.GetRelativePosition(this);
        set => Position = ISpacePositionable.ConvertToRelativePosition(this, value);
    }

    public ISpace? Space { get; set; }

    public override void Update(GameTime gameTime)
    {
        lastReadPos = (GameWindow is null ? Mouse.GetState() : Mouse.GetState(GameWindow)).Position.ToVector2();
    }
}