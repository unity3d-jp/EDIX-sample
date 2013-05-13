using UnityEngine;
using System.Collections;

public class SetupGUI : MonoBehaviour
{
	public Texture2D background;
	public Rect contentArea;
		// Where on the background should the content be set up?
	
	
	private bool editorVerified = false;
	
	
	public bool Verified
	{
		get
		{
			return editorVerified && Screen.width == 854 && Screen.height == 480;
		}
	}


	void Awake ()
	{
		editorVerified = !Application.isEditor;
	}
	

	void OnGUI ()
	{
		if (Verified)
		{
			return;
		}
		
		GUI.DrawTexture (new Rect (0.0f, 0.0f, Screen.width, Screen.height), background);
		
		GUILayout.BeginArea (contentArea);
			if (Screen.width != 854 || Screen.height != 480)
			{
				GUILayout.Label ("This game is designed to run on the Sony Ericsson Experia Play phone. Therefore the resolution is expected to be 854 x 480.");
				if (Application.isEditor)
				{
					GUILayout.Label ("Please switch the resolution of the game view to \"MotorolaDroid Wide (854x480)\" from the dropdown at the top of the Game view tab.");
				}
			}
			else
			{
				GUILayout.Label ("The playmode controls for this game are WASD + space for movement, mouse for look, left and right keys for over-the-shoulder camera and backspace for crystal collection.");
				if (Screen.width == 854 && Screen.height == 480 && GUILayout.Button ("OK"))
				{
					editorVerified = true;
				}
			}
		GUILayout.EndArea ();
	}
}
