using System.Collections.Immutable;
using System.Diagnostics;
using System.Numerics;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace DiegoG.MonoGame.Extended;

public static class EnumData
{
    public static readonly ImmutableArray<CardinalDirection> CardinalDirectionValues =
        Enum.GetValues<CardinalDirection>().ToImmutableArray();

    public static T GetAngleRadians<T>(this CardinalDirection direction) where T : IFloatingPointIeee754<T>
        => direction switch
        {
            CardinalDirection.North => T.Zero,
            CardinalDirection.NorthWest => T.DegreesToRadians(T.CreateSaturating(45)),
            CardinalDirection.West => T.DegreesToRadians(T.CreateSaturating(90)),
            CardinalDirection.SouthWest => T.DegreesToRadians(T.CreateSaturating(135)),
            CardinalDirection.South => T.DegreesToRadians(T.CreateSaturating(180)),
            CardinalDirection.SouthEast => T.DegreesToRadians(T.CreateSaturating(225)),
            CardinalDirection.East => T.DegreesToRadians(T.CreateSaturating(270)),
            CardinalDirection.NorthEast => T.DegreesToRadians(T.CreateSaturating(315)),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };

    public static CardinalDirection GetFacingDirection<T>(this T angleRadians) where T : IFloatingPointIeee754<T>
        => (angleRadians % (T.Pi * T.CreateSaturating(2))) switch
        {
            // We need to check the angle to get the cardinal direction
            // Imagine 8 cones, one for each direction, all of equal size
            // From the center of each cone, the border is at the 22.5deg step. From -22.5deg, the left border of north,
            // the beginning of the north-western cone is 45deg to the right
            
            // 337.5 deg
            >= 5.8904867 => CardinalDirection.North,
            // 292.5 deg
            >= 5.105088 => CardinalDirection.NorthEast,
            // 247.5 deg
            >= 4.3196898 => CardinalDirection.East,
            // 202.5 deg
            >= 3.534292 => CardinalDirection.SouthEast,
            // 157.5 deg
            >= 2.7488935 => CardinalDirection.South,
            // 112.5 deg
            >= 1.9634954 => CardinalDirection.SouthWest,
            // 67.5 deg
            >= 1.1780972 => CardinalDirection.West,
            // 22.5 deg
            >= 0.3926991 => CardinalDirection.NorthWest,
            // From 337.5 deg to just before 22.5 deg, remember it's %'d with a full rotation
            _ => CardinalDirection.North
        };
}

/// <summary>
/// Provides the cardinal directions; ordered clockwise from North to NorthEast
/// </summary>
public enum CardinalDirection
{
    North,
    NorthWest,
    West,
    SouthWest,
    South,
    SouthEast,
    East,
    NorthEast,
}

public enum BoundsCheckReaction
{
    Stop,
    Bounce,
    Reset,
    ResetAndChangeDirection,
    Slide
}

public struct DirectionMovementState(int x, int y)
{
    public int InitialX { get; set; } = x;
    public int InitialY { get; set; } = y;
    public int X { get; set; } = x;
    public int Y { get; set; } = y;

    public Point Position
    {
        get => new(X, Y);
        set
        {
            X = value.X;
            Y = value.Y;
        }
    }
    public CardinalDirection CardinalDirection { get; set; }

