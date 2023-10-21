// Decompiled with JetBrains decompiler
// Type: LuxsFlamesAndOrnaments.LuxsFlamesAndOrnamentsPlugin
// Assembly: lfo, Version=0.9.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2965BBBA-49CA-4B3F-B886-3391858B1BD3
// Assembly location: C:\Kerbal Space Program 2\BepInEx\plugins\lfo\lfo.dll

using BepInEx;
using HarmonyLib;
using KSP.Game.Flow;
using SpaceWarp.API.Loading;
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
// using System;
// using System.Collections.Generic;

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

    public static void LogLevel(LogLevel Level, object Data) => Instance.Logger.Log(Level, Data);
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
        private List<string> requestedShaders;
        public LoadShadersFlowAction(List<string> requestedShaders) : base("Loading LFO Shaders...")
        {
            this.requestedShaders = requestedShaders;
        }

        public override void DoAction(Action resolve, Action<string> reject)
        {
            LuxsFlamesAndOrnamentsPlugin.Log((object)("Loading LFO Shaders. " + string.Join(", ", (IEnumerable<string>)this.requestedShaders)));
            for (int index = 0; index < this.requestedShaders.Count; ++index)
            {
                string requestedShader = this.requestedShaders[index];
                if (!LFO.Instance.LoadedShaders.ContainsKey(requestedShader))
                {
                    string str = LFO.SHADERS_PATH + requestedShader.Replace('/', '-') + ".mat";
                    Material asset;
                    try
                    {
                        asset = AssetManager.GetAsset<Material>(str);
                    }
                    catch (IndexOutOfRangeException ex)
                    {
                        LuxsFlamesAndOrnamentsPlugin.LogError((object)("Error loading " + requestedShader + ". Shader material does not exists or can't be found.\n Key: " + str));
                        continue;
                    }
                    if (asset == null)
                        LuxsFlamesAndOrnamentsPlugin.LogError((object)("Error loading " + requestedShader + ". Loaded object at " + str + "'s is not a material!"));
                    else if (asset.shader == null)
                    {
                        LuxsFlamesAndOrnamentsPlugin.LogError((object)("Error loading " + requestedShader + ". Loaded object at " + str + "'s material doesn't have a shader!"));
                    }
                    else
                    {
                        if (asset.shader.name.ToLower() != requestedShader.ToLower())
                            LuxsFlamesAndOrnamentsPlugin.LogWarning((object)("Shader name '" + asset.shader.name.ToLower() + "' is different from key '" + requestedShader.ToLower() + "'!"));
                        LFO.Instance.LoadedShaders.Add(requestedShader, asset.shader);
                    }
                }
            }
            LFO.Instance.LoadedShaders.Keys.ForEach<string>((Action<string>)(a => this.requestedShaders.Remove(a)));
            LuxsFlamesAndOrnamentsPlugin.Log((object)("LFO Shaders loaded. " + string.Join(", ", (IEnumerable<string>)LFO.Instance.LoadedShaders.Keys)));
            resolve();
        }

        private void StoreShader(Shader shader)
        {
        }
    }
}
