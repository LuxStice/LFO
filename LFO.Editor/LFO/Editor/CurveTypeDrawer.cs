// Decompiled with JetBrains decompiler
// Type: LFO.Editor.CurveTypeDrawer
// Assembly: LFO.Editor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1B2710C6-D792-44BD-9937-785792B862AE
// Assembly location: C:\KSP2Mods\LFO\Unity\LFO\Editor\LFO.Editor.dll

using System;
using UnityEditor;
using UnityEngine;

namespace LFO.Editor
{
  [CustomPropertyDrawer(typeof (CurveType))]
  internal class CurveTypeDrawer : PropertyDrawer
  {
    private bool expanded = false;

    public virtual void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      EditorGUI.BeginProperty(position, label, property);
      property.intValue = (int) EditorGUI.EnumPopup(position, label, (Enum) (CurveType) property.intValue);
      EditorGUI.EndProperty();
    }

    public virtual float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      int num = 1;
      return (float) ((double) EditorGUIUtility.singleLineHeight * (double) num + (double) EditorGUIUtility.standardVerticalSpacing * (double) (num - 1));
    }
  }
}
