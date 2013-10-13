using System;

[AttributeUsage(AttributeTargets.Method)]
public class TestSetupAttribute : Attribute
{
	public TestSetupAttribute()
	{
	}
}

