using LFO.Shared;
using UnityEngine;
using ILogger = LFO.Shared.ILogger;
using UnityObject = UnityEngine.Object;

namespace LFO
{
    public class AssetManager : IAssetManager
    {
        private readonly Dictionary<string, (Type, UnityObject)> _cachedAssets = new();

        private readonly ILogger _logger;

        private static readonly Dictionary<string, string> RenamedAssets = new()
        {
            {"vfx_exh_bell_j_01", "bell_j_1"},
            {"vfx_exh_bell_p2_1", "bell_p2_1"},
            {"vfx_exh_shock_p1_s1_0", "shock_1_pt1"},
            {"vfx_exh_shock_p2_s1_0", "shock_1_pt2"},
            {"vfx_exh_shock_p3_s1_0", "shock_1_pt3"},
            {"vfx_exh_shock_p4_s1_0", "shock_1_pt4"},
        };

        public AssetManager(string bundlePath)
        {
            _logger = ServiceProvider.GetService<ILogger>();

            AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
            string[] paths = bundle.GetAllAssetNames();

            foreach (string path in paths)
            {
                UnityObject asset = bundle.LoadAsset(path);
                Type assetType = asset.GetType();
                _cachedAssets[asset.name] = (assetType, asset);
                _logger.LogDebug($"Loaded {assetType.Name} {asset.name} from {path}");
            }
        }

        public T GetAsset<T>(string name) where T : UnityObject
        {
            if (_cachedAssets.TryGetValue(name, out (Type, UnityObject) asset))
            {
                if (typeof(T).IsAssignableFrom(asset.Item1))
                {
                    return (T) asset.Item2;
                }

                _logger.LogError($"Asset {name} is not of type {typeof(T)}");
                return null;
            }

            if (RenamedAssets.TryGetValue(name, out string newName))
            {
                return GetAsset<T>(newName);
            }

            _logger.LogError($"Couldn't find asset {name}");
            return null;
        }

        public Mesh GetMesh(string meshPath)
        {
            if (GetAsset<GameObject>(meshPath) is { } fbxPrefab)
            {
                return fbxPrefab.TryGetComponent(out SkinnedMeshRenderer skinnedRenderer)
                    ? skinnedRenderer.sharedMesh
                    : fbxPrefab.GetComponent<MeshFilter>().mesh;
            }

            // obj's meshes are named as "meshName_#" with # being the meshID
            return GetAsset<GameObject>(meshPath.Remove(meshPath.Length - 2))
                ?.GetComponentInChildren<MeshFilter>()
                ?.mesh;
        }
    }
}