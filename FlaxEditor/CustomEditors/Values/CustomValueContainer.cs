// Copyright (c) 2012-2019 Wojciech Figat. All rights reserved.

using System;
using FlaxEngine;

namespace FlaxEditor.CustomEditors
{
    /// <summary>
    /// Custom <see cref="ValueContainer"/> for any type of storage and data management logic.
    /// </summary>
    /// <seealso cref="FlaxEditor.CustomEditors.ValueContainer" />
    public sealed class CustomValueContainer : ValueContainer
    {
        /// <summary>
        /// Get value delegate.
        /// </summary>
        /// <param name="instance">The parent object instance.</param>
        /// <param name="index">The index (for multi selected objects).</param>
        /// <returns>The value.</returns>
        public delegate object GetDelegate(object instance, int index);

        /// <summary>
        /// Set value delegate.
        /// </summary>
        /// <param name="instance">The parent object instance.</param>
        /// <param name="index">The index (for multi selected objects).</param>
        /// <param name="value">The value.</param>
        public delegate void SetDelegate(object instance, int index, object value);

        private readonly GetDelegate _getter;
        private readonly SetDelegate _setter;
        private readonly object[] _attributes;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomValueContainer"/> class.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <param name="getter">The value getter.</param>
        /// <param name="setter">The value setter.</param>
        /// <param name="attributes">The custom type attributes used to override the value editor logic or appearance (eg. instance of <see cref="LimitAttribute"/>).</param>
        public CustomValueContainer(Type valueType, GetDelegate getter, SetDelegate setter, object[] attributes = null)
        : base(null, valueType)
        {
            if (getter == null || setter == null)
                throw new ArgumentNullException();

            _getter = getter;
            _setter = setter;
            _attributes = attributes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomValueContainer"/> class.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <param name="initialValue">The initial value.</param>
        /// <param name="getter">The value getter.</param>
        /// <param name="setter">The value setter.</param>
        /// <param name="attributes">The custom type attributes used to override the value editor logic or appearance (eg. instance of <see cref="LimitAttribute"/>).</param>
        public CustomValueContainer(Type valueType, object initialValue, GetDelegate getter, SetDelegate setter, object[] attributes = null)
        : this(valueType, getter, setter, attributes)
        {
            Add(initialValue);
        }

        /// <inheritdoc />
        public override object[] GetAttributes()
        {
            return _attributes ?? base.GetAttributes();
        }

        /// <inheritdoc />
        public override void Refresh(ValueContainer instanceValues)
        {
            if (instanceValues == null || instanceValues.Count != Count)
                throw new ArgumentException();

            for (int i = 0; i < Count; i++)
            {
                var v = instanceValues[i];
                this[i] = _getter(v, i);
            }
        }

        /// <inheritdoc />
        public override void Set(ValueContainer instanceValues, object value)
        {
            if (instanceValues == null || instanceValues.Count != Count)
                throw new ArgumentException();

            for (int i = 0; i < Count; i++)
            {
                var v = instanceValues[i];
                _setter(v, i, value);
                this[i] = value;
            }
        }

        /// <inheritdoc />
        public override void Set(ValueContainer instanceValues, ValueContainer values)
        {
            if (instanceValues == null || instanceValues.Count != Count)
                throw new ArgumentException();
            if (values == null || values.Count != Count)
                throw new ArgumentException();

            for (int i = 0; i < Count; i++)
            {
                var v = instanceValues[i];
                var value = ((CustomValueContainer)values)[i];
                _setter(v, i, value);
                this[i] = value;
            }
        }

        /// <inheritdoc />
        public override void Set(ValueContainer instanceValues)
        {
            if (instanceValues == null || instanceValues.Count != Count)
                throw new ArgumentException();

            for (int i = 0; i < Count; i++)
            {
                var v = instanceValues[i];
                _setter(v, i, _getter(v, i));
            }
        }

        /// <inheritdoc />
        public override void RefreshReferenceValue(object instanceValue)
        {
            // Not supported
        }
    }
}
