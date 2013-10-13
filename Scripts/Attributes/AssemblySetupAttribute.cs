using System;

namespace UnTested
{
	[AttributeUsage(AttributeTargets.Method)]
	public class AssemblySetupAttribute : Attribute
	{
		public AssemblySetupAttribute()
		{
		}
	}
}