using System;
using System.Collections.Generic;
using UitkForKsp2.Controls.zHelpers;
using UnityEngine;
using UnityEngine.UIElements;

namespace UitkForKsp2.Controls
{
    public class CurveControl : BindableElement, INotifyValueChanged<AnimationCurve>
    {
        public const int graphResolution = 5;
        public Label label;
        public VisualElement CurveView;
        public AnimationCurve curve;
        Selector keyframeSelector;
        KeyframeEditor keyframeEditor;
        Button deleteButton;
        Keyframe selectedKeyframe => curve[selectedKey];


        private Rect rect => CurveView.contentRect;

        public AnimationCurve value
        {
            get => curve;
            set
            {
                if (value != curve)
                {
                    var changeEvent = ChangeEvent<AnimationCurve>.GetPooled(curve, value);
                    curve = value;
                    SendEvent(changeEvent);
                }
            }
        }

        public void DrawToView(Rect newRect)
        {
            StyleBackground styleBackground = new StyleBackground();
            styleBackground.value = Background.FromTexture2D(DrawCurve(newRect));
            CurveView.style.backgroundImage = styleBackground;
        }
        public void DrawToView()
        {
            StyleBackground styleBackground = new StyleBackground();
            styleBackground.value = Background.FromTexture2D(DrawCurve(CurveView.contentRect));
            CurveView.style.backgroundImage = styleBackground;
        }


        public Texture2D DrawCurve(Rect rect)
        {
            Texture2D view = new Texture2D((int)rect.width, (int)rect.height);

            for (int x = 0; x < view.width; x++)
            {
                for (int y = 0; y < view.height; y++)
                {
                    view.SetPixel(x, y, Color.clear);
                }
            }

            for (float t = 0; t < 1; t += .1f / rect.width)
            {
                int x = (int)Math.Floor(t * rect.width);
                int y = (int)Math.Floor(curve.Evaluate(t) * rect.height);
                if (x >= 0 && x <= rect.width && y >= 0 && y <= rect.height)
                    DrawCircle(view, x, y, 1, Color.white);
            }
            for (int i = 0; i < curve.keys.Length; i++)
            {
                Keyframe keyframe = curve.keys[i];
                Color keyframeColor = Color.red;
                if (i == selectedKey)
                    keyframeColor = Color.green;
                DrawCircle(view, (int)(keyframe.time * view.width), (int)(keyframe.value * view.height), 5, keyframeColor);
            }

            view.Apply();

            return view;
        }
        private void DrawCircle(Texture2D tgt, int x, int y, float radius, Color color)
        {
            Vector2 origin = new Vector2(x, y);
            for (float x2 = x - radius; x2 < x + radius; x2++)
            {
                for (float y2 = y - radius; y2 < y + radius; y2++)
                {
                    Vector2 curPos = new Vector2(x2, y2);

                    if (x2 >= 0 && x2 < tgt.width)
                    {
                        if (y2 >= 0 && y2 < tgt.height)
                        {
                            if (Vector2.Distance(origin, curPos) <= radius)
                            {
                                tgt.SetPixel((int)x2, (int)y2, color);
                            }
                        }
                    }
                }
            }
        }

        Clickable doubleClick;
        Vector2 lastPos;
        int selectedKey = 0;

        public void UpdateUI()
        {
            DrawToView();

            bool showEditor = curve.keys.Length > 0;
            if (showEditor)
            {
                UpdateSelector();
                UpdateEditor();
            }

            keyframeSelector.SetEnabled(showEditor);
            keyframeEditor.SetEnabled(showEditor);
            deleteButton.SetEnabled(showEditor);
        }

        public void OnDoubleClick()
        {
            Debug.Log(doubleClick.lastMousePosition);
            lastPos = new Vector2(doubleClick.lastMousePosition.x, CurveView.contentRect.height - doubleClick.lastMousePosition.y);
            AddKeyframe(lastPos.x / CurveView.contentRect.width, lastPos.y / CurveView.contentRect.height);
        }

