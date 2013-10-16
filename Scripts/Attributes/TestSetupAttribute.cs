using System;

namespace UnTested
{
	/// <summary>
	/// Test setup attribute - methods using this attribute, within a TestFixture class, will get called before each Test is Run on that Fixture.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class TestSetupAttribute : Attribute
	{
		public TestSetupAttribute()
		{
		}
	}
}