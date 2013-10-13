using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Threading;

namespace UnTested
{
	public class UnTestedMenuItems {

		#region Constants								
		private const string UNIT_TESTS_SCENE = "Assets/UnTested/Scenes/Tests.unity";
		#endregion

		#region Configure Helpers
		private static void SetPlayerSettingsForTesting () 
		{
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "TESTS");
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iPhone, "TESTS");
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "TESTS");
		}
		
		private static void SetPlayerSettingsForNonTesting () 
		{
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "");
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iPhone, "");
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "");
		}
		
		private static void SetBuildSettingsForTesting () 
		{
			List<EditorBuildSettingsScene> newScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
			
			bool foundTestScene = false;
			foreach(EditorBuildSettingsScene scene in newScenes) {
				scene.enabled = false;
				
				if(scene.path == UNIT_TESTS_SCENE) {
					scene.enabled = true;
					foundTestScene = true;
				}
			}
			
			if(!foundTestScene) {
				EditorBuildSettingsScene testScene = new EditorBuildSettingsScene(UNIT_TESTS_SCENE, true);
				newScenes.Add(testScene);
			}
			
			EditorBuildSettings.scenes = newScenes.ToArray();
			
			if(EditorApplication.currentScene != UNIT_TESTS_SCENE) {
				EditorApplication.OpenScene(UNIT_TESTS_SCENE);
			}
		}
		
		private static void SetBuildSettingsForNonTesting () 
		{
			foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
				if(scene.path == UNIT_TESTS_SCENE) {
					scene.enabled = false;
				}
			}
		}
		#endregion

		#region Menu Items
		[MenuItem("Scopely/UnTested/Disable Unit Testing", true)]
		public static bool ValidateDUT ()
		{
			bool valid = !EditorApplication.isPlaying;

			if(!Introspection.Testing) {
				valid = false;
			}

			return valid;
		}

		[MenuItem("Scopely/UnTested/Disable Unit Testing", false, 1)]
	    public static void DisableTesting ()
	    {
			EditorUtil.WriteSMCSFileWithString("");

			SetPlayerSettingsForNonTesting();
			SetBuildSettingsForNonTesting();
			
			EditorUtil.RefreshProject();
							
			Debug.Log("Unit Tests Disabled");
	    }

		[MenuItem("Scopely/UnTested/Enable Unit Testing", true)]
		public static bool ValidateCUT ()
		{
			bool valid = !EditorApplication.isPlaying;

			if(Introspection.Testing) {
				valid = false;
			}

			return valid; 
		}
		
		[MenuItem("Scopely/UnTested/Enable Unit Testing", false, 1)]
	    public static void UnitTestBuild ()
	    {
			EditorUtil.WriteSMCSFileWithString("-define:TESTS");

			SetPlayerSettingsForTesting();
			SetBuildSettingsForTesting();
			
			EditorUtil.RefreshProject();
							
			Debug.Log("Unit Test Build Selected");
	    }

		[MenuItem("Scopely/UnTested/Run Unit Tests in Editor", true)]
		public static bool ValidateRUT ()
		{
			return !EditorApplication.isPlaying;
		}
		
		[MenuItem("Scopely/UnTested/Run Unit Tests in Editor", false, 1)]
	    public static void RunUnitTestsInEditor ()
	    {
			UnitTestBuild();
			EditorUtil.PlayEditor();
	    }
	    
	    public static void RunAllUnitTests ()
	    {
			UnitTestBuild();
			EditorUtil.PlayEditor();
	    }
	    #endregion
	}
}