using System;

namespace UnTested 
{
	[AttributeUsage(AttributeTargets.Class)]
	public class AssemblyFixtureAttribute : Attribute
	{
		public AssemblyFixtureAttribute()
		{
		}
	}
}