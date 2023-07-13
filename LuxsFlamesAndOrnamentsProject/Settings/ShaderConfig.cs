using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using UnityEngine;
using static LuxsFlamesAndOrnaments.LuxsFlamesAndOrnamentsPlugin;

namespace LuxsFlamesAndOrnaments.Settings
{
    [Serializable]
    public struct ShaderConfig
    {
        [JsonRequired]
        public string ShaderName;
        [JsonRequired]
        public Dictionary<string, object> ShaderParams;
        internal void Add(string paramName, object value)
        {
            ShaderParams.Add(paramName, value);
        }

        public Material ToMaterial()
        {
            Shader shader = LFO.GetShader(ShaderName);
            if (shader is null)
            {
                LogError($"Couldn't find shader {ShaderName}");
                return null;
            }
            Material material = new(shader);

            foreach (var kvp in ShaderParams)
            {
                switch (kvp.Value)
                {
                    case Color color:
                        material.SetColor(kvp.Key, color);
                        break;
                    case Vector2 vector2:
                        material.SetVector(kvp.Key, vector2);
                        break;
                    case Vector3 vector3:
                        material.SetVector(kvp.Key, vector3);
                        break;
                    case Vector4 vector4:
                        material.SetVector(kvp.Key, vector4);
                        break;
                    case Single number:
                        material.SetFloat(kvp.Key, number);
                        break;
                    case int integer:
                        material.SetFloat(kvp.Key, integer);
                        break;
                    case string textureName:
                        if (!Application.isEditor)
                        {
                            if (!SpaceWarp.API.Assets.AssetManager.TryGetAsset(LFO.NOISES_PATH + textureName + ".png", out Texture2D texture))
                            {
                                throw new NullReferenceException($"Couldn't find texture with path {"lfo/resources/textures" + textureName}. Make sure the textures have the right name!");
                            }
                            material.SetTexture(kvp.Key, texture);
                        }
                        break;
                }
            }

            return material;
        }

        public static ShaderConfig GenerateConfig(Material material)
        {
            Shader shader = material.shader;
            ShaderConfig config = new ShaderConfig();

            config.ShaderName = shader.name;
            config.ShaderParams = new Dictionary<string, object>();


            for (int i = 0; i < shader.GetPropertyCount(); i++)
            {
                string paramName = shader.GetPropertyName(i);
                var paramType = shader.GetPropertyType(i);

                if (paramType is UnityEngine.Rendering.ShaderPropertyType.Color)
                {
                    Color defaultColor = shader.GetPropertyDefaultVectorValue(i);
                    Color setColor = material.GetColor(paramName);

                    if (setColor != defaultColor)
                    {
                        config.Add(paramName, setColor);
                    }
                }
                else if (paramType is UnityEngine.Rendering.ShaderPropertyType.Vector)
                {
                    Vector4 defaultVector = shader.GetPropertyDefaultVectorValue(i);
                    Vector4 setVector = material.GetVector(paramName);

                    if (setVector != defaultVector)
                    {
                        config.Add(paramName, setVector);
                    }
                }
                else if (paramType is UnityEngine.Rendering.ShaderPropertyType.Float)
                {
                    float defaultFloat = shader.GetPropertyDefaultFloatValue(i);
                    float setFloat = material.GetFloat(paramName);

                    if (setFloat != defaultFloat)
                    {
                        config.Add(paramName, setFloat);
                    }
                }
                else if (paramType is UnityEngine.Rendering.ShaderPropertyType.Range)
                {
                    Vector2 defaultRange = shader.GetPropertyRangeLimits(i);
                    float defaultValue = shader.GetPropertyDefaultFloatValue(i);
                    float setRange = material.GetFloat(paramName);


                    if (setRange != defaultValue)
                    {
                        config.Add(paramName, setRange);
                    }
                }
                else if (paramType is UnityEngine.Rendering.ShaderPropertyType.Texture)
                {
                    string defaultTexture = shader.GetPropertyTextureDefaultName(i);
                    string setTexture = material.GetTexture(paramName).name;

                    if (setTexture != defaultTexture)
                    {
                        config.Add(paramName, setTexture);
                    }
                }
            }

            return config;
        }

        public override string ToString()
        {
            return ShaderName + $" ({ShaderParams.Count} changes)";
        }

        public static ShaderConfig Default => new()
        {
             ShaderName = "LFOAdditive"
        };

        [JsonConverter(typeof(ShaderConfig))]
        public class ShaderConfigJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                if (objectType == typeof(ShaderConfig))
                {
                    return true;
                }
                return false;
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JObject jObject = JObject.Load(reader);
                ShaderConfig toReturn = new();

                toReturn.ShaderName = (string)jObject["ShaderName"];
                toReturn.ShaderParams = new();
                foreach(JToken element in jObject["ShaderParams"].Children())
                {
                    if (element is JProperty property)
                    {
                        
                        string paramName = property.Name;
                        JToken value = property.Value;

                        switch (value.Type)
                        {
                            case JTokenType.Object:

                                JToken xToken = value["x"];
                                JToken rToken = value["r"];

                                if(xToken != null)
                                {
                                    Vector4 vector = Vector4.zero;

                                    vector.x = (float)value["x"];
                                    vector.y = (float)value["y"];
                                    vector.z = (float)value["z"];
                                    vector.w = (float)value["w"];
                                    toReturn.Add(paramName, vector);
                                }
                                else if(rToken is not null)
                                {
                                    Color color = new Color(0,0,0,0);

                                    color.r = (float)value["r"];
                                    color.g = (float)value["g"];
                                    color.b = (float)value["b"];
                                    color.a = (float)value["a"];
                                    toReturn.Add(paramName, color);
                                }
                                break;
                            case JTokenType.Integer:
                            case JTokenType.Float:
                                toReturn.Add(paramName, value.ToObject<float>());
                                break;
                            case JTokenType.String:
                                toReturn.Add(paramName, value.ToString());
                                break;
                        }
                    }
                }

                //serializer.Populate(jObject.CreateReader(), toReturn);

                return toReturn;
            }

            public override bool CanWrite => false;

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {

            }
        }
    }
}
