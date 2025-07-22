using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace DiegoG.MonoGame.Extended.SystemTextJsonConverters;

public sealed class RectangleConverter : JsonConverter<Rectangle>
{
    private struct BufferRect
    {
        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public readonly Rectangle ToRectangle() 
            => Unsafe.BitCast<BufferRect, Rectangle>(this);

        public static BufferRect FromRectangle(Rectangle rect)
            => Unsafe.BitCast<Rectangle, BufferRect>(rect);
    }

    private RectangleConverter() { }

    public static RectangleConverter Instance { get; } = new();

    public override Rectangle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => JsonSerializer.Deserialize<BufferRect>(ref reader, options).ToRectangle();

    public override void Write(Utf8JsonWriter writer, Rectangle value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, BufferRect.FromRectangle(value), options);
}
