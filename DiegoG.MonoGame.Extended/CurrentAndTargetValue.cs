using System.Numerics;
using Microsoft.Xna.Framework;

namespace DiegoG.MonoGame.Extended;

public sealed class CurrentAndTargetValue<TNumber>(IValueInterpolator<TNumber> interpolator)
    where TNumber : unmanaged, IFloatingPointIeee754<TNumber>
{
    public TNumber Current { get; set; }
    public TNumber Target { get; set; }

    public IValueInterpolator<TNumber> Interpolator
    {
        get;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            field = value;
        }
    } = interpolator ?? throw new ArgumentNullException(nameof(interpolator));

    public void Update(GameTime gameTime)
    {
        Current = Interpolator.Interpolate(Current, Target,
            TNumber.CreateSaturating(gameTime.ElapsedGameTime.TotalSeconds));
    }

    public void ForceToTarget()
        => Current = Target;
}