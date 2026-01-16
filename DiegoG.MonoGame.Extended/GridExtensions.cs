using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace DiegoG.MonoGame.Extended;

public static class GridExtensions
{
    public static void DrawGrid(this SquareGrid grid, Vector2 bounds, SpriteBatch spriteBatch, Color color, Vector2 offset = default, float thickness = 1f, float layerDepth = 0)
    {
        var whitePixelTex = spriteBatch.GetWhitePixelTexture();
        var steps = bounds / new Vector2(grid.XScale, grid.YScale);

        var xSteps = (int)float.Ceiling(steps.X);
        var ySteps = (int)float.Ceiling(steps.Y);

        Vector2 pos = offset;
        pos.X += thickness / 2;
        Vector2 scale = new(bounds.X, thickness);

        for (int y = 0; y <= ySteps; y++)
        {
            spriteBatch.Draw(whitePixelTex, pos, null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
            pos.Y += grid.YScale;
        }

        pos = offset;
        pos.Y += thickness / 2;
        scale = new(thickness, bounds.Y);

        for (int x = 0; x <= xSteps; x++)
        {
            spriteBatch.Draw(whitePixelTex, pos, null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
            pos.X += grid.XScale;
        }
    }

    public static void SnapToGrid(this IPositionable2D positionable, SquareGrid grid, GridPositionOffset xoffset = GridPositionOffset.None, GridPositionOffset yoffset = GridPositionOffset.None)
    {
        var (x, y) = positionable.Position;
        positionable.Position = grid.GetPosition((int)(x / grid.XScale), (int)(y / grid.YScale), xoffset, yoffset);
    }

    public static void SetPositionByGrid(this IPositionable2D positionable, SquareGrid grid, int x, int y, GridPositionOffset xoffset = GridPositionOffset.None, GridPositionOffset yoffset = GridPositionOffset.None)
    {
        positionable.Position = grid.GetPosition(x, y, xoffset, yoffset);
    }
}
