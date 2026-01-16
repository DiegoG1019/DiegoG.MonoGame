using Microsoft.Xna.Framework;

namespace DiegoG.MonoGame.Extended;

public interface IAsyncUpdateable
{
    public Task UpdateAsync(GameTime gameTime);
}