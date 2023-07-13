using KSP.Game;
using LuxsFlamesAndOrnaments.Settings;
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
        public static LFO Instance
        {
            get
            {
                if (_instance == null)
                    new LFO();
                return _instance;
            }
        }


        private static LFO _instance;

        public Dictionary<string, LFOConfig> PartNameToConfigDict = new();
        public Dictionary<string, Dictionary<string, PlumeConfig>> GameObjectToPlumeDict = new();
        public Dictionary<string, Shader> LoadedShaders = new();

        public LFO()
        {
            _instance = this;
        }

        public static void RegisterLFOConfig(string partName, LFOConfig config)
        {
            if (!Instance.PartNameToConfigDict.ContainsKey(partName))
            {
                Instance.PartNameToConfigDict.Add(partName, new());
            }
            if (!Instance.GameObjectToPlumeDict.ContainsKey(partName))
            {
                Instance.GameObjectToPlumeDict.Add(partName, new());
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
            else
            {
                config = null;
                return false;
            }
        }

        public static bool TryGetMesh(string meshPath, out Mesh mesh)
        {
            mesh = null;
            if (SpaceWarp.API.Assets.AssetManager.TryGetAsset(LFO.MESHES_PATH + meshPath.ToLower() + ".fbx", out GameObject fbxPrefab))
            {
                if(fbxPrefab.TryGetComponent<SkinnedMeshRenderer>(out var skinnedRenderer))
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

        public static Shader GetShader(string name) => Instance.LoadedShaders[name];

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
            else
            {
                Debug.LogWarning($"{partName} has no registered plume");
                config = null;
                return false;
            }
        }
    }
}
