using BepInEx;
using HarmonyLib;
using KSP.UI.Binding;
using SpaceWarp;
using SpaceWarp.API.Assets;
using SpaceWarp.API.Mods;
using SpaceWarp.API.Game;
using SpaceWarp.API.Game.Extensions;
using SpaceWarp.API.UI;
using SpaceWarp.API.UI.Appbar;
using UnityEngine;
using System.IO;
using LuxsFlamesAndOrnaments.Settings;
using BepInEx.Logging;
using KSP.Game;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json;
using UnityEngine.ResourceManagement.AsyncOperations;
using KSP.Game.Flow;
using RTG;
using System.Runtime.InteropServices;

namespace LuxsFlamesAndOrnaments;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
public class LuxsFlamesAndOrnamentsPlugin : BaseSpaceWarpPlugin
{
    // These are useful in case some other mod wants to add a dependency to this one
    public const string ModGuid = MyPluginInfo.PLUGIN_GUID;
    public const string ModName = MyPluginInfo.PLUGIN_NAME;
    public const string ModVer = MyPluginInfo.PLUGIN_VERSION;
    public static LuxsFlamesAndOrnamentsPlugin Instance { get; set; }

    public static void Log(LogLevel Level, object Data) => Instance.Logger.Log(Level, Data);
    public static void Log(object Data) => Instance.Logger.LogInfo(Data);
    public static void LogDebug(object Data) => Instance.Logger.LogDebug(Data);
    public static void LogWarning(object Data) => Instance.Logger.LogWarning(Data);
    public static void LogError(object Data) => Instance.Logger.LogError(Data);

    public static string Path;

    public List<string> requestedShaders = new();
    public List<string> requestedMeshes = new();//Add caching of meshes

    void Awake()
    {
        SpaceWarp.API.Loading.Loading.AddAssetLoadingAction("plumes", "Loading LFO plumes", ImportPlumes, "json");
    }

    public override void OnPreInitialized()
    {
        base.OnPreInitialized();
        Path = PluginFolderPath;
    }

    public override void OnInitialized()
    {
        base.OnInitialized();

        Instance = this;

        Harmony.CreateAndPatchAll(typeof(LuxsFlamesAndOrnamentsPlugin).Assembly);

        SpaceWarp.API.Loading.SaveLoad.AddFlowActionToCampaignLoadAfter(new LoadShadersFlowAction(requestedShaders), "Loading Colony Data");
    }

    private List<(string name, UnityEngine.Object asset)> ImportPlumes(string internalPath, string filename)
    {
        var jsonData = File.ReadAllText(filename);
        List<(string name, UnityEngine.Object asset)> assets = new();

        LFOConfig config = LFOConfig.Deserialize(jsonData);

        if (config.PlumeConfigs is null && config.partName is null)
            throw new Exception($"Plume config found at {filename} is not valid!");

        LFO.RegisterLFOConfig(config.partName, config);
        assets.Add(new($"lfo/plumes/{internalPath}", new TextAsset(jsonData)));

        foreach (List<PlumeConfig> plumeConfigs in config.PlumeConfigs.Values)
        {
            foreach (PlumeConfig plumeConfig in plumeConfigs)
            {
                string toLoad = plumeConfig.ShaderSettings.ShaderName;
                if (!requestedShaders.Contains(toLoad))
                {
                    requestedShaders.Add(toLoad);
                }
            }
        }

        return assets;
    }

    public class LoadShadersFlowAction : FlowAction
    {
        private List<string> keys;
        public LoadShadersFlowAction(List<string> requestedShaders) : base("Loading LFO Shaders...")
        {
            this.keys = requestedShaders;
        }

        public override void DoAction(Action resolve, Action<string> reject)
        {
            for (int i = 0; i < keys.Count; i++)
            {
                string toLoad = keys[i];

                try
                {
                    string path = LFO.RESOURCES_PATH + toLoad.ToLower() + ".prefab";
                    GameObject gameObject = SpaceWarp.API.Assets.AssetManager.GetAsset<GameObject>(path);

                    if (gameObject is null)
                        throw new ArgumentNullException($"Couldn't find material at {toLoad}");
                    if (!gameObject.TryGetComponent(out Renderer renderer))
                        throw new ArgumentNullException($"Loaded object at {gameObject.name} doesn't have a renderer!");
                    if (renderer.material is null)
                        throw new ArgumentNullException($"Loaded object at {gameObject.name}'s renderer doesn't have a material!");
                    if (renderer.material.shader is null)
                        throw new ArgumentNullException($"Loaded object at {gameObject.name}'s material doesn't have a shader!");
                    if (renderer.material.shader.name != toLoad)
                        Debug.LogWarning($"Shader name '{renderer.material.shader.name}' is different from key '{toLoad}'!");

                    LFO.Instance.LoadedShaders.Add(toLoad, renderer.material.shader);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            resolve();
        }

        private void StoreShader(Shader shader)
        {
        }
    }
}
