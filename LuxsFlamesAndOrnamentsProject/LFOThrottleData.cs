using UnityEngine;
using UnityEngine.Serialization;
using KSP.Game;
using LuxsFlamesAndOrnaments;

[DisallowMultipleComponent]
public class LFOThrottleData : KerbalMonoBehaviour, IEngineFXData
{
    public Action<float, float, float, Vector3> TriggerUpdateVisuals { get; set; }
    public float Seed;
    public Renderer _renderer;
    public LFOConfig config;
    public List<FloatParam> FloatParams = new List<FloatParam>();

    public bool IsVisible()
    {
        return this._renderer != null && this._renderer.enabled;
    }

    public void ToggleVisibility(bool doTurnOn, ParticleSystemStopBehavior stopBehaviour = ParticleSystemStopBehavior.StopEmitting)
    {
        if (_renderer is not null)
            _renderer.enabled = doTurnOn;
    }

    void Awake()
    {
        _renderer = GetComponent<Renderer>();

        string path = LFOConfig.GetHierarchyPath(this.transform);
        if (!Application.isEditor)
        {
            try
            {
                if (LuxsFlamesAndOrnamentsPlugin.Instance != null)
                {
                    if (LuxsFlamesAndOrnamentsPlugin.Instance.TryGetLFOConfig(path, out var config))
                    {
                        this.config = config;
                        this.FloatParams = config.FloatParams;
                        System.Random rng = new(this.GetHashCode());
                        this.Seed = (float)rng.NextDouble();
                        _renderer.material.SetFloat("_Seed", this.Seed);
                    }
                    else
                    {
                        Debug.LogWarning($"{name} has no config!");
                    }
                }
            }
            catch
            {

            }

            //Load FloatParams from dictionary
            if (FloatParams is null)
            {
                Debug.LogError($"Params for {name} is null! disabling self");
                //enabled = false;
                return;
            }
            if (FloatParams.Count == 0)
            {
                Debug.LogError($"No params for {name}! disabling self");
                //enabled = false;
                return;
            }
        }
    }

    void OnEnable()
    {
        this.TriggerUpdateVisuals = (Action<float, float, float, Vector3>)Delegate.Combine(this.TriggerUpdateVisuals, new Action<float, float, float, Vector3>(this.UpdateVisuals));
    }

    private void OnDisable()
    {
        this.TriggerUpdateVisuals = (Action<float, float, float, Vector3>)Delegate.Remove(this.TriggerUpdateVisuals, new Action<float, float, float, Vector3>(this.UpdateVisuals));
    }

    private void UpdateVisuals(float curThrottle, float curAtmo, float curAngleVel, Vector3 curAccelerationDir)
    {
        foreach (FloatParam param in FloatParams)
        {
            param.ApplyToMaterial(curThrottle, curAtmo, _renderer.sharedMaterial);
        }
    }

    void OnValidate()
    {
        _renderer ??= GetComponent<Renderer>();

        TriggerUpdateVisuals ??= (Action<float, float, float, Vector3>)Delegate.Combine(this.TriggerUpdateVisuals, new Action<float, float, float, Vector3>(this.UpdateVisuals));

    }
}
