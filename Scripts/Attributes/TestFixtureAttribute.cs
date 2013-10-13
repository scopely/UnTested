using System;

[AttributeUsage(AttributeTargets.Class)]
public class TestFixtureAttribute : Attribute
{
	public TestFixtureAttribute()
	{
	}
}