using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace UitkForKsp2.Controls.zHelpers
{
    internal class KeyframeEditor : BaseControl, INotifyValueChanged<Keyframe>
    {
        public Keyframe value
        {
            get => _keyframe;
            set
            {
                _keyframe = value;
                SendEvent(value);
            }
        }

        private Keyframe _keyframe;
        public int index;

        public TextField TimeField;
        public TextField ValueField;
        public TextField TangInField;
        public TextField TangOutField;
        public DropdownField TangModeDropdown;
        public TextField WeightInField;
        public TextField WeightOutField;
        public DropdownField WeightModeDropdown;

        public event Action<int, Keyframe> KeyframeChanged;

        public void SetKeyframe(int index, Keyframe newValue)
        {
            this.index = index;
            this.value = newValue;
        }

        private void UpdateVisuals()
        {
            TimeField.SetValueWithoutNotify(_keyframe.time.ToString(Format));
            ValueField.SetValueWithoutNotify(_keyframe.value.ToString(Format));


            TangModeDropdown.SetValueWithoutNotify(((WeightedMode)_keyframe.tangentMode).ToString());
            TangInField.SetValueWithoutNotify(_keyframe.inTangent.ToString(Format));
            TangOutField.SetValueWithoutNotify(_keyframe.outTangent.ToString(Format));


            WeightModeDropdown.SetValueWithoutNotify(_keyframe.weightedMode.ToString());
            WeightInField.SetValueWithoutNotify(_keyframe.inWeight.ToString(Format));
            WeightOutField.SetValueWithoutNotify(_keyframe.outWeight.ToString(Format));


            TangInField.SetEnabled(_keyframe.tangentMode == (int)WeightedMode.In || _keyframe.tangentMode == (int)WeightedMode.Both);
            TangOutField.SetEnabled(_keyframe.tangentMode == (int)WeightedMode.Out || _keyframe.tangentMode == (int)WeightedMode.Both);

            WeightInField.SetEnabled(_keyframe.weightedMode == WeightedMode.In || _keyframe.weightedMode == WeightedMode.Both);
            WeightOutField.SetEnabled(_keyframe.weightedMode == WeightedMode.Out || _keyframe.weightedMode == WeightedMode.Both);
        }

        private void SendEvent(Keyframe newValue)
        {
            SendEvent(ChangeEvent<Keyframe>.GetPooled(_keyframe, newValue));
            SetValueWithoutNotify(newValue);
            KeyframeChanged?.Invoke(index, _keyframe);
        }

        public KeyframeEditor(int index, Keyframe keyframe) : this()
        {
            this._keyframe = keyframe;
            this.index = index;
        }

        public KeyframeEditor() : base("Keyframe", new VisualElement())
        {
            AddToClassList(UssClassName);
            TimeField = new TextField("Time", 7, false, false, char.MinValue)//allows for 100.000 (7 chars)
            {
                name = "time-field",
                isDelayed = true,
                value = "000.000"
            };
            TimeField.AddToClassList(UssTimeValueFieldClassName);
            InputContainer.Add(TimeField);
            ValueField = new TextField("Value", 7, false, false, char.MinValue)//allows for 100.000 (7 chars)
            {
                name = "value-field",
                isDelayed = true,
                value = "000.000"
            };
            ValueField.AddToClassList(UssTimeValueFieldClassName);
            InputContainer.Add(ValueField);



            Label tangLabel = new Label("Tangent")
            {
                name = "sub-header"
            };
            InputContainer.Add(tangLabel);
            TangModeDropdown = new DropdownField()
            {
                name = "Tang-dropdown",
                choices = Enum.GetNames(typeof(WeightedMode)).ToList(),
                value = WeightedMode.Both.ToString()
            };
            TangModeDropdown.AddToClassList(UssTangDropdownClassName);
            InputContainer.Add(TangModeDropdown);
            TangInField = new TextField("In", 7, false, false, char.MinValue)//allows for 100.000 (7 chars)
            {
                name = "tang-in-field",
                isDelayed = true,
                value = "000.000"
            };
            TangInField.AddToClassList(UssTangFieldsClassName);
            InputContainer.Add(TangInField);
            TangOutField = new TextField("Out", 7, false, false, char.MinValue)//allows for 100.000 (7 chars)
            {
                name = "tang-out-field",
                isDelayed = true,
                value = "000.000"
            };
            TangOutField.AddToClassList(UssTangFieldsClassName);
            InputContainer.Add(TangOutField);



            Label weightLabel = new Label("Weight")
            {
                name = "sub-header"
            };
            InputContainer.Add(weightLabel);
            WeightModeDropdown = new DropdownField()
            {
                name = "weigth-mode-dropdown",
                choices = Enum.GetNames(typeof(WeightedMode)).ToList(),
                value = WeightedMode.Both.ToString()
            };
            WeightModeDropdown.AddToClassList(UssWeightDropdownClassName);
            InputContainer.Add(WeightModeDropdown);
            WeightInField = new TextField("In", 7, false, false, char.MinValue)//allows for 100.000 (7 chars)
            {
                name = "weight-in-field",
                isDelayed = true,
                value = "000.000"
            };
            WeightInField.AddToClassList(UssWeightFieldClassName);
            InputContainer.Add(WeightInField);
            WeightOutField = new TextField("Out", 7, false, false, char.MinValue)//allows for 100.000 (7 chars)
            {
                name = "wieght-out-field",
                isDelayed = true,
                value = "000.000"
            };
            WeightOutField.AddToClassList(UssWeightFieldClassName);
            InputContainer.Add(WeightOutField);



            SetBinding();
            _keyframe = default;
            UpdateVisuals();
        }

        private static readonly string Format = "#00.00#";

        private void SetBinding()
        {
            TimeField.RegisterValueChangedCallback((evt) =>
            {
                string newEntry = evt.newValue.Replace('.', ',');
                if (float.TryParse(newEntry, out float newValue))
                {
                    _keyframe.time = newValue;
                    newEntry = newValue.ToString(Format);
                    value = _keyframe;
                }
                else
                {
                    newEntry = evt.previousValue;
                }
                TimeField.value = newEntry;
            });
            ValueField.RegisterValueChangedCallback((evt) =>
            {
                string newEntry = evt.newValue.Replace('.', ',');
                if (float.TryParse(newEntry, out float newValue))
                {
                    _keyframe.value = newValue;
                    newEntry = newValue.ToString(Format);
                    value = _keyframe;
                }
                else
                {
                    newEntry = evt.previousValue;
                }
                ValueField.value = newEntry;
            });


            TangModeDropdown.RegisterValueChangedCallback((evt) =>
            {
                WeightedMode newMode = (WeightedMode)Enum.Parse(typeof(WeightedMode), evt.newValue);

                _keyframe.tangentMode = (int)newMode;
                value = _keyframe;
            });
            TangInField.RegisterValueChangedCallback((evt) =>
            {
                string newEntry = evt.newValue.Replace('.', ',');
                if (float.TryParse(newEntry, out float newTime))
                {
                    _keyframe.inTangent = newTime;
                    newEntry = newTime.ToString(Format);
                    value = _keyframe;
                }
                else
                {
                    newEntry = evt.previousValue;
                }
                TangInField.value = newEntry;
            });
            TangOutField.RegisterValueChangedCallback((evt) =>
            {
                string newEntry = evt.newValue.Replace('.', ',');
                if (float.TryParse(newEntry, out float newTime))
                {
                    _keyframe.outTangent = newTime;
                    newEntry = newTime.ToString(Format);
                    value = _keyframe;
                }
                else
                {
                    newEntry = evt.previousValue;
                }
                TangOutField.value = newEntry;
            });


            WeightModeDropdown.RegisterValueChangedCallback((evt) =>
            {
                WeightedMode newMode = (WeightedMode)Enum.Parse(typeof(WeightedMode), evt.newValue);

                _keyframe.weightedMode = newMode;
                value = _keyframe;
            });
            WeightInField.RegisterValueChangedCallback((evt) =>
            {
                string newEntry = evt.newValue.Replace('.', ',');
                if (float.TryParse(newEntry, out float newTime))
                {
                    _keyframe.inWeight = newTime;
                    newEntry = newTime.ToString(Format);
                    value = _keyframe;
                }
                else
                {
                    newEntry = evt.previousValue;
                }
                WeightInField.value = newEntry;
            });
            WeightOutField.RegisterValueChangedCallback((evt) =>
            {
                string newEntry = evt.newValue.Replace('.', ',');
                if (float.TryParse(newEntry, out float newTime))
                {
                    _keyframe.outWeight = newTime;
                    newEntry = newTime.ToString(Format);
                    value = _keyframe;
                }
                else
                {
                    newEntry = evt.previousValue;
                }
                WeightOutField.value = newEntry;
            });
        }

        public void SetValueWithoutNotify(Keyframe newValue)
        {
            _keyframe = newValue;
            UpdateVisuals();
        }

        internal void UpdateUI(Keyframe keyframe)
        {
            SetValueWithoutNotify(keyframe);
        }

        public new class UxmlFactory : UxmlFactory<KeyframeEditor, UxmlTraits> { }

        public new class UxmlTraits : BaseControl.UxmlTraits
        {
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                if(ve is KeyframeEditor ke)
                {
                    ke.Label = label.GetValueFromBag(bag, cc);
                }
            }
        }

        public static new readonly string UssClassName = "keyframe-editor";
        public static readonly string UssTimeValueFieldClassName = UssClassName + "__time-value-fields";
        public static readonly string UssTangFieldsClassName = UssClassName + "__tang-fields";
        public static readonly string UssWeightFieldClassName = UssClassName + "__weight-fields";
        public static readonly string UssTangDropdownClassName = UssClassName + "__tang-dropdown";
        public static readonly string UssWeightDropdownClassName = UssClassName + "__tang-dropdown";
    }
}
