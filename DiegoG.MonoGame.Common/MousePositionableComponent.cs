using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DiegoG.MonoGame.Common;

public class MousePositionableComponent(Game game) : GameComponent(game), ISpacePositionable
{
    public GameWindow? GameWindow { get; init; }
    public MouseState LastState { get; private set; }

    public Vector2 Position
    {
        get => (GameWindow is null ? Mouse.GetState() : Mouse.GetState(GameWindow)).Position.ToVector2();
        set => Mouse.SetPosition((int)value.X, (int)value.Y);
    }

    public Vector2 AbsolutePosition
    {
        get => ISpacePositionable.GetAbsolutePosition(this);
        set => Position = ISpacePositionable.ConvertToAbsolutePosition(this, value);
    }

    public ISpace? Space { get; set; }
}