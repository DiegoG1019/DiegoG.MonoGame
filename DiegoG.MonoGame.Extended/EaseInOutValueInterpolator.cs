using System.Globalization;
using System.Numerics;
using ImGuiNET;
using MonoGame.Extended;

namespace DiegoG.MonoGame.Extended;

public sealed class EaseInOutValueInterpolator<TNumber> : IValueInterpolator<TNumber>
    where TNumber : unmanaged, IFloatingPointIeee754<TNumber>
{
    public TNumber Exponent { get; init; } = TNumber.CreateSaturating(2);

    public TNumber Interpolate(TNumber number, TNumber target, TNumber deltaSeconds)
        => ((number - (number - target) * Exponent * deltaSeconds));

    public static EaseInOutValueInterpolator<TNumber> Default { get; } = new();
}