using KSP;
using Newtonsoft.Json;
using UnityEngine;

namespace LuxsFlamesAndOrnaments
{
    [Serializable]
    public struct LFOConfig
    {
        public string partName; //Target partName
        public string hierarchyPath; //Something like SeaLevelPlume>Mach1 (parent>target), Support for ##FIND(gameObject name)
        //public string Shader; //LFO Base
        public List<FloatParam> FloatParams;

        public static string Serialize(List<LFOConfig> config)
        {
            return JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented);
        }

        public static bool TryDeserialize(string rawJson, out LFOConfig result)
        {
            try
            {
                result = JsonUtility.FromJson<LFOConfig>(rawJson);
                return true;
            }
            catch
            {
                result = new LFOConfig();
                return false;
            }
        }
        public static List<LFOConfig> Deserialize(string rawJson)
        {
            return JsonConvert.DeserializeObject<List<LFOConfig>>(rawJson);
        }

        public void ApplyToPrefab(GameObject prefab)
        {
            LFOThrottleData throttleData = null;
            if(hierarchyPath.StartsWith("##FIND(") && hierarchyPath.EndsWith(")"))
            {
                string targetGoName = hierarchyPath.Substring(6, hierarchyPath.Length - 7);

                var possibleTargets = prefab.GetComponentsInChildren<LFOThrottleData>(true);

                foreach(var possibleTarget in possibleTargets)
                {
                    if (possibleTarget.name == targetGoName)
                    {
                        throttleData = possibleTarget;
                        break;
                    }
                }
            }
            else
            {
                string[] goNames = hierarchyPath.Split('>');
                var possibleTargets = prefab.GetComponentsInChildren<LFOThrottleData>(true);

                foreach (var possibleTarget in possibleTargets)
                {
                    if (possibleTarget.name == goNames[1] && possibleTarget.transform.parent.name == goNames[0])
                    {
                        throttleData = possibleTarget;
                        break;
                    }
                }
            }

            if(throttleData is null)
            {
                throw new NullReferenceException($"Couldn't find a LFOThrottleData with path {hierarchyPath}");
            }

            throttleData.FloatParams = FloatParams;
        }

        public static string GetHierarchyPath(Transform transform)
        {
            return $"{transform.parent.name}>{transform.name}";
        }

        public static LFOConfig CreateConfig(LFOThrottleData data)
        {
            return new()
            {
                partName = data.transform.GetComponentInParent<CorePartData>(true).Data.partName,
                hierarchyPath = LFOConfig.GetHierarchyPath(data.transform),
                FloatParams = data.FloatParams
            };
        }
    }
}
