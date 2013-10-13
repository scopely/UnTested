using System;

namespace UnTested
{
	[AttributeUsage(AttributeTargets.Method)]
	public class TestAttribute : Attribute
	{
		public TestAttribute()
		{
		}
	}
}