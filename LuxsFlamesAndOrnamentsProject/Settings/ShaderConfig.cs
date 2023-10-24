// Decompiled with JetBrains decompiler
// Type: LuxsFlamesAndOrnaments.Settings.ShaderConfig
// Assembly: lfo, Version=0.9.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2965BBBA-49CA-4B3F-B886-3391858B1BD3
// Assembly location: C:\Kerbal Space Program 2\BepInEx\plugins\lfo\lfo.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
// using SpaceWarp.API.Assets;
// using System;
// using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.Rendering;
using static LuxsFlamesAndOrnaments.LuxsFlamesAndOrnamentsPlugin;

namespace LuxsFlamesAndOrnaments.Settings
{
  [Serializable]
  public struct ShaderConfig
  {
    [JsonRequired]
    public string ShaderName;
    [JsonRequired]
    public Dictionary<string, object> ShaderParams;

    internal void Add(string paramName, object value) => this.ShaderParams.Add(paramName, value);

    public Material ToMaterial()
    {
      Shader shader = LFO.GetShader(this.ShaderName);
      if (shader == null)
      {
        LuxsFlamesAndOrnamentsPlugin.LogError((object) ("Couldn't find shader " + this.ShaderName));
        return (Material) null;
      }
      Material material = new Material(shader);
      foreach (KeyValuePair<string, object> shaderParam in this.ShaderParams)
      {
        switch (shaderParam.Value)
        {
          case Color color:
            material.SetColor(shaderParam.Key, color);
            continue;
          case Vector2 vector2:
            material.SetVector(shaderParam.Key, (Vector4) vector2);
            continue;
          case Vector3 vector3:
            material.SetVector(shaderParam.Key, (Vector4) vector3);
            continue;
          case Vector4 vector4:
            material.SetVector(shaderParam.Key, vector4);
            continue;
          case float num1:
            material.SetFloat(shaderParam.Key, num1);
            continue;
          case int num2:
            material.SetFloat(shaderParam.Key, (float) num2);
            continue;
          case string str:
            if (!Application.isEditor)
            {
              try
              {
                Texture texture;
                if (!SpaceWarp.API.Assets.AssetManager.TryGetAsset<Texture>(LFO.NOISES_PATH + str + ".png", out texture) && !SpaceWarp.API.Assets.AssetManager.TryGetAsset<Texture>(LFO.NOISES_PATH + str + ".asset", out texture) && !SpaceWarp.API.Assets.AssetManager.TryGetAsset<Texture>(LFO.PROFILES_PATH + str + ".png", out texture))
                  throw new NullReferenceException("Couldn't find texture with path lfo/resources/textures" + str + ". Make sure the textures have the right name!");
                material.SetTexture(shaderParam.Key, texture);
                continue;
              }
              catch (Exception ex)
              {
                Debug.LogError((object) ex.Message);
                continue;
              }
            }
            else
              continue;
          default:
            continue;
        }
      }
      return material;
    }

    public static ShaderConfig GenerateConfig(Material material)
    {
      Shader shader = material.shader;
      ShaderConfig config = new ShaderConfig();
      config.ShaderName = shader.name;
      config.ShaderParams = new Dictionary<string, object>();
      for (int propertyIndex = 0; propertyIndex < shader.GetPropertyCount(); ++propertyIndex)
      {
        string propertyName = shader.GetPropertyName(propertyIndex);
        switch (shader.GetPropertyType(propertyIndex))
        {
          case UnityEngine.Rendering.ShaderPropertyType.Color:
            Color defaultVectorValue1 = (Color) shader.GetPropertyDefaultVectorValue(propertyIndex);
            Color color = material.GetColor(propertyName);
            if (color != defaultVectorValue1)
            {
              config.Add(propertyName, (object) color);
              break;
            }
            break;
          case UnityEngine.Rendering.ShaderPropertyType.Vector:
            Vector4 defaultVectorValue2 = shader.GetPropertyDefaultVectorValue(propertyIndex);
            Vector4 vector = material.GetVector(propertyName);
            if (vector != defaultVectorValue2)
            {
              config.Add(propertyName, (object) vector);
              break;
            }
            break;
          case UnityEngine.Rendering.ShaderPropertyType.Float:
            float defaultFloatValue1 = shader.GetPropertyDefaultFloatValue(propertyIndex);
            float num1 = material.GetFloat(propertyName);
            if ((double) num1 != (double) defaultFloatValue1)
            {
              config.Add(propertyName, (object) num1);
              break;
            }
            break;
          case UnityEngine.Rendering.ShaderPropertyType.Range:
            shader.GetPropertyRangeLimits(propertyIndex);
            float defaultFloatValue2 = shader.GetPropertyDefaultFloatValue(propertyIndex);
            float num2 = material.GetFloat(propertyName);
            if ((double) num2 != (double) defaultFloatValue2)
            {
              config.Add(propertyName, (object) num2);
              break;
            }
            break;
          case UnityEngine.Rendering.ShaderPropertyType.Texture:
            string textureDefaultName = shader.GetPropertyTextureDefaultName(propertyIndex);
            string name = material.GetTexture(propertyName).name;
            if (name != textureDefaultName)
            {
              config.Add(propertyName, (object) name);
              break;
            }
            break;
        }
      }
      return config;
    }

    public override string ToString() => this.ShaderName + string.Format(" ({0} changes)", (object) this.ShaderParams.Count);

    public static ShaderConfig Default => new ShaderConfig()
    {
      ShaderName = "LFOAdditive"
    };

    [JsonConverter(typeof (ShaderConfig))]
    public class ShaderConfigJsonConverter : JsonConverter
    {
      public override bool CanConvert(Type objectType) => objectType == typeof (ShaderConfig);

      public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
      {
        JObject jobject = JObject.Load(reader);
        ShaderConfig shaderConfig = new ShaderConfig();
        shaderConfig.ShaderName = (string) jobject["ShaderName"];
        shaderConfig.ShaderParams = new Dictionary<string, object>();
        foreach (JToken child in jobject["ShaderParams"].Children())
        {
          if (child is JProperty jproperty)
          {
            string name = jproperty.Name;
            JToken jtoken1 = jproperty.Value;
            switch (jtoken1.Type)
            {
              case JTokenType.Object:
                JToken jtoken2 = jtoken1[(object) "x"];
                JToken jtoken3 = jtoken1[(object) "r"];
                if (jtoken2 != null)
                {
                  Vector4 zero = Vector4.zero with
                  {
                    x = (float) jtoken1[(object) "x"],
                    y = (float) jtoken1[(object) "y"],
                    z = (float) jtoken1[(object) "z"],
                    w = (float) jtoken1[(object) "w"]
                  };
                  shaderConfig.Add(name, (object) zero);
                  continue;
                }
                if (jtoken3 != null)
                {
                  shaderConfig.Add(name, (object) new Color(0.0f, 0.0f, 0.0f, 0.0f)
                  {
                    r = (float) jtoken1[(object) "r"],
                    g = (float) jtoken1[(object) "g"],
                    b = (float) jtoken1[(object) "b"],
                    a = (float) jtoken1[(object) "a"]
                  });
                  continue;
                }
                continue;
              case JTokenType.Integer:
              case JTokenType.Float:
                shaderConfig.Add(name, (object) jtoken1.ToObject<float>());
                continue;
              case JTokenType.String:
                shaderConfig.Add(name, (object) jtoken1.ToString());
                continue;
              default:
                continue;
            }
          }
        }

        //serializer.Populate(jObject.CreateReader(), toReturn);

        return (object) shaderConfig;
      }

      public override bool CanWrite => false;

      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {

      }
    }
  }
}
