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
	/// Empty actor is usefull to create hierarchy and/or hold scripts. See <see cref="Script"/>.
	/// </summary>
	[Serializable]
	public sealed partial class EmptyActor : Actor
	{
		/// <summary>
		/// Creates new <see cref="EmptyActor"/> object.
		/// </summary>
		private EmptyActor() : base()
		{
		}

		/// <summary>
		/// Creates new instance of <see cref="EmptyActor"/> object.
		/// </summary>
		/// <returns>Created object.</returns>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public static EmptyActor New() 
		{
#if UNIT_TEST_COMPILANT
			throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
			return Internal_Create(typeof(EmptyActor)) as EmptyActor;
#endif
		}

#region Internal Calls
#if !UNIT_TEST_COMPILANT
#endif
#endregion
	}
}

