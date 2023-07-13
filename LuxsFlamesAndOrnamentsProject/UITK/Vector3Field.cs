using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using static UnityEngine.UIElements.TextField;

namespace UitkForKsp2.Controls.Vectors
{
    public class Vector3Field : TextInputBaseField<Vector3>, INotifyValueChanged<Vector3>
    {
        public class Vector3Input : TextInputBase
        {
            public override bool AcceptCharacter(char c)
            {
                return char.IsNumber(c) || (char.IsSeparator(c));
            }
            public Vector3Field parentTextField => (Vector3Field)base.parent;

            public override Vector3 StringToValue(string str)
            {
                // Remove the parentheses
                if (str.StartsWith("(") && str.EndsWith(")"))
                {
                    str = str.Substring(1, str.Length - 2);
                }

                // split the items
                string[] sArray = str.Split(',');

                // store as a Vector3
                Vector3 result = new Vector3(
                    float.Parse(sArray[0]),
                    float.Parse(sArray[1]),
                    float.Parse(sArray[2]));

                return result;
            }

            public void SelectRange(int cursorIndex, int selectionIndex)
            {
                if (base.editorEngine != null)
                {
                    base.editorEngine.cursorIndex = cursorIndex;
                    base.editorEngine.selectIndex = selectionIndex;
                }
            }

            public override void ExecuteDefaultActionAtTarget(EventBase evt)
            {
                base.ExecuteDefaultActionAtTarget(evt);
                if (evt == null)
                {
                    return;
                }

                if (evt.eventTypeId == EventBase<KeyDownEvent>.TypeId())
                {
                    KeyDownEvent keyDownEvent = evt as KeyDownEvent;
                    if (!parentTextField.isDelayed || (((keyDownEvent != null && keyDownEvent.keyCode == KeyCode.KeypadEnter) || (keyDownEvent != null && keyDownEvent.keyCode == KeyCode.Return))))
                    {
                        parentTextField.value = StringToValue(text);
                    }
                    if (keyDownEvent?.character == '\u0003' || keyDownEvent?.character == '\n')
                    {
                        base.parent.Focus();
                        evt.StopPropagation();
                        evt.PreventDefault();
                    }
                }
                else if (evt.eventTypeId == EventBase<ExecuteCommandEvent>.TypeId())
                {
                    string commandName = (evt as ExecuteCommandEvent).commandName;
                    if (!parentTextField.isDelayed && (commandName == "Paste" || commandName == "Cut"))
                    {
                        parentTextField.value = StringToValue(text);
                    }
                }
                else if (evt.eventTypeId == EventBase<NavigationSubmitEvent>.TypeId() || evt.eventTypeId == EventBase<NavigationCancelEvent>.TypeId() || evt.eventTypeId == EventBase<NavigationMoveEvent>.TypeId())
                {
                    evt.StopPropagation();
                    evt.PreventDefault();
                }
            }

            public override void ExecuteDefaultAction(EventBase evt)
            {
                base.ExecuteDefaultAction(evt);
                if (parentTextField.isDelayed && evt?.eventTypeId == EventBase<BlurEvent>.TypeId())
                {
                    parentTextField.value = StringToValue(text);
                }
            }
        }

        public new static readonly string ussClassName = "unity-vector3-field";

        public new static readonly string labelUssClassName = ussClassName + "__label";

        public new static readonly string inputUssClassName = ussClassName + "__input";

        public Vector3Input vector3Input => (Vector3Input)base.textInputBase;

        public override Vector3 value
        {
            get
            {
                return base.value;
            }
            set
            {
                base.value = value;
                base.text = base.rawValue.ToString();
            }
        }

        public void SelectRange(int rangeCursorIndex, int selectionIndex)
        {
            vector3Input.SelectRange(rangeCursorIndex, selectionIndex);
        }

        public Vector3Field()
            : this(null)
        {
        }

        public Vector3Field(int maxLength)
            : this(null, maxLength)
        {
        }

        public Vector3Field(string label)
            : this(label, -1)
        {

        }

        public Vector3Field(string label, int maxLength)
            : base(label, maxLength, '*', (TextInputBase)new Vector3Input())
        {
            AddToClassList(ussClassName);
            base.labelElement.AddToClassList(labelUssClassName);
            base.visualInput.AddToClassList(inputUssClassName);
            base.pickingMode = PickingMode.Ignore;
            SetValueWithoutNotify(Vector3.zero);
        }

        public override void SetValueWithoutNotify(Vector3 newValue)
        {
            base.SetValueWithoutNotify(newValue);
            base.text = base.rawValue.ToString();
        }

        public override void OnViewDataReady()
        {
            base.OnViewDataReady();
            string fullHierarchicalViewDataKey = GetFullHierarchicalViewDataKey();
            OverwriteFromViewData(this, fullHierarchicalViewDataKey);
            base.text = base.rawValue.ToString();
            ///TODO: Add X,Y,Z update here?
        }

        public override string ValueToString(Vector3 value)
        {
            return value.ToString();
        }

        public override Vector3 StringToValue(string str)
        {
            // Remove the parentheses
            if (str.StartsWith("(") && str.EndsWith(")"))
            {
                str = str.Substring(1, str.Length - 2);
            }

            // split the items
            string[] sArray = str.Split(',');

            // store as a Vector3
            Vector3 result = new Vector3(
                float.Parse(sArray[0]),
                float.Parse(sArray[1]),
                float.Parse(sArray[2]));

            return result;
        }
        public new class UxmlFactory : UxmlFactory<Vector3Field, UxmlTraits>
        {
        }

        public new class UxmlTraits : TextInputBaseField<Vector3>.UxmlTraits
        {
        }
    }
}
