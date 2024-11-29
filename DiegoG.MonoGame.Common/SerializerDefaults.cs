using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DiegoG.MonoGame.Common;
public static class SerializerDefaults
{
    public static JsonSerializerOptions SystemTextJsonOptions { get; } = new();
}
