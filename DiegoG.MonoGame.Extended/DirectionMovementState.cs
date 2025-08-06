using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace DiegoG.MonoGame.Extended;

public enum CardinalDirection
{
    North,
    West,
    East,
    South
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
    public Point Position => new(X, Y);
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
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void MoveWithinBounds(RectangleF rectangle, BoundsCheckReaction boundsCheck = BoundsCheckReaction.Stop)
        => MoveWithinBounds(new((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height), boundsCheck);

    public void MoveWithinBounds(Rectangle rectangle, BoundsCheckReaction boundsCheck = BoundsCheckReaction.Stop)
    {
        if (boundsCheck is BoundsCheckReaction.Stop)
        {
            if (CardinalDirection is CardinalDirection.East && X >= rectangle.Left 
                || CardinalDirection is CardinalDirection.West && X < rectangle.Right
                || CardinalDirection is CardinalDirection.North && Y >= rectangle.Top
                || CardinalDirection is CardinalDirection.South && Y < rectangle.Bottom)
                Move();
        }

        if (CardinalDirection is CardinalDirection.East or CardinalDirection.West && X == rectangle.Left || X + 1 >= rectangle.Right)
        {
            if (X >= rectangle.Right) X = rectangle.Right - 1;
            else if (X < 0) X = 0;
            
            switch (boundsCheck)
            {
                case BoundsCheckReaction.Bounce:
                    CardinalDirection = CardinalDirection switch
                    {
                        CardinalDirection.West => CardinalDirection.East,
                        CardinalDirection.East => CardinalDirection.West,
                        _ => throw new InvalidOperationException()
                    };
                    break;
                case BoundsCheckReaction.Slide:
                    CardinalDirection = Y - rectangle.Top > rectangle.Bottom - Y ? CardinalDirection.North : CardinalDirection.South;
                    break;
                case BoundsCheckReaction.Reset or BoundsCheckReaction.ResetAndChangeDirection:
                {
                    X = InitialX;
                    Y = InitialY;
                    if (boundsCheck is BoundsCheckReaction.ResetAndChangeDirection) 
                        RandomizeDirection();
                    break;
                }
            }
        }
        else if (CardinalDirection is CardinalDirection.North && Y <= rectangle.Top || CardinalDirection is CardinalDirection.South && Y + 1 >= rectangle.Bottom)
        {
            if (Y >= rectangle.Bottom) Y = rectangle.Bottom - 1;
            else if (Y < 0) Y = 0;
            
            switch (boundsCheck)
            {
                case BoundsCheckReaction.Bounce:
                    CardinalDirection = CardinalDirection switch
                    {
                        CardinalDirection.North => CardinalDirection.South,
                        CardinalDirection.South => CardinalDirection.North,
                        _ => throw new InvalidOperationException()
                    };
                    break;
                case BoundsCheckReaction.Slide:
                    CardinalDirection = X - rectangle.Left > rectangle.Right - X ? CardinalDirection.East : CardinalDirection.West;
                    break;
                case BoundsCheckReaction.Reset or BoundsCheckReaction.ResetAndChangeDirection:
                {
                    X = InitialX;
                    Y = InitialY;
                    if (boundsCheck is BoundsCheckReaction.ResetAndChangeDirection) 
                        RandomizeDirection();
                    break;
                }
            }
        }
            //TODO: This looks ugly as hell. Revisit, please
        Move();
    }

    public void RandomizeDirection(Random? random = null)
    {
        CardinalDirection = (CardinalDirection)((random ?? Random.Shared).Next() % 4);
    }
}