using System;

namespace UnTested 
{
	[AttributeUsage(AttributeTargets.Class)]
	public class TestFixtureAttribute : Attribute
	{
		public TestFixtureAttribute()
		{
		}
	}
}