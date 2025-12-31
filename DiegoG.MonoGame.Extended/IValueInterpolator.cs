using System.Numerics;

namespace DiegoG.MonoGame.Extended;

public interface IValueInterpolator<TNumber> where TNumber : unmanaged, IFloatingPointIeee754<TNumber>
{
    public TNumber Interpolate(TNumber number, TNumber target, TNumber deltaSeconds);
}