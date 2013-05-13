using UnityEngine;
using System.Collections;

// Depends on DemoControl, but DemoControl already requires this component
public class GameGUI : MonoBehaviour
{
	public ActionController actionController;
		// The action controller needs visualization of interaction availability
	public Player player;
		// The player instance holds cargo data
	public Texture2D
		// Interface resources
		fullCargo,
		emptyCargo,
		pauseIcon,
		audioIcon,
		interactionNotice;
	public Font timeFont;
	
	
	private const float
		// GUI scales
		messageBottomOffset = 40.0f,
		cargoTextureSize = 80.0f,
		cargoOffset = 20.0f,
		cornerTextureSize = 60.0f;
	
	
	private DemoControl demoControl;
	
	
	void Reset ()
	// Run setup on component attach, so it is visually more clear which references are used
	{
		Setup ();
	}
	
	
	void Setup ()
	// If action controller and player are not set, try using fallbacks
	{
		if (actionController == null)
		{
			actionController = FindObjectOfType (typeof (ActionController)) as ActionController;
		}
		
		if (player == null)
		{
			player = FindObjectOfType (typeof (Player)) as Player;
		}
	}
	
	
	void Start ()
	// Verify setup and additional setup
	{
		Setup ();
			// Retry setup if references were cleared post-add
		
		if (actionController == null)
		{
			Debug.LogError ("No action controller assigned. Please correct and restart.", this);
			enabled = false;
			return;
		}
		
		if (player == null)
		{
			Debug.LogError ("No player assigned. Please correct and restart.", this);
			enabled = false;
			return;
		}
			
		demoControl = GetComponent<DemoControl> ();
	}
	
	
	void OnGUI ()
	{
		Rect pauseRect = new Rect (Screen.width - cornerTextureSize, 0.0f, cornerTextureSize, cornerTextureSize),
			audioRect = new Rect (0.0f, 0.0f, cornerTextureSize, cornerTextureSize);
		
		switch (Event.current.type)
		// Draw and handle interaction notice and corner buttons
		{
			case EventType.Repaint:
				if (actionController.CurrentObject != null)
				{
					GUI.DrawTexture (
						new Rect (
							(Screen.width - interactionNotice.width) * 0.5f,
							Screen.height - interactionNotice.height - messageBottomOffset,
							interactionNotice.width,
							interactionNotice.height
						),
						interactionNotice
					);
				}
				GUI.DrawTexture (pauseRect, pauseIcon);
				GUI.DrawTexture (audioRect, audioIcon);
			break;
			case EventType.MouseDown:
				if (pauseRect.Contains (Event.current.mousePosition))
				{
					Time.timeScale = 0.0f;
					Event.current.Use ();
				}
				else if (audioRect.Contains (Event.current.mousePosition))
				{
					PlayerPrefs.SetInt ("Play audio", PlayerPrefs.GetInt ("Play audio", 1) != 0 ? 0 : 1);
					Event.current.Use ();
				}
			break;
		}
		
		// Cargo visualization //
		
		GUILayout.BeginArea (new Rect (cargoOffset, Screen.height - cargoTextureSize - cargoOffset, cargoTextureSize * Player.kMaxCargo, cargoTextureSize));
			GUILayout.BeginHorizontal ();
				for (int i = 0; i < Player.kMaxCargo; i++)
				{
					GUI.DrawTexture (GUILayoutUtility.GetRect (cargoTextureSize, cargoTextureSize), i < player.Cargo ? fullCargo : emptyCargo);
				}
				GUILayout.Label ("Points: " + player.Points);
			GUILayout.EndHorizontal ();
		GUILayout.EndArea ();
		
		// Timer //
		
		float minutesLeft = demoControl.TimeLeft / 60, secondsLeft = demoControl.TimeLeft - minutesLeft * 60;
		GUIContent timeContent = new GUIContent ("" + minutesLeft + ":" + secondsLeft);
		
		Font guiFont = GUI.skin.font;
		GUI.skin.font = timeFont;
		Vector2 size = GUI.skin.GetStyle ("Label").CalcSize (timeContent);
		GUI.Label (new Rect ((Screen.width - size.x) * 0.5f, 0.0f, size.x, size.y), timeContent);
		GUI.skin.font = guiFont;
	}
}
