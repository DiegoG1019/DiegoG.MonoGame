using MonoGame.Extended;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DiegoG.MonoGame.Extended;

//public readonly record struct SpriteDescription(string TexturePath, Rectangle? Region)
//{
//    public Sprite CreateSprite(ContentManager content)
//    {
//        var tex = content.Load<Texture2DRegion>(TexturePath);
//        return new Sprite();
//    }
//}

public class CameraSpace(Camera<Vector2> camera) : ISpace
{
    public Camera<Vector2> Camera { get; } = camera;
    public Matrix Transform => Camera.GetViewMatrix();
    public Matrix InverseTransform => Camera.GetInverseViewMatrix();
}
