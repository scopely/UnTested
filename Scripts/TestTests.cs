using UnityEngine;
using System.Collections;
using System;
using UnTested;

[AssemblyFixture]
public class AssemblyTests {

	[AssemblySetup]
	public IEnumerator TestAssemblySetup ()
	{
		yield return new WaitForSeconds (2.0f);
		Assert.IsNull (null, "THIS SHOULD PASS");
	}

	[AssemblyTeardown]
	public IEnumerator TestAssmeblyTeardown ()
	{
		yield return new WaitForSeconds (2.0f);
		Assert.IsNull (null, "THIS SHOULD PASS");
	}
}

[TestFixture]
public class TestTests {

	[TestSetup]
	public IEnumerator TestSetup ()
	{
		yield return new WaitForSeconds (2.0f);
		Assert.IsNull (null, "THIS SHOULD PASS");
	}

	[Test]
	public IEnumerator Test ()
	{
		yield return new WaitForSeconds (2.0f);
		Assert.IsNull (null, "THIS SHOULD PASS");
	}

	[TestTeardown]
	public IEnumerator TestTeardown ()
	{
		yield return new WaitForSeconds (2.0f);
		Assert.IsNull (null, "THIS SHOULD Pass");
	}
}
