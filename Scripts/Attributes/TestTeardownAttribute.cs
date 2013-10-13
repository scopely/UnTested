using System;

namespace UnTested
{
	[AttributeUsage(AttributeTargets.Method)]
	public class TestTeardownAttribute : Attribute
	{
		public TestTeardownAttribute()
		{
		}
	}
}