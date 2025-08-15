using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace DiegoG.MonoGame.Extended;

public static class VectorExtensions
{
    // these functions are so tiny it's almost unnecessary to use this attribute; but I wanna make sure nonetheless
    [MethodImpl(MethodImplOptions.AggressiveInlining)] 
    public static ulong Pack(int a, int b)
        => Pack((uint)a, (uint)b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Pack(uint a, uint b)
        => unchecked(a | ((ulong)b << 32));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Unpack(ulong packed, out int a, out int b)
    {
        unchecked
        {
            a = (int)packed;
            b = (int)((packed) >> 32);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ToLong(this Point point)
        => Pack(point.X, point.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point ToPoint(this ulong value)
        => unchecked(new((int)value, (int)((value) >> 32)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rectangle ToRectangle(this Vector4 vec)
        => new(
            (int)vec.X,
            (int)vec.Y,
            (int)vec.Z,
            (int)vec.W
        );

    public static Point TopRight(this Rectangle rect, int mult)
        => new(rect.X * mult + rect.Width * mult, rect.Y * mult);

    public static Point BottomLeft(this Rectangle rect, int mult)
        => new(rect.X * mult, rect.Y * mult + rect.Height * mult);

    public static Point TopLeft(this Rectangle rect, int mult)
        => new(rect.X * mult, rect.Y * mult);

    public static Point BottomRight(this Rectangle rect, int mult)
        => new(rect.X * mult + rect.Width * mult, rect.Y * mult + rect.Height * mult);

    public static Point TopRight(this Rectangle rect)
        => new(rect.X + rect.Width, rect.Y);

    public static Point BottomLeft(this Rectangle rect)
        => new(rect.X, rect.Y + rect.Height);

    public static Point TopLeft(this Rectangle rect)
        => rect.Location;

    public static Point BottomRight(this Rectangle rect)
        => new(rect.X + rect.Width, rect.Y + rect.Height);
    
    public static Point FindCentroid(this List<Point> points) 
    {
        int x = 0;
        int y = 0;

        foreach (var p in points)
        {
            x += p.X;
            y += p.Y;
        }
        
        return new(x / points.Count, y / points.Count);
    }
    
    public static void SortVertices(this List<Point> points) 
    {
        // get centroid
        var center = points.FindCentroid();
        points.Sort((a, b) =>
        {
            double a1 = (double.RadiansToDegrees(double.Atan2(a.X - center.X, a.Y - center.Y)) + 360) % 360;
            double a2 = (double.RadiansToDegrees(double.Atan2(b.X - center.X, b.Y - center.Y)) + 360) % 360;
            return (int) (a1 - a2); 
        });
    }
}
