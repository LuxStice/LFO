using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace UitkForKsp2.Controls
{
    internal class BaseControl : VisualElement
    {
        public VisualElement LabelContainer;
        public Label LabelElement;
        public string Label
        {
            get => LabelElement.text;
            set
            {
                LabelElement.text = value;
                if (string.IsNullOrEmpty(value.Trim()))
                {
                    LabelContainer.style.display = DisplayStyle.None;
                }
                else
                {

                    LabelContainer.style.display = DisplayStyle.Flex;
                }
            }
        }
        public VisualElement InputContainer
        {
            get => _inputContainer;
            set
            {
                if(_inputContainer != null)
                {
                    _inputContainer.RemoveFromHierarchy();
                }

                if(value == null)
                {
                    _inputContainer = new VisualElement();
                    _inputContainer.name = "input-container";
                }
                else
                {
                    _inputContainer = value;
                }


                InputContainer.AddToClassList(UssInputContainerClassName);
                Add(InputContainer);
            }
        }
        private VisualElement _inputContainer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Label"></param>
        /// <param name="visualInput">Everything that should be affected by said control</param>
        public BaseControl(string Label, VisualElement visualInput)
        {
            AddToClassList(UssClassName);
            LabelContainer = new VisualElement()
            {
                name = "label-container"
            };
            LabelContainer.AddToClassList(UssLabelContainerClassName);
            LabelElement = new Label()
            {
                name = "label"
            };
            this.Label = Label;
            LabelContainer.Add(LabelElement);
            Add(LabelContainer);

            InputContainer = new VisualElement()
            {
                name = "input-container"
            };
            InputContainer.AddToClassList(UssInputContainerClassName);
            Add(InputContainer);
        }


        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public UxmlStringAttributeDescription label = new UxmlStringAttributeDescription() { name = "label", defaultValue = "Label" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                if (ve is Selector selector)
                {
                    selector.Label = label.GetValueFromBag(bag, cc);
                }
            }
        }

        public static readonly string UssClassName = "uitkforksp2-base";
        public static readonly string UssLabelContainerClassName = UssClassName + "__label-container";
        public static readonly string UssInputContainerClassName = UssClassName + "__input-container";
    }
}
