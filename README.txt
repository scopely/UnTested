UnTested v1.0

Creating Test Fixtures
----------------------
1. Create a C# Script, add using UnTested; at the top of the file.
If you want to write Async Methods you will need to include:
using System.Collections;

2. Create a C# Class with the [TestFixture] Attribute.
All Attributted methods in this class must be public and have a return type of
void (Normal) or IEnumerator (Async).

3. (Optional) Create methods within this Class with the [TestSetup] Attribute
These methods will get called before each Test in the Fixture is Run.

4. Create methods within this Class with the [Test] Attribute.
These methods will be the actual Tests use the static Assert Functions to compose your Tests.

5. (Optional) Create methods within this Class with the [TestTeardown] Attribute
These methods will get called after each Test in the Fixture has Run.

Example:

using System.Collections;
using UnityEngine;
using UnTested;

[TestFixture]
public class ExampleTests 
{
	[TestSetup]
	public void ExampleSetup ()
	{
		Assert.IsNull (null, "THIS SHOULD PASS");
	}

	[Test]
	public IEnumerator AsyncTest ()
	{
		yield return new WaitForSeconds (0.5f);
		Assert.IsFalse (false, "THIS SHOULD PASS");
	}

	[Test]
	public void NormalTest ()
	{
		Assert.IsTrue (true, "THIS SHOULD PASS");
	}

	[TestTeardown]
	public void ExampleTeardown ()
	{
		Assert.IsNull (null, "THIS SHOULD PASS");
	}
}

(Optional) Creating Assembly Fixtures
-------------------------------------
1. Create a C# Script, add using UnTested; at the top of the file.
If you want to write Async Methods you will need to include:
using System.Collections;

2. Create a C# Class with the [AssemblyFixture] Attribute.
All Attributted methods in this class must be public and have a return type of
void (Normal) or IEnumerator (Async).

3. (Optional) Create methods within this Class with the [AssemblySetup] Attribute
These methods will get called before ALL Test are Run.

4. (Optional) Create methods within this Class with the [AssemblyTeardown] Attribute
These methods will get called after ALL Tests have Run.

Example:

using System.Collections;
using UnityEngine;
using UnTested;

[AssemblyFixture]
public class AssemblyFixtureExample 
{
	[AssemblySetup]
	public void ExampleSetup ()
	{
		Assert.IsNotNull (null, "THIS SHOULD FAIL");
	}

	[AssemblyTeardown]
	public void ExampleTeardown ()
	{
		Assert.IsNull (null, "THIS SHOULD PASS");
	}
}

Running Tests
-------------
1. From the Unity Menu Bar go to Window -> UnTested.

2. This will open the included Scene (Assets/UnTested/Scenes/Tests).
This Scene has a GameObject TestRunner in the Scene who will run the Tests on Enable.

3. The UnTested Window will popup as well, you can dock this window as you like.

4. You should see any Tests you created populate the Window under their associated Fixtures.

5. Toggle which Tests you want to Run.

6. Click the Run / Run Paused Button to start Running your Tests.

7. While the Editor is Running the Tests, the UnTested Window will update with the status of the
Fixtures and Tests. You will also have buttons to Pause/Resume and Stop the Editor.

8. Select a Test or Fixture from the List to see the Logs Output for the scope of that selected item
in the Log Window at the bottom of the UnTested Window.

9. Once all Tests have Finished a Summary Log will Output to the Console Window. 

10. Click Done to Stop Running.