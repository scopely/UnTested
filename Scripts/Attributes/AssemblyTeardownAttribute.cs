using System;

namespace UnTested
{
	[AttributeUsage(AttributeTargets.Method)]
	public class AssemblyTeardownAttribute : Attribute
	{
		public AssemblyTeardownAttribute()
		{
		}
	}
}