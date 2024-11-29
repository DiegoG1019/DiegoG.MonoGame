using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace DiegoG.MonoGame.Common;

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
}