        public CurveControl()
        {
            CurveView = new VisualElement()
            {
                name = "curve-view"
            };

            doubleClick = new Clickable(OnDoubleClick);
            doubleClick.activators.Add(new ManipulatorActivationFilter() { button = MouseButton.LeftMouse, clickCount = 2 });
            CurveView.AddManipulator(doubleClick);

            Add(CurveView);
            curve = new AnimationCurve(
                new Keyframe(0, 0, 0, 0f, .25f, .25f) { weightedMode = WeightedMode.Both },
                new Keyframe(.4f, .25f, 0, 0, .25f, .25f) { weightedMode = WeightedMode.Both },
                new Keyframe(.6f, .75f, 0, 0, .25f, .25f) { weightedMode = WeightedMode.Both },
                new Keyframe(1, 1, 0, 0, .25f, .25f) { weightedMode = WeightedMode.Both });
            style.flexDirection = FlexDirection.Column;

            CurveView.style.minHeight = 150;
            CurveView.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            CurveView.style.height = new StyleLength(new Length(150, LengthUnit.Pixel));


            keyframeSelector = new Selector("", "Keyframe #0");
            keyframeSelector.OnValueChanged += (newValue) =>
            {
                selectedKey = newValue;
                keyframeEditor.SetKeyframe(newValue,curve[newValue]);
                UpdateUI();
            };
            Add(keyframeSelector);
            DrawSelector();


            keyframeEditor = new KeyframeEditor(selectedKey, curve[selectedKey]);
            keyframeEditor.Label = "";
            Add(keyframeEditor);

            deleteButton = new Button()
            {
                name = "delete-button",
                text = "Delete"
            };
            deleteButton.clicked += () => DeleteKeyframe(selectedKey);
            Add(deleteButton);

            keyframeEditor.KeyframeChanged += OnKeyframeChanged;

            RegisterCallback<GeometryChangedEvent>((evt) =>
            {
                DrawToView();
                if (!initialized)
                {
                    UpdateEditor();
                    UpdateSelector();
                    initialized = true;
                }
            });
        }

        public void AddKeyframe(float time, float value, bool selectAdded = true)
        {
            Keyframe toAdd = new Keyframe(time, value);
            AddKeyframe(toAdd, selectAdded);
        }

        public void AddKeyframe(Keyframe newKeyframe, bool selectAdded = true)
        {
            int addedIndex = curve.AddKey(newKeyframe);
            if (selectAdded)
                selectedKey = addedIndex;

            UpdateUI();
        }

        public void DeleteKeyframe(int index)
        {
            curve.RemoveKey(index);

            UpdateUI();
        }

        private void OnKeyframeChanged(int arg1, Keyframe arg2)
        {
            curve.MoveKey(arg1, arg2);
            UpdateUI();
        }

        private bool initialized;
        private void DrawSelector()
        {
            List<string> keyframeChoices = new List<string>();

            for (int i = 0; i < curve.length; i++)
            {
                keyframeChoices.Add($"Keyframe #{i}");
            }
            keyframeSelector.SetChoices(selectedKey,true, keyframeChoices.ToArray());
        }
        private void UpdateSelector()
        {
            List<string> keyframeChoices = new List<string>();

            for (int i = 0; i < curve.length; i++)
            {
                keyframeChoices.Add($"Keyframe #{i}");
            }
            keyframeSelector.SetChoices(selectedKey, false, keyframeChoices.ToArray());
        }
        private void UpdateEditor()
        {
            keyframeEditor.UpdateUI(curve[selectedKey]);
        }

        public void SetValueWithoutNotify(AnimationCurve newValue)
        {
            curve = newValue;
            UpdateUI();
        }

        public new class UxmlFactory : UxmlFactory<CurveControl, UxmlTraits> { }

        public new class UxmlTraits : BindableElement.UxmlTraits
        {

        }
    }
}