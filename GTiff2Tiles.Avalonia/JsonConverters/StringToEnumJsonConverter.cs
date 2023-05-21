using System.Text.Json;
using System.Text.Json.Serialization;

#pragma warning disable CA1308 // Normalize strings to uppercase

namespace GTiff2Tiles.Avalonia.JsonConverters;

/// <summary>
/// Converts any case-insensitive <see cref="Enum"/> to/from <see cref="string"/>
/// </summary>
/// <typeparam name="T"><see cref="Enum"/></typeparam>
public class StringToEnumJsonConverter<T> : JsonConverter<T> where T : Enum
{
    /// <inheritdoc />
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString();

        if (string.IsNullOrWhiteSpace(str)) throw new ArgumentOutOfRangeException(nameof(reader));

        return (T)Enum.Parse(typeof(T), str, true);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        if (writer == null) throw new ArgumentNullException(nameof(writer));

        writer.WriteStringValue(value.ToString().ToLowerInvariant());
    }
}
