using Microsoft.Xna.Framework;

namespace DiegoG.MonoGame.Extended;

public enum GridPositionOffset
{
    None = 0,
    Center = 1,
    CellSize = 2
}

public readonly record struct BoundedSquareGrid(SquareGrid Grid, int XCells, int YCells)
{
    public Vector2 this[int x, int y]
    {
        get
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(x, XCells);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(y, YCells);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(x);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(y);

            return Grid.GetPosition(x, y);
        }
    }

    public Vector4 GetArea(
        int x1, int y1, int x2, int y2,
        GridPositionOffset x1offset = GridPositionOffset.None,
        GridPositionOffset y1offset = GridPositionOffset.None,
        GridPositionOffset x2offset = GridPositionOffset.CellSize,
        GridPositionOffset y2offset = GridPositionOffset.CellSize
    )
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(x1, XCells);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(x2, XCells);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(y1, YCells);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(y2, YCells);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(x1);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(y1);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(x2);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(y2);

        return Grid.GetArea(x1, y1, x2, y2, x1offset, y1offset, x2offset, y2offset);
    }

    public Vector2 GetPosition(int x, int y, GridPositionOffset xoffset = GridPositionOffset.None, GridPositionOffset yoffset = GridPositionOffset.None)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(x, XCells);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(y, YCells);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(x);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(y);

        return Grid.GetPosition(x, y, xoffset, yoffset);
    }
}

public readonly record struct SquareGrid(float XScale, float YScale)
{
    public SquareGrid(float scale) : this(scale, scale)
    { }

    public Vector2 this[int x, int y]
        => GetPosition(x, y);

    public Vector4 GetArea(
        int x1, int y1, int x2, int y2,
        GridPositionOffset x1offset = GridPositionOffset.None,
        GridPositionOffset y1offset = GridPositionOffset.None,
        GridPositionOffset x2offset = GridPositionOffset.CellSize,
        GridPositionOffset y2offset = GridPositionOffset.CellSize
    )
        => GetArea(x1, y1, x2, y2, XScale, YScale, x1offset, y1offset, x2offset, y2offset);

    public Vector2 GetPosition(int x, int y, GridPositionOffset xoffset = GridPositionOffset.None, GridPositionOffset yoffset = GridPositionOffset.None)
        => GetPosition(x, y, XScale, YScale, xoffset, yoffset);

    public float GetXOffset(GridPositionOffset offset)
        => GetOffset(offset, XScale);

    public float GetYOffset(GridPositionOffset offset)
        => GetOffset(offset, YScale);

    // ----------- static

    public static Vector4 GetArea(
        int x1, int y1, int x2, int y2,
        float xscale, float yscale,
        GridPositionOffset x1offset = GridPositionOffset.None,
        GridPositionOffset y1offset = GridPositionOffset.None,
        GridPositionOffset x2offset = GridPositionOffset.CellSize,
        GridPositionOffset y2offset = GridPositionOffset.CellSize
    )
        => new(
            x1 * xscale + GetOffset(x1offset, xscale),
            y1 * yscale + GetOffset(y1offset, yscale),
            x2 * xscale + GetOffset(x2offset, xscale),
            y2 * yscale + GetOffset(y2offset, yscale)
        );

    public static Vector2 GetPosition(
        int x, int y, float xscale, float yscale,
        GridPositionOffset xoffset = GridPositionOffset.None, GridPositionOffset yoffset = GridPositionOffset.None
    )
        => new(x * xscale + GetOffset(xoffset, xscale), y * yscale + GetOffset(yoffset, yscale));

    public static float GetOffset(GridPositionOffset offset, float scale)
        => offset switch
        {
            GridPositionOffset.None => 0,
            GridPositionOffset.Center => scale / 2f,
            GridPositionOffset.CellSize => scale,
            _ => throw new ArgumentException($"Unknown GridPositionOffset: {offset}")
        };
}
