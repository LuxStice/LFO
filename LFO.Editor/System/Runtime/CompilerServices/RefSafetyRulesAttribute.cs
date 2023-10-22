// Decompiled with JetBrains decompiler
// Type: System.Runtime.CompilerServices.RefSafetyRulesAttribute
// Assembly: LFO.Editor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1B2710C6-D792-44BD-9937-785792B862AE
// Assembly location: C:\KSP2Mods\LFO\Unity\LFO\Editor\LFO.Editor.dll

using Microsoft.CodeAnalysis;
using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
  [CompilerGenerated]
  [Embedded]
  [AttributeUsage(AttributeTargets.Module, AllowMultiple = false, Inherited = false)]
  internal sealed class RefSafetyRulesAttribute : Attribute
  {
    public readonly int Version;

    public RefSafetyRulesAttribute([In] int obj0) => this.Version = obj0;
  }
}
