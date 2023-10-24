// Decompiled with JetBrains decompiler
// Type: LFOThrottleDataMasterGroupEditor
// Assembly: LFO.Editor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1B2710C6-D792-44BD-9937-785792B862AE
// Assembly location: C:\KSP2Mods\LFO\Unity\LFO\Editor\LFO.Editor.dll

using KSP;
using LuxsFlamesAndOrnaments.Monobehaviours;
using LuxsFlamesAndOrnaments.Settings;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (LFOThrottleDataMasterGroup))]
public class LFOThrottleDataMasterGroupEditor : Editor
{
  public bool UseNewShader;

  public virtual void OnInspectorGUI()
  {
    LFOThrottleDataMasterGroup target = (LFOThrottleDataMasterGroup) this.target;
    this.UseNewShader = EditorGUILayout.Toggle("Use New Shader", this.UseNewShader, Array.Empty<GUILayoutOption>());
    EditorGUI.BeginChangeCheck();
    EditorGUILayout.LabelField("Group Throttle", Array.Empty<GUILayoutOption>());
    target.GroupThrottle = EditorGUILayout.Slider(target.GroupThrottle, 0.0f, 100f, Array.Empty<GUILayoutOption>());
    EditorGUILayout.LabelField("Group Atmospheric Pressure", Array.Empty<GUILayoutOption>());
    target.GroupAtmo = EditorGUILayout.Slider(target.GroupAtmo, 0.0f, 1.1f, Array.Empty<GUILayoutOption>());
    EditorGUI.BeginDisabledGroup((double) target.GroupAtmo > 0.0092000002041459084);
    EditorGUILayout.LabelField("UpperAtmo Fine tune", Array.Empty<GUILayoutOption>());
    float num = EditorGUILayout.Slider(target.GroupAtmo, 0.0f, 0.0092f, Array.Empty<GUILayoutOption>());
    if ((double) target.GroupAtmo <= 0.0092000002041459084)
      target.GroupAtmo = num;
    EditorGUI.EndDisabledGroup();
    if (EditorGUI.EndChangeCheck())
    {
      foreach (LFOThrottleDataMasterGroup throttleGroup in UnityEngine.Object.FindObjectsOfType<LFOThrottleDataMasterGroup>())
        this.UpdateVisuals(throttleGroup);
    }
    EditorGUI.BeginChangeCheck();
    target.Active = EditorGUILayout.Toggle("Active?", target.Active, Array.Empty<GUILayoutOption>());
    if (EditorGUI.EndChangeCheck())
    {
      target.ToggleVisibility(target.Active);
      EditorUtility.SetDirty((UnityEngine.Object) target);
    }
    GUILayout.Label(string.Format("{0} children", (object) target.throttleDatas.Count));
    if (GUILayout.Button("Collect children"))
      target.throttleDatas = ((IEnumerable<LFOThrottleData>) ((Component) target).GetComponentsInChildren<LFOThrottleData>(true)).ToList<LFOThrottleData>();
    CorePartData componentInParent = ((Component) target).GetComponentInParent<CorePartData>();
    string path = Application.dataPath + "/LFO/configs/";
    string fileName = !((UnityEngine.Object) componentInParent != (UnityEngine.Object) null) ? ((UnityEngine.Object) target).name + ".json" : componentInParent.Data.partName + ".json";
    EditorGUILayout.Space(5f);
    if (GUILayout.Button("Save config"))
    {
      LFOConfig config = new LFOConfig();
      if ((UnityEngine.Object) componentInParent != (UnityEngine.Object) null)
        config.partName = ((Component) target).GetComponentInParent<CorePartData>().Data.partName;
      config.PlumeConfigs = new Dictionary<string, List<PlumeConfig>>();
      foreach (LFOThrottleData componentsInChild in ((Component) target).GetComponentsInChildren<LFOThrottleData>())
      {
        PlumeConfig plumeConfig = new PlumeConfig();
        Material sharedMaterial = ((Component) componentsInChild).GetComponent<UnityEngine.Renderer>().sharedMaterial;
        Shader shader = sharedMaterial.shader;
        plumeConfig.ShaderSettings = ShaderConfig.GenerateConfig(sharedMaterial);
        plumeConfig.Position = ((Component) componentsInChild).transform.localPosition;
        plumeConfig.Scale = ((Component) componentsInChild).transform.localScale;
        plumeConfig.Rotation = ((Component) componentsInChild).transform.localRotation.eulerAngles;
        plumeConfig.FloatParams = componentsInChild.FloatParams;
        plumeConfig.meshPath = ((Component) componentsInChild).GetComponent<MeshFilter>().sharedMesh.name;
        plumeConfig.targetGameObject = ((UnityEngine.Object) componentsInChild).name;
        if (!config.PlumeConfigs.ContainsKey(((Component) componentsInChild).transform.parent.name))
          config.PlumeConfigs.Add(((Component) componentsInChild).transform.parent.name, new List<PlumeConfig>());
        config.PlumeConfigs[((Component) componentsInChild).transform.parent.name].Add(plumeConfig);
        componentsInChild.config = plumeConfig;
      }
      ((MonoBehaviour) target).StartCoroutine(this.SaveToJson(config, path, fileName));
    }
    if (GUILayout.Button("Reload config"))
    {
      foreach (LFOThrottleData componentsInChild in ((Component) target).GetComponentsInChildren<LFOThrottleData>())
        ((Component) componentsInChild).GetComponent<UnityEngine.Renderer>().sharedMaterial = componentsInChild.config.GetMaterial();
    }
    EditorGUILayout.Space(5f);
    if (!GUILayout.Button("Load config"))
      return;
    LFOConfig lfoConfig = this.LoadFromJson(path, fileName);
    foreach (LFOThrottleData componentsInChild in ((Component) target).GetComponentsInChildren<LFOThrottleData>())
    {
      LFOThrottleData throttleData = componentsInChild;
      int index = lfoConfig.PlumeConfigs[((Component) throttleData).transform.parent.name].FindIndex((Predicate<PlumeConfig>) (a => a.targetGameObject == ((UnityEngine.Object) throttleData).name));
      if (index >= 0)
      {
        throttleData.config = lfoConfig.PlumeConfigs[((Component) throttleData).transform.parent.name][index];
        ((Component) throttleData).GetComponent<UnityEngine.Renderer>().sharedMaterial = throttleData.config.GetEditorMaterial();
        if (this.UseNewShader)
          ((Component) throttleData).GetComponent<UnityEngine.Renderer>().sharedMaterial.shader = Shader.Find("LFO/Additive");
        ((Component) throttleData).GetComponent<UnityEngine.Renderer>().sharedMaterial.name = ((UnityEngine.Object) throttleData).name + " Plume Material";
      }
    }
  }

