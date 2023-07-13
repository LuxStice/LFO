using UnityEngine;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;

public class Vec4Conv : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        if (objectType == typeof(Vector4))
        {
            return true;
        }
        return false;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jObject = JObject.Load(reader);
        Vector4 toReturn = new();

        toReturn.x = jObject["x"].ToObject<float>();
        toReturn.y = jObject["y"].ToObject<float>();
        toReturn.z = jObject["z"].ToObject<float>();
        toReturn.w = jObject["w"].ToObject<float>();

        serializer.Populate(jObject.CreateReader(), toReturn);

        return toReturn;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Vector4 v = (Vector4)value;

        writer.WriteStartObject();
        writer.WritePropertyName("x");
        writer.WriteValue(v.x);
        writer.WritePropertyName("y");
        writer.WriteValue(v.y);
        writer.WritePropertyName("z");
        writer.WriteValue(v.z);
        writer.WritePropertyName("w");
        writer.WriteValue(v.w);
        writer.WriteEndObject();
    }
}

public class Vec3Conv : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        if (objectType == typeof(Vector3))
        {
            return true;
        }
        return false;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jObject = JObject.Load(reader);
        Vector3 toReturn = new();

        toReturn.x = jObject["x"].ToObject<float>();
        toReturn.y = jObject["y"].ToObject<float>();
        toReturn.z = jObject["z"].ToObject<float>();

        serializer.Populate(jObject.CreateReader(), toReturn);

        return toReturn;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Vector3 v = (Vector3)value;

        writer.WriteStartObject();
        writer.WritePropertyName("x");
        writer.WriteValue(v.x);
        writer.WritePropertyName("y");
        writer.WriteValue(v.y);
        writer.WritePropertyName("z");
        writer.WriteValue(v.z);
        writer.WriteEndObject();
    }
}

public class Vec2Conv : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        if (objectType == typeof(Vector2))
        {
            return true;
        }
        return false;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jObject = JObject.Load(reader);
        Vector2 toReturn = new();

        toReturn.x = jObject["x"].ToObject<float>();
        toReturn.y = jObject["y"].ToObject<float>();

        serializer.Populate(jObject.CreateReader(), toReturn);

        return toReturn;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Vector2 v = (Vector2)value;

        writer.WriteStartObject();
        writer.WritePropertyName("x");
        writer.WriteValue(v.x);
        writer.WritePropertyName("y");
        writer.WriteValue(v.y);
        writer.WriteEndObject();
    }
}
public class ColorConv : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        if (objectType == typeof(Color))
        {
            return true;
        }
        return false;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jObject = JObject.Load(reader);
        Color toReturn = new();

        toReturn.r = jObject["r"].ToObject<float>();
        toReturn.g = jObject["g"].ToObject<float>();
        toReturn.b = jObject["b"].ToObject<float>();
        toReturn.a = jObject["a"].ToObject<float>();

        serializer.Populate(jObject.CreateReader(), toReturn);

        return toReturn;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Color v = (Color)value;

        writer.WriteStartObject();
        writer.WritePropertyName("r");
        writer.WriteValue(v.r);
        writer.WritePropertyName("g");
        writer.WriteValue(v.g);
        writer.WritePropertyName("b");
        writer.WriteValue(v.b);
        writer.WritePropertyName("a");
        writer.WriteValue(v.a);
        writer.WriteEndObject();
    }
}
