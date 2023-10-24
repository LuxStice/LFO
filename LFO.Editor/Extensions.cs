// Decompiled with JetBrains decompiler
// Type: Extensions
// Assembly: LFO.Editor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1B2710C6-D792-44BD-9937-785792B862AE
// Assembly location: C:\KSP2Mods\LFO\Unity\LFO\Editor\LFO.Editor.dll

using LuxsFlamesAndOrnaments.Settings;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class Extensions
{
  public static Material GetEditorMaterial(this PlumeConfig config)
  {
    Shader shader = Shader.Find(config.ShaderSettings.ShaderName);
    if (shader == null)
    {
      Debug.LogError((object) ("Couldn't find shader " + config.ShaderSettings.ShaderName));
      return (Material) null;
    }
    Material editorMaterial = new Material(shader);
    foreach (KeyValuePair<string, object> shaderParam in config.ShaderSettings.ShaderParams)
    {
      if (shaderParam.Value is JObject jobject)
      {
        if (jobject.ContainsKey("r"))
        {
          Color color = new Color(jobject["r"].ToObject<float>(), jobject["g"].ToObject<float>(), jobject["b"].ToObject<float>(), jobject["a"].ToObject<float>());
          editorMaterial.SetColor(shaderParam.Key, color);
        }
        else if (jobject.ContainsKey("x"))
        {
          Vector4 zero = Vector4.zero with
          {
            x = jobject["x"].ToObject<float>(),
            y = jobject["y"].ToObject<float>()
          };
          if (jobject.ContainsKey("z"))
            zero.z = jobject["z"].ToObject<float>();
          if (jobject.ContainsKey("w"))
            zero.w = jobject["w"].ToObject<float>();
          editorMaterial.SetVector(shaderParam.Key, zero);
        }
      }
      else
      {
        switch (shaderParam.Value)
        {
          case Color color1:
            editorMaterial.SetColor(shaderParam.Key, color1);
            break;
          case Vector2 vector2:
            editorMaterial.SetVector(shaderParam.Key, (Vector4) vector2);
            break;
          case Vector3 vector3:
            editorMaterial.SetVector(shaderParam.Key, (Vector4) vector3);
            break;
          case Vector4 vector4:
            editorMaterial.SetVector(shaderParam.Key, vector4);
            break;
          case float num1:
            editorMaterial.SetFloat(shaderParam.Key, num1);
            break;
          case double num2:
            editorMaterial.SetFloat(shaderParam.Key, (float) num2);
            break;
          case int num3:
            editorMaterial.SetFloat(shaderParam.Key, (float) num3);
            break;
          case string str:
            string message = Path.Combine("Assets", "LFO", "Noise", str + ".png");
            Debug.Log((object) message);
            editorMaterial.SetTexture(shaderParam.Key, (Texture) AssetDatabase.LoadAssetAtPath<Texture2D>(message));
            break;
        }
      }
    }
    return editorMaterial;
  }
}
