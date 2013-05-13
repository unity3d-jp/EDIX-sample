using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Depends on MenuGUI, but DemoControl already requires both components
public class CreditsGUI : MonoBehaviour
{
	public Texture2D background;
	public Rect contentArea;
		// Where on the background should the content be set up?
	public Font largeFont, smallFont;
	
	
	private MenuGUI menuGUI;
		// MenuGUI controls whether to show credits or default menu GUI
	private GUIStyle taskStyle = null, nameStyle = null, buttonStyle = null;
	
	
	void Start ()
	{
		menuGUI = GetComponent<MenuGUI> ();
		enabled = false;
	}
	
	
	void VerifyStyles ()
	// If needed, build styles used in this view
	{
		if (taskStyle == null)
		{
			taskStyle = new GUIStyle (GUI.skin.GetStyle ("Label"));
			taskStyle.font = smallFont;
			taskStyle.alignment = TextAnchor.UpperRight;
		}
		
		if (nameStyle == null)
		{
			nameStyle = new GUIStyle (GUI.skin.GetStyle ("Label"));
			nameStyle.font = smallFont;
			nameStyle.alignment = TextAnchor.LowerLeft;
		}
		
		if (buttonStyle == null)
		{
			buttonStyle = new GUIStyle (GUI.skin.GetStyle ("Label"));
			buttonStyle.font = largeFont;
		}
	}
	
	
	void OnGUI ()
	{
		if (!menuGUI.showCredits)
		{
			return;
		}
		
		VerifyStyles ();

		GUI.DrawTexture (new Rect (0.0f, 0.0f, Screen.width, Screen.height), background);
		
		GUILayout.BeginArea (contentArea);
			GUILayout.Space (10.0f);
		
			Notice ("Code", "Emil");
			Notice ("Shader", "Ole");
			Notice ("Graphics", "Roald");
			Notice ("GUI", "Mads");
			Notice ("Music", "Kevin\nMacLeod");
			Notice ("Score & credits font", "Dan\nZadorozny");
			
			GUILayout.FlexibleSpace ();
		
			GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				
				GUIContent content = new GUIContent ("Back");
				Vector2 size = buttonStyle.CalcSize (content);
				Rect rect = GUILayoutUtility.GetRect (size.x, size.y);
				
				switch (Event.current.type)
				// We just respond to touches - no need for the mouse down + mouse up cycle here
				{
					case EventType.Repaint:
						buttonStyle.Draw (rect, content, false, false, false, false);
					break;
					case EventType.MouseDown:
						if (rect.Contains (Event.current.mousePosition))
						{
							Event.current.Use ();
							menuGUI.showCredits = false;
						}
					break;
				}
			GUILayout.EndHorizontal ();
		GUILayout.EndArea ();
	}
	
	
	void Notice (string title, string name)
	{
		GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			
			GUILayout.Label (title, taskStyle, GUILayout.Width (contentArea.width * 0.5f));
			GUILayout.Space (20.0f);
			GUILayout.Label (name, nameStyle, GUILayout.Width (contentArea.width * 0.5f));
			
			GUILayout.FlexibleSpace ();
		GUILayout.EndHorizontal ();
		GUILayout.Space (5.0f);
	}
}
