using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class DefaultDateTimeConverter : JsonConverter<DateTime>
{
    private static readonly DateTime defaultValue = new DateTime(1970, 1, 1);

    public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return reader.TokenType == JsonToken.Null ? defaultValue : Convert.ToDateTime(reader.Value);
    }

    public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
    {
        writer.WriteValue(value);
    }
}
