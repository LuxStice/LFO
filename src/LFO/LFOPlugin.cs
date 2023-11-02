using BepInEx;
using HarmonyLib;
using SpaceWarp;
using SpaceWarp.API.Mods;
using UnityEngine;
using System.Reflection;
using LFO.Shared.Settings;
using JetBrains.Annotations;
using SpaceWarp.API.Loading;
using UnityObject = UnityEngine.Object;

namespace LFO;

/// <summary>
/// Main plugin class for the mod.
/// </summary>
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
public class LFOPlugin : BaseSpaceWarpPlugin
{
    /// <summary>
    /// The GUID of the mod.
    /// </summary>
    [PublicAPI] public const string ModGuid = MyPluginInfo.PLUGIN_GUID;

    /// <summary>
    /// The name of the mod.
    /// </summary>
    [PublicAPI] public const string ModName = MyPluginInfo.PLUGIN_NAME;

    /// <summary>
    /// The version of the mod.
    /// </summary>
    [PublicAPI] public const string ModVer = MyPluginInfo.PLUGIN_VERSION;

    public static LFOPlugin Instance;

    public static void Log(object data) => Instance.Logger.LogInfo(data);
    public static void LogDebug(object data) => Instance.Logger.LogDebug(data);
    public static void LogWarning(object data) => Instance.Logger.LogWarning(data);
    public static void LogError(object data) => Instance.Logger.LogError(data);

    public static string Path;

    public List<string> RequestedShaders = new();
    public List<string> RequestedMeshes = new(); // TODO: Add caching of meshes

    private void Awake()
    {
        Loading.AddAssetLoadingAction("plumes", "Loading LFO plumes", ImportPlumes, "json");
    }

    public override void OnPreInitialized()
    {
        base.OnPreInitialized();
        Path = PluginFolderPath;

        Assembly.LoadFrom(System.IO.Path.Combine(Path, "LFO.Editor.dll"));
    }

    public override void OnInitialized()
    {
        base.OnInitialized();

        Instance = this;

        Harmony.CreateAndPatchAll(typeof(LFOPlugin).Assembly);

        SaveLoad.AddFlowActionToCampaignLoadAfter(
            new LoadShadersFlowAction(RequestedShaders),
            "Loading Colony Data"
        );
    }

    private List<(string name, UnityObject asset)> ImportPlumes(string internalPath, string filename)
    {
        string jsonData = File.ReadAllText(filename);
        var assets = new List<(string name, UnityObject asset)>();

        var config = LFOConfig.Deserialize(jsonData);

        if (config.PlumeConfigs is null && config.PartName is null)
        {
            throw new Exception($"Plume config found at {filename} is not valid!");
        }

        Shared.LFO.RegisterLFOConfig(config.PartName, config);
        assets.Add(new ValueTuple<string, UnityObject>($"lfo/plumes/{internalPath}", new TextAsset(jsonData)));

        foreach (List<PlumeConfig> plumeConfigList in config.PlumeConfigs!.Values)
        {
            foreach (PlumeConfig plumeConfig in plumeConfigList)
            {
                string shaderName = plumeConfig.ShaderSettings.ShaderName;
                if (!RequestedShaders.Contains(shaderName))
                {
                    RequestedShaders.Add(shaderName);
                }
            }
        }

        return assets;
    }
}