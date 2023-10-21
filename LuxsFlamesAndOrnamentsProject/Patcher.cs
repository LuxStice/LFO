// Decompiled with JetBrains decompiler
// Type: LuxsFlamesAndOrnaments.Patcher
// Assembly: lfo, Version=0.9.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2965BBBA-49CA-4B3F-B886-3391858B1BD3
// Assembly location: C:\Kerbal Space Program 2\BepInEx\plugins\lfo\lfo.dll

using HarmonyLib;
using KSP.Game;
using KSP.OAB;
using KSP.Sim;
using KSP.Sim.impl;
using LuxsFlamesAndOrnaments.Settings;
using RTG;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static RTG.Object2ObjectSnap;

namespace LuxsFlamesAndOrnaments
{
  [HarmonyPatch]
  internal static class Patcher
  {
    [HarmonyPrefix]
    [HarmonyPatch(typeof (ObjectAssemblyPartTracker), nameof(ObjectAssemblyPartTracker.OnPartPrefabLoaded))]
    internal static void CreatePlumesForPart(IObjectAssemblyAvailablePart obj, ref GameObject prefab)
    {
            if(LFO.TryGetConfig(obj.Name, out LFOConfig config))
            {
      config.InstantiatePlume(obj.Name, ref prefab);
    }
        }

    [HarmonyPrefix]
    [HarmonyPatch(typeof (SimulationObjectView), nameof(SimulationObjectView.InitializeView))]
    internal static void CreatePlumesForSimObjView(ref GameObject instance, IUniverseView universe, SimulationObjectModel model)
    {
            if (model is not null && model.Part is not null)
            {
                if (LFO.TryGetConfig(model.Part.PartName, out LFOConfig config))
                {
      config.InstantiatePlume(model.Part.PartName, ref instance);
    }
  }
        }
    }
}
