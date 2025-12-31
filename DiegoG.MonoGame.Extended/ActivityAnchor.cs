using MonoGame.Extended;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace DiegoG.MonoGame.Extended;

public class ActivityAnchor : IPositionable
{
    private float radius;
    private float radiusSquared;

    private readonly Func<IPositionable, bool> GetObjectsInRangePredicate;
    private readonly Func<IPositionable, bool> GetObjectsNotInRangePredicate;
    private bool GetObjectsNotInRangePredicateMethod(IPositionable positionable)
        => Vector2.DistanceSquared(Position, positionable.Position) > radiusSquared;

    private bool GetObjectsInRangePredicateMethod(IPositionable positionable)
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

    public IEnumerable<IPositionable> GetObjectsInRange(IEnumerable<IPositionable> positionables)
        => positionables.Where(GetObjectsInRangePredicate);

    public IEnumerable<IPositionable> GetObjectsNotInRange(IEnumerable<IPositionable> positionables)
        => positionables.Where(GetObjectsNotInRangePredicate);
}
