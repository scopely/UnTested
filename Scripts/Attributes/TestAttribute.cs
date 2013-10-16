using System;

namespace UnTested
{
	/// <summary>
	/// Test attribute - methods using this attribute, within a TestFixture class, will get called when running unit tests.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class TestAttribute : Attribute
	{
		public TestAttribute()
		{
		}
	}
}