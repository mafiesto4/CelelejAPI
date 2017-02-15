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
		/// Font asset contains glyph collection and cached data used to render text
		/// </summary>
		public partial class FontAsset : Asset
		{
			/// <summary>
			/// Gets font family name
			/// </summary>
			[UnmanagedCall]
			public string FamilyName
			{
#if UNIT_TEST_COMPILANT
				get; set;
#else
				get { return Internal_GetFamilyName(unmanagedPtr); }
#endif
			}

			/// <summary>
			/// Gets font style name
			/// </summary>
			[UnmanagedCall]
			public string StyleName
			{
#if UNIT_TEST_COMPILANT
				get; set;
#else
				get { return Internal_GetStyleName(unmanagedPtr); }
#endif
			}

			/// <summary>
			/// Creates font object of given characters size.
			/// </summary>
			/// <param name="size">Characters size.</param>
			/// <returns>Font object.</returns>
#if UNIT_TEST_COMPILANT
			[Obsolete("Unit tests, don't support methods calls.")]
#endif
			[UnmanagedCall]
			public Font CreateFont(int size) 
			{
#if UNIT_TEST_COMPILANT
				throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
				return Internal_CreateFont(unmanagedPtr, size);
#endif
			}

#region Internal Calls
#if !UNIT_TEST_COMPILANT
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string Internal_GetFamilyName(IntPtr obj);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string Internal_GetStyleName(IntPtr obj);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Font Internal_CreateFont(IntPtr obj, int size);
#endif
#endregion
	}
}

