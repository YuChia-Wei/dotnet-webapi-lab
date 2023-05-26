using System.Text.Json;
using System.Text.Json.Serialization;

namespace dotnet.WebApi.Infrastructure.CustomJsonConverter;

/// <summary>
/// 轉換Utc時間以應用 System.Text.Json
/// </summary>
public class UtcDateTimeConverter : JsonConverter<DateTime>
{
    /// <summary>
    /// read
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="typeToConvert"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var tryParse = DateTime.TryParse(reader.GetString() , out var outputDateTime);
        return outputDateTime;
    }

    /// <summary>
    /// write
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    /// <param name="options"></param>
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssZ"));
    }
}