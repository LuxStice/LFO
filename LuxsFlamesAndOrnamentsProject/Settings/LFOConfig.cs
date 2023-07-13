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
using UnityEngine;
using static KSP.VFX.ThrottleVFXManager;
using static LuxsFlamesAndOrnaments.LuxsFlamesAndOrnamentsPlugin;

namespace LuxsFlamesAndOrnaments.Settings
{

    [Serializable]
    public class LFOConfig
    {
        public static JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
        {
            Converters = new JsonConverter[] { new Vec4Conv(), new Vec3Conv(), new Vec2Conv(), new ColorConv() },
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public string partName;
        public Dictionary<string, List<PlumeConfig>> PlumeConfigs;

        public static string Serialize(LFOConfig config)
        {
            return JsonConvert.SerializeObject(config, serializerSettings);
        }
        public static LFOConfig Deserialize(string rawJson)
        {
            return JsonConvert.DeserializeObject<LFOConfig>(rawJson, serializerSettings);
        }

        internal void InstantiatePlume(string partName, ref GameObject prefab)
         {
            var VFXManager = prefab.GetComponent<ThrottleVFXManager>();
            List<EngineEffect> effects = new List<EngineEffect>();
            List<string> deletedObjects = new();

            foreach (var kvp in PlumeConfigs)
            {
                Transform tParent = prefab.transform.FindChildRecursive(kvp.Key);
                if (tParent is null)
                {
                    string sanitizedKey = "[LFO] " + kvp.Key + " [vfx_exh]";
                    tParent = prefab.transform.FindChildRecursive(sanitizedKey);
                    if (tParent is null)
                    {
                        throw new NullReferenceException($"Couldn't find GameObject named {kvp.Key} to be set as parent");
                    }
                }
                if (tParent is not null)
                {
                    GameObject parent = tParent.gameObject;
                    int childCount = tParent.childCount;
                    for (int i = childCount-1; i >= 0; i--)
                    {
                        deletedObjects.Add(prefab.transform.FindChildRecursive(tParent.name).GetChild(i).gameObject.name);
                        prefab.transform.FindChildRecursive(tParent.name).GetChild(i).gameObject.DestroyGameObjectImmediate();//Cleanup of every other plume (could be more efficient)
                        ///TODO: Add way to avoid certain cleanups (particles etc)
                    }

                    foreach (PlumeConfig config in kvp.Value)
                    {
                        try
                        {
                            GameObject plume = new("[LFO] " + config.targetGameObject + " [vfx_exh]", typeof(MeshRenderer), typeof(MeshFilter), typeof(LFOThrottleData));
                            LFOThrottleData throttleData = plume.GetComponent<LFOThrottleData>();
                            throttleData.partName = partName;
                            throttleData.material = config.GetMaterial();

                            MeshRenderer renderer = plume.GetComponent<MeshRenderer>();
                            MeshFilter filter = plume.GetComponent<MeshFilter>();

                            LFO.TryGetMesh(config.meshPath, out Mesh mesh);
                            if (mesh is not null)
                            {
                                filter.mesh = mesh;
                            }
                            else
                            {
                                Debug.LogWarning($"Couldn't find mesh at {config.meshPath} for {config.targetGameObject}");
                            }
                            LFO.RegisterPlumeConfig(partName, plume.name, config);

                            plume.transform.parent = prefab.transform.FindChildRecursive(tParent.name).transform;
                            plume.layer = 1;//TransparentFX layer

                            plume.transform.localPosition = config.Position;
                            plume.transform.localRotation = Quaternion.Euler(config.Rotation);
                            plume.transform.localScale = config.Scale;
                            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                            renderer.enabled = false;

                            effects.Add(new EngineEffect() { EffectReference = plume });
                        }
                        catch (Exception e)
                        {
                            LogError($"{config.targetGameObject} was not created! \tException:\n{e}");
                        }
                    }
                }
            }

            if(VFXManager.FXModeActionEvents.Length == 0)
            {
                VFXManager.FXModeActionEvents.AddItem(new FXModeActionEvent());
            }

            var firstEngineMode = VFXManager.FXModeActionEvents[0];
            var runningActionEvent = firstEngineMode.ActionEvents.First(a => a.ModeEvent == FXmodeEvent.FXModeRunning);
            if(runningActionEvent is null)
            {
                firstEngineMode.ActionEvents.AddItem(new FXActionEvent() { ModeEvent = FXmodeEvent.FXModeRunning, EngineEffects = new EngineEffect[0] });
                runningActionEvent = firstEngineMode.ActionEvents.First(a => a.ModeEvent == FXmodeEvent.FXModeRunning);
            }

            runningActionEvent.EngineEffects = runningActionEvent.EngineEffects.AddRangeToArray(effects.ToArray());

        }
    }
}