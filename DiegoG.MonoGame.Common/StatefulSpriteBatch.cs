using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DiegoG.MonoGame.Common;

public class StatefulSpriteBatch(GraphicsDevice graphicsDevice) : SpriteBatch(graphicsDevice)
{
    private ConcurrentDictionary<string, object>? stateVault;

    public SpriteSortMode SortMode { get; set; }
    public Matrix TransformationMatrix { get; set; }
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
}
