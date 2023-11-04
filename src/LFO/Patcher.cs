using HarmonyLib;
using KSP.OAB;
using KSP.Sim;
using KSP.Sim.impl;
using KSP.VFX;
using LFO.Shared;
using LFO.Shared.Components;
using LFO.Shared.Configs;
using UnityEngine;
using ILogger = LFO.Shared.ILogger;
using UnityObject = UnityEngine.Object;

namespace LFO
{
    [HarmonyPatch]
    internal static class Patcher
    {
        private static readonly ILogger Logger = ServiceProvider.GetService<ILogger>();
        private static readonly IAssetManager AssetManager = ServiceProvider.GetService<AssetManager>();

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ObjectAssemblyPartTracker), nameof(ObjectAssemblyPartTracker.OnPartPrefabLoaded))]
        internal static void CreatePlumesForPart(IObjectAssemblyAvailablePart obj, ref GameObject prefab)
        {
            if (ConfigManager.GetConfig(obj.Name) is { } config)
            {
                InstantiatePlume(config, obj.Name, ref prefab);
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

            if (ConfigManager.GetConfig(model.Part.PartName) is { } config)
            {
                InstantiatePlume(config, model.Part.PartName, ref instance);
            }
        }

        private static void InstantiatePlume(LFOConfig lfoConfig, string partName, ref GameObject prefab)
        {
            var vfxManager = prefab.GetComponent<ThrottleVFXManager>();
            var effects = new List<ThrottleVFXManager.EngineEffect>();
            var deletedObjects = new List<string>();

            foreach ((string key, List<PlumeConfig> configs) in lfoConfig.PlumeConfigs)
            {
                Transform tParent = prefab.transform.FindChildRecursive(key);
                if (tParent == null)
                {
                    string sanitizedKey = $"[LFO] {key} [vfx_exh]";
                    tParent = prefab.transform.FindChildRecursive(sanitizedKey);
                    if (tParent == null)
                    {
                        Logger.LogWarning(
                            $"Couldn't find GameObject named {key} to be set as parent. Trying to create under thrustTransform"
                        );
                        Transform tTransform = prefab.transform.FindChildRecursive("thrustTransform");
                        if (tTransform == null)
                        {
                            throw new NullReferenceException(
                                "[LFO] Couldn't find GameObject named thrustTransform to enforce plume creation"
                            );
                        }

                        tParent = new GameObject(key).transform;
                        tParent.SetParent(prefab.transform.FindChildRecursive("thrustTransform"));
                        tParent.localRotation = Quaternion.Euler(270, 0, 0);
                        tParent.localPosition = Vector3.zero;
                        tParent.localScale = Vector3.one;
                    }
                }

                if (tParent == null)
                {
                    continue;
                }

                int childCount = tParent.childCount;
                for (int i = childCount - 1; i >= 0; i--)
                {
                    deletedObjects.Add(
                        prefab.transform
                            .FindChildRecursive(tParent.name)
                            .GetChild(i)
                            .gameObject
                            .name
                    );
                    prefab.transform
                        .FindChildRecursive(tParent.name)
                        .GetChild(i)
                        .gameObject
                        .DestroyGameObjectImmediate(); // Cleanup of every other plume (could be more efficient)
                    // TODO: Add way to avoid certain cleanups (particles etc)
                }

                foreach (PlumeConfig config in configs)
                {
                    SetupConfig(prefab, config, partName, tParent, ref effects);
                }
            }

            SetupVfxManager(vfxManager, effects);
        }

        private static void SetupConfig(
            GameObject prefab,
            PlumeConfig config,
            string partName,
            Transform tParent,
            ref List<ThrottleVFXManager.EngineEffect> effects
        )
        {
            try
            {
                var plume = new GameObject(
                    "[LFO] " + config.TargetGameObject + " [vfx_exh]",
                    typeof(MeshRenderer),
                    typeof(MeshFilter),
                    typeof(LFOThrottleData)
                );
                var throttleData = plume.GetComponent<LFOThrottleData>();
                throttleData.PartName = partName;
                throttleData.Material = config.GetMaterial();
                bool volumetric = false;

                if (throttleData.Material.shader.name.ToLower().Contains("volumetric"))
                {
                    volumetric = true;
                    plume.AddComponent<LFOVolume>();
                }

                var renderer = plume.GetComponent<MeshRenderer>();
                var filter = plume.GetComponent<MeshFilter>();

                if (volumetric)
                {
                    var gameObject = GameObject.CreatePrimitive(
                        config.MeshPath.ToLower() == "cylinder"
                            ? PrimitiveType.Cylinder
                            : PrimitiveType.Cube
                    );

                    filter.mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
                    UnityObject.Destroy(gameObject);
                }
                else
                {
                    if (AssetManager.GetMesh(config.MeshPath) is { } mesh)
                    {
                        filter.mesh = mesh;
                    }
                    else
                    {
                        Logger.LogWarning(
                            $"Couldn't find mesh at {config.MeshPath} for {config.TargetGameObject}"
                        );
                    }
                }

                ConfigManager.RegisterPlumeConfig(partName, plume.name, config);

                plume.transform.parent = prefab.transform.FindChildRecursive(tParent.name).transform;
                plume.layer = 1; //TransparentFX layer

                plume.transform.localPosition = config.Position;
                plume.transform.localRotation = Quaternion.Euler(config.Rotation);
                plume.transform.localScale = config.Scale;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.enabled = false;

                effects.Add(new ThrottleVFXManager.EngineEffect
                {
                    EffectReference = plume
                });
            }
            catch (Exception e)
            {
                Logger.LogError($"{config.TargetGameObject} was not created! \tException:\n{e}");
            }
        }

        private static void SetupVfxManager(
            ThrottleVFXManager vfxManager,
            List<ThrottleVFXManager.EngineEffect> effects
        )
        {
            vfxManager.FXModeActionEvents ??= new ThrottleVFXManager.FXModeActionEvent[]
            {
                new()
                {
                    EngineModeIndex = 0,
                    ActionEvents = new ThrottleVFXManager.FXActionEvent[]
                    {
                        new()
                        {
                            ModeEvent = ThrottleVFXManager.FXmodeEvent.FXModeRunning,
                            EngineEffects = effects.ToArray()
                        }
                    }
                }
            };

            if (vfxManager.FXModeActionEvents.Length == 0)
            {
                vfxManager.FXModeActionEvents = vfxManager.FXModeActionEvents
                    .AddCollectionItem(new ThrottleVFXManager.FXModeActionEvent())
                    .ToArray();
            }

            ThrottleVFXManager.FXModeActionEvent firstEngineMode = vfxManager.FXModeActionEvents[0];

            firstEngineMode.ActionEvents ??= Array.Empty<ThrottleVFXManager.FXActionEvent>();

            ThrottleVFXManager.FXActionEvent runningActionEvent = firstEngineMode.ActionEvents.FirstOrDefault(
                a => a.ModeEvent == ThrottleVFXManager.FXmodeEvent.FXModeRunning
            );
            if (runningActionEvent is null)
            {
                firstEngineMode.ActionEvents = firstEngineMode.ActionEvents.AddCollectionItem(
                    new ThrottleVFXManager.FXActionEvent
                    {
                        ModeEvent = ThrottleVFXManager.FXmodeEvent.FXModeRunning,
                        EngineEffects = Array.Empty<ThrottleVFXManager.EngineEffect>()
                    }).ToArray();
                runningActionEvent = firstEngineMode.ActionEvents.FirstOrDefault(
                    a => a.ModeEvent == ThrottleVFXManager.FXmodeEvent.FXModeRunning
                );
            }

            runningActionEvent!.EngineEffects ??= Array.Empty<ThrottleVFXManager.EngineEffect>();

            runningActionEvent.EngineEffects = runningActionEvent.EngineEffects.AddRange(effects.ToArray());
        }
    }
}