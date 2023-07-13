using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LuxsFlamesAndOrnaments.ShaderEditor
{
    public class ShaderEditor
    {
        public static AnimationCurve Flat_1 => new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
        public static AnimationCurve Flat_0 => new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
        public static AnimationCurve Linear_01 => new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        public static AnimationCurve Linear_10 => new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));
    }
}
