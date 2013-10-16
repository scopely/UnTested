using System;

namespace UnTested 
{
	/// <summary>
	/// Test fixture attribute - Classes using this attribute can specify methods to be run by UnTested:
	/// 	TestSetup
	/// 	Test
	/// 	TestTeardown
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class TestFixtureAttribute : Attribute
	{
		public TestFixtureAttribute()
		{
		}
	}
}