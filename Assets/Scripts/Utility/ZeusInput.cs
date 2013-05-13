using UnityEngine;
using System.Collections;
using System.IO;

public class ZeusInput : MonoBehaviour {
	
	float scr_w;
	float scr_h;
	float pad_w;
	float pad_h;
	
	AndroidJavaObject currentConfig = null;
	
	// Use this for initialization
	void Start () {
		scr_w = Screen.width;
		scr_h = Screen.height;
		pad_w = AndroidInput.secondaryTouchWidth;
		pad_h = AndroidInput.secondaryTouchHeight;

		using (AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
			currentConfig = activity.Call<AndroidJavaObject>("getResources").Call<AndroidJavaObject>("getConfiguration");
		}
	}
		
	// The input mapping here is preliminary and subject to change..
	void OnGUI()
	{
		if (GUILayout.Button ("Exit"))
		{
			Application.LoadLevel (0);
		}
		
		if (Input.GetKey (KeyCode.LeftShift))
			GUI.Button(new Rect(0.2f * scr_w, 0.1f * scr_h, 0.2f * scr_w, 0.1f * scr_h), "KeyCode.LeftShift");
		if (Input.GetKey (KeyCode.RightShift))
			GUI.Button(new Rect(0.6f * scr_w, 0.1f * scr_h, 0.2f * scr_w, 0.1f * scr_h), "KeyCode.RightShift");
		
		if (Input.GetKey (KeyCode.UpArrow))
			GUI.Button(new Rect(0.2f * scr_w, 0.2f * scr_h, 0.2f * scr_w, 0.1f * scr_h), "KeyCode.UpArrow");
		if (Input.GetKey (KeyCode.LeftArrow))
			GUI.Button(new Rect(0.1f * scr_w, 0.3f * scr_h, 0.2f * scr_w, 0.1f * scr_h), "KeyCode.LeftArrow");
		if (Input.GetKey (KeyCode.RightArrow))
			GUI.Button(new Rect(0.3f * scr_w, 0.3f * scr_h, 0.2f * scr_w, 0.1f * scr_h), "KeyCode.RightArrow");
		if (Input.GetKey (KeyCode.DownArrow))
			GUI.Button(new Rect(0.2f * scr_w, 0.4f * scr_h, 0.2f * scr_w, 0.1f * scr_h), "KeyCode.DownArrow");
		
		if (Input.GetKey ("joystick button 2"))
			GUI.Button(new Rect(0.6f * scr_w, 0.2f * scr_h, 0.2f * scr_w, 0.1f * scr_h), "\"joystick button 2\"");
		if (Input.GetKey ("joystick button 1"))
			GUI.Button(new Rect(0.5f * scr_w, 0.3f * scr_h, 0.2f * scr_w, 0.1f * scr_h), "\"joystick button 1\"");
		if (Input.GetKey ("joystick button 3"))
			GUI.Button(new Rect(0.7f * scr_w, 0.3f * scr_h, 0.2f * scr_w, 0.1f * scr_h), "\"joystick button 3\"");
		if (Input.GetKey ("joystick button 0"))
			GUI.Button(new Rect(0.6f * scr_w, 0.4f * scr_h, 0.2f * scr_w, 0.1f * scr_h), "\"joystick button 0\"");
		
		if (Input.GetKey (KeyCode.F2))
			GUI.Button(new Rect(0.8f * scr_w, 0.1f * scr_h, 0.2f * scr_w, 0.1f * scr_h), "KeyCode.F2");
		if (Input.GetKey (KeyCode.Menu))
			GUI.Button(new Rect(0.8f * scr_w, 0.3f * scr_h, 0.2f * scr_w, 0.1f * scr_h), "KeyCode.Menu");
		if (Input.GetKey (KeyCode.Escape))
			GUI.Button(new Rect(0.8f * scr_w, 0.7f * scr_h, 0.2f * scr_w, 0.1f * scr_h), "KeyCode.Escape");
		
		if (Input.GetKey (KeyCode.Menu))
			GUI.Button(new Rect(0.2f * scr_w, 0.8f * scr_h, 0.2f * scr_w, 0.1f * scr_h), "KeyCode.Menu");
		if (Input.GetKey (KeyCode.Pause))
			GUI.Button(new Rect(0.4f * scr_w, 0.8f * scr_h, 0.2f * scr_w, 0.1f * scr_h), "KeyCode.Pause");
		if (Input.GetKey (KeyCode.Return))
			GUI.Button(new Rect(0.6f * scr_w, 0.8f * scr_h, 0.2f * scr_w, 0.1f * scr_h), "KeyCode.Return");
		
		for (int i = 0; i < Input.touchCount; ++i)
		{
			Touch touch = Input.GetTouch(i);
			GUI.Button(new Rect((touch.position.x - 120.0f), (scr_h - touch.position.y - 85.0f), 150.0f, 20.0f), "GetTouch(" + i + ")");
		}
		
		if (!AndroidInput.secondaryTouchEnabled)
			return;
		
		// Based on http://developer.android.com/reference/android/content/res/Configuration.html
		const int NAVIGATIONHIDDEN_UNDEFINED = 0;
		const int NAVIGATIONHIDDEN_NO = 1;
		const int NAVIGATIONHIDDEN_YES = 2;

		string nav_str = null;
		int nav = currentConfig.Get<int>("navigationHidden");
		if (nav == NAVIGATIONHIDDEN_NO)
			nav_str = "NAVIGATIONHIDDEN_NO";
		else if (nav == NAVIGATIONHIDDEN_YES)
			nav_str = "NAVIGATIONHIDDEN_YES";
		if (nav != NAVIGATIONHIDDEN_UNDEFINED)
			GUI.Button(new Rect(0.35f * scr_w, 0.0f * scr_h, 0.3f * scr_w, 0.1f * scr_h), nav_str);
		
		GUILayout.Label ("Pad W: " + pad_w);
		GUILayout.Label ("Pad H: " + pad_h);
		
		for (int i = 0; i < AndroidInput.touchCountSecondary; ++i)
		{
			Touch touch = AndroidInput.GetSecondaryTouch(i);
			// normalize
			float x = touch.position.x / pad_w;
			float y = touch.position.y / pad_h;
			GUILayout.Label (string.Format (
				"GetSecondaryTouch ({0}) : ({1:f2}, {2:f2})", i, x, y
			));
			
			// center on screen
			x = scr_w * (0.25f + x * 0.5f);
			y = scr_h * (0.25f + y * 0.5f);
			GUI.Button(new Rect((x - 120.0f), (y - 85.0f), 150.0f, 20.0f), "GetSecondaryTouch (" + i + ")");
		}
	}
}
