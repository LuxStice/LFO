// Decompiled with JetBrains decompiler
// Type: FloatParam
// Assembly: lfo, Version=0.9.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2965BBBA-49CA-4B3F-B886-3391858B1BD3
// Assembly location: C:\Kerbal Space Program 2\BepInEx\plugins\lfo\lfo.dll

// using System;
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
