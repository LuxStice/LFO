using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace LFO.Assets
{
    public class PlumeResourceProvider : ResourceProviderBase
    {
        public override void Provide(ProvideHandle provideHandle)
        {
            try
            {
                string file = provideHandle.Location.InternalId;
                LFOPlugin.Instance.Logger.LogDebug($"Fetching plume config {file} from addressables.");

                TextAsset asset = LFOPlugin.Instance.PlumeConfigCache[file];
                provideHandle.Complete(asset, true, null);
            }
            catch (Exception e)
            {
                LFOPlugin.Instance.Logger.LogError(e);
                provideHandle.Complete<AssetBundle>(null, false, e);
            }
        }
    }
}