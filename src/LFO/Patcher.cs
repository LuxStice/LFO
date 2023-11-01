using HarmonyLib;
using KSP.OAB;
using KSP.Sim;
using KSP.Sim.impl;
using LFO.Shared.Settings;
using UnityEngine;

namespace LFO
{
    [HarmonyPatch]
    internal static class Patcher
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ObjectAssemblyPartTracker), nameof(ObjectAssemblyPartTracker.OnPartPrefabLoaded))]
        internal static void CreatePlumesForPart(IObjectAssemblyAvailablePart obj, ref GameObject prefab)
        {
            if (Shared.LFO.TryGetConfig(obj.Name, out LFOConfig config))
            {
                config.InstantiatePlume(obj.Name, ref prefab);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SimulationObjectView), nameof(SimulationObjectView.InitializeView))]
        internal static void CreatePlumesForSimObjView(
            ref GameObject instance,
            IUniverseView universe,
            SimulationObjectModel model
        )
        {
            if (model?.Part == null)
            {
                return;
            }

            if (Shared.LFO.TryGetConfig(model.Part.PartName, out LFOConfig config))
            {
                config.InstantiatePlume(model.Part.PartName, ref instance);
            }
        }
    }
}