// Decompiled with JetBrains decompiler
// Type: LFOParam
// Assembly: lfo, Version=0.9.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2965BBBA-49CA-4B3F-B886-3391858B1BD3
// Assembly location: C:\Kerbal Space Program 2\BepInEx\plugins\lfo\lfo.dll

// using System;
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