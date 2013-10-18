using System.Collections;
using UnityEngine;
using UnTested;

[TestFixture]
public class ExampleTest {

	GameObject sphere = null;
	GameObject plane = null;

	[TestSetup]
	public IEnumerator GetSphere ()
	{
		if(sphere == null)
		{
			sphere = GameObject.Find ("Sphere");
			sphere.rigidbody.useGravity = true;
		}
		if(plane == null)
		{
			plane = GameObject.Find ("Plane");
		}

		yield return new WaitForFixedUpdate ();
	}

	[Test]
	public IEnumerator TestSphereCollidesWithPlane ()
	{
		float timer = 5.0f;

		while(sphere.rigidbody.velocity.y != 0.0f && sphere.transform.position.y > plane.transform.position.y && timer > 0.0f)
		{
			yield return new WaitForFixedUpdate ();
			timer -= Time.deltaTime;
		}

		Assert.AreEqual (0.0f, sphere.rigidbody.velocity.y);
	}
}