using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace DiegoG.MonoGame.Extended;

public static class GeometryExtensions
{
    extension(in RectangleF rect)
    {
        public Vector2 CenterTop => new Vector2(rect.X + rect.Width * 0.5f, rect.Y); 
        public Vector2 CenterBottom => new Vector2(rect.X + rect.Width * 0.5f, rect.Y + rect.Height);
        public Vector2 CenterRight => new Vector2(rect.X + rect.Width, rect.Y + rect.Height * 0.5f);
        public Vector2 CenterLeft => new Vector2(rect.X, rect.Y + rect.Height * 0.5f);
    }

    extension(in Rectangle rect)
    {
        public Point CenterTop => new Point(rect.X + rect.Width / 2, rect.Y); 
        public Point CenterBottom => new Point(rect.X + rect.Width / 2, rect.Y + rect.Height);
        public Point CenterRight => new Point(rect.X + rect.Width, rect.Y + rect.Height / 2);
        public Point CenterLeft => new Point(rect.X, rect.Y + rect.Height / 2);
    }
}