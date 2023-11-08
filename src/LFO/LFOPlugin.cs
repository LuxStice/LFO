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

    private const string PlumeConfigAssetPrefix = "lfo/plumes";

    private void Awake()
    {
        Assembly.LoadFrom(System.IO.Path.Combine(
            System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
            "LFO.Editor.dll"
        ));

        Loading.AddAssetLoadingAction(
            "plumes",
            "Loading LFO plumes",
            ImportPlumes,
            "json"
        );

        Loading.AddAddressablesLoadingAction<TextAsset>(
            "Loading FLO plumes from addressables",
            Constants.ConfigLabel,
            ImportAddressablePlumes
        );
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

        ServiceProvider.RegisterService<ILogger>(new BepInExLogger(Logger));
        ServiceProvider.RegisterService<IAssetManager>(new RuntimeAssetManager(Constants.AssetLabel));

        Harmony.CreateAndPatchAll(typeof(LFOPlugin).Assembly);
    }

    private static List<(string name, UnityObject asset)> ImportPlumes(string internalPath, string filename)
    {
        string jsonData = File.ReadAllText(filename);

        RegisterPlumeConfig(filename, jsonData);

        return new List<(string name, UnityObject asset)>
        {
            ($"{PlumeConfigAssetPrefix}/{internalPath}", new TextAsset(jsonData))
        };
    }

    private static void ImportAddressablePlumes(TextAsset asset)
    {
        RegisterPlumeConfig(asset.name, asset.text);
    }

    private static void RegisterPlumeConfig(string filename, string json)
    {
        LFOConfig config = LFOConfig.Deserialize(json);

        if (config.PlumeConfigs is null && config.PartName is null)
        {
            throw new Exception($"Plume config {filename} is not valid!");
        }

        ConfigManager.RegisterLFOConfig(config.PartName, config);
    }
}