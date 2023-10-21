// Decompiled with JetBrains decompiler
// Type: LuxsFlamesAndOrnaments.Monobehaviours.LFOThrottleDataMasterGroup
// Assembly: lfo, Version=0.9.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2965BBBA-49CA-4B3F-B886-3391858B1BD3
// Assembly location: C:\Kerbal Space Program 2\BepInEx\plugins\lfo\lfo.dll

using KSP.Game;
using KSP.Messages.PropertyWatchers;
using RTG;
using System;
using System.Collections.Generic;
// using System.Linq;
using System.Text;
using UnityEngine;

namespace LuxsFlamesAndOrnaments.Monobehaviours
{
  [ExecuteInEditMode]
  public class LFOThrottleDataMasterGroup : KerbalMonoBehaviour, IEngineFXData
  {
    public Action<float, float, float, Vector3> TriggerUpdateVisuals { get; set; }
    List<IEngineFXData> children => throttleDatas.Select(a => (IEngineFXData)a).ToList();
    public List<LFOThrottleData> throttleDatas = new List<LFOThrottleData>();

    public bool OverrideControls = false;
    public bool Active = false;
    [Range(0.0f, 100f)]
    public float GroupThrottle;
    [Range(0.0f, 1.1f)]
    public float GroupAtmo;

    private float oldThrottle = -1f, oldAtmo = -1f;
    private System.Random rng;

    public bool IsVisible()
    {
      foreach (var child in children)
      {
        if (!child.IsVisible())
          return false;
      }
      return true;
    }

    void Start()
    {
        if (Application.isEditor)
        {
            Active = true;
        }
        throttleDatas = GetComponentsInChildren<LFOThrottleData>(true).ToList();
        rng = new System.Random(gameObject.GetHashCode());
        NewSeedForAll();
    }

    public void NewSeedForAll()
    {
        throttleDatas.ForEach(a => a.Seed = (float)rng.NextDouble());
    }

    public void ToggleVisibility(bool doTurnOn, ParticleSystemStopBehavior stopBehaviour = ParticleSystemStopBehavior.StopEmitting)
    {
        throttleDatas.ForEach(a => a.ToggleVisibility(doTurnOn));
    }

    private void UpdateVisuals(float curThrottle, float curAtmo, float curAngleVel, Vector3 curAccelerationDir)
    {
        throttleDatas.ForEach(a => a.TriggerUpdateVisuals(curThrottle, curAtmo, curAngleVel, curAccelerationDir));
    }

        void OnEnable()
        {
            TriggerUpdateVisuals += UpdateVisuals;
        }
        private void OnDisable()
        {
            TriggerUpdateVisuals -= UpdateVisuals;
        }

    void Update()
    {
            if (Application.isEditor)
            {
                if (GroupThrottle != oldThrottle || GroupAtmo != oldAtmo)
                {
                    UpdateVisuals(GroupThrottle / 100f, GroupAtmo, 0, Vector3.zero);
                    oldAtmo = GroupAtmo;
                    oldThrottle = GroupThrottle;
                }
            }
    }
  }
}