    public void Move()
    {
        switch (CardinalDirection)
        {
            case CardinalDirection.West:
                X++;
                break;
            case CardinalDirection.East:
                X--;
                break;
            case CardinalDirection.South:
                Y++;
                break;
            case CardinalDirection.North:
                Y--;
                break;
            case CardinalDirection.NorthWest:
                Y--;
                X++;
                break;
            case CardinalDirection.SouthWest:
                Y++;
                X++;
                break;
            case CardinalDirection.SouthEast:
                Y++;
                X--;
                break;
            case CardinalDirection.NorthEast:
                Y--;
                X--;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void MoveWithinBounds(RectangleF rectangle, BoundsCheckReaction boundsCheck = BoundsCheckReaction.Stop)
        => MoveWithinBounds(new((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height), boundsCheck);

    public bool TryMoveWithinBounds(RectangleF rectangle)
        => TryMoveWithinBounds(new((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height));

    public Point TruncatePositionWithinBounds(RectangleF rectangle)
        => TruncatePositionWithinBounds(new((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height));

    public Point TruncatePositionWithinBounds(Rectangle rectangle)
        => new(int.Clamp(X, 0, rectangle.Width), int.Clamp(Y, 0, rectangle.Height));
    
    public bool TryMoveWithinBounds(Rectangle rectangle)
    {
        var prev = Position;
        Move();

        if ((CardinalDirection is CardinalDirection.East && X >= rectangle.Left)
            || (CardinalDirection is CardinalDirection.West && X < rectangle.Right)
            || (CardinalDirection is CardinalDirection.North && Y >= rectangle.Top)
            || (CardinalDirection is CardinalDirection.South && Y < rectangle.Bottom))
        {
            CheckPosition(rectangle);
            return true;
        }

        Position = prev;
        CheckPosition(rectangle);
        return false;
    }
    
    public void MoveWithinBounds(Rectangle rectangle, BoundsCheckReaction boundsCheck = BoundsCheckReaction.Stop)
    {
        var prev = Position;
        Move();
        if (CheckPosition(rectangle))
            return;

        if (boundsCheck is BoundsCheckReaction.Stop)
        {
            if (CardinalDirection is CardinalDirection.East && X >= rectangle.Left
                || CardinalDirection is CardinalDirection.West && X < rectangle.Right
                || CardinalDirection is CardinalDirection.North && Y >= rectangle.Top
                || CardinalDirection is CardinalDirection.South && Y < rectangle.Bottom)
                return;
            
            Position = prev;
        }

        if (CardinalDirection is CardinalDirection.East or CardinalDirection.West && X == rectangle.Left || X + 1 >= rectangle.Right)
        {
            switch (boundsCheck)
            {
                case BoundsCheckReaction.Bounce:
                    Position = prev;
                    CardinalDirection = CardinalDirection switch
                    {
                        CardinalDirection.West => CardinalDirection.East,
                        CardinalDirection.East => CardinalDirection.West,
                        _ => throw new InvalidOperationException()
                    };
                    Move();
                    break;
                case BoundsCheckReaction.Slide:
                    Position = prev;
                    CardinalDirection = Y - rectangle.Top > rectangle.Bottom - Y ? CardinalDirection.North : CardinalDirection.South;
                    Move();
                    break;
                case BoundsCheckReaction.Reset or BoundsCheckReaction.ResetAndChangeDirection:
                {
                    X = InitialX;
                    Y = InitialY;
                    if (boundsCheck is BoundsCheckReaction.ResetAndChangeDirection) 
                        RandomizeDirection();
                    Move();
                    break;
                }
            }
        }
        else if (CardinalDirection is CardinalDirection.North && Y <= rectangle.Top || CardinalDirection is CardinalDirection.South && Y + 1 >= rectangle.Bottom)
        {
            switch (boundsCheck)
            {
                case BoundsCheckReaction.Bounce:
                    Position = prev;
                    CardinalDirection = CardinalDirection switch
                    {
                        CardinalDirection.North => CardinalDirection.South,
                        CardinalDirection.South => CardinalDirection.North,
                        _ => throw new InvalidOperationException()
                    };
                    Move();
                    break;
                case BoundsCheckReaction.Slide:
                    Position = prev;
                    CardinalDirection = X - rectangle.Left > rectangle.Right - X ? CardinalDirection.East : CardinalDirection.West;
                    Move();
                    break;
                case BoundsCheckReaction.Reset or BoundsCheckReaction.ResetAndChangeDirection:
                {
                    X = InitialX;
                    Y = InitialY;
                    if (boundsCheck is BoundsCheckReaction.ResetAndChangeDirection) 
                        RandomizeDirection();
                    Move();
                    break;
                }
            }
        }

#if DEBUG
        CheckPosition(rectangle);
#endif
        //TODO: This looks ugly as hell. Revisit, please
    }

    public void RandomizeDirection(Random? random = null)
    {
        CardinalDirection = (CardinalDirection)((random ?? Random.Shared).Next() % 4);
    }
    
    private bool CheckPosition(in Rectangle rectangle)
    {
        if (Position.X < 0 || Position.Y < 0 || Position.X >= rectangle.Width || Position.Y >= rectangle.Height)
        {
            return false;
        }
        return true;
    }
}