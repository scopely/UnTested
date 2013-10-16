using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace UnTested 
{
	#region Enums
	public enum TestState 
	{
		None,
		InProgress,
		Failed,
		Passed,
		MAX
	};
	#endregion

	#region Entry Classes
	/// <summary>
	/// Log entry.
	/// </summary>
	public class LogEntry
	{
		public string LogMsg { get; set; }
		public LogType LogType { get; set; }

		public LogEntry(string msg, LogType logType)
		{
			LogMsg = msg;
			LogType = logType;
		}
	}

	/// <summary>
	/// Assembly entry.
	/// </summary>
	public class AssemblyEntry
	{
		public TestState State { get; set; }
		public MethodInfo Method { get; set; }
		public List<LogEntry> Logs { get; set; }
	}

	/// <summary>
	/// Fixture entry.
	/// </summary>
	public class FixtureEntry
	{
		public bool WillRun { get; set; }
		public TestState State { get; set; }
		public Type FixtureType { get; set; }
		public List<LogEntry> Logs { get; set; }
	}

	/// <summary>
	/// Test entry.
	/// </summary>
	public class TestEntry
	{
		public bool WillRun { get; set; }
		public TestState State { get; set; }
		public MethodInfo TestMethod { get; set; }
		public TestState SetupState { get; set; }
		public TestState TeardownState { get; set; }
		public List<LogEntry> Logs { get; set; }
	}
	#endregion

	/// <summary>
	/// Tests config - Serializable Configuration Singleton, which looks through Assembly for Fixtures and Tests
	/// determining whether they should run or not.
	/// </summary>
	[Serializable]
	public class TestsConfig : ScriptableObject 
	{
		#region Collections
		public Dictionary<FixtureEntry, List<AssemblyEntry>> AssemblySetups { get; set; }
		public Dictionary<FixtureEntry, List<AssemblyEntry>> AssemblyTeardowns { get; set; }
		public Dictionary<FixtureEntry, List<TestEntry>> Tests { get; set; }
		public int NumberOfTestsToRun = 0;
		#endregion

		#region Private Fields
		[SerializeField]
		private string configureString;
		private string oldStr;
		#endregion
		
		#region Lifecycle
		private static TestsConfig instance = null;

		/// <summary>
		/// Gets the Singleton instance.
		/// </summary>
		/// <value>The instance.</value>
		public static TestsConfig Instance 
		{
			get
			{
				if(instance == null) {
					instance = ScriptableObject.CreateInstance<TestsConfig> ();
				}

				return instance;
			}
		}

		/// <summary>
		/// Set Instance and Reload on enable.
		/// </summary>
		private void OnEnable ()
		{
			hideFlags = HideFlags.HideAndDontSave;
			instance = this;
			Reload ();
		}

		/// <summary>
		/// Remove the instance on disable.
		/// </summary>
		private void OnDisable ()
		{
			instance = null;
		}

		/// <summary>
		/// Remove the instance on destroy.
		/// </summary>
		private void OnDestroy ()
		{
			instance = null;
		}

		/// <summary>
		/// Update this instance.
		/// </summary>
		public void Update ()
		{			
			if(oldStr != configureString)
			{
				Reload();
			}
		}
		#endregion
		
		#region Configuration Helpers
		/// <summary>
		/// Sets all fixture and test entries to the bool passed in.
		/// </summary>
		/// <param name="onOff">If set to <c>true</c> on off.</param>
		public void SetAllOn(bool onOff) {
			foreach (FixtureEntry fixtureEntry in Tests.Keys) 
			{
				fixtureEntry.WillRun = onOff;
				foreach (TestEntry entry in Tests[fixtureEntry]) 
				{
					entry.WillRun = onOff;
				}
			}

			Persist ();
		}

		/// <summary>
		/// Reload this instance.
		/// </summary>
		public void Reload () {
			LoadTestsFromAssembly ();
			ReadConfigFromString ();
		}
		#endregion

		#region Data Management
		/// <summary>
		/// Persist this instance.
		/// </summary>
		public void Persist(){

			// Save as Text
			configureString = "";

			foreach(FixtureEntry fixEntry in Tests.Keys) 
			{
				if(fixEntry.WillRun) 
				{
					configureString += string.Format ("{0}|", fixEntry.FixtureType.Name);

					List<TestEntry> tests = Tests [fixEntry];
					for (int t = 0; t < tests.Count; ++t) 
					{
						TestEntry test = tests [t];
						if (test.WillRun) 
						{
							configureString += string.Format ("{0}", test.TestMethod.Name);
							if (t < tests.Count - 1) 
							{
								configureString += ",";
							}
						}

					}
					configureString += "|\n";
				}
			}

			oldStr = configureString;
		}

		/// <summary>
		/// Reads the config from string.
		/// </summary>
		private void ReadConfigFromString()
		{
			NumberOfTestsToRun = 0;
		
			if (configureString != null) 
			{
				// Load from Text
				string[] fixtures = configureString.Split ('\n');

				for (int f = 0; f < fixtures.Length; ++f) 
				{
					string[] fixtureComponents = fixtures [f].Split ('|');
					string fixtureName = fixtureComponents [0];

					foreach (FixtureEntry fixEntry in Tests.Keys) 
					{
						if (fixEntry.FixtureType.Name == fixtureName) 
						{
							fixEntry.WillRun = true;

							string[] tests = fixtureComponents [1].Split (',');
							for (int t = 0; t < tests.Length; ++t) 
							{
								string testName = tests [t];
								foreach (TestEntry testEntry in Tests[fixEntry]) 
								{
									if (testEntry.TestMethod.Name == testName) 
									{
										testEntry.WillRun = true;
										++NumberOfTestsToRun;
										break;
									}
								}
							}
							break;
						}
					}
				}
			}
			
			oldStr = configureString;
		}

		/// <summary>
		/// Loads the tests from assembly.
		/// </summary>
		private void LoadTestsFromAssembly () 
		{
			Tests = new Dictionary<FixtureEntry, List<TestEntry>> ();
			AssemblySetups = new Dictionary<FixtureEntry, List<AssemblyEntry>> ();
			AssemblyTeardowns = new Dictionary<FixtureEntry, List<AssemblyEntry>> ();

			foreach (Type currentType in typeof(TestRunner).Assembly.GetTypes()) 
			{
				foreach (object classAttribute in currentType.GetCustomAttributes(true)) 
				{
					// Assembly Fixtures
					if(classAttribute is AssemblyFixtureAttribute)
					{
						FixtureEntry fixEntrySetup = new FixtureEntry () {
							WillRun = true,
							State = TestState.None,
							FixtureType = currentType,
							Logs = new List<LogEntry>(),
						};
						
						FixtureEntry fixEntryTeardown = new FixtureEntry () {
							WillRun = true,
							State = TestState.None,
							FixtureType = currentType,
							Logs = new List<LogEntry>(),
						};

						List<AssemblyEntry> setups = new List<AssemblyEntry> ();
						List<AssemblyEntry> teardowns = new List<AssemblyEntry> ();
						AssemblySetups.Add (fixEntrySetup, setups);
						AssemblyTeardowns.Add (fixEntryTeardown, teardowns);

						foreach (MethodInfo methodCanidate in currentType.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)) 
						{
							foreach (object att in methodCanidate.GetCustomAttributes(true)) 
							{
								List<AssemblyEntry> list = null;

								if (att is AssemblySetupAttribute)
								{
									list = setups;
								} 
								else if(att is AssemblyTeardownAttribute)
								{
									list = teardowns;
								}

								if(list != null) 
								{
									AssemblyEntry entry = new AssemblyEntry () {
										State = TestState.None,
										Method = methodCanidate,
										Logs = new List<LogEntry>(),
									};

									list.Add(entry);
								}
							}
						}
					}

					// Test Fixtures
					else if (classAttribute is TestFixtureAttribute) 
					{
						FixtureEntry fixEntry = new FixtureEntry () {
							WillRun = false,
							State = TestState.None,
							FixtureType = currentType,
							Logs = new List<LogEntry>(),
						};

						List<TestEntry> tests = new List<TestEntry> ();
						Tests.Add (fixEntry, tests);

						foreach (MethodInfo testMethodCanidate in currentType.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)) 
						{
							foreach (object att in testMethodCanidate.GetCustomAttributes(true)) 
							{
								if (att is TestAttribute)
								{
									TestEntry entry = new TestEntry () {
										WillRun = false,
										State = TestState.None,
										TestMethod = testMethodCanidate,
										SetupState = TestState.None,
										TeardownState = TestState.None,
										Logs = new List<LogEntry>(),
									};

									tests.Add(entry);
								}
							}
						}
					}
				}
			}
		}
		#endregion
	}
}