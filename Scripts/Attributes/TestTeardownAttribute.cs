using System;

[AttributeUsage(AttributeTargets.Method)]
public class TestTeardownAttribute : Attribute
{
	public TestTeardownAttribute()
	{
	}
}