using Newtonsoft.Json;
using UnityEngine;

namespace LuxsFlamesAndOrnaments.Settings
{
    [Serializable]
    public class PlumeConfig
    {
        public string meshPath;
        public string targetGameObject;//Name that the gameObject will have
        public ShaderConfig ShaderSettings = new();
        public Vector3 Position, Scale = Vector3.one, Rotation;
        public List<FloatParam> FloatParams = new();

        public static string Serialize(List<PlumeConfig> config)
        {
            return JsonConvert.SerializeObject(config, Formatting.Indented);
        }
        public static List<PlumeConfig> Deserialize(string rawJson)
        {
            return JsonConvert.DeserializeObject<List<PlumeConfig>>(rawJson);
        }

        public static PlumeConfig CreateConfig(LFOThrottleData data)
        {
            return new()
            {
                meshPath = data.GetComponent<MeshFilter>().mesh.name,
                Position = data.transform.localPosition,
                Rotation = data.transform.localRotation.eulerAngles,
                Scale = data.transform.localScale,
                FloatParams = data.FloatParams
            };
        }

        public Material GetMaterial()
        {
            Material material = ShaderSettings.ToMaterial();
            return material;
        }

        public override string ToString()
        {
            return $"{targetGameObject} - {meshPath}";
        }
    }
}
