// Decompiled with JetBrains decompiler
// Type: LFOThrottleData
// Assembly: lfo, Version=0.9.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2965BBBA-49CA-4B3F-B886-3391858B1BD3
// Assembly location: C:\Kerbal Space Program 2\BepInEx\plugins\lfo\lfo.dll

using UnityEngine;
using UnityEngine.Serialization;
using KSP.Game;
using LuxsFlamesAndOrnaments.Settings;
using LuxsFlamesAndOrnaments;
using KSP;
using KSP.Modules;
// using System;
// using System.Collections.Generic;

[DisallowMultipleComponent]
[RequireComponent(typeof (Renderer))]
public class LFOThrottleData : KerbalMonoBehaviour, IEngineFXData
{
  public float Seed;
  public Renderer renderer;
  public Material material
  {
    get => renderer.material;
    set => renderer.material = value;
  }
  
  public PlumeConfig config = new PlumeConfig();
  public List<FloatParam> FloatParams => config.FloatParams;
  public string partName = "";
  public bool IsRCS;

    public bool IsVisible()
    {
        return this.renderer != null && this.renderer.enabled;
    }

  public void ToggleVisibility(bool doTurnOn, ParticleSystemStopBehavior stopBehaviour = ParticleSystemStopBehavior.StopEmitting)
  {
        if (renderer is not null)
            renderer.enabled = doTurnOn;
  }

    void Awake()
  {
        renderer = GetComponent<Renderer>();
        if(name.Contains("RCS"))
            IsRCS = true;
  }

    void Start()
  {
    //var engineComp = this.getcomponentpar<Module_Engine>(true);
    //string partName = (engineComp.PartBackingMode == KSP.Sim.Definitions.PartBehaviourModule.PartBackingModes.OAB) ? engineComp.OABPart.PartName : engineComp.part.name;

    if (IsRCS || !string.IsNullOrEmpty(this.partName) && LFO.TryGetPlumeConfig(partName, this.name, out config))
      return;
    enabled = false;
  }

    void OnEnable()
    {
        this.TriggerUpdateVisuals += this.UpdateVisuals;
    }

    private void OnDisable()
    {
        this.TriggerUpdateVisuals -= this.UpdateVisuals;
    }

  private void UpdateVisuals(float curThrottle, float curAtmo, float curAngleVel, Vector3 curAccelerationDir)
  {
        foreach (FloatParam param in FloatParams)
    {
            param.ApplyToMaterial(curThrottle, curAtmo, renderer.sharedMaterial);
    }
  }

  void OnValidate()
  {
        renderer ??= GetComponent<Renderer>();

        TriggerUpdateVisuals ??= (Action<float, float, float, Vector3>)Delegate.Combine(this.TriggerUpdateVisuals, new Action<float, float, float, Vector3>(this.UpdateVisuals));

  }

  public Action<float, float, float, Vector3> TriggerUpdateVisuals { get; set; }
}