  private void UpdateVisuals(LFOThrottleDataMasterGroup throttleGroup)
  {
    if (!throttleGroup.Active)
      return;
    throttleGroup.throttleDatas.ForEach((Action<LFOThrottleData>) (a =>
    {
      if (!a.IsVisible())
        return;
      if (a.config == null)
        Debug.LogWarning((object) ("Config for " + ((UnityEngine.Object) a).name + " is null"));
      else
        a.TriggerUpdateVisuals(throttleGroup.GroupThrottle / 100f, throttleGroup.GroupAtmo, 0.0f, Vector3.zero);
    }));
  }

  private LFOConfig LoadFromJson(string path, string fileName) => LFOConfig.Deserialize(File.OpenText(Path.Combine(path, fileName)).ReadToEnd());

  private IEnumerator SaveToJson(LFOConfig config, string path, string fileName)
  {
    Directory.CreateDirectory(path);
    JsonSerializerSettings settings = new JsonSerializerSettings()
    {
      ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    };
    string json = LFOConfig.Serialize(config);
    using (StreamWriter sw = File.CreateText(Path.Combine(path, fileName)))
      sw.Write(json);
    LFOConfig fromJson = JsonConvert.DeserializeObject<LFOConfig>(json);
    yield return (object) null;
    AssetDatabase.Refresh();
  }
}
