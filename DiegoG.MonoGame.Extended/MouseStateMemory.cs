using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DiegoG.MonoGame.Extended;

public readonly struct MouseStateMemory
{
    public MouseStateMemory(MouseState state)
    {
        LastState = state;
        PositionDelta = default;
        WheelDelta = default;
    }

    public MouseStateMemory(MouseState oldState, MouseState newState)
    {
        LastState = newState;
        PositionDelta = newState.Position - oldState.Position;
        WheelDelta = new(
            x: newState.HorizontalScrollWheelValue - oldState.HorizontalScrollWheelValue,
            y: newState.ScrollWheelValue - oldState.ScrollWheelValue
        );
    }

    public MouseStateMemory(MouseStateMemory oldState, MouseState newState)
        : this(oldState.LastState, newState) { }

    public MouseState LastState { get; }

    public Point PositionDelta { get; }
    public Vector2 WheelDelta { get; }
}
