using HarmonyLib;
using KSP.OAB;
using KSP.Sim;
using KSP.Sim.impl;
using UnityEngine;
using LFODeprecated = LuxsFlamesAndOrnaments.LFO;

namespace LFO
{
    [HarmonyPatch]
    internal static class Patcher
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ObjectAssemblyPartTracker), nameof(ObjectAssemblyPartTracker.OnPartPrefabLoaded))]
        internal static void CreatePlumesForPart(IObjectAssemblyAvailablePart obj, ref GameObject prefab)
        {
            if (Shared.LFO.TryGetConfig(obj.Name, out Shared.Settings.LFOConfig config))
            {
                config.InstantiatePlume(obj.Name, ref prefab);
            }

            if (LFODeprecated.TryGetConfig(obj.Name, out LuxsFlamesAndOrnaments.Settings.LFOConfig config2))
            {
                config2.InstantiatePlume(obj.Name, ref prefab);
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

            if (Shared.LFO.TryGetConfig(model.Part.PartName, out Shared.Settings.LFOConfig config))
            {
                config.InstantiatePlume(model.Part.PartName, ref instance);
                return;
            }

            if (LFODeprecated.TryGetConfig(model.Part.PartName, out LuxsFlamesAndOrnaments.Settings.LFOConfig config2))
            {
                config2.InstantiatePlume(model.Part.PartName, ref instance);
            }
        }
    }
}