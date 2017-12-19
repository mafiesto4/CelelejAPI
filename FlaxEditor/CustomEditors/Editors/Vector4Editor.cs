////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2012-2017 Flax Engine. All rights reserved.
////////////////////////////////////////////////////////////////////////////////////

using System.Linq;
using FlaxEditor.CustomEditors.Elements;
using FlaxEngine;
using FlaxEngine.GUI;

namespace FlaxEditor.CustomEditors.Editors
{
    /// <summary>
    /// Default implementation of the inspector used to edit Vector4 value type properties.
    /// </summary>
    [CustomEditor(typeof(Vector4)), DefaultEditor]
    public class Vector4Editor : CustomEditor
    {
        /// <summary>
        /// The X component editor.
        /// </summary>
        protected FloatValueElement XElement;

        /// <summary>
        /// The Y component editor.
        /// </summary>
        protected FloatValueElement YElement;

        /// <summary>
        /// The Z component editor.
        /// </summary>
        protected FloatValueElement ZElement;

        /// <summary>
        /// The W component editor.
        /// </summary>
        protected FloatValueElement WElement;

        /// <inheritdoc />
        public override DisplayStyle Style => DisplayStyle.Inline;

        /// <inheritdoc />
        public override void Initialize(LayoutElementsContainer layout)
        {
            var grid = layout.CustomContainer<UniformGridPanel>();
            var gridControl = grid.CustomControl;
            gridControl.ClipChildren = false;
            gridControl.Height = TextBox.DefaultHeight;
            gridControl.SlotsHorizontally = 4;
            gridControl.SlotsVertically = 1;

            LimitAttribute limit = null;
            if (Values.Info != null)
            {
                var attributes = Values.Info.GetCustomAttributes(true);
                limit = (LimitAttribute)attributes.FirstOrDefault(x => x is LimitAttribute);
            }

            XElement = grid.FloatValue();
            XElement.SetLimits(limit);
            XElement.FloatValue.ValueChanged += OnValueChanged;

            YElement = grid.FloatValue();
            YElement.SetLimits(limit);
            YElement.FloatValue.ValueChanged += OnValueChanged;

            ZElement = grid.FloatValue();
            ZElement.SetLimits(limit);
            ZElement.FloatValue.ValueChanged += OnValueChanged;

            WElement = grid.FloatValue();
            WElement.SetLimits(limit);
            WElement.FloatValue.ValueChanged += OnValueChanged;
        }

        private void OnValueChanged()
        {
            if (IsSetBlocked)
                return;

            SetValue(new Vector4(
                         XElement.FloatValue.Value,
                         YElement.FloatValue.Value,
                         ZElement.FloatValue.Value,
                         WElement.FloatValue.Value));
        }

        /// <inheritdoc />
        public override void Refresh()
        {
            if (HasDiffrentValues)
            {
                // TODO: support different values for ValueBox<T>
            }
            else
            {
                var value = (Vector4)Values[0];
                XElement.FloatValue.Value = value.X;
                YElement.FloatValue.Value = value.Y;
                ZElement.FloatValue.Value = value.Z;
                WElement.FloatValue.Value = value.W;
            }
        }
    }
}
