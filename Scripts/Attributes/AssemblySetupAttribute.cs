using System;

namespace UnTested
{
	/// <summary>
	/// Assembly setup attribute - methods using this attribute, within a AssemblyFixture class, will get called before ALL Tests are run.
	/// NOTE: Failures within these methods will be caught by TestRunner but will only be outputed in the Console Window.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class AssemblySetupAttribute : Attribute
	{
		public AssemblySetupAttribute()
		{
		}
	}
}