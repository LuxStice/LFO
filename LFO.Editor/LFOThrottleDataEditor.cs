// Decompiled with JetBrains decompiler
// Type: LFOThrottleDataEditor
// Assembly: LFO.Editor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1B2710C6-D792-44BD-9937-785792B862AE
// Assembly location: C:\KSP2Mods\LFO\Unity\LFO\Editor\LFO.Editor.dll

using LuxsFlamesAndOrnaments.Monobehaviours;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (LFOThrottleData))]
public class LFOThrottleDataEditor : Editor
{
  private bool groupDropdown;

  public virtual void OnInspectorGUI()
  {
    LFOThrottleData target = (LFOThrottleData) this.target;
    if ((UnityEngine.Object) ((Component) target).GetComponent<UnityEngine.Renderer>().sharedMaterial == (UnityEngine.Object) null)
    {
      Material material = new Material(Shader.Find("LFOAdditive 2.0"));
      ((Component) target).GetComponent<UnityEngine.Renderer>().sharedMaterial = material;
    }
    else if (GUILayout.Button("New Material Instance"))
    {
      Material material = new Material(((Component) target).GetComponent<UnityEngine.Renderer>().sharedMaterial);
      material.name = ((UnityEngine.Object) target).name + " Plume Material";
      ((Component) target).GetComponent<UnityEngine.Renderer>().sharedMaterial = material;
      target.config.ShaderSettings.ShaderName = material.shader.name;
      target.config.ShaderSettings.ShaderParams = new Dictionary<string, object>();
    }
    LFOThrottleDataMasterGroup componentInParent = ((Component) target).gameObject.transform.GetComponentInParent<LFOThrottleDataMasterGroup>();
    if ((UnityEngine.Object) componentInParent != (UnityEngine.Object) null)
    {
      this.groupDropdown = EditorGUILayout.Foldout(this.groupDropdown, "Group Controls");
      if (this.groupDropdown)
      {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.LabelField("Group Throttle", Array.Empty<GUILayoutOption>());
        componentInParent.GroupThrottle = EditorGUILayout.Slider(componentInParent.GroupThrottle, 0.0f, 100f, Array.Empty<GUILayoutOption>());
        EditorGUILayout.LabelField("Group Atmospheric Pressure", Array.Empty<GUILayoutOption>());
        componentInParent.GroupAtmo = EditorGUILayout.Slider(componentInParent.GroupAtmo, 0.0f, 1.1f, Array.Empty<GUILayoutOption>());
        EditorGUI.BeginDisabledGroup((double) componentInParent.GroupAtmo > 0.0092000002041459084);
        EditorGUILayout.LabelField("UpperAtmo Fine tune", Array.Empty<GUILayoutOption>());
        float num = EditorGUILayout.Slider(componentInParent.GroupAtmo, 0.0f, 0.0092f, Array.Empty<GUILayoutOption>());
        if ((double) componentInParent.GroupAtmo <= 0.0092000002041459084)
          componentInParent.GroupAtmo = num;
        EditorGUI.EndDisabledGroup();
        if (EditorGUI.EndChangeCheck())
          this.UpdateVisuals(componentInParent);
      }
    }
    EditorGUI.BeginChangeCheck();
    if (EditorGUI.EndChangeCheck() && (UnityEngine.Object) componentInParent != (UnityEngine.Object) null)
      this.UpdateVisuals(componentInParent);
    EditorGUI.BeginDisabledGroup(true);
    EditorGUILayout.PropertyField(this.serializedObject.FindProperty("Seed"), Array.Empty<GUILayoutOption>());
    EditorGUILayout.PropertyField(this.serializedObject.FindProperty("renderer"), Array.Empty<GUILayoutOption>());
    EditorGUI.EndDisabledGroup();
    if (target.config == null)
      return;
    SerializedProperty property = this.serializedObject.FindProperty("config");
    if (property != null)
      EditorGUILayout.PropertyField(property, Array.Empty<GUILayoutOption>());
  }

  private void UpdateVisuals(LFOThrottleDataMasterGroup throttleGroup) => throttleGroup.TriggerUpdateVisuals(throttleGroup.GroupThrottle / 100f, throttleGroup.GroupAtmo, 0.0f, Vector3.zero);
}
