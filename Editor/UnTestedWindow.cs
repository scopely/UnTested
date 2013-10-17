using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnTested
{
	/// <summary>
	/// UnTested Window - The Editor Window for Interacting with TestsConfig.
	/// </summary>
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
		private const float START_WIDTH_PADDING = 5.0f;
		private const float END_WIDTH_PADDING = 10.0f;
		private const float HEIGHT_SPACING = 2.0f;
		#endregion

		#region Initialization
		/// <summary>
		/// Initialize the Window by Loading Images.
		/// </summary>
		private void OnEnable ()
		{
			LoadImages ();
		}

		/// <summary>
		/// Loads the Test State Icon Images.
		/// </summary>
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
		/// <summary>
		/// Opens Tests Scene and Shows the Window.
		/// </summary>
		[MenuItem("Window/UnTested")]
		private static void ShowWindow()
		{
			OpenUnitTestScene ();
			EditorWindow.GetWindow<UnTestedWindow>("UnTested");
		}
		/// <summary>
		/// Validates the Menu Item based on if the Editor is Playing.
		/// </summary>
		/// <returns><c>true</c>, if RT was validated, <c>false</c> otherwise.</returns>
		[MenuItem("Window/UnTested", true)]
		private static bool ValidateRTD ()
		{
			return !EditorApplication.isPlaying;
		}

		/// <summary>
		/// Update this instance.
		/// </summary>
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
		/// <summary>
		/// Runs all unit tests
		/// NOTE: This can be called in batch mode (-executeMethod UnTested.UnTestedWindow.RunAllUnitTests)
		/// Exit Values:
		/// 	0 = All Tests Pass
		/// 	1 = Assembly Setups Failed
		/// 	2 = Tests Failed
		/// 	3 = Assembly Teardowns Failed
		/// </summary>
		public static void RunAllUnitTests ()
		{
			OpenUnitTestScene();
			TestsConfig.Instance.SetAllOn(true);
			EditorUtil.PlayEditor();
		}

		/// <summary>
		/// Asks User if they want to Save the Current Scene and Opens the Unit Tests Scene.
		/// </summary>
		private static void OpenUnitTestScene ()
		{
			EditorUtil.SwitchToScene (UNIT_TESTS_SCENE);
		}

		/// <summary>
		/// Saves the tests.
		/// </summary>
		private void SaveTests ()
		{
			Undo.RegisterUndo (TestsConfig.Instance, "Config Change");
			TestsConfig.Instance.Persist ();
		}
		#endregion
		
		#region OnGUI
		
		#region Getters
		/// <summary>
		/// Gets the Color associated with the passed in TestState.
		/// </summary>
		/// <returns>The color from test state.</returns>
		/// <param name="state">State.</param>
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

		/// <summary>
		/// Gets the Texture2D associated with the passed in TestState for the Icons.
		/// </summary>
		/// <returns>The texture for test state.</returns>
		/// <param name="state">State.</param>
		private Texture2D GetTextureForTestState(TestState state) 
		{
			return testStateImages [(int)state];
		}

		/// <summary>
		/// Sets the Background Color and Returns the GUIStyle based on if we are asking for a regular entry or a selected entry.
		/// </summary>
		/// <returns>The selection box.</returns>
		/// <param name="isSelection">If set to <c>true</c> is selection.</param>
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
		/// <summary>
		/// Draws the not playing buttons: (All, None, Run, Run Paused).
		/// </summary>
		private void DrawNotPlayingButtons ()
		{
			EditorGUILayout.BeginHorizontal ();
			{
				// All / None Buttons
				if (GUILayout.Button ("All")) {
					Undo.RegisterUndo (TestsConfig.Instance, "Config Change");
					TestsConfig.Instance.SetAllOn (true);
					return;
				}

				if (GUILayout.Button ("None")) {
					Undo.RegisterUndo (TestsConfig.Instance, "Config Change");
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

		/// <summary>
		/// Draws the progess bar based on the current state of the TestRunner.
		/// </summary>
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

			EditorUtil.DrawProgessBar(new Vector2 (START_WIDTH_PADDING, HEIGHT_SPACING), 
			                          new Vector2 (this.position.width - END_WIDTH_PADDING, PROGRESS_BAR_HEIGHT),
			                          percentDone, percentMsg, bgColor, fgColor, "BoldLabel", HEIGHT_SPACING);	
		}

		/// <summary>
		/// Draws the while playing buttons: (Done, Pause/Resume, Stop)
		/// </summary>
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

		/// <summary>
		/// Draws the pause/resume button.
		/// </summary>
		private void DrawPauseResumeButton ()
		{
			string pauseButtonStr = EditorApplication.isPaused ? "Resume" : "Pause";
			if (GUILayout.Button (pauseButtonStr)) {
				EditorApplication.isPaused = !EditorApplication.isPaused;
			}
		}

		/// <summary>
		/// Draws the stop/done button.
		/// </summary>
		/// <param name="title">Title.</param>
		private void DrawStopButton (string title)
		{
			if (GUILayout.Button (title)) {
				EditorApplication.isPlaying = false;
			}
		}

		/// <summary>
		/// Draws the test state icon.
		/// </summary>
		/// <param name="refRect">Reference rect.</param>
		/// <param name="state">State.</param>
		private void DrawTestStateIcon(Rect refRect, TestState state)
		{
			GUI.color = Color.white;
			GUI.contentColor = Color.white;

			float x = refRect.width - PASS_FAIL_SIZE;
			float y = refRect.y + PASS_FAIL_HEIGHT_PADDING;
			Rect stateRect = new Rect (x, y, PASS_FAIL_SIZE, PASS_FAIL_SIZE);

			GUI.DrawTexture (stateRect, GetTextureForTestState (state));
		}

		/// <summary>
		/// Draws the log window.
		/// </summary>
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
		/// <summary>
		/// OnGUI - Determine what is drawn based on state of the Editor.
		/// </summary>
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

		/// <summary>
		/// Draws the elements relevant to the Editor not playing. (Not Playing Buttons, Test Fixture and Tests Toggles)
		/// </summary>
		private void OnGUINotPlaying ()
		{
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
							SaveTests ();

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
									SaveTests ();
								} else if(originalTestOn != entry.WillRun) {
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

		/// <summary>
		/// Draws the elements relevant to the Editor playing. (While Playing Buttons, Test Fixture and Tests Selection Boxes, Log Window)
		/// </summary>
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