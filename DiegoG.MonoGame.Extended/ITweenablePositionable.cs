using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace DiegoG.MonoGame.Extended;

public interface ITweenablePositionable : IPositionable
{
    public Vector2 TargetPosition { get; set; }
}