////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2012-2017 Flax Engine. All rights reserved.
////////////////////////////////////////////////////////////////////////////////////
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
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
		/// Gets asset name
		/// </summary>
		[UnmanagedCall]
		public string Name
		{
#if UNIT_TEST_COMPILANT
			get; set;
#else
			get { return Internal_GetName(unmanagedPtr); }
#endif
		}

		/// <summary>
		/// Returns true if asset is loaded.
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
		/// Gets amount of references to that asset
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
		/// <param name="timeoutInMiliseconds">Custom timeout value in miliseconds.</param>
		/// <returns>True if cannot load that asset (failed or has been cancelled), otherwise false.</returns>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public bool WaitForLoaded(double timeoutInMiliseconds = 10000.0) 
		{
#if UNIT_TEST_COMPILANT
			throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
			return Internal_WaitForLoaded(unmanagedPtr, timeoutInMiliseconds);
#endif
		}

#region Internal Calls
#if !UNIT_TEST_COMPILANT
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string Internal_GetName(IntPtr obj);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool Internal_GetIsLoaded(IntPtr obj);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int Internal_GetRefCount(IntPtr obj);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool Internal_WaitForLoaded(IntPtr obj, double timeoutInMiliseconds);
#endif
#endregion
	}
}

