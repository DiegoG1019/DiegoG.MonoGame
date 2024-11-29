using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.MonoGame.Common;

public static class InputHelpers
{
    public static void Update(this ref MouseStateMemory oldMem, MouseState newState)
        => oldMem = new(oldMem, newState);

    public static Vector2 GetMouseCameraPanDelta(Point newp, Point mouseLastPos, float zoomScale = 1, float panSensitivity = 10)
        => (newp - mouseLastPos).ToVector2() * panSensitivity / (zoomScale == 0 ? 1 : zoomScale);

    public static Vector2 GetMouseCameraPanDelta(this in MouseStateMemory memory, float zoomScale = 1, float panSensitivity = 10)
        => memory.PositionDelta.ToVector2() * panSensitivity / (zoomScale == 0 ? 1 : zoomScale);

    public static float GetMouseCameraZoomDelta(this in MouseStateMemory memory, float zoomSensitivity = 0.001f)
        => memory.WheelDelta.Y * zoomSensitivity;

    public static float GetMouseCameraZoomDelta(float wheelScroll, float lastWheelScroll, float zoomSensitivity = 0.001f)
        => (wheelScroll - lastWheelScroll) * zoomSensitivity;
}
