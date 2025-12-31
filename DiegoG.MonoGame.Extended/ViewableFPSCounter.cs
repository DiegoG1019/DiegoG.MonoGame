using System.Text;
using GLV.Shared.Common.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace DiegoG.MonoGame.Extended;

public class ViewableFPSCounter(Game game, string font, SpriteBatch batch) : FramesPerSecondCounterComponent(game)
{
    private SpriteFont? spriteFont;

    public bool DisplayFPS { get; set; } = true;

    public Vector2 DisplayPosition { get; set; } = new(10, 10);
    public Color Color { get; set; } = Color.Black;

    protected override void LoadContent()
    {
        base.LoadContent();
        spriteFont = Game.Content.Load<SpriteFont>(font);
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        if (!DisplayFPS) return;

        using (StringBuilderPool.Shared.ObtainSafe(out var sb))
        {
            sb.Clear();
            sb.Append(FramesPerSecond);
            for (int i = 0; i < 10 - sb.Length; i++)
                sb.Insert(0, ' ');
            
            batch.DrawString(spriteFont, sb, DisplayPosition, Color);
        }
    }
}
