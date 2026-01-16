using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace DiegoG.MonoGame.Extended;

public interface ISpacePositionable3D : IPositionable3D
{
    public Vector3 RelativePosition { get; set; }
    public ISpace? Space { get; set; }

    public static Vector3 TranslateSpace(Vector3 position, Matrix inverseTransform)
        => Vector3.Transform(position, inverseTransform);

    public static Vector3 ConvertToRelativePosition(ISpacePositionable3D space, Vector3 value)
        => space.Space is not null ? Vector3.Transform(value, space.Space.Transform) : value;

    public static Vector3 GetRelativePosition(ISpacePositionable3D spacePositionable)
        => spacePositionable.Space is not null 
            ? TranslateSpace(spacePositionable.Position, spacePositionable.Space.InverseTransform)
            : spacePositionable.Position;
}

public interface ISpacePositionable2D : IPositionable2D
{
    public Vector2 RelativePosition { get; set; }
    public ISpace? Space { get; set; }

    public static Vector2 TranslateSpace(Vector2 position, Matrix inverseTransform)
        => Vector2.Transform(position, inverseTransform);

    public static Vector2 ConvertToRelativePosition(ISpacePositionable2D spacePositionable, Vector2 value)
        => spacePositionable.Space is not null ? Vector2.Transform(value, spacePositionable.Space.Transform) : value;

    public static Vector2 GetRelativePosition(ISpacePositionable2D positionable)
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
public class SpacePositionable3D : ISpacePositionable3D
{
    public Vector3 RelativePosition
    {
        get => ISpacePositionable3D.GetRelativePosition(this);
        set => Position = ISpacePositionable3D.ConvertToRelativePosition(this, value);
    }

    public Vector3 GetRelativePositionIn(ISpace space)
        => ISpacePositionable3D.TranslateSpace(Position, space.InverseTransform);

    public Vector3 GetRelativePositionIn(Matrix inverseTransformMatrix)
        => ISpacePositionable3D.TranslateSpace(Position, inverseTransformMatrix);

    public virtual ISpace? Space { get; set; }
    public virtual Vector3 Position { get; set; }
}

public class SpacePositionableGameComponent3D(Game game) : GameComponent(game), ISpacePositionable3D
{
    public Vector3 RelativePosition
    {
        get => ISpacePositionable3D.GetRelativePosition(this);
        set => Position = ISpacePositionable3D.ConvertToRelativePosition(this, value);
    }

    public Vector3 GetRelativePositionIn(ISpace space)
        => ISpacePositionable3D.TranslateSpace(Position, space.InverseTransform);

    public Vector3 GetRelativePositionIn(Matrix inverseTransformMatrix)
        => ISpacePositionable3D.TranslateSpace(Position, inverseTransformMatrix);

    public virtual ISpace? Space { get; set; }
    public virtual Vector3 Position { get; set; }
}

public class SpacePositionableDrawableGameComponent3D(Game game) : DrawableGameComponent(game), ISpacePositionable3D
{
    public Vector3 RelativePosition
    {
        get => ISpacePositionable3D.GetRelativePosition(this);
        set => Position = ISpacePositionable3D.ConvertToRelativePosition(this, value);
    }

    public Vector3 GetRelativePositionIn(ISpace space)
        => ISpacePositionable3D.TranslateSpace(Position, space.InverseTransform);

    public Vector3 GetRelativePositionIn(Matrix inverseTransformMatrix)
        => ISpacePositionable3D.TranslateSpace(Position, inverseTransformMatrix);

    public virtual ISpace? Space { get; set; }
    public virtual Vector3 Position { get; set; }
}

public class SpacePositionable2D : ISpacePositionable2D
{
    public Vector2 RelativePosition
    {
        get => ISpacePositionable2D.GetRelativePosition(this);
        set => Position = ISpacePositionable2D.ConvertToRelativePosition(this, value);
    }

    public Vector2 GetRelativePositionIn(ISpace space)
        => ISpacePositionable2D.TranslateSpace(Position, space.InverseTransform);

    public Vector2 GetRelativePositionIn(Matrix inverseTransformMatrix)
        => ISpacePositionable2D.TranslateSpace(Position, inverseTransformMatrix);

    public virtual ISpace? Space { get; set; }
    public virtual Vector2 Position { get; set; }
}

public class SpacePositionableGameComponent2D(Game game) : GameComponent(game), ISpacePositionable2D
{
    public Vector2 RelativePosition
    {
        get => ISpacePositionable2D.GetRelativePosition(this);
        set => Position = ISpacePositionable2D.ConvertToRelativePosition(this, value);
    }

    public Vector2 GetRelativePositionIn(ISpace space)
        => ISpacePositionable2D.TranslateSpace(Position, space.InverseTransform);

    public Vector2 GetRelativePositionIn(Matrix inverseTransformMatrix)
        => ISpacePositionable2D.TranslateSpace(Position, inverseTransformMatrix);

    public virtual ISpace? Space { get; set; }
    public virtual Vector2 Position { get; set; }
}

public class SpacePositionableDrawableGameComponent2D(Game game) : DrawableGameComponent(game), ISpacePositionable2D
{
    public Vector2 RelativePosition
    {
        get => ISpacePositionable2D.GetRelativePosition(this);
        set => Position = ISpacePositionable2D.ConvertToRelativePosition(this, value);
    }

    public Vector2 GetRelativePositionIn(ISpace space)
        => ISpacePositionable2D.TranslateSpace(Position, space.InverseTransform);

    public Vector2 GetRelativePositionIn(Matrix inverseTransformMatrix)
        => ISpacePositionable2D.TranslateSpace(Position, inverseTransformMatrix);

    public virtual ISpace? Space { get; set; }
    public virtual Vector2 Position { get; set; }
}

#endregion