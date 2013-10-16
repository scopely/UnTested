using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnTested
{
	public class UnTestedWindow : EditorWindow
	{
		#region Constants								
		private const string UNIT_TESTS_SCENE = "Assets/UnTested/Scenes/Tests.unity";
		#endregion

		#region Private Fields
		private Texture2D[] testStateImages = null;
		private Vector2 scrollPos = Vector2.zero;
		private Vector2 logWindowScrollPos = Vector2.zero;
		private FixtureEntry selectedFixture = null;
		private TestEntry selectedTest = null;
		#endregion

		#region GUI Constants
		private const int TESTS_INDENT = 18;
		private const int PASS_FAIL_SIZE = 16;
		private const int PASS_FAIL_HEIGHT_PADDING = 2;
		private const int PROGRESS_BAR_HEIGHT = 30;
		private const float PROGRESS_BAR_BG_FILL = 0.2f;
		private const float PROGRESS_BAR_FG_FILL = 0.75f;
		private const float LOG_WINDOW_HEIGHT = 200.0f;
		private const float PRO_SELECTION_BLUE = 0.5f;
		#endregion

		#region Initialization
		private void OnEnable ()
		{
			LoadImages ();
		}
		
		private void LoadImages ()
		{
			testStateImages = new Texture2D[(int)TestState.MAX];
			testStateImages [(int)TestState.None] = Resources.Load ("None") as Texture2D;
			testStateImages [(int)TestState.InProgress] = Resources.Load ("Running") as Texture2D;
			testStateImages [(int)TestState.Failed] = Resources.Load ("Fail") as Texture2D;
			testStateImages [(int)TestState.Passed] = Resources.Load ("Pass") as Texture2D;
		}
		#endregion
		
		#region Window Lifetime
		[MenuItem("Window/UnTested")]
		private static void ShowWindow()
		{
			OpenUnitTestScene ();
			EditorWindow.GetWindow<UnTestedWindow>("UnTested");
		}
		[MenuItem("Window/UnTested", true)]
		private static bool ValidateRTD ()
		{
			return !EditorApplication.isPlaying;
		}
		
		private void Update () 
		{
			if(!EditorApplication.isCompiling && !Application.isPlaying) 
			{
				TestsConfig.Instance.Update();	
			}
		
			this.Repaint ();
		}
		#endregion
		
		#region Configuration
		public static void RunAllUnitTests ()
		{
			OpenUnitTestScene();
			TestsConfig.Instance.SetAllOn(true);
			EditorUtil.PlayEditor();
		}
		
		private static void OpenUnitTestScene ()
		{
			if(EditorApplication.currentScene != UNIT_TESTS_SCENE) 
			{
				if (EditorApplication.SaveCurrentSceneIfUserWantsTo ()) 
				{
					EditorApplication.SaveScene ();
				}
				EditorApplication.OpenScene(UNIT_TESTS_SCENE);
			}
		}
		#endregion
		
		#region OnGUI
		
		#region Getters
		private Color GetColorFromTestState(TestState state)
		{
			Color color = Color.white;
		
			switch(state) {
			case TestState.None:
				color = Color.white;
				break;
			case TestState.Failed:
				color = Color.red;
				break;
			case TestState.InProgress:
				color = Color.yellow;
				break;
			case TestState.Passed:
				color = Color.green;
				break;
			}
			
			return color;
		}

		private Texture2D GetTextureForTestState(TestState state) 
		{
			return testStateImages [(int)state];
		}
		
		private GUIStyle GetSelectionBox(bool isSelection)
		{
			GUIStyle boxStyle = GUI.skin.box;
			Color boxColor = Color.white;
			if(isSelection)
			{
				GUIStyle selectedBoxStyle = new GUIStyle (GUI.skin.box);
				selectedBoxStyle.normal.background = EditorGUIUtility.whiteTexture;
				boxStyle = selectedBoxStyle;
				boxColor = EditorGUIUtility.isProSkin ? new Color(0.0f, 0.0f, PRO_SELECTION_BLUE) : Color.cyan;
			}

			GUI.backgroundColor = boxColor;
			
			return boxStyle;
		}
		#endregion

		#region Draw Functions
		private void DrawNotPlayingButtons ()
		{
			EditorGUILayout.BeginHorizontal ();
			{
				// All / None Buttons
				if (GUILayout.Button ("All")) {
					TestsConfig.Instance.SetAllOn (true);
					return;
				}

				if (GUILayout.Button ("None")) {
					TestsConfig.Instance.SetAllOn (false);
					return;
				}

				// Run Buttons
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
		}
		
		private void DrawProgessBar()
		{
			float percentDone = 0.0f;
			if (TestsConfig.Instance.NumberOfTestsToRun > 0) {
				percentDone = (float)TestRunner.Instance.NumberOfTestsCompleted / (float)TestsConfig.Instance.NumberOfTestsToRun;
			}
			string percentMsg = string.Format ("Tests Completed ({0}/{1})", TestRunner.Instance.NumberOfTestsCompleted, TestsConfig.Instance.NumberOfTestsToRun);
		
			Color bgColor = new Color (0.0f, PROGRESS_BAR_BG_FILL, 0.0f);
			Color fgColor = new Color (0.0f, PROGRESS_BAR_FG_FILL, 0.0f);

			if(TestRunner.Instance.FailedTestCounter > 0) {
				bgColor = new Color (PROGRESS_BAR_BG_FILL, 0.0f, 0.0f);
				fgColor = new Color (PROGRESS_BAR_FG_FILL, 0.0f, 0.0f);
			}

			EditorUtil.DrawProgessBar(new Vector2 (0.0f, 0.0f), new Vector2 (this.position.width, PROGRESS_BAR_HEIGHT), percentDone, percentMsg, bgColor, fgColor, "BoldLabel");	
		}
		
		private void DrawWhilePlayingButtons ()
		{
			EditorGUILayout.BeginHorizontal ();
			{
				if (TestRunner.Instance.FinishedRunning) {
					DrawStopButton ("Done");
				} else {
					DrawPauseResumeButton ();
					DrawStopButton ("Stop");
				}
			}
			EditorGUILayout.EndHorizontal ();
		}

		private void DrawPauseResumeButton ()
		{
			string pauseButtonStr = EditorApplication.isPaused ? "Resume" : "Pause";
			if (GUILayout.Button (pauseButtonStr)) {
				EditorApplication.isPaused = !EditorApplication.isPaused;
			}
		}

		private void DrawStopButton (string title)
		{
			if (GUILayout.Button (title)) {
				EditorApplication.isPlaying = false;
			}
		}
		
		private void DrawTestStateIcon(Rect refRect, TestState state)
		{
			GUI.color = Color.white;
			GUI.contentColor = Color.white;

			float x = refRect.width - PASS_FAIL_SIZE;
			float y = refRect.y + PASS_FAIL_HEIGHT_PADDING;
			Rect stateRect = new Rect (x, y, PASS_FAIL_SIZE, PASS_FAIL_SIZE);

			GUI.DrawTexture (stateRect, GetTextureForTestState (state));
		}
		
		private void DrawLogWindow()
		{
			EditorUtil.SetAllColors(Color.white);
			logWindowScrollPos = EditorGUILayout.BeginScrollView(logWindowScrollPos, true, true, GUI.skin.horizontalScrollbar,
			                                                     GUI.skin.verticalScrollbar, GUI.skin.box,
			                                                     GUILayout.Width (this.position.width), GUILayout.Height (LOG_WINDOW_HEIGHT));
			{
				if (selectedFixture != null) {
					foreach (LogEntry log in selectedFixture.Logs) {
						EditorUtil.HelpBoxFromLogType (log.LogMsg, log.LogType);
					}
				} else if (selectedTest != null) {
					foreach (LogEntry log in selectedTest.Logs) {
						EditorUtil.HelpBoxFromLogType (log.LogMsg, log.LogType);
					}
				}
			}
			EditorGUILayout.EndScrollView();
		}
		#endregion

		#region OnGUI Heads
		private void OnGUI()
		{
			if(EditorApplication.isCompiling) {
				GUILayout.Label ("Compiling...");
			} else if(Application.isPlaying) {
				OnGUIWhilePlaying ();
			} else {
				OnGUINotPlaying ();
			}
		}
		
		private void OnGUINotPlaying ()
		{
			Undo.SetSnapshotTarget(TestsConfig.Instance, "Config Changed");
			Undo.CreateSnapshot();
		
			DrawNotPlayingButtons();

			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
			{
				foreach (FixtureEntry fixtureEntry in TestsConfig.Instance.Tests.Keys) 
				{
					bool originalFixtureOn = fixtureEntry.WillRun;
					fixtureEntry.WillRun = GUILayout.Toggle (fixtureEntry.WillRun, fixtureEntry.FixtureType.Name);

					EditorUtil.StartIndent (TESTS_INDENT);
					{
						if (originalFixtureOn != fixtureEntry.WillRun) 
						{
							foreach (TestEntry entry in TestsConfig.Instance.Tests[fixtureEntry])
							{
								entry.WillRun = fixtureEntry.WillRun;
							}
							TestsConfig.Instance.Persist ();

						} else {
							foreach (TestEntry entry in TestsConfig.Instance.Tests[fixtureEntry]) 
							{
								bool originalTestOn = entry.WillRun;
								entry.WillRun = GUILayout.Toggle (entry.WillRun, entry.TestMethod.Name);

								bool allOff = true;
								foreach (TestEntry otherEntry in TestsConfig.Instance.Tests[fixtureEntry])
								{
									if (otherEntry.WillRun) 
									{
										allOff = false;
									}
								}

								if (fixtureEntry.WillRun != !allOff) {
									fixtureEntry.WillRun = !allOff;
									TestsConfig.Instance.Persist ();
								} else if(originalTestOn != entry.WillRun) {
									TestsConfig.Instance.Persist ();
								}
							}
						}
					}
					EditorUtil.EndIndent ();
				}
			}
			EditorGUILayout.EndScrollView();
			
			if (GUI.changed)
		    {
		        EditorUtility.SetDirty(TestsConfig.Instance);
		        Undo.RegisterSnapshot();
		    }
		}
		
		private void OnGUIWhilePlaying ()
		{
			DrawProgessBar ();
			DrawWhilePlayingButtons();
			
			EditorUtil.SetAllColors(Color.white);
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
			{
				// Test Fixtures
				foreach (FixtureEntry fixtureEntry in TestsConfig.Instance.Tests.Keys)
				{
					if (!fixtureEntry.WillRun)
						continue;

					Rect fixtureRect = EditorGUILayout.BeginHorizontal (GetSelectionBox(fixtureEntry == selectedFixture));
					{
						GUI.contentColor = GetColorFromTestState (fixtureEntry.State);

						if(GUILayout.Button (fixtureEntry.FixtureType.Name, GUI.skin.label))
						{
							selectedFixture = fixtureEntry;
							selectedTest = null;
						}
					
						DrawTestStateIcon(fixtureRect, fixtureEntry.State);
					}
					EditorGUILayout.EndHorizontal ();

					EditorUtil.StartIndent (TESTS_INDENT);
					{
						foreach (TestEntry entry in TestsConfig.Instance.Tests[fixtureEntry]) 
						{
							if (entry.WillRun) 
							{
								Rect entryRect = EditorGUILayout.BeginHorizontal (GetSelectionBox(entry == selectedTest));
								{
									GUI.contentColor = GetColorFromTestState (entry.State);

									if(GUILayout.Button (entry.TestMethod.Name, GUI.skin.label))
									{
										selectedTest = entry;
										selectedFixture = null;
									}

									Rect drawRect = fixtureRect;
									drawRect.y = entryRect.y;
									DrawTestStateIcon(drawRect, entry.State);
								}
								EditorGUILayout.EndHorizontal ();
							}
						}
					}
					EditorUtil.EndIndent ();
				}
			}
			EditorGUILayout.EndScrollView();

			DrawLogWindow();
		}
		#endregion
		#endregion
	}
}