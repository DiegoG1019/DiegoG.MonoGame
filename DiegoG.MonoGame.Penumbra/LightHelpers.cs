using Microsoft.Xna.Framework;
using Penumbra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.MonoGame.Penumbra;
public static class LightHelpers
{
    public static PointLight TorchLight(Vector2 position)
        => new()
        {
            CastsShadows = true,
            Color = new(220, 180, 100, 255),
            Enabled = true,
            Intensity = 3,
            Scale = new Vector2(250, 150),
            Position = position,
            ShadowType = ShadowType.Solid
        };

    public static void FlickerLights(float baseValue, GameTime time, IEnumerable<Light> lights, Random? random = null, int minScale = -1, int maxScale = 1)
    {
        if (maxScale < minScale)
            throw new ArgumentException($"{nameof(maxScale)} cannot be less than minScale", nameof(maxScale));

        if (maxScale == minScale)
            throw new ArgumentException($"{nameof(maxScale)} cannot be equal to minScale", nameof(maxScale));

        random ??= Random.Shared;
        if (time.TotalGameTime.TotalMilliseconds % 200 < 10)
        {
            foreach (var light in lights)
            {
                if (light.Enabled is false)
                    continue;
                light.Intensity = baseValue + (float)(random.Next(minScale, maxScale) * (random.NextDouble() / 8));
            }
        }
    }
}
