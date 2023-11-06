using LFO.Shared;
using UnityEngine;
using ILogger = LFO.Shared.ILogger;
using UnityObject = UnityEngine.Object;

namespace LFO
{
    public class AssetManager : BaseAssetManager
    {
        private readonly Dictionary<string, (Type, UnityObject)> _cachedAssets = new();

        private readonly ILogger _logger;

        public AssetManager(string bundlePath)
        {
            _logger = ServiceProvider.GetService<ILogger>();

            AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
            string[] paths = bundle.GetAllAssetNames();

            foreach (string path in paths)
            {
                UnityObject asset = bundle.LoadAsset(path);
                Type assetType = asset.GetType();
                _cachedAssets[asset.name.ToLowerInvariant()] = (assetType, asset);
                _logger.LogDebug($"Loaded {assetType.Name} {asset.name} from {path}");
            }
        }

        public override T GetAsset<T>(string name)
        {
            name = name.ToLowerInvariant();

            if (_cachedAssets.TryGetValue(name, out (Type, UnityObject) asset))
            {
                if (typeof(T).IsAssignableFrom(asset.Item1))
                {
                    return (T)asset.Item2;
                }

                _logger.LogError($"Asset {name} is not of type {typeof(T)}");
                return null;
            }

            if (GetRenamedAssetName(name) is { } newName)
            {
                return GetAsset<T>(newName);
            }

            _logger.LogError($"Couldn't find asset {name}");
            return null;
        }

        public override bool TryGetAsset<T>(string name, out T asset)
        {
            asset = default;

            name = name.ToLowerInvariant();

            if (!_cachedAssets.TryGetValue(name, out (Type, UnityObject) foundAsset))
            {
                return GetRenamedAssetName(name) is { } newName && TryGetAsset(newName, out asset);
            }

            if (!typeof(T).IsAssignableFrom(foundAsset.Item1))
            {
                return false;
            }

            asset = (T)foundAsset.Item2;
            return true;
        }
    }
}