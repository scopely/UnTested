using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnTested
{
	public class UnTestedWindow : EditorWindow
	{
		#region Private Members
		private Vector2 scrollPos = Vector2.zero;
		private Vector2 consoleScrollPos = Vector2.zero;
		private FixtureEntry selectedFixture = null;
		private TestEntry selectedTest = null;
		private bool loading = false;
		#endregion

		#region GUI Constants
		private const int TESTS_INDENT = 18;
		private const int PASS_FAIL_SIZE = 16;
		private const int PASS_FAIL_SIZE_PADDING = 5;
		private const int PASS_FAIL_HEIGHT_PADDING = 2;
		private const int PROGRESS_BAR_HEIGHT = 30;
		#endregion

		private Texture2D[] testStateImages = null;
		private void LoadImages ()
		{
			testStateImages = new Texture2D[(int)TestState.MAX];
			testStateImages [(int)TestState.None] = Resources.Load ("None") as Texture2D;
			testStateImages [(int)TestState.InProgress] = Resources.Load ("Running") as Texture2D;
			testStateImages [(int)TestState.Failed] = Resources.Load ("Fail") as Texture2D;
			testStateImages [(int)TestState.Passed] = Resources.Load ("Pass") as Texture2D;


			loading = false;
		}
		
		#region Window Lifetime
		[MenuItem("Scopely/UnTested/Run Tests With Dialog...", true)]
		public static bool ValidateRTD ()
		{
			return !EditorApplication.isPlaying;
		}

		[MenuItem("Scopely/UnTested/Run Tests With Dialog...")]
		public static void ShowWindow()
		{
			UnTestedMenuItems.UnitTestBuild ();
			EditorWindow.GetWindow<UnTestedWindow>("UnTested");
		}

		public void OnEnable ()
		{
			loading = false;
			LoadImages ();
		}
		#endregion
		void SetAllOn(bool onOff) {
			foreach (FixtureEntry fixtureEntry in TestsConfig.Instance.Tests.Keys) {
				fixtureEntry.WillFixtureTests = onOff;
				foreach (TestEntry entry in TestsConfig.Instance.Tests[fixtureEntry]) {
					entry.WillRunTest = onOff;
				}
			}
		}

		void SaveTests()
		{
			TestsConfig.Instance.Persist ();
			EditorUtility.SetDirty (TestsConfig.Instance);
		}

		#region OnGUI calls
		void SetColorFromState(TestState state)
		{
			switch(state) {
			case TestState.None:
				GUI.contentColor = Color.white;
				break;
			case TestState.Failed:
				GUI.contentColor = Color.red;
				break;
			case TestState.InProgress:
				GUI.contentColor = Color.yellow;
				break;
			case TestState.Passed:
				GUI.contentColor = Color.green;
				break;
			}
		}

		void SetColorFromLogType(LogType logType)
		{
			switch(logType) {
			case LogType.Log:
				GUI.contentColor = Color.white;
				break;
			case LogType.Error:
				GUI.contentColor = Color.red;
				break;
			case LogType.Exception:
				GUI.contentColor = Color.red;
				break;
			case LogType.Assert:
				GUI.contentColor = Color.red;
				break;
			case LogType.Warning:
				GUI.contentColor = Color.yellow;
				break;
			}
		}

		void HelpBoxFromLogType(string msg, LogType logType)
		{
			MessageType msgType = MessageType.None;

			switch(logType) {
			case LogType.Log:
				msgType = MessageType.Info;
				break;
			case LogType.Error:
				msgType = MessageType.Error;
				break;
			case LogType.Exception:
				msgType = MessageType.Error;
				break;
			case LogType.Assert:
				msgType = MessageType.Error;
				break;
			case LogType.Warning:
				msgType = MessageType.Warning;
				break;
			}

			EditorGUILayout.HelpBox (msg, msgType);
		}

		Texture2D GetTextureForState(TestState state) 
		{
			return testStateImages [(int)state];
		}

		void DrawProgessBar(Vector2 location, Vector2 size, float progress, string msg)
		{
			Rect backRect = new Rect (location.x, location.y, size.x, size.y);

			Color bgColor = new Color (0.0f, 0.2f, 0.0f);
			Color fgColor = new Color (0.0f, 0.75f, 0.0f);

			if(TestRunner.Instance.FailedTestCounter > 0) {
				bgColor = new Color (0.2f, 0.0f, 0.0f);
				fgColor = new Color (0.75f, 0.0f, 0.0f);
			}

			EditorGUI.DrawRect(backRect, bgColor);
			EditorGUI.DrawRect(new Rect(location.x, location.y, size.x * progress, size.y), fgColor);

			GUILayout.Space (PROGRESS_BAR_HEIGHT);
			GUILayout.Label (msg, "BoldLabel");
		}

		void DrawPauseResume ()
		{
			string pauseButtonStr = EditorApplication.isPaused ? "Resume" : "Pause";
			if (GUILayout.Button (pauseButtonStr)) {
				EditorApplication.isPaused = !EditorApplication.isPaused;
			}
		}

		void DrawStopButton (string title)
		{
			if (GUILayout.Button (title)) {
				EditorApplication.isPlaying = false;
			}
		}

		void OnGUINotConfiguredInEditor ()
		{
			GUILayout.Label ("Unit Testing is Disabled");

			if(GUILayout.Button("Enable Unit Testing")) {
				Application.Quit ();
				UnTestedMenuItems.UnitTestBuild ();
				TestsConfig.Instance.Reload ();
			}
		}

		void OnGUINotConfiguredWhilePlaying ()
		{
			GUILayout.Label ("Unit Testing is Disabled\nStop Running to Enable Unit Testing");
		}

		void OnGUINotRunning ()
		{
			EditorGUILayout.BeginHorizontal ();
			{
				// All / None Buttons
				if (GUILayout.Button ("All")) {
					SetAllOn (true);
					SaveTests ();
					return;
				}

				if (GUILayout.Button ("None")) {
					SetAllOn (false);
					SaveTests ();
					return;
				}

				if (GUILayout.Button ("Run")) {
					EditorUtil.PlayEditor ();
					return;
				}

				if (GUILayout.Button ("Run Paused")) {
					EditorUtil.PlayEditor ();
					EditorApplication.isPaused = true;
					return;
				}
			}
			EditorGUILayout.EndHorizontal ();

			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
			{

				foreach (FixtureEntry fixtureEntry in TestsConfig.Instance.Tests.Keys) {

					bool originalFixtureOn = fixtureEntry.WillFixtureTests;
					fixtureEntry.WillFixtureTests = GUILayout.Toggle (fixtureEntry.WillFixtureTests, fixtureEntry.FixtureType.Name);

					EditorUtil.StartIndent (TESTS_INDENT);
					{
						if (originalFixtureOn != fixtureEntry.WillFixtureTests) {
							foreach (TestEntry entry in TestsConfig.Instance.Tests[fixtureEntry]) {
								entry.WillRunTest = fixtureEntry.WillFixtureTests;
							}
							SaveTests ();

						} else {
							foreach (TestEntry entry in TestsConfig.Instance.Tests[fixtureEntry]) {
								entry.WillRunTest = GUILayout.Toggle (entry.WillRunTest, entry.TestMethod.Name);

								bool allOff = true;
								foreach (TestEntry otherEntry in TestsConfig.Instance.Tests[fixtureEntry]) {
									if (otherEntry.WillRunTest) {
										allOff = false;
									}
								}

								if (fixtureEntry.WillFixtureTests != !allOff) {
									fixtureEntry.WillFixtureTests = !allOff;
									SaveTests ();
								}
							}

						}
					}
					EditorUtil.EndIndent ();
				}
			}
			EditorGUILayout.EndScrollView();
		}
		
		void OnGUIWhilePlaying ()
		{
			// Progress Bar
			float percentDone = 0.0f;
			if (TestsConfig.Instance.NumberOfTestsToRun > 0) {
				percentDone = (float)TestRunner.Instance.NumCompleted / (float)TestsConfig.Instance.NumberOfTestsToRun;
			}
			string percentMsg = string.Format ("Tests Completed ({0}/{1})", TestRunner.Instance.NumCompleted, TestsConfig.Instance.NumberOfTestsToRun);

			DrawProgessBar (new Vector2 (0.0f, 0.0f), new Vector2 (this.position.width, PROGRESS_BAR_HEIGHT), percentDone, percentMsg);

			EditorGUILayout.BeginHorizontal ();
			{
				if (TestRunner.Instance.FinishedRunning) {
					DrawStopButton ("Done");
				} else {
					DrawPauseResume ();
					DrawStopButton ("Stop");
				}
			}
			EditorGUILayout.EndHorizontal ();

			GUI.color = Color.white; 

			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
			{
				// Test Fixtures
				foreach (FixtureEntry fixtureEntry in TestsConfig.Instance.Tests.Keys) {

					if (fixtureEntry.WillFixtureTests == false)
						continue;

					GUI.backgroundColor = fixtureEntry == selectedFixture ? Color.cyan : Color.white;
					Rect fixtureRect = EditorGUILayout.BeginHorizontal (GUI.skin.box);
					{
						SetColorFromState (fixtureEntry.State);

						if(GUILayout.Button (fixtureEntry.FixtureType.Name, GUI.skin.label))
						{
							selectedFixture = fixtureEntry;
							selectedTest = null;
						}

						GUI.color = Color.white;

						float x = fixtureRect.width - PASS_FAIL_SIZE;
						float y = fixtureRect.y + PASS_FAIL_HEIGHT_PADDING;
						Rect stateRect = new Rect (x, y, PASS_FAIL_SIZE, PASS_FAIL_SIZE);

						GUI.DrawTexture (stateRect, GetTextureForState (fixtureEntry.State));
					}
					EditorGUILayout.EndHorizontal ();

					EditorUtil.StartIndent (TESTS_INDENT);
					{
						foreach (TestEntry entry in TestsConfig.Instance.Tests[fixtureEntry]) {
							if (entry.WillRunTest) {

								GUI.backgroundColor = entry == selectedTest ? Color.cyan : Color.white;
								Rect entryRect = EditorGUILayout.BeginHorizontal (GUI.skin.box);
								{
									SetColorFromState (entry.State);

									if(GUILayout.Button (entry.TestMethod.Name, GUI.skin.label))
									{
										selectedTest = entry;
										selectedFixture = null;
									}

									GUI.color = Color.white;
									GUI.contentColor = Color.white;

									float x = fixtureRect.width - (PASS_FAIL_SIZE);
									float y = entryRect.y + PASS_FAIL_HEIGHT_PADDING;
									Rect stateImageRect = new Rect (x, y, PASS_FAIL_SIZE, PASS_FAIL_SIZE);
									GUI.DrawTexture (stateImageRect, GetTextureForState (entry.State));
								}
								EditorGUILayout.EndHorizontal ();

							}
						}
					}
					EditorUtil.EndIndent ();
				}
			}

			GUI.color = Color.white;
			GUI.backgroundColor = Color.white;
			GUI.contentColor = Color.white;

			EditorGUILayout.EndScrollView();

			consoleScrollPos = EditorGUILayout.BeginScrollView(consoleScrollPos, true, true, GUI.skin.horizontalScrollbar,
			                                                   GUI.skin.verticalScrollbar, GUI.skin.box,
			                                                   GUILayout.Width (this.position.width), GUILayout.Height (200.0f));
			{
				if (selectedFixture != null) {
					foreach (LogEntry log in selectedFixture.Logs) {
						HelpBoxFromLogType (log.LogMsg, log.LogType);
					}
				} else if (selectedTest != null) {
					foreach (LogEntry log in selectedTest.Logs) {
						HelpBoxFromLogType (log.LogMsg, log.LogType);
					}
				}
			}
			EditorGUILayout.EndScrollView();
		}

		void OnGUI()
		{
			if(EditorApplication.isCompiling || loading) {
				GUILayout.Label ("Compiling...");
			} else if(!Introspection.Testing && !Application.isPlaying) {
				OnGUINotConfiguredInEditor ();
			} else if(!Introspection.Testing && Application.isPlaying) {
				OnGUINotConfiguredWhilePlaying ();
			} else if(Application.isPlaying) {
				OnGUIWhilePlaying ();
			} else {
				OnGUINotRunning ();
			}
		}
		#endregion

		void Update () 
		{
			this.Repaint ();
		}
	}
}