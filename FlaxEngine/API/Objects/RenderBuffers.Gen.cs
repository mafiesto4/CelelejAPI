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

namespace FlaxEngine.Rendering
{
	/// <summary>
	/// Allows to perform custom rendering to texture.
	/// </summary>
	public sealed partial class RenderBuffers : Object
	{
		/// <summary>
		/// Creates new <see cref="RenderBuffers"/> object.
		/// </summary>
		private RenderBuffers() : base()
		{
		}

		/// <summary>
		/// Gets buffer textures width (in pixels).
		/// </summary>
		[UnmanagedCall]
		public int Width
		{
#if UNIT_TEST_COMPILANT
			get; set;
#else
			get { return Internal_GetWidth(unmanagedPtr); }
#endif
		}

		/// <summary>
		/// Gets buffer textures height (in pixels).
		/// </summary>
		[UnmanagedCall]
		public int Height
		{
#if UNIT_TEST_COMPILANT
			get; set;
#else
			get { return Internal_GetHeight(unmanagedPtr); }
#endif
		}

		/// <summary>
		/// Gets buffer textures aspect ratio (width / height).
		/// </summary>
		[UnmanagedCall]
		public float AspectRatio
		{
#if UNIT_TEST_COMPILANT
			get; set;
#else
			get { return Internal_GetAspectRatio(unmanagedPtr); }
#endif
		}

		/// <summary>
		/// Gets or sets buffer textures size (in pixels).
		/// </summary>
		[UnmanagedCall]
		public Vector2 Size
		{
#if UNIT_TEST_COMPILANT
			get; set;
#else
			get { Vector2 resultAsRef; Internal_GetSize(unmanagedPtr, out resultAsRef); return resultAsRef; }
			set { Internal_SetSize(unmanagedPtr, ref value); }
#endif
		}

		/// <summary>
		/// Creates the new render buffers object.
		/// </summary>
		/// <returns>Created render buffers object.</returns>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public static RenderBuffers Create() 
		{
#if UNIT_TEST_COMPILANT
			throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
			return Internal_Create();
#endif
		}

		/// <summary>
		/// Initializes render buffers.
		/// </summary>
		/// <param name="width">The surface width in pixels.</param>
		/// <param name="height">The surface height in pixels.</param>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public void Init(int width, int height) 
		{
#if UNIT_TEST_COMPILANT
			throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
			Internal_Init(unmanagedPtr, width, height);
#endif
		}

		/// <summary>
		/// Disposes render buffers data.
		/// </summary>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public void Dispose() 
		{
#if UNIT_TEST_COMPILANT
			throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
			Internal_Dispose(unmanagedPtr);
#endif
		}

#region Internal Calls
#if !UNIT_TEST_COMPILANT
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int Internal_GetWidth(IntPtr obj);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int Internal_GetHeight(IntPtr obj);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern float Internal_GetAspectRatio(IntPtr obj);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_GetSize(IntPtr obj, out Vector2 resultAsRef);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_SetSize(IntPtr obj, ref Vector2 val);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern RenderBuffers Internal_Create();
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_Init(IntPtr obj, int width, int height);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_Dispose(IntPtr obj);
#endif
#endregion
	}
}

