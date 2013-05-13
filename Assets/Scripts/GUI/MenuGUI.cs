using UnityEngine;
using System.Collections;

public class MenuGUI : MonoBehaviour
{
	public Texture2D background;
	public Rect
		// Active areas on the background image, identifying touch buttons
		startButtonRect,
		inputDebuggerButtonRect,
		quitButtonRect,
		creditsButtonRect;
	public bool showCredits = false;
	
	
	void Start ()
	{
		if (background == null)
		{
			Debug.LogError ("Background texture not set. Please correct and restart.", this);
			enabled = false;
			return;
		}
	}
	
	
	void OnGUI ()
	{
		if (showCredits)
		{
			return;
		}
		
		switch (Event.current.type)
		// Render background and handle button presses
		{
			case EventType.MouseDown:
				if (startButtonRect.Contains (Event.current.mousePosition))
				{
					Event.current.Use ();
					Time.timeScale = 1.0f;
				}
				else if (inputDebuggerButtonRect.Contains (Event.current.mousePosition))
				{
					Event.current.Use ();
					Application.LoadLevel ("InputDemo");
				}
				else if (quitButtonRect.Contains (Event.current.mousePosition))
				{
					Event.current.Use ();
					Application.Quit ();
				}
				else if (creditsButtonRect.Contains (Event.current.mousePosition))
				{
					Event.current.Use ();
					showCredits = true;
				}
			break;
			case EventType.Repaint:
				GUI.DrawTexture (new Rect (0.0f, 0.0f, Screen.width, Screen.height), background);
			break;
		}
	}
}
