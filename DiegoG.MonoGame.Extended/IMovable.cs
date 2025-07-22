using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace DiegoG.MonoGame.Extended;

public interface ISpacePositionable : IPositionable
{
    public Vector2 RelativePosition { get; set; }
    public ISpace? Space { get; set; }

    public static Vector2 TranslateSpace(Vector2 position, Matrix inverseTransform)
        => Vector2.Transform(position, inverseTransform);

    public static Vector2 ConvertToRelativePosition(ISpacePositionable spacePositionable, Vector2 value)
        => spacePositionable.Space is not null ? Vector2.Transform(value, spacePositionable.Space.Transform) : value;

    public static Vector2 GetRelativePosition(ISpacePositionable positionable)
        => positionable.Space is not null 
            ? TranslateSpace(positionable.Position, positionable.Space.InverseTransform)
            : positionable.Position;
}

public interface ISpace
{
    public Matrix Transform { get; }
    public Matrix InverseTransform => Matrix.Invert(Transform);
}

#region Implementations

public class SpacePositionable : ISpacePositionable
{
    public Vector2 RelativePosition
    {
        get => ISpacePositionable.GetRelativePosition(this);
        set => Position = ISpacePositionable.ConvertToRelativePosition(this, value);
    }

    public Vector2 GetRelativePositionIn(ISpace space)
        => ISpacePositionable.TranslateSpace(Position, space.InverseTransform);

    public Vector2 GetRelativePositionIn(Matrix inverseTransformMatrix)
        => ISpacePositionable.TranslateSpace(Position, inverseTransformMatrix);

    public virtual ISpace? Space { get; set; }
    public virtual Vector2 Position { get; set; }
}

public class SpacePositionableGameComponent(Game game) : GameComponent(game), ISpacePositionable
{
    public Vector2 RelativePosition
    {
        get => ISpacePositionable.GetRelativePosition(this);
        set => Position = ISpacePositionable.ConvertToRelativePosition(this, value);
    }

    public Vector2 GetRelativePositionIn(ISpace space)
        => ISpacePositionable.TranslateSpace(Position, space.InverseTransform);

    public Vector2 GetRelativePositionIn(Matrix inverseTransformMatrix)
        => ISpacePositionable.TranslateSpace(Position, inverseTransformMatrix);

    public virtual ISpace? Space { get; set; }
    public virtual Vector2 Position { get; set; }
}

public class SpacePositionableDrawableGameComponent(Game game) : DrawableGameComponent(game), ISpacePositionable
{
    public Vector2 RelativePosition
    {
        get => ISpacePositionable.GetRelativePosition(this);
        set => Position = ISpacePositionable.ConvertToRelativePosition(this, value);
    }

    public Vector2 GetRelativePositionIn(ISpace space)
        => ISpacePositionable.TranslateSpace(Position, space.InverseTransform);

    public Vector2 GetRelativePositionIn(Matrix inverseTransformMatrix)
        => ISpacePositionable.TranslateSpace(Position, inverseTransformMatrix);

    public virtual ISpace? Space { get; set; }
    public virtual Vector2 Position { get; set; }
}

#endregion