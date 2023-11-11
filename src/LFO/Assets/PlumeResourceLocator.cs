using LFO.Shared;
using UnityEngine;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace LFO.Assets
{
    public class PlumeResourceLocator : IResourceLocator
    {
        public string LocatorId => GetType().FullName;
        public IEnumerable<object> Keys { get; } = new List<object> { Constants.ConfigLabel };

        public bool Locate(object key, Type type, out IList<IResourceLocation> locations)
        {
            if (!Keys.Contains(key.ToString()))
            {
                locations = new List<IResourceLocation>();
                return false;
            }

            locations = LFOPlugin.Instance.PlumeConfigCache.Keys.Select(filename => new ResourceLocationBase(
                filename,
                filename,
                typeof(PlumeResourceProvider).FullName,
                typeof(TextAsset)
            )).Cast<IResourceLocation>().ToList();

            return true;
        }
    }
}