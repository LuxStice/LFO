using UnityEngine;

[Serializable]
public class FloatParam : LFOParam
{
    public float Value;
    public FloatParam() : base()
    {
        Value = float.MinValue;
    }

    public override void ApplyToMaterial(float curThrottle, float curAtmo, Material material)
    {
        float calculatedValue = Value;

        if (UseAtmoCurve)
        {
            float evaluated = AtmoMultiplierCurve.Evaluate(curAtmo);
            switch (AtmoCurveType)
            {
                case CurveType.Base:
                    calculatedValue = evaluated;
                    break;
                case CurveType.Multiply:
                    calculatedValue *= evaluated;
                    break;
                case CurveType.Add:
                    calculatedValue += evaluated;
                    break;
            }
        }

        if (UseThrottleCurve)
        {
            float evaluated = ThrottleMultiplierCurve.Evaluate(curThrottle);
            switch (ThrottleCurveType)
            {
                case CurveType.Base:
                    calculatedValue = evaluated;
                    break;
                case CurveType.Multiply:
                    calculatedValue *= evaluated;
                    break;
                case CurveType.Add:
                    calculatedValue += evaluated;
                    break;
            }
        }

        if (ParamHash == 0)
            ParamHash = Shader.PropertyToID(ParamName);

        material.SetFloat(ParamName, calculatedValue);
    }
}
