using LFO.Shared;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using ILogger = LFO.Shared.ILogger;
using UnityObject = UnityEngine.Object;

namespace LFO
{
    public class RuntimeAssetManager : BaseAssetManager
    {
        private readonly Dictionary<string, (Type type, UnityObject asset)> _cachedAssets = new();

        private static readonly ILogger Logger = ServiceProvider.GetService<ILogger>();

        public override string GetAssetPath<T>(string name)
        {
            throw new Exception("This method is not supported in runtime.");
        }

        public RuntimeAssetManager(string assetLabel)
        {
            Addressables.LoadAssetsAsync<UnityObject>(
                assetLabel,
                null,
                true
            ).Completed += OnCompleted;

            return;

            void OnCompleted(AsyncOperationHandle<IList<UnityObject>> results)
            {
                if (results.Status != AsyncOperationStatus.Succeeded)
                {
                    Logger.LogError($"Failed to load assets from {assetLabel}: {results.OperationException}");
                    return;
                }

                foreach (UnityObject asset in results.Result)
                {
                    Type assetType = asset.GetType();
                    _cachedAssets[asset.name.ToLowerInvariant()] = (assetType, asset);
                    Logger.LogDebug($"Loaded {assetType.Name} {asset.name} from {assetLabel}");
                }
            }
        }

        public override T GetAsset<T>(string name)
        {
            name = name.ToLowerInvariant();

            if (_cachedAssets.TryGetValue(name, out (Type type, UnityObject asset) result))
            {
                if (typeof(T).IsAssignableFrom(result.type))
                {
                    return (T)result.asset;
                }

                Logger.LogError($"Asset {name} is not of type {typeof(T)}");
                return null;
            }

            if (GetRenamedAssetName(name) is { } newName)
            {
                return GetAsset<T>(newName);
            }

            Logger.LogError($"Couldn't find asset {name}");
            return null;
        }

        public override bool TryGetAsset<T>(string name, out T asset)
        {
            asset = default;

            name = name.ToLowerInvariant();

            if (!_cachedAssets.TryGetValue(name, out (Type type, UnityObject asset) result))
            {
                return GetRenamedAssetName(name) is { } newName && TryGetAsset(newName, out asset);
            }

            if (!typeof(T).IsAssignableFrom(result.type))
            {
                return false;
            }

            asset = (T)result.asset;
            return true;
        }
    }
}