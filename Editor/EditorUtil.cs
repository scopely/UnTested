using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnTested
{
	public static class EditorUtil 
	{

		#region GUI Helpers
		public static void StartIndent(int indent) {
			GUILayout.BeginHorizontal ();		
			GUILayout.Space (indent);
			GUILayout.BeginVertical ();
		}

		public static void EndIndent() {
			GUILayout.EndVertical ();
			GUILayout.EndHorizontal ();
		}
		
		public static void HelpBoxFromLogType(string msg, LogType logType)
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
		
		public static void DrawProgessBar(Vector2 location, Vector2 size, float progress, string msg, Color bgColor, Color fgColor, GUIStyle msgStyle)
		{
			Rect backRect = new Rect (location.x, location.y, size.x, size.y);
		
			EditorGUI.DrawRect(backRect, bgColor);
			EditorGUI.DrawRect(new Rect(location.x, location.y, size.x * progress, size.y), fgColor);

			GUILayout.Space (size.y);
			GUILayout.Label (msg, msgStyle);
		}
		
		public static void SetAllColors(Color color)
		{
			GUI.color = color;
			GUI.contentColor = color;
			GUI.backgroundColor = color;
		}
		#endregion

		#region Editor Helpers		
		public static void PlayEditor () {
			EditorApplication.isPlaying = true;
		}
		#endregion
	}
}