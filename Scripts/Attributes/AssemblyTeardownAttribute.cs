using System;

namespace UnTested
{
	/// <summary>
	/// Assembly teardown attribute - methods using this attribute, within a AssemblyFixture class, will get called after ALL Tests have run.
	/// NOTE: Failures within these methods will be caught by TestRunner but will only be outputed in the Console Window.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class AssemblyTeardownAttribute : Attribute
	{
		public AssemblyTeardownAttribute()
		{
		}
	}
}