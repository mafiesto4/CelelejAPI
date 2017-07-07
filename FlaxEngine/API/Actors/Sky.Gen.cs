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
	/// Sky actor renders atmosphere around the scene with fog and sky
	/// </summary>
	public sealed partial class Sky : Actor
	{
		/// <summary>
		/// Creates new <see cref="Sky"/> object.
		/// </summary>
		private Sky() : base()
		{
		}

		/// <summary>
		/// Gets or sets value indicating if visual element affects the world
		/// </summary>
		[UnmanagedCall]
		public bool AffectsWorld
		{
#if UNIT_TEST_COMPILANT
			get; set;
#else
			get { return Internal_GetAffectsWorld(unmanagedPtr); }
			set { Internal_SetAffectsWorld(unmanagedPtr, value); }
#endif
		}

		/// <summary>
		/// Gets or sets linked directional light actor that is used to simulate sun
		/// </summary>
		[UnmanagedCall]
		public DirectionalLight SunLight
		{
#if UNIT_TEST_COMPILANT
			get; set;
#else
			get { return Internal_GetSunLight(unmanagedPtr); }
			set { Internal_SetSunLight(unmanagedPtr, Object.GetUnmanagedPtr(value)); }
#endif
		}

#region Internal Calls
#if !UNIT_TEST_COMPILANT
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool Internal_GetAffectsWorld(IntPtr obj);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_SetAffectsWorld(IntPtr obj, bool val);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern DirectionalLight Internal_GetSunLight(IntPtr obj);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_SetSunLight(IntPtr obj, IntPtr val);
#endif
#endregion
	}
}

