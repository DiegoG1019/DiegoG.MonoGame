using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace DiegoG.MonoGame.Extended;

public static class SpriteBatchExtensions
{
    public const string WhitePixelTextureKey = "_whitepixeltexture";
    private static readonly Color[] SingleWhitePixel = [Color.White];

    public static void DrawMissingTextureSquare(this SpriteBatch batch, Rectangle area, Color? colorA = null, Color? colorB = null)
    {
        var ca = colorA ?? new Color(35, 35, 35, 255);
        var cb = colorB ?? Color.Purple;

        Rectangle q1 = new(area.X, area.Y, area.Width / 4, area.Height / 4);
        Rectangle q2 = q1 with { Y = q1.Y + q1.Height };
        Rectangle q3 = q1 with { X = q1.X + q1.Width, Y = q1.Y + q1.Height };
        Rectangle q4 = q1 with { X = q1.X + q1.Width };
        
        batch.DrawRectangle(q1, ca);
        batch.DrawRectangle(q2, cb);
        batch.DrawRectangle(q3, ca);
        batch.DrawRectangle(q4, cb);
    }

    public static Texture2D GetWhitePixelTexture(this StatefulSpriteBatch batch)
    {
        if (batch.StateVault.TryGetValue(WhitePixelTextureKey, out var obj) && obj is Texture2D whitePixel)
            return whitePixel;

        whitePixel = new(batch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
        whitePixel.SetData(SingleWhitePixel);

        return whitePixel;
    }

    public static Texture2D GetWhitePixelTexture(this SpriteBatch batch)
    {
        if (batch is StatefulSpriteBatch stateful)
            return GetWhitePixelTexture(stateful);

        Texture2D whitePixel = new(batch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
        whitePixel.SetData(SingleWhitePixel);

        return whitePixel;
    }
}
