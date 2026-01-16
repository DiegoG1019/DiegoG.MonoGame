namespace DiegoG.MonoGame.Extended;

public readonly record struct FacingDirection
{
    public FacingDirection(float angleRadians)
    {
        AngleRadians = angleRadians;
        CardinalDirection = angleRadians.GetFacingDirection();
    }

    public FacingDirection(CardinalDirection direction)
    {
        CardinalDirection = direction;
        AngleRadians = direction.GetAngleRadians<float>();
    }
    
    public float AngleRadians { get; }
    public CardinalDirection CardinalDirection { get; }

    public static implicit operator FacingDirection(float angleRadians) => new(angleRadians);
    public static implicit operator FacingDirection(CardinalDirection direction) => new(direction);
    public static implicit operator float (FacingDirection direction) => direction.AngleRadians;
    public static implicit operator CardinalDirection (FacingDirection direction) => direction.CardinalDirection;
}