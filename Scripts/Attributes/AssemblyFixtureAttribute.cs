using System;

namespace UnTested 
{
	/// <summary>
	/// Assembly fixture attribute - Classes using this attribute can specify methods to be run by UnTested:
	/// 	AssemblySetup
	/// 	AssemblyTeardown
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class AssemblyFixtureAttribute : Attribute
	{
		public AssemblyFixtureAttribute()
		{
		}
	}
}