using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DiegoG.MonoGame.Common;

public static class SpriteBatchExtensions
{
    public const string WhitePixelTextureKey = "_whitepixeltexture";
    private readonly static Color[] SingleWhitePixel = [Color.White];

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
