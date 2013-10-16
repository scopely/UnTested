using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnTested
{
	/// <summary>
	/// Editor Util - Helper Class with Static Methods for Editor Functionality.
	/// </summary>
	public static class EditorUtil 
	{
		#region GUI Helpers
		/// <summary>
		/// Starts an indent of indent size in pixels
		/// </summary>
		/// <param name="indent">Indent.</param>
		public static void StartIndent(int indent) {
			GUILayout.BeginHorizontal ();		
			GUILayout.Space (indent);
			GUILayout.BeginVertical ();
		}

		/// <summary>
		/// Ends the last indent.
		/// </summary>
		public static void EndIndent() {
			GUILayout.EndVertical ();
			GUILayout.EndHorizontal ();
		}

		/// <summary>
		/// Draws a Help Box based on the provided message and LogType. 
		/// </summary>
		/// <param name="msg">Message.</param>
		/// <param name="logType">Log type.</param>
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

		/// <summary>
		/// Draws a Progress Bar with a Label underneath.
		/// </summary>
		/// <param name="location">Location.</param>
		/// <param name="size">Size.</param>
		/// <param name="progress">Progress.</param>
		/// <param name="msg">Message.</param>
		/// <param name="bgColor">Background color.</param>
		/// <param name="fgColor">Foreground color.</param>
		/// <param name="msgStyle">Message style.</param>
		public static void DrawProgessBar(Vector2 location, Vector2 size, float progress, string msg, Color bgColor, Color fgColor, GUIStyle msgStyle)
		{
			Rect backRect = new Rect (location.x, location.y, size.x, size.y);
		
			EditorGUI.DrawRect(backRect, bgColor);
			EditorGUI.DrawRect(new Rect(location.x, location.y, size.x * progress, size.y), fgColor);

			GUILayout.Space (size.y);
			GUILayout.Label (msg, msgStyle);
		}

		/// <summary>
		/// Sets all GUI colors.
		/// </summary>
		/// <param name="color">Color.</param>
		public static void SetAllColors(Color color)
		{
			GUI.color = color;
			GUI.contentColor = color;
			GUI.backgroundColor = color;
		}
		#endregion

		#region Editor Helpers
		/// <summary>
		/// Plays the Editor.
		/// </summary>
		public static void PlayEditor () 
		{
			EditorApplication.isPlaying = true;
		}

		/// <summary>
		/// Asks User if they want to Save the Current Scene and Opens the Passed in Scene.
		/// </summary>
		/// <param name="scenePath">Scene path.</param>
		public static void SwitchToScene(string scenePath)
		{
			if(EditorApplication.currentScene != scenePath) 
			{
				if (EditorApplication.SaveCurrentSceneIfUserWantsTo ()) 
				{
					EditorApplication.SaveScene ();
				}
				EditorApplication.OpenScene(scenePath);
			}
		}
		#endregion
	}
}