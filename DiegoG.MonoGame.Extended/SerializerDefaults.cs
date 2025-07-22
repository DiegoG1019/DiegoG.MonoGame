using System.Text.Json;

namespace DiegoG.MonoGame.Extended;
public static class SerializerDefaults
{
    public static JsonSerializerOptions SystemTextJsonOptions { get; } = new();
}
