namespace Urban.AI.Infrastructure.Common.Extensions;

#region Usings
using System.Buffers;
using System.Text.Json;
#endregion

internal static class DataConversionExtension
{
    internal static T FromUtf8Json<T>(this byte[] bytes)
    {
        return JsonSerializer.Deserialize<T>(bytes)!;
    }

    internal static byte[] ToUtf8Json<T>(this T value)
    {
        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer);
        JsonSerializer.Serialize(writer, value);
        return buffer.WrittenSpan.ToArray();
    }

    internal static T FromJson<T>(this string json)
    {
        return JsonSerializer.Deserialize<T>(json)!;
    }

    internal static string ToJson<T>(this T value)
    {
        return JsonSerializer.Serialize(value);
    }
}
