using HarmonyLib;
using KSP;
using KSP.Game;
using KSP.VFX;
using LuxsFlamesAndOrnaments.Settings;
using UitkForKsp2.API;
using UitkForKsp2.Controls;
using UitkForKsp2.Controls.Vectors;
using UnityEngine;
using UnityEngine.UIElements;
using static KSP.VFX.ThrottleVFXManager;

namespace LuxsFlamesAndOrnaments.Editor
{
    /// <summary>
    /// Should function something like this:
    /// - select the part (Raycast) and it will show a new window for that part
    /// - load existing configs if any
    /// - header will contain a GameObject tree view to select desired mesh
    /// - normal GO will show as italic/greyed, they'll have a + sign next to them to add LFOPlumes to them
    /// - LFOPlumes go will be normal text
    /// - after this there will be a foldout with controls (or maybe on the side?) throttle and atmo
    /// - clicking on a LFOPlume will show a window starting with transform(pos, rot, scale)
    /// - followed by texture
    /// - followed by LFOParams (reorganizeable if possible)
    /// - a list of all possible params will be shown (greyed) clicking on them will initialize a config for it
    /// - params will be controlled via keyframes
    /// 
    /// Ideally the workflow would be, change value as you want, when satifies, press keyframe, rinse and repeat
    /// </summary>
    internal class LFOEditorController : KerbalMonoBehaviour
    {
        public static string WindowUxmlPath = "lfo/ui/lfoeditor.uxml";
        public static string SavePath = Path.Combine(LFO.PLUGIN_PATH, "output");
        private GameObject target;
        private string partName
        {
            get
            {
                return target.GetComponent<CorePartData>().Data.partName;
            }
        }
        private ThrottleVFXManager throttleVFXManager;
        public LFOConfig config;
        private VisualElement _container;

        public LFOEditorController(GameObject target)
        {
            this.target = target;
            throttleVFXManager = target.GetComponent<ThrottleVFXManager>();
            config = new() { partName = this.partName };
            target.GetComponentsInChildren<LFOThrottleData>().ToList().ForEach(a =>
            {
                if(!config.PlumeConfigs.ContainsKey(a.transform.parent.name))
                {
                    config.PlumeConfigs.Add(a.transform.parent.name, new());
                }
                config.PlumeConfigs[a.transform.parent.name].Add(a.config);
            });
        }

        public void ChangeAtmoType(FloatParam target, CurveType newType)
        {
            target.AtmoCurveType = newType;
        }
        public void ChangeThrottleType(FloatParam target, CurveType newType)
        {
            target.ThrottleCurveType = newType;
        }

        public void AddAtmoKeyframe(FloatParam target, float time, float value)
        {
            target.AtmoMultiplierCurve.AddKey(time, value);
        }
        public void AddThrottleKeyframe(FloatParam target, float time, float value)
        {
            target.ThrottleMultiplierCurve.AddKey(time, value);
        }

        public void AddNewPlume(string plumeParent, string shader, string meshName)
        {
            var parent = target.transform.FindChildRecursive(plumeParent).gameObject;
            if (parent != null)
            {
                GameObject plume = new("[LFO] new plume [vfx_exh]", typeof(MeshRenderer), typeof(MeshFilter), typeof(LFOThrottleData));
                LFOThrottleData throttleData = plume.GetComponent<LFOThrottleData>();
                PlumeConfig config = new();
                config.targetGameObject = parent.name;
                config.ShaderSettings.ShaderName = shader;
                throttleData.material = config.GetMaterial();

                MeshRenderer renderer = plume.GetComponent<MeshRenderer>();
                MeshFilter filter = plume.GetComponent<MeshFilter>();

                LFO.TryGetMesh(meshName, out Mesh mesh);
                if (mesh is not null)
                {
                    filter.mesh = mesh;
                }
                LFO.RegisterPlumeConfig(plume.name, config);

                plume.transform.parent = parent.transform;
                plume.layer = 1;//TransparentFX layer

                throttleVFXManager.FXModeActionEvents[0].ActionEvents[0].EngineEffects.AddItem(new EngineEffect() { EffectReference = plume });
            }
        }
        public void Save()
        {
            Directory.CreateDirectory(SavePath);
            string fileName = partName + ".json";

            using (StreamWriter sw = File.CreateText(Path.Combine(SavePath, fileName)))
            {
                sw.Write(LFOConfig.Serialize(config));
            }

            LFO.RegisterLFOConfig(partName, config);
        }

        private void SetupDocument()
        {
            var document = GetComponent<UIDocument>();
            if (document.TryGetComponent<DocumentLocalization>(out var localization))
            {
                localization.Localize();
            }
            else
            {
                document.EnableLocalization();
            }

            _container = document.rootVisualElement;
            _container[0].CenterByDefault();
            SetVisible(false);
        }

        private void InitializeElements()
        {
            partNameLabel = _container.Q<Label>("part-name-label");
            parentSelectionDropdown = _container.Q<DropdownField>("parent-dropdown");
            layerScrollview = _container.Q<ScrollView>("layer-scroll-view");
            positionField = _container.Q<TextField>("position-field");
        }

        public void SetVisible(bool isVisible = true)
        {
            _container.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        #region UI Elements
        Label partNameLabel;
        DropdownField parentSelectionDropdown;
        ScrollView layerScrollview;
        Dictionary<PlumeConfig, Toggle> layerToggles;
        (TextField x, TextField y, TextField z) positionField, rotationField, scaleField;
        CurveControl atmoCurveControl, throttleCurveControl;
        (Slider input, Label display) atmoController, throttleController;
        #endregion
    }
}
