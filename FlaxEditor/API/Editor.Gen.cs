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

namespace FlaxEditor
{
	/// <summary>
	/// The main managed editor class. Editor root object.
	/// </summary>
	public partial class Editor
	{
		/// <summary>
		/// Closes editor splash screen popup window.
		/// </summary>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public static void CloseSplashScreen() 
		{
#if UNIT_TEST_COMPILANT
			throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
			Internal_CloseSplashScreen();
#endif
		}

		/// <summary>
		/// Creates new asset at the target location.
		/// </summary>
		/// <param name="type">New asset type.</param>
		/// <param name="outputPath">Output asset path.</param>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public static bool CreateAsset(NewAssetType type, string outputPath) 
		{
#if UNIT_TEST_COMPILANT
			throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
			return Internal_CreateAsset(type, outputPath);
#endif
		}

		/// <summary>
		/// Checks if can import asset with the given extension.
		/// </summary>
		/// <param name="extension">The file extension.</param>
		/// <returns>True if can import files with given extension, otherwise false.</returns>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public static bool CanImport(string extension) 
		{
#if UNIT_TEST_COMPILANT
			throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
			return Internal_CanImport(extension);
#endif
		}
        
		/// <summary>
		/// Checks if every managed assembly has been loaded (including user scripts assembly).
		/// </summary>
		[UnmanagedCall]
		public static bool IsEveryAssemblyLoaded
		{
#if UNIT_TEST_COMPILANT
			get; set;
#else
			get { return Internal_GetIsEveryAssemblyLoaded(); }
#endif
		}

#region Internal Calls
#if !UNIT_TEST_COMPILANT
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_CloseSplashScreen();
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool Internal_CreateAsset(NewAssetType type, string outputPath);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool Internal_CanImport(string extension);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string Internal_GetProjectName();
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool Internal_GetIsEveryAssemblyLoaded();
#endif
#endregion
	}
}

