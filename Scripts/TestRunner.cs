using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnTested
{
	public class TestRunner : Singleton<TestRunner>
	{
		#region Report Classes
		public abstract class ITestReport
		{
			public abstract string MessageReport ();
		}

		public class PassedTestReport : ITestReport
		{
			public PassedTestReport (string _fixtureName, string _testName)
			{
				fixtureName = _fixtureName;
				testName = _testName;
			}

			public string FixtureName {	get { return fixtureName; } }
			public string TestName 	  {	get { return testName; } }

			protected string fixtureName;
			protected string testName;

			public override string MessageReport()
			{
				return string.Format ("Fixture: [{0}] Test: [{1}]\n", fixtureName, testName);
			}
		}

		private class FailedTestReport : ITestReport
		{
			public FailedTestReport (string _fixtureName, string _testName, string _errorMessage)
			{
				fixtureName = _fixtureName;
				testName = _testName;
				errorMessage = _errorMessage;
			}

			protected string fixtureName;
			protected string testName;
			protected string errorMessage;

			public override string MessageReport()
			{
				return string.Format ("Fixture: [{0}] Test: [{1}] Error: {2}\n", fixtureName, testName, errorMessage);
			}
		}

		private class FailedSetupReport : FailedTestReport
		{
			public FailedSetupReport (string _fixtureName, string _setupName, string _testName, string _errorMessage) : base(_fixtureName, _testName, _errorMessage)
			{
				setupName = _setupName;
			}

			protected string setupName;

			public override string MessageReport()
			{
				return string.Format ("Fixture: [{0}] Setup: [{1}] on Test: [{2}] Error: {3}\n", fixtureName, setupName, testName, errorMessage);
			}
		}

		private class FailedTeardownReport : FailedTestReport
		{
			public FailedTeardownReport (string _fixtureName, string _teardownName, string _testName, string _errorMessage) : base(_fixtureName, _testName, _errorMessage)
			{
				teardownName = _teardownName;
			}

			protected string teardownName;

			public override string MessageReport()
			{
				return string.Format ("Fixture: [{0}] Teardown: [{1}] on Test: [{2}] Error: {3}\n", fixtureName, teardownName, testName, errorMessage);
			}
		}
		#endregion
	
		#region Events
		public event Action OnAllTestsFinished;
		#endregion
		
		#region Public Properties
		[HideInInspector]
		public int NumberOfTestsCompleted = 0;
		[HideInInspector]
		public int FailedTestCounter = 0;
		[HideInInspector]
		public bool FinishedRunning = false;
		#endregion

		#region Private Fields
		private List<ITestReport> failedSetupReports = null;
		private List<ITestReport> failedTestReports = null;
		private List<ITestReport> failedTeardownReports = null;
		private FixtureEntry currentFixture = null;
		private TestEntry currentTest = null;
		private int failedSetupCounter = 0;
		private int failedTeardownCounter = 0;
		#endregion

		#region Reports
		private void ReportSetupError(Type fixtureType, MethodInfo setupMethod, MethodInfo testMethod, Exception e)
		{
			++failedSetupCounter;
			failedSetupReports.Add (new FailedSetupReport (fixtureType.Name, setupMethod.Name, testMethod.Name, e.Message));
		}

		private void ReportTestError(Type fixtureType, MethodInfo testMethod, Exception e)
		{
			++FailedTestCounter;
			failedTestReports.Add (new FailedTestReport (fixtureType.Name, testMethod.Name, e.Message));
		}

		private void ReportTeardownError(Type fixtureType, MethodInfo teardownMethod, MethodInfo testMethod, Exception e)
		{
			++failedTeardownCounter;
			failedTeardownReports.Add (new FailedTeardownReport (fixtureType.Name, teardownMethod.Name, testMethod.Name, e.Message));
		}

		private void OutputSummary () 
		{
			if(FailedTestCounter > 0 || failedSetupCounter > 0 || failedTeardownCounter > 0) {
				string log = string.Format ("{0} Tests Run, {1} Tests Failed, {2} Tests Passed, {3} Setups Failed, {4} Teardowns Failed\n",
				                            NumberOfTestsCompleted, FailedTestCounter, NumberOfTestsCompleted - FailedTestCounter, failedSetupCounter, failedTeardownCounter);

				if(failedSetupCounter > 0)
					log += CreateErrorReportChunk ("Failed Setups", failedSetupReports);
				if(FailedTestCounter > 0)
					log += CreateErrorReportChunk ("Failed Tests", failedTestReports);
				if(failedTeardownCounter > 0)
					log += CreateErrorReportChunk ("Failed Teardowns", failedTeardownReports);

				Debug.LogError(log);
			} else {
				Debug.Log (string.Format ("All {0} Tests Passed", NumberOfTestsCompleted));
			}
		}

		private string CreateErrorReportChunk(string title, List<ITestReport> failList) 
		{
			string log = string.Format ("\n{0}:\n", title);

			int failCount = 0;
			foreach(FailedTestReport report in failList) {
				++failCount;
				log += string.Format("{0}. {1}", failCount, report.MessageReport());
			}

			return log;
		}
		#endregion

		#region Initialization
		private void OnEnable ()
		{
			RunTests ();
		}
		
		private void RunTests() 
		{
			failedSetupReports = new List<ITestReport>();
			failedTestReports = new List<ITestReport> ();
			failedTeardownReports = new List<ITestReport> ();

			StartCoroutine(RunTestsWithOutputCoroutine());
		}
		
		private IEnumerator RunTestsWithOutputCoroutine() 
		{
			yield return StartCoroutine(RunAssemblySetupCoroutine());
			yield return StartCoroutine(RunTestsCoroutine());
			yield return StartCoroutine(RunAssemblyTeardownCoroutine());
			OutputSummary ();
			TestsFinished (); 
		}
		#endregion

		#region Tests Finished
		private void TestsFinished ()
		{
			#if UNITY_EDITOR
			// If Headless Quit	
			if(AreWeRunningHeadless())
				EditorApplication.Exit(GetExitCode());
			#endif

			if (OnAllTestsFinished != null)
				OnAllTestsFinished ();

			FinishedRunning = true;
		}
		
		private bool AreWeRunningHeadless () {
			String[] arguments = Environment.GetCommandLineArgs();
			string args = string.Join(", ", arguments);
			return args.Contains("-batchmode");
		}
		
		private int GetExitCode ()
		{
			int code = 0;
			if(FailedTestCounter > 0)
				code = 1;
			return code;
		}
		#endregion
		
		#region Running
		private Exception RunNormalTest(Type fixtureType, object fixtureInstance, MethodInfo methodInfo)
		{
			try {
				methodInfo.Invoke(fixtureInstance, new object[] {});
			} catch(Exception e) {
				Debug.LogException (e.InnerException);
				return e.InnerException;
			}

			return null;
		}

		private TestCoroutine<Exception> RunAsyncTest(object fixtureInstance, MethodInfo testMethod)
		{
			return TestExtensions.StartTestCoroutine<Exception>(this, (IEnumerator)testMethod.Invoke(fixtureInstance, new object[] {}));
		}

		private void HandleTestLog(string logString, string stackTrace, LogType logType)
		{
			bool showStack = !logString.Contains("TestFlowException:");
			if(showStack && (logType == LogType.Assert || logType == LogType.Error || logType == LogType.Exception))
			{
				logString += "\n\n" + stackTrace;
			}

			LogEntry logEntry = new LogEntry (logString, logType);
			currentFixture.Logs.Add (logEntry);
			currentTest.Logs.Add (logEntry);
		}
		
		private IEnumerator RunAssemblySetupCoroutine()
		{
			foreach (FixtureEntry fixtureEntry in TestsConfig.Instance.AssemblySetups.Keys) 
			{
				object fixtureInstance = fixtureEntry.FixtureType.GetConstructor (Type.EmptyTypes).Invoke (new object[] { });
			
				foreach (AssemblyEntry entry in TestsConfig.Instance.AssemblySetups[fixtureEntry]) 
				{
					Debug.Log ("Running Assembly Setup [" + entry.Method.Name + "] on [" + fixtureEntry.FixtureType.Name + "]");
					entry.State = TestState.InProgress;
					Exception setupException = null;

					// Async or Regular
					if (entry.Method.ReturnType == typeof(IEnumerator)) {
						TestCoroutine<Exception> setupCoroutine = RunAsyncTest (fixtureInstance, entry.Method);
						yield return setupCoroutine.coroutine;
						if (setupCoroutine.Exception != null) {
							Debug.LogException (setupCoroutine.Exception);
							setupException = setupCoroutine.Exception;
						}
					} else {
						setupException = RunNormalTest (fixtureEntry.FixtureType, fixtureInstance, entry.Method);
					}

					if (setupException != null) {
						ReportSetupError (fixtureEntry.FixtureType, entry.Method, entry.Method, setupException);
						entry.State = TestState.Failed;
						Debug.LogError ("TestFlowException: Failed Assembly Setup [" + entry.Method.Name + "] on [" + fixtureEntry.FixtureType.Name + "]");

					} else {
						entry.State = TestState.Passed;
						Debug.Log ("Finished Assembly Setup [" + entry.Method.Name + "] on [" + fixtureEntry.FixtureType.Name + "]");
					}
				}
			}
		}
		
		private IEnumerator RunTestsCoroutine()
		{
			Application.RegisterLogCallback (HandleTestLog);

			foreach (FixtureEntry fixtureEntry in TestsConfig.Instance.Tests.Keys) 
			{
				if (fixtureEntry.WillRun) 
				{
					currentFixture = fixtureEntry;
					fixtureEntry.State = TestState.InProgress;
					bool fixtureError = false;

					foreach (TestEntry testEntry in TestsConfig.Instance.Tests[fixtureEntry]) 
					{
						if (testEntry.WillRun) 
						{
							currentTest = testEntry;

							object fixtureInstance = null;
							testEntry.State = TestState.InProgress;

							fixtureInstance = fixtureEntry.FixtureType.GetConstructor (Type.EmptyTypes).Invoke (new object[] { });
							bool hasSetup = false;

							// Run Setup NOTE: We need too loop backward through the Methods to Handle Base Dependencies
							MethodInfo[] setupMethodInfos = fixtureEntry.FixtureType.GetMethods (System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
							for (int m = setupMethodInfos.Length-1; m >= 0; --m) 
							{
								MethodInfo setupCandidateMethod = setupMethodInfos [m];
								foreach (object setupCandidateAttribute in setupCandidateMethod.GetCustomAttributes(true)) 
								{
									if (setupCandidateAttribute is TestSetupAttribute) 
									{
										Debug.Log ("Running Setup [" + setupCandidateMethod.Name + "] on [" + fixtureEntry.FixtureType.Name + "]");
										testEntry.SetupState = TestState.InProgress;
										hasSetup = true;

										Exception setupException = null;

										// Async or Regular
										if (setupCandidateMethod.ReturnType == typeof(IEnumerator)) 
										{
											TestCoroutine<Exception> setupCoroutine = RunAsyncTest (fixtureInstance, setupCandidateMethod);
											yield return setupCoroutine.coroutine;
											if (setupCoroutine.Exception != null) {
												Debug.LogException (setupCoroutine.Exception);
												setupException = setupCoroutine.Exception;
											}
										} else {
											setupException = RunNormalTest (fixtureEntry.FixtureType, fixtureInstance, setupCandidateMethod);
										}

										if (setupException != null) 
										{
											ReportSetupError (fixtureEntry.FixtureType, setupCandidateMethod, testEntry.TestMethod, setupException);
											fixtureError = true;
											testEntry.SetupState = TestState.Failed;
											Debug.LogError ("TestFlowException: Failed Setup [" + setupCandidateMethod.Name + "] on [" + fixtureEntry.FixtureType.Name + "]");

										} else {
											testEntry.SetupState = TestState.Passed;
											Debug.Log ("Finished Setup [" + setupCandidateMethod.Name + "] on [" + fixtureEntry.FixtureType.Name + "]");
										}
									}
								}
							}

							if(!hasSetup) 
							{
								testEntry.SetupState = TestState.Passed;
							}

							bool testPassed = false;
							if(testEntry.SetupState == TestState.Passed) 
							{
								// Run Test
								Debug.Log ("Running Test [" + testEntry.TestMethod.Name + "] on [" + fixtureEntry.FixtureType.Name + "]");
								Exception testException = null;

								if (testEntry.TestMethod.ReturnType == typeof(IEnumerator)) 
								{
									TestCoroutine<Exception> testRoutine = RunAsyncTest (fixtureInstance, testEntry.TestMethod);
									yield return testRoutine.coroutine;
									if (testRoutine.Exception != null) {
										Debug.LogException (testRoutine.Exception);
										testException = testRoutine.Exception;
									}
								} else {
									testException = RunNormalTest (fixtureEntry.FixtureType, fixtureInstance, testEntry.TestMethod);
								}

								if (testException != null)
								{
									ReportTestError (fixtureEntry.FixtureType, testEntry.TestMethod, testException);
									testEntry.State = TestState.Failed;
									fixtureError = true;
									Debug.LogError ("TestFlowException: Failed Test [" + testEntry.TestMethod.Name + "] on [" + fixtureEntry.FixtureType.Name + "]");
								} else {
									testPassed = true;
								}
							} else {
								ReportTestError (fixtureEntry.FixtureType, testEntry.TestMethod, new Exception("TestFlowException: Setup Failed"));
								testEntry.State = TestState.Failed;
								Debug.LogError ("TestFlowException: Failed Test [" + testEntry.TestMethod.Name + "] on [" + fixtureEntry.FixtureType.Name + "]");
							}

							// Tear Down
							bool hasTeardown = false;
							foreach (MethodInfo teardownCandidateMethod in fixtureEntry.FixtureType.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
							{
								foreach (object teardownCandidateAttribute in teardownCandidateMethod.GetCustomAttributes(true)) 
								{
									if (teardownCandidateAttribute is TestTeardownAttribute) {
										Debug.Log ("Running Teardown [" + teardownCandidateMethod.Name + "] on [" + fixtureEntry.FixtureType.Name + "]");
										testEntry.TeardownState = TestState.InProgress;
										Exception teardownException = null;
										hasTeardown = true;

										if (teardownCandidateMethod.ReturnType == typeof(IEnumerator)) 
										{
											TestCoroutine<Exception> teardownRoutine = RunAsyncTest (fixtureInstance, teardownCandidateMethod);
											yield return teardownRoutine.coroutine;
											if (teardownRoutine.Exception != null) {
												Debug.LogException (teardownRoutine.Exception);
												teardownException = teardownRoutine.Exception;
											}
										} else {
											teardownException = RunNormalTest (fixtureEntry.FixtureType, fixtureInstance, teardownCandidateMethod);
										}

										if (teardownException != null) 
										{
											ReportTeardownError (fixtureEntry.FixtureType, teardownCandidateMethod, testEntry.TestMethod, teardownException);
											fixtureError = true;
											testEntry.TeardownState = TestState.Failed;
											Debug.LogError ("TestFlowException: Failed Teardown [" + teardownCandidateMethod.Name + "] on [" + fixtureEntry.FixtureType.Name + "]");

											if(testPassed) {
												ReportTestError (fixtureEntry.FixtureType, testEntry.TestMethod, new Exception("TestFlowException: Teardown Failed"));
												testEntry.State = TestState.Failed;
												Debug.LogError ("TestFlowException: Failed Test [" + testEntry.TestMethod.Name + "] on [" + fixtureEntry.FixtureType.Name + "]");
											}
										} else {
											testEntry.TeardownState = TestState.Passed;
											Debug.Log ("Finished Teardown [" + teardownCandidateMethod.Name + "] on [" + fixtureEntry.FixtureType.Name + "]");
										}
									}
								}
							}

							if(!hasTeardown) 
							{
								testEntry.TeardownState = TestState.Passed;
							}

							if(testEntry.State != TestState.Failed) 
							{
								testEntry.State = TestState.Passed;
								Debug.Log ("Passed Test [" + testEntry.TestMethod.Name + "] on [" + fixtureEntry.FixtureType.Name + "]");
							}

							++NumberOfTestsCompleted;
						}
					}

					if(fixtureError) 
					{
						fixtureEntry.State = TestState.Failed;
					} 
					else 
					{
						fixtureEntry.State = TestState.Passed;
					}
				}
			}

			Application.RegisterLogCallback (null);
		}
		
		private IEnumerator RunAssemblyTeardownCoroutine()
		{
			foreach (FixtureEntry fixtureEntry in TestsConfig.Instance.AssemblyTeardowns.Keys) 
			{
				object fixtureInstance = fixtureEntry.FixtureType.GetConstructor (Type.EmptyTypes).Invoke (new object[] { });
			
				foreach (AssemblyEntry entry in TestsConfig.Instance.AssemblyTeardowns[fixtureEntry]) 
				{
					Debug.Log ("Running Assembly Teardown [" + entry.Method.Name + "] on [" + fixtureEntry.FixtureType.Name + "]");
					entry.State = TestState.InProgress;
					Exception setupException = null;

					// Async or Regular
					if (entry.Method.ReturnType == typeof(IEnumerator)) 
					{
						TestCoroutine<Exception> teardownCoroutine = RunAsyncTest (fixtureInstance, entry.Method);
						yield return teardownCoroutine.coroutine;
						if (teardownCoroutine.Exception != null) {
							Debug.LogException (teardownCoroutine.Exception);
							setupException = teardownCoroutine.Exception;
						}
					} else {
						setupException = RunNormalTest (fixtureEntry.FixtureType, fixtureInstance, entry.Method);
					}

					if (setupException != null) 
					{
						ReportTeardownError (fixtureEntry.FixtureType, entry.Method, entry.Method, setupException);
						entry.State = TestState.Failed;
						Debug.LogError ("TestFlowException: Failed Assembly Teardown [" + entry.Method.Name + "] on [" + fixtureEntry.FixtureType.Name + "]");

					} else {
						entry.State = TestState.Passed;
						Debug.Log ("Finished Assembly Teardown [" + entry.Method.Name + "] on [" + fixtureEntry.FixtureType.Name + "]");
					}
				}
			}
		}
		#endregion
	}
}