// Decompiled with JetBrains decompiler
// Type: Vec4Conv
// Assembly: lfo, Version=0.9.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2965BBBA-49CA-4B3F-B886-3391858B1BD3
// Assembly location: C:\Kerbal Space Program 2\BepInEx\plugins\lfo\lfo.dll

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
    JObject jobject = JObject.Load(reader);
    Vector4 target = new Vector4();
    
    target.x = jobject["x"].ToObject<float>();
    target.y = jobject["y"].ToObject<float>();
    target.z = jobject["z"].ToObject<float>();
    target.w = jobject["w"].ToObject<float>();
    
    serializer.Populate(jobject.CreateReader(), (object) target);
    
    return (object) target;
  }

  public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
  {
    Vector4 vector4 = (Vector4) value;

    writer.WriteStartObject();
    writer.WritePropertyName("x");
    writer.WriteValue(vector4.x);
    writer.WritePropertyName("y");
    writer.WriteValue(vector4.y);
    writer.WritePropertyName("z");
    writer.WriteValue(vector4.z);
    writer.WritePropertyName("w");
    writer.WriteValue(vector4.w);
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
        JObject jobject = JObject.Load(reader);
        Vector3 target = new Vector3();
        
        target.x = jobject["x"].ToObject<float>();
        target.y = jobject["y"].ToObject<float>();
        target.z = jobject["z"].ToObject<float>();
        
        serializer.Populate(jobject.CreateReader(), (object)target);
        
        return (object)target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Vector3 vector3 = (Vector3)value;

        writer.WriteStartObject();
        writer.WritePropertyName("x");
        writer.WriteValue(vector3.x);
        writer.WritePropertyName("y");
        writer.WriteValue(vector3.y);
        writer.WritePropertyName("z");
        writer.WriteValue(vector3.z);
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
        JObject jobject = JObject.Load(reader);
        Vector2 target = new Vector2();
        
        target.x = jobject["x"].ToObject<float>();
        target.y = jobject["y"].ToObject<float>();
        
        serializer.Populate(jobject.CreateReader(), (object)target);
        
        return (object)target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Vector2 vector2 = (Vector2)value;

        writer.WriteStartObject();
        writer.WritePropertyName("x");
        writer.WriteValue(vector2.x);
        writer.WritePropertyName("y");
        writer.WriteValue(vector2.y);
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
        JObject jobject = JObject.Load(reader);
        Color target = new Color();
        
        target.r = jobject["r"].ToObject<float>();
        target.g = jobject["g"].ToObject<float>();
        target.b = jobject["b"].ToObject<float>();
        target.a = jobject["a"].ToObject<float>();
        
        serializer.Populate(jobject.CreateReader(), (object)target);
        
        return (object)target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Color color = (Color)value;

        writer.WriteStartObject();
        writer.WritePropertyName("r");
        writer.WriteValue(color.r);
        writer.WritePropertyName("g");
        writer.WriteValue(color.g);
        writer.WritePropertyName("b");
        writer.WriteValue(color.b);
        writer.WritePropertyName("a");
        writer.WriteValue(color.a);
        writer.WriteEndObject();
    }
}
