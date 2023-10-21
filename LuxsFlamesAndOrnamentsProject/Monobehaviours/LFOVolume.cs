// Decompiled with JetBrains decompiler
// Type: LFOVolume
// Assembly: lfo, Version=0.9.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2965BBBA-49CA-4B3F-B886-3391858B1BD3
// Assembly location: C:\Kerbal Space Program 2\BepInEx\plugins\lfo\lfo.dll

using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof (UnityEngine.Renderer))]
public class LFOVolume : MonoBehaviour
{
  private static Resolution _resolution;
  public Resolution Resolution = Resolution.Medium;

  private Material material => Application.isEditor ? this.GetComponent<UnityEngine.Renderer>().sharedMaterial : this.GetComponent<UnityEngine.Renderer>().material;

  private void Start()
  {
    if (this.material == null)
      return;
    this.material.SetFloat("_TimeOffset", UnityEngine.Random.Range(-10f, 10f));
    this.material.SetInt("_Resolution", (int) LFOVolume._resolution);
    this.material.SetVector("scale", (Vector4) this.transform.lossyScale);
    this.material.SetMatrix("rotation", Matrix4x4.Rotate(Quaternion.Inverse(this.transform.rotation)));
    this.material.SetVector("position", (Vector4) this.transform.position);
  }

  private void LateUpdate()
  {
    if (this.material == null)
      return;
    this.material.SetVector("scale", (Vector4) this.transform.lossyScale);
    this.material.SetMatrix("rotation", Matrix4x4.Rotate(Quaternion.Inverse(this.transform.rotation)));
    this.material.SetVector("position", (Vector4) this.transform.position);
    if (this.Resolution == LFOVolume._resolution)
      return;
    LFOVolume._resolution = this.Resolution;
    this.material.SetInt("_Resolution", (int) LFOVolume._resolution);
  }
}
