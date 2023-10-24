// Decompiled with JetBrains decompiler
// Type: LuxsFlamesAndOrnaments.LFO
// Assembly: lfo, Version=0.9.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2965BBBA-49CA-4B3F-B886-3391858B1BD3
// Assembly location: C:\Kerbal Space Program 2\BepInEx\plugins\lfo\lfo.dll

using KSP.Game;
using LuxsFlamesAndOrnaments.Settings;
using SpaceWarp.API.Assets;
// using System;
// using System.Collections.Generic;
using UnityEngine;
using static LuxsFlamesAndOrnaments.LuxsFlamesAndOrnamentsPlugin;

namespace LuxsFlamesAndOrnaments
{
    public class LFO
    {
        public const string RESOURCES_PATH = "lfo/lfo-resources/lfo/";
        public const string MESHES_PATH = RESOURCES_PATH + "meshes/";
        public const string NOISES_PATH = RESOURCES_PATH + "noise/";
        public const string TEXTURES_PATH = RESOURCES_PATH + "textures/";
        public const string PROFILES_PATH = RESOURCES_PATH + "profiles/";
        public const string SHADERS_PATH = RESOURCES_PATH + "shaders/";
        public static LFO Instance
        {
            get
            {
                if (LFO._instance == null)
                {
                    LFO lfo = new LFO();
                }
                return LFO._instance;
            }
        }


        private static LFO _instance;

        public Dictionary<string, LFOConfig> PartNameToConfigDict = new Dictionary<string, LFOConfig>();
        public Dictionary<string, Dictionary<string, PlumeConfig>> GameObjectToPlumeDict = new Dictionary<string, Dictionary<string, PlumeConfig>>();
        public Dictionary<string, Shader> LoadedShaders = new Dictionary<string, Shader>();

        public LFO()
        {
            _instance = this;
        }

        public static void RegisterLFOConfig(string partName, LFOConfig config)
        {
            if (!Instance.PartNameToConfigDict.ContainsKey(partName))
            {
            	Instance.PartNameToConfigDict.Add(partName, new LFOConfig());
            }
            if (!Instance.GameObjectToPlumeDict.ContainsKey(partName))
            {
            	Instance.GameObjectToPlumeDict.Add(partName, new Dictionary<string, PlumeConfig>());
            }

            Instance.PartNameToConfigDict[partName] = config;
        }

        public static bool TryGetConfig(string partName, out LFOConfig config)
        {
            if (Instance.PartNameToConfigDict.ContainsKey(partName))
            {
                config = Instance.PartNameToConfigDict[partName];
                return true;
            }
            config = (LFOConfig)null;
            return false;
        }

        public static bool TryGetMesh(string meshPath, out Mesh mesh)
        {
            mesh = null;
            if (SpaceWarp.API.Assets.AssetManager.TryGetAsset(LFO.MESHES_PATH + meshPath.ToLower() + ".fbx", out GameObject fbxPrefab))
            {
                if (fbxPrefab.TryGetComponent<SkinnedMeshRenderer>(out var skinnedRenderer))
                {
                    mesh = skinnedRenderer.sharedMesh;
                }
                else
                {
                    mesh = fbxPrefab.GetComponent<MeshFilter>().mesh;
                }
                return true;
            }
            else if (SpaceWarp.API.Assets.AssetManager.TryGetAsset(LFO.MESHES_PATH + meshPath.ToLower().Remove(meshPath.Length - 2) + ".obj", out GameObject objPrefab)) //obj's meshes are named as "meshName_#" with # being the meshID
            {
                if (objPrefab.GetComponentInChildren<MeshFilter>() is not null)
                {
                    mesh = objPrefab.GetComponentInChildren<MeshFilter>().mesh;
                    return true;
                }
            }
            return false;
        }

        public static Shader GetShader(string name)
        {
            if (LFO.Instance.LoadedShaders.ContainsKey(name))
                return LFO.Instance.LoadedShaders[name];
            throw new IndexOutOfRangeException($"[LFO] Shader {name} is not present on internal shader collection. Check logs for more information.");
        }

        internal static void RegisterPlumeConfig(string partName, string ID, PlumeConfig config)
        {
            if (Instance.GameObjectToPlumeDict.ContainsKey(partName))
            {
                if (Instance.GameObjectToPlumeDict[partName].ContainsKey(ID))
                {
                    Instance.GameObjectToPlumeDict[partName][ID] = config;
                }
                else
                {
                    Instance.GameObjectToPlumeDict[partName].Add(ID, config);
                }
            }
            else
            {
                Debug.LogWarning($"{partName} has no registered plume");
            }
        }

        internal static bool TryGetPlumeConfig(string partName, string ID, out PlumeConfig config)
        {
            if (Instance.GameObjectToPlumeDict.ContainsKey(partName))
                return Instance.GameObjectToPlumeDict[partName].TryGetValue(ID, out config);
            Debug.LogWarning($"{partName} has no registered plume");
            config = (PlumeConfig)null;
            return false;
        }
    }
}
