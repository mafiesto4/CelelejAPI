// Copyright (c) 2012-2020 Wojciech Figat. All rights reserved.
// This code was generated by a tool. Changes to this file may cause
// incorrect behavior and will be lost if the code is regenerated.

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FlaxEngine
{
    /// <summary>
    /// Asset objects base class.
    /// </summary>
    public abstract partial class Asset : Object
    {
        /// <summary>
        /// Creates new <see cref="Asset"/> object.
        /// </summary>
        protected Asset() : base()
        {
        }

        /// <summary>
        /// Gets the asset path.
        /// </summary>
        [UnmanagedCall]
        public string Path
        {
#if UNIT_TEST_COMPILANT
            get; set;
#else
            get { return Internal_GetPath(unmanagedPtr); }
#endif
        }

        /// <summary>
        /// Returns true if asset is loaded, otherwise false.
        /// </summary>
        [UnmanagedCall]
        public bool IsLoaded
        {
#if UNIT_TEST_COMPILANT
            get; set;
#else
            get { return Internal_GetIsLoaded(unmanagedPtr); }
#endif
        }

        /// <summary>
        /// Returns true if last asset loading failed, otherwise false.
        /// </summary>
        [UnmanagedCall]
        public bool LastLoadFailed
        {
#if UNIT_TEST_COMPILANT
            get; set;
#else
            get { return Internal_GetLastLoadFailed(unmanagedPtr); }
#endif
        }

        /// <summary>
        /// Determines whether this asset is virtual (generated or temporary, has no storage so it won't be saved).
        /// </summary>
        [UnmanagedCall]
        public bool IsVirtual
        {
#if UNIT_TEST_COMPILANT
            get; set;
#else
            get { return Internal_GetIsVirtual(unmanagedPtr); }
#endif
        }

        /// <summary>
        /// Gets amount of references to that asset.
        /// </summary>
        [UnmanagedCall]
        public int RefCount
        {
#if UNIT_TEST_COMPILANT
            get; set;
#else
            get { return Internal_GetRefCount(unmanagedPtr); }
#endif
        }

        /// <summary>
        /// Stops the current thread execution and waits until asset will be loaded (loading will fail, success or be cancelled).
        /// </summary>
        /// <param name="timeoutInMilliseconds">Custom timeout value in milliseconds.</param>
        /// <returns>True if cannot load that asset (failed or has been cancelled), otherwise false.</returns>
#if UNIT_TEST_COMPILANT
        [Obsolete("Unit tests, don't support methods calls.")]
#endif
        [UnmanagedCall]
        public bool WaitForLoaded(double timeoutInMilliseconds = 30000.0)
        {
#if UNIT_TEST_COMPILANT
            throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
            return Internal_WaitForLoaded(unmanagedPtr, timeoutInMilliseconds);
#endif
        }

        /// <summary>
        /// Reloads the asset.
        /// </summary>
#if UNIT_TEST_COMPILANT
        [Obsolete("Unit tests, don't support methods calls.")]
#endif
        [UnmanagedCall]
        public bool Reload()
        {
#if UNIT_TEST_COMPILANT
            throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
            return Internal_Reload(unmanagedPtr);
#endif
        }

        /// <summary>
        /// Gets the asset references. Supported only in Editor.
        /// </summary>
        /// <remarks>
        /// For some asset types (e.g. scene or prefab) it may contain invalid asset ids due to not perfect gather method, which is optimized to perform scan very quickly. Before using those ids perform simple validation via Content cache API. The result collection contains only 1-level-deep references (only direct ones) and is invalid if asset is not loaded. Also the output data may have duplicated asset ids or even invalid ids (Guid.Empty).
        /// </remarks>
        /// <returns>The output collection of the asset ids referenced by this asset.</returns>
#if UNIT_TEST_COMPILANT
        [Obsolete("Unit tests, don't support methods calls.")]
#endif
        [UnmanagedCall]
        public Guid[] GetReferences()
        {
#if UNIT_TEST_COMPILANT
            throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
            return Internal_GetReferences(unmanagedPtr);
#endif
        }

        #region Internal Calls

#if !UNIT_TEST_COMPILANT
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string Internal_GetPath(IntPtr obj);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool Internal_GetIsLoaded(IntPtr obj);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool Internal_GetLastLoadFailed(IntPtr obj);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool Internal_GetIsVirtual(IntPtr obj);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int Internal_GetRefCount(IntPtr obj);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool Internal_WaitForLoaded(IntPtr obj, double timeoutInMilliseconds);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool Internal_Reload(IntPtr obj);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern Guid[] Internal_GetReferences(IntPtr obj);
#endif

        #endregion
    }
}
