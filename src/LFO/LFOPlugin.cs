using System.Reflection;
using BepInEx;
using HarmonyLib;
using JetBrains.Annotations;
using LFO.Shared;
using LFO.Shared.Configs;
using SpaceWarp;
using SpaceWarp.API.Loading;
using SpaceWarp.API.Mods;
using UnityEngine;
using ILogger = LFO.Shared.ILogger;
using UnityObject = UnityEngine.Object;

namespace LFO;

/// <summary>
/// Main plugin class for the mod.
/// </summary>
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
public class LFOPlugin : BaseSpaceWarpPlugin
{
    #region API fields

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

    /// <summary>
    /// The instance of the mod.
    /// </summary>
    [PublicAPI] public static LFOPlugin Instance;

    /// <summary>
    /// The path to the mod folder.
    /// </summary>
    [PublicAPI] public static string Path;

    #endregion

    private const string BundlePath = "assets/bundles/lfo-resources";
    private const string PlumeConfigAssetPrefix = "lfo/plumes";

    public List<string> RequestedShaders = new();
    public List<string> RequestedMeshes = new(); // TODO: Add caching of meshes

    private void Awake()
    {
        Assembly.LoadFrom(System.IO.Path.Combine(
            System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
            "LFO.Editor.dll"
        ));

        Loading.AddAssetLoadingAction("plumes", "Loading LFO plumes", ImportPlumes, "json");
    }

    public override void OnPreInitialized()
    {
        base.OnPreInitialized();
        Path = PluginFolderPath;

        ServiceProvider.RegisterService<ILogger>(new BepInExLogger(Logger));
        ServiceProvider.RegisterService<IAssetManager>(new AssetManager($"{PluginFolderPath}/{BundlePath}"));
    }

    public override void OnInitialized()
    {
        base.OnInitialized();

        Instance = this;

        Harmony.CreateAndPatchAll(typeof(LFOPlugin).Assembly);
    }

    private List<(string name, UnityObject asset)> ImportPlumes(string internalPath, string filename)
    {
        string jsonData = File.ReadAllText(filename);
        var assets = new List<(string name, UnityObject asset)>();

        LFOConfig config = LFOConfig.Deserialize(jsonData);

        if (config.PlumeConfigs is null && config.PartName is null)
        {
            throw new Exception($"Plume config found at {filename} is not valid!");
        }

        ConfigManager.RegisterLFOConfig(config.PartName, config);
        assets.Add(($"{PlumeConfigAssetPrefix}/{internalPath}", new TextAsset(jsonData)));

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