// Decompiled with JetBrains decompiler
// Type: LuxsFlamesAndOrnaments.Settings.LFOConfig
// Assembly: lfo, Version=0.9.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2965BBBA-49CA-4B3F-B886-3391858B1BD3
// Assembly location: C:\Kerbal Space Program 2\BepInEx\plugins\lfo\lfo.dll

using HarmonyLib;
using KSP;
using KSP.Game;
using KSP.Modules;
using KSP.Sim.impl;
using KSP.VFX;
using MoonSharp.Interpreter.Tree.Statements;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RTG;
// using System;
// using System.Collections.Generic;
// using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using static KSP.VFX.ThrottleVFXManager;
using static LuxsFlamesAndOrnaments.LuxsFlamesAndOrnamentsPlugin;

namespace LuxsFlamesAndOrnaments.Settings
{

  [Serializable]
  public class LFOConfig
  {
    public static JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
    {
      Converters = (IList<JsonConverter>) new JsonConverter[4]
      {
        (JsonConverter) new Vec4Conv(),
        (JsonConverter) new Vec3Conv(),
        (JsonConverter) new Vec2Conv(),
        (JsonConverter) new ColorConv()
      },
      Formatting = Formatting.Indented,
      NullValueHandling = NullValueHandling.Ignore,
      ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    };

    public string partName;
    public Dictionary<string, List<PlumeConfig>> PlumeConfigs;

    public static string Serialize(LFOConfig config) => JsonConvert.SerializeObject((object) config, LFOConfig.serializerSettings);

    public static LFOConfig Deserialize(string rawJson) => JsonConvert.DeserializeObject<LFOConfig>(rawJson, LFOConfig.serializerSettings);

    internal void InstantiatePlume(string partName, ref GameObject prefab)
    {
      ThrottleVFXManager component1 = prefab.GetComponent<ThrottleVFXManager>();
      List<ThrottleVFXManager.EngineEffect> engineEffectList = new List<ThrottleVFXManager.EngineEffect>();
      List<string> stringList = new List<string>();
      foreach (KeyValuePair<string, List<PlumeConfig>> plumeConfig in this.PlumeConfigs)
      {
        Transform transform = TransformExtension.FindChildRecursive(prefab.transform, plumeConfig.Key);
        if (transform == null)
        {
          string str = "[LFO] " + plumeConfig.Key + " [vfx_exh]";
          transform = TransformExtension.FindChildRecursive(prefab.transform, str);
          if (transform == null)
          {
            LuxsFlamesAndOrnamentsPlugin.LogWarning((object) ("Couldn't find GameObject named " + plumeConfig.Key + " to be set as parent. Trying to create under thrustTransform"));
            if (TransformExtension.FindChildRecursive(prefab.transform, "thrustTransform") == null)
              throw new NullReferenceException("Couldn't find GameObject named thrustTransform to enforce plume creation");
            transform = new GameObject(plumeConfig.Key).transform;
            transform.SetParent(TransformExtension.FindChildRecursive(prefab.transform, "thrustTransform"));
            transform.localRotation = Quaternion.Euler(270f, 0.0f, 0.0f);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
          }
        }
        if (transform != null)
        {
          GameObject gameObject1 = transform.gameObject;
          for (int index = transform.childCount - 1; index >= 0; --index)
          {
            stringList.Add(TransformExtension.FindChildRecursive(prefab.transform, transform.name).GetChild(index).gameObject.name);
            TransformExtension.FindChildRecursive(prefab.transform, transform.name).GetChild(index).gameObject.DestroyGameObjectImmediate();//Cleanup of every other plume (could be more efficient)
          }
          foreach (PlumeConfig config in plumeConfig.Value)
          {
            try
            {
              GameObject gameObject2 = new GameObject("[LFO] " + config.targetGameObject + " [vfx_exh]", new System.Type[3]
              {
                typeof (MeshRenderer),
                typeof (MeshFilter),
                typeof (LFOThrottleData)
              });
              LFOThrottleData component2 = gameObject2.GetComponent<LFOThrottleData>();
              component2.partName = partName;
              component2.material = config.GetMaterial();
              bool flag = false;
              if (component2.material.shader.name.ToLower().Contains("volumetric"))
              {
                flag = true;
                gameObject2.AddComponent<LFOVolume>();
              }
              MeshRenderer component3 = gameObject2.GetComponent<MeshRenderer>();
              MeshFilter component4 = gameObject2.GetComponent<MeshFilter>();
              if (flag)
              {
                GameObject gameObject3 = !(config.meshPath.ToLower() == "cylinder") ? GameObject.CreatePrimitive(PrimitiveType.Cube) : GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                component4.mesh = gameObject3.GetComponent<MeshFilter>().sharedMesh;
                UnityEngine.Object.Destroy((UnityEngine.Object) gameObject3);
              }
              else
              {
                Mesh mesh;
                LFO.TryGetMesh(config.meshPath, out mesh);
                if (mesh != null)
                  component4.mesh = mesh;
                else
                  Debug.LogWarning((object) ("Couldn't find mesh at " + config.meshPath + " for " + config.targetGameObject));
              }
              LFO.RegisterPlumeConfig(partName, gameObject2.name, config);
              gameObject2.transform.parent = TransformExtension.FindChildRecursive(prefab.transform, transform.name).transform;
              gameObject2.layer = 1;
              gameObject2.transform.localPosition = config.Position;
              gameObject2.transform.localRotation = Quaternion.Euler(config.Rotation);
              gameObject2.transform.localScale = config.Scale;
              component3.shadowCastingMode = ShadowCastingMode.Off;
              component3.enabled = false;
              engineEffectList.Add(new ThrottleVFXManager.EngineEffect()
              {
                EffectReference = gameObject2
              });
            }
            catch (Exception ex)
            {
              LuxsFlamesAndOrnamentsPlugin.LogError((object) string.Format("{0} was not created! \tException:\n{1}", (object) config.targetGameObject, (object) ex));
            }
          }
        }
      }
      ThrottleVFXManager throttleVfxManager1 = component1;
      if (throttleVfxManager1.FXModeActionEvents == null)
      {
        ThrottleVFXManager throttleVfxManager2 = throttleVfxManager1;
        ThrottleVFXManager.FXModeActionEvent[] fxModeActionEventArray = new ThrottleVFXManager.FXModeActionEvent[1];
        ThrottleVFXManager.FXModeActionEvent fxModeActionEvent = new ThrottleVFXManager.FXModeActionEvent();
        fxModeActionEvent.EngineModeIndex = 0;
        fxModeActionEvent.ActionEvents = new ThrottleVFXManager.FXActionEvent[1]
        {
          new ThrottleVFXManager.FXActionEvent()
          {
            ModeEvent = (ThrottleVFXManager.FXmodeEvent) 10,
            EngineEffects = engineEffectList.ToArray()
          }
        };
        fxModeActionEventArray[0] = fxModeActionEvent;
        throttleVfxManager2.FXModeActionEvents = fxModeActionEventArray;
      }
      if (component1.FXModeActionEvents.Length == 0)
        component1.FXModeActionEvents = CollectionExtensions.AddItem<ThrottleVFXManager.FXModeActionEvent>((IEnumerable<ThrottleVFXManager.FXModeActionEvent>) component1.FXModeActionEvents, new ThrottleVFXManager.FXModeActionEvent()).ToArray<ThrottleVFXManager.FXModeActionEvent>();
      ThrottleVFXManager.FXModeActionEvent fxModeActionEvent1 = component1.FXModeActionEvents[0];
      ThrottleVFXManager.FXModeActionEvent fxModeActionEvent2 = fxModeActionEvent1;
      if (fxModeActionEvent2.ActionEvents == null)
        fxModeActionEvent2.ActionEvents = new ThrottleVFXManager.FXActionEvent[0];
      var fxActionEvent1 = fxModeActionEvent1.ActionEvents.FirstOrDefault<ThrottleVFXManager.FXActionEvent>(a => a.ModeEvent == FXmodeEvent.FXModeRunning);
      if (fxActionEvent1 == null)
      {
        fxModeActionEvent1.ActionEvents = CollectionExtensions.AddItem<ThrottleVFXManager.FXActionEvent>((IEnumerable<ThrottleVFXManager.FXActionEvent>) fxModeActionEvent1.ActionEvents, new ThrottleVFXManager.FXActionEvent()
        {
          ModeEvent = (ThrottleVFXManager.FXmodeEvent) 10,
          EngineEffects = new ThrottleVFXManager.EngineEffect[0]
        }).ToArray<ThrottleVFXManager.FXActionEvent>();
        fxActionEvent1 = fxModeActionEvent1.ActionEvents.FirstOrDefault<ThrottleVFXManager.FXActionEvent>(a => a.ModeEvent == FXmodeEvent.FXModeRunning);
      }
      ThrottleVFXManager.FXActionEvent fxActionEvent2 = fxActionEvent1;
      if (fxActionEvent2.EngineEffects == null)
        fxActionEvent2.EngineEffects = new ThrottleVFXManager.EngineEffect[0];
      fxActionEvent1.EngineEffects = CollectionExtensions.AddRangeToArray<ThrottleVFXManager.EngineEffect>(fxActionEvent1.EngineEffects, engineEffectList.ToArray());
    }
  }
}
