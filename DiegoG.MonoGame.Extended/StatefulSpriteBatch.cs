using System.Collections.Concurrent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DiegoG.MonoGame.Extended;

public class StatefulSpriteBatch : SpriteBatch, ISpace
{
    public StatefulSpriteBatch(GraphicsDevice graphicsDevice) : base(graphicsDevice) { }

    private ConcurrentDictionary<string, object>? stateVault;

    public SpriteSortMode SortMode { get; set; }
    public Matrix TransformationMatrix { get; set; } = Matrix.Identity;
    public Effect? Effect { get; set; }
    public BlendState? BlendState { get; set; }
    public SamplerState? SamplerState { get; set; }
    public DepthStencilState? DepthStencilState { get; set; }
    public RasterizerState? RasterizerState { get; set; }

    public ConcurrentDictionary<string, object> StateVault
    {
        get
        {
            lock (this)
                return stateVault ??= new();
        }
    }

    public void BeginWithState()
    {
        Begin(SortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, TransformationMatrix);
    }

    Matrix ISpace.Transform => TransformationMatrix;
}
