// Decompiled with JetBrains decompiler
// Type: LuxsFlamesAndOrnaments.Settings.PlumeConfig
// Assembly: lfo, Version=0.9.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2965BBBA-49CA-4B3F-B886-3391858B1BD3
// Assembly location: C:\Kerbal Space Program 2\BepInEx\plugins\lfo\lfo.dll

using Newtonsoft.Json;
// using System;
// using System.Collections.Generic;
using UnityEngine;

namespace LuxsFlamesAndOrnaments.Settings
{
  [Serializable]
  public class PlumeConfig
  {
    public string meshPath;
    public string targetGameObject;//Name that the gameObject will have
    public ShaderConfig ShaderSettings;
    public Vector3 Position, Scale = Vector3.one, Rotation;
    public List<FloatParam> FloatParams = new List<FloatParam>();

    public static string Serialize(List<PlumeConfig> config) => JsonConvert.SerializeObject((object) config, Formatting.Indented);

    public static List<PlumeConfig> Deserialize(string rawJson) => JsonConvert.DeserializeObject<List<PlumeConfig>>(rawJson);

    public static PlumeConfig CreateConfig(LFOThrottleData data) => new PlumeConfig()
    {
      meshPath = data.GetComponent<MeshFilter>().mesh.name,
      Position = data.transform.localPosition,
      Rotation = data.transform.localRotation.eulerAngles,
      Scale = data.transform.localScale,
      FloatParams = data.FloatParams
    };

    public Material GetMaterial() => this.ShaderSettings.ToMaterial();

    public override string ToString() => this.targetGameObject + " - " + this.meshPath;
  }
}
