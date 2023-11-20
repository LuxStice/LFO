using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;
using LFO.Assets;
using LFO.Shared;
using LFO.Shared.Configs;
using SpaceWarp;
using SpaceWarp.API.Loading;
using SpaceWarp.API.Mods;
using UnityEngine;
using UnityEngine.AddressableAssets;
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

    internal new ManualLogSource Logger;

    internal readonly Dictionary<string, TextAsset> PlumeConfigCache = new();

    private void Awake()
    {
        Instance = this;
        Logger = base.Logger;

        Assembly.LoadFrom(System.IO.Path.Combine(
            System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
            "LFO.Editor.dll"
        ));

        Logger.LogDebug($"Registering {nameof(PlumeResourceProvider)} as resource provider.");
        Addressables.ResourceManager.ResourceProviders.Add(new PlumeResourceProvider());
        Logger.LogDebug($"Registering {nameof(PlumeResourceLocator)} as resource locator.");
        Addressables.AddResourceLocator(new PlumeResourceLocator());

        Logger.LogDebug($"Registering 'RegisterJsonPlumesAsAddressables' as a loading action.");
        Loading.AddAssetLoadingAction(
            "plumes",
            "Loading LFO plumes",
            RegisterJsonPlumesAsAddressables,
            "json"
        );

        Logger.LogDebug($"Registering 'ImportAddressablePlumes' as a loading action.");
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

        ServiceProvider.RegisterService<ILogger>(new BepInExLogger(Logger));
        ServiceProvider.RegisterService<IAssetManager>(new RuntimeAssetManager(Constants.AssetLabel));

        Harmony.CreateAndPatchAll(typeof(LFOPlugin).Assembly);
    }

    private List<(string name, UnityObject asset)> RegisterJsonPlumesAsAddressables(
        string internalPath,
        string filename
    )
    {
        var assets = new List<(string name, UnityObject asset)>();

        try
        {
            Logger.LogDebug($"Registering {filename} as addressable asset.");
            var asset = new TextAsset(File.ReadAllText(filename))
            {
                name = filename
            };

            assets.Add(($"{PlumeConfigAssetPrefix}/{internalPath}", asset));
            PlumeConfigCache[filename] = asset;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
        }

        return assets;
    }

    private void ImportAddressablePlumes(TextAsset asset)
    {
        Logger.LogDebug($"Registering {asset.name} as a plume config from addressables.");
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