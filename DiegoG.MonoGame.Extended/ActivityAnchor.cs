using MonoGame.Extended;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace DiegoG.MonoGame.Extended;

public class ActivityAnchor : IPositionable2D
{
    private float radius;
    private float radiusSquared;

    private readonly Func<IPositionable2D, bool> GetObjectsInRangePredicate;
    private readonly Func<IPositionable2D, bool> GetObjectsNotInRangePredicate;
    private bool GetObjectsNotInRangePredicateMethod(IPositionable2D positionable)
        => Vector2.DistanceSquared(Position, positionable.Position) > radiusSquared;

    private bool GetObjectsInRangePredicateMethod(IPositionable2D positionable)
        => Vector2.DistanceSquared(Position, positionable.Position) <= radiusSquared;

    public ActivityAnchor()
    {
        GetObjectsInRangePredicate = GetObjectsInRangePredicateMethod;
        GetObjectsNotInRangePredicate = GetObjectsNotInRangePredicateMethod;
    }

    public Vector2 Position { get; set; }

    public float Radius
    {
        get => radius;
        set
        {
            radius = value;
            radiusSquared = value * value;
        }
    }

    public float RadiusSquared
    {
        get => radiusSquared;
        set
        {
            radiusSquared = value;
            radius = float.Sqrt(value);
        }
    }

    public IEnumerable<IPositionable2D> GetObjectsInRange(IEnumerable<IPositionable2D> positionables)
        => positionables.Where(GetObjectsInRangePredicate);

    public IEnumerable<IPositionable2D> GetObjectsNotInRange(IEnumerable<IPositionable2D> positionables)
        => positionables.Where(GetObjectsNotInRangePredicate);
}
