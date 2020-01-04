// Copyright (c) 2012-2020 Wojciech Figat. All rights reserved.
// This code was generated by a tool. Changes to this file may cause
// incorrect behavior and will be lost if the code is regenerated.

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FlaxEngine
{
    /// <summary>
    /// Generic type of Json-format asset. It provides the managed representation of this resource data so it can be accessed via C# API.
    /// </summary>
    public partial class JsonAsset : Asset
    {
        /// <summary>
        /// Creates new <see cref="JsonAsset"/> object.
        /// </summary>
        protected JsonAsset() : base()
        {
        }

        /// <summary>
        /// Gets the data type name from the header. Allows to recognize the stored data type.
        /// </summary>
        [UnmanagedCall]
        public string DataTypeName
        {
#if UNIT_TEST_COMPILANT
            get; set;
#else
            get { return Internal_GetDataTypeName(unmanagedPtr); }
#endif
        }

        /// <summary>
        /// Gets the data from the asset. Allows to deserialize stored object properties (from json format).
        /// </summary>
        [UnmanagedCall]
        public string Data
        {
#if UNIT_TEST_COMPILANT
            get; set;
#else
            get { return Internal_GetData(unmanagedPtr); }
#endif
        }

        #region Internal Calls

#if !UNIT_TEST_COMPILANT
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string Internal_GetDataTypeName(IntPtr obj);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string Internal_GetData(IntPtr obj);
#endif

        #endregion
    }
}
