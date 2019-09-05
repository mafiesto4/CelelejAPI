// Copyright (c) 2012-2019 Wojciech Figat. All rights reserved.
// This code was generated by a tool. Changes to this file may cause
// incorrect behavior and will be lost if the code is regenerated.

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FlaxEngine
{
    /// <summary>
    /// The prefab manager handles the prefabs creation, synchronization and serialization.
    /// </summary>
    public static partial class PrefabManager
    {
        /// <summary>
        /// Spawns the instance of the prefab objects. If parent actor is specified then created actors are fully initialized. Otherwise it won't be attached to any scene.
        /// </summary>
        /// <param name="prefab">The prefab asset.</param>
        /// <param name="parent">The parent actor to add spawned object instance. Can be null to just deserialize contents of the prefab.</param>
        /// <returns>The created actor (root) or null if failed</returns>
#if UNIT_TEST_COMPILANT
        [Obsolete("Unit tests, don't support methods calls.")]
#endif
        [UnmanagedCall]
        public static Actor SpawnPrefab(Prefab prefab, Actor parent)
        {
#if UNIT_TEST_COMPILANT
            throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
            return Internal_SpawnPrefab(Object.GetUnmanagedPtr(prefab), Object.GetUnmanagedPtr(parent));
#endif
        }

        #region Internal Calls

#if !UNIT_TEST_COMPILANT
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern Actor Internal_SpawnPrefab(IntPtr prefab, IntPtr parent);
#endif

        #endregion
    }
}
