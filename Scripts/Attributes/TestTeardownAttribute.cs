using System;

namespace UnTested
{
	/// <summary>
	/// Test teardown attribute - methods using this attribute, within a TestFixture class, will get called after each Test is Run on that Fixture.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class TestTeardownAttribute : Attribute
	{
		public TestTeardownAttribute()
		{
		}
	}
}