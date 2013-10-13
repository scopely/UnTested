using System;

namespace UnTested
{
	[AttributeUsage(AttributeTargets.Method)]
	public class TestSetupAttribute : Attribute
	{
		public TestSetupAttribute()
		{
		}
	}
}