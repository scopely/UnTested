using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class UnTestedEditorUtil {

	#region GUI Helpers
	public static void StartIndent(int indent) {
		GUILayout.BeginHorizontal ();		
		GUILayout.Space (indent);
		GUILayout.BeginVertical ();
	}

	public static  void EndIndent() {
		GUILayout.EndVertical ();
		GUILayout.EndHorizontal ();
	}
	#endregion

	public static bool IsHeadless () {
		string[] arguments = Environment.GetCommandLineArgs();
		string args = string.Join(", ", arguments);
		return args.Contains("-batchmode");
	}
	
	public static BuildTarget GetBuildTargetFromString(string os) 
	{
		if(os.Contains("Mac")) {
			return BuildTarget.StandaloneOSXIntel;
		} else if(os.Contains("Windows")) {
			return BuildTarget.StandaloneWindows;
		} else if(os.Contains("Linux")) {
			return BuildTarget.StandaloneLinux;
		} else {
			return EditorUserBuildSettings.activeBuildTarget;
		}
	}
	
	public static void WriteSMCSFileWithString(string s){
		string path = Directory.GetCurrentDirectory() + "/Assets/smcs.rsp";
		File.Delete (path);
		using (StreamWriter outfile = new StreamWriter(path))
        {
            outfile.Write(s);
        }
	}
	
	public static void RefreshProject ()
	{
		UnityEditorInternal.InternalEditorUtility.RequestScriptReload();
		UnityEditor.AssetDatabase.Refresh ();
	}
	
	public static void PlayEditor () {
		EditorApplication.isPlaying = true;
	}
	
	public static void QuitEditor (int code = 0) {
		EditorApplication.Exit(code);
	}
}