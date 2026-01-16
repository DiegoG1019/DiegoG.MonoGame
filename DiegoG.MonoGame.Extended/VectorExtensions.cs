using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace DiegoG.MonoGame.Extended;

public static class VectorExtensions
{
    public static Vector2 GetPositionFromDistanceAndAngle(float angleRadians, float distance)
        => new Vector2(distance * float.Cos(angleRadians), distance * float.Sin(angleRadians));

    extension(Vector2 vec)
    {
        /// <summary>
        /// Calculates the angle between the current vector and <paramref name="b"/>
        /// </summary>
        public float AngleBetween(Vector2 b) 
            => float.Atan2(b.Y - vec.Y, b.X - vec.X);

        /// <summary>
        /// Calculates the angle between the currenct vector and a Vector pointing upwards (x: 0, y: -1)
        /// </summary>
        public float AngleBetweenUp()
            => vec.AngleBetween(new Vector2(0, -1));

        /// <summary>
        /// Calculates the Vector that would be ahead of <paramref name="vec"/> at a given angle and distance
        /// </summary>
        /// <remarks>
        /// This method uses <see cref="VectorExtensions.GetPositionFromDistanceAndAngle"/> and adds the original vector to the result
        /// </remarks>
        public Vector2 GetVectorAhead(float angleRadians, float distance)
            => GetPositionFromDistanceAndAngle(angleRadians, distance) + vec;

        public Vector2 RotateAroundCopy(Vector2 origin, float radians)
        {
            var v = vec;
            v -= origin;
            float cos = float.Cos(radians);
            float sin = float.Sin(radians);
            float x = v.X;
            v.X = v.X * cos - v.Y * sin;
            v.Y = x * sin + v.Y * cos;
            v += origin;
            return v;
        }

        public Vector2 RotateCopy(float radians)
        {
            var v = vec;
            float cos = float.Cos(radians);
            float sin = float.Sin(radians);
            float x = v.X;
            v.X = v.X * cos - v.Y * sin;
            v.Y = x * sin + v.Y * cos;
            return v;
        }
    }

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

    extension(Rectangle rect)
    {
        public Point GetTopRight(int mult)
            => new(rect.X * mult + rect.Width * mult, rect.Y * mult);

        public Point GetBottomLeft(int mult)
            => new(rect.X * mult, rect.Y * mult + rect.Height * mult);

        public Point GetTopLeft(int mult)
            => new(rect.X * mult, rect.Y * mult);

        public Point GetBottomRight(int mult)
            => new(rect.X * mult + rect.Width * mult, rect.Y * mult + rect.Height * mult);

        public Point TopRight
            => new(rect.X + rect.Width, rect.Y);

        public Point BottomLeft
            => new(rect.X, rect.Y + rect.Height);

        public Point TopLeft
            => rect.Location;

        public Point BottomRight
            => new(rect.X + rect.Width, rect.Y + rect.Height);
    }

    extension(List<Point> points)
    {
        public Point FindCentroid() 
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

        public void SortVertices() 
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
}
