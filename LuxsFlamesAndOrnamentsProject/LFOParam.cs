using UnityEngine;

[Serializable]
public abstract class LFOParam
{
    public string ParamName;
    [HideInInspector]
    public int ParamHash = -1;
    public bool UseAtmoCurve;
    public CurveType AtmoCurveType;
    public AnimationCurve AtmoMultiplierCurve;
    public bool UseThrottleCurve;
    public CurveType ThrottleCurveType;
    public AnimationCurve ThrottleMultiplierCurve;
    [HideInInspector]
    public bool isDirty;

    public abstract void ApplyToMaterial(float curThrottle, float curAtmo, Material material);
}