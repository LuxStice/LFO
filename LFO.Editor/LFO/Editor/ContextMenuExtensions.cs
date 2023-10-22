// Decompiled with JetBrains decompiler
// Type: LFO.Editor.ContextMenuExtensions
// Assembly: LFO.Editor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1B2710C6-D792-44BD-9937-785792B862AE
// Assembly location: C:\KSP2Mods\LFO\Unity\LFO\Editor\LFO.Editor.dll

using UnityEditor;
using UnityEngine;

namespace LFO.Editor
{
  internal class ContextMenuExtensions
  {
    [MenuItem("GameObject/LFO/New Volumetric Plume")]
    private static void CreateVolumetricPlume(MenuCommand command)
    {
      GameObject context = (GameObject) command.context;
      GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
      primitive.name = "Plume";
      Object.DestroyImmediate((Object) primitive.GetComponent<Collider>());
      primitive.AddComponent<LFOVolume>();
      primitive.AddComponent<LFOThrottleData>();
      primitive.GetComponent<UnityEngine.Renderer>().sharedMaterial = new Material(Shader.Find("LFO/Volumetric (Additive)"));
      if ((Object) context != (Object) null)
      {
        primitive.transform.SetParent(context.transform);
        primitive.transform.localPosition = Vector3.zero;
        primitive.transform.localRotation = Quaternion.identity;
      }
      primitive.transform.localScale = Vector3.one * 5f;
    }

    [MenuItem("GameObject/LFO/New Volumetric Profiled Plume")]
    private static void CreateVolumetricProfiledPlume(MenuCommand command)
    {
      GameObject context = (GameObject) command.context;
      GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
      primitive.name = "Plume";
      Object.DestroyImmediate((Object) primitive.GetComponent<Collider>());
      primitive.AddComponent<LFOVolume>();
      primitive.AddComponent<LFOThrottleData>();
      primitive.GetComponent<UnityEngine.Renderer>().sharedMaterial = new Material(Shader.Find("LFO/Volumetric (Profiled)"));
      if ((Object) context != (Object) null)
      {
        primitive.transform.SetParent(context.transform);
        primitive.transform.localPosition = Vector3.zero;
        primitive.transform.localRotation = Quaternion.identity;
      }
      primitive.transform.localScale = Vector3.one * 5f;
    }
  }
}
