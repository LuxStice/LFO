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

    public List<LFOConfig> configs;

    public override void OnInitialized()
    {
        base.OnInitialized();

        Instance = this;

        Harmony.CreateAndPatchAll(typeof(LuxsFlamesAndOrnamentsPlugin).Assembly);

        configs = new List<LFOConfig>();



        var allConfigsFiles = Directory.GetParent(PluginFolderPath).GetFiles("*.lfo", SearchOption.AllDirectories);
        Logger.LogDebug($"Started loading configs, found {allConfigsFiles.Length} instances");
        List<string> failedConfigs = new();
        foreach(var file in allConfigsFiles)
        {
            using (StreamReader sr = file.OpenText())
            {
                try
                {
                    string rawJson = sr.ReadToEnd();
                    sr.Close();

                    configs.AddRange(LFOConfig.Deserialize(rawJson));
                }
                catch
                {
                    Logger.LogError($"Couldn't load config at {file.FullName}");
                    failedConfigs.Add(file.FullName);
                }
            }
        }
        Logger.LogDebug($"Ended loading with {configs.Count} configs. {((failedConfigs.Count == 0) ? string.Empty : "\n\tMissing " + string.Join(", ", failedConfigs))}");
    }

    public bool TryGetLFOConfig(string key, out LFOConfig config)
    {
        config = default;
        try
        {
            config = configs.First(a => a.hierarchyPath == key);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
