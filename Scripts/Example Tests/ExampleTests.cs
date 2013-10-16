//using System.Collections;
//using UnityEngine;
//using UnTested;
//
//[TestFixture]
//public class ExampleTests {
//
//	[TestSetup]
//	public void ExampleSetup ()
//	{
//		Assert.IsNull (null, "THIS SHOULD PASS");
//	}
//
//	[Test]
//	public IEnumerator AsyncTest ()
//	{
//		yield return new WaitForSeconds (0.5f);
//		Assert.IsFalse (false, "THIS SHOULD PASS");
//	}
//
//	[Test]
//	public void NormalTest ()
//	{
//		Assert.IsTrue (true, "THIS SHOULD PASS");
//	}
//
//	[TestTeardown]
//	public void ExampleTeardown ()
//	{
//		Assert.IsNull (null, "THIS SHOULD PASS");
//	}
//}