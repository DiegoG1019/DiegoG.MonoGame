using DiegoG.MonoGame.Common;
using MonoGame.Extended;
using Microsoft.Xna.Framework;

namespace DiegoG.MonoGame.Extended;

public class CameraSpace(Camera<Vector2> camera) : ISpace
{
    public Camera<Vector2> Camera { get; } = camera;
    public Matrix Transform => Camera.GetViewMatrix();
    public Matrix InverseTransform => Camera.GetInverseViewMatrix();
}
