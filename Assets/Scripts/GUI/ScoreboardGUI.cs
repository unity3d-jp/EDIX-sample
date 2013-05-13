using UnityEngine;
using System.Collections;

public class ScoreboardGUI : MonoBehaviour
{
	public Texture2D scoreBackground;
	public Rect contentArea;
		// Where on the background should the content be set up?
	public Font smallFont, largeFont;
	
	
	private const int maxNameLength = 8, maxScoreSlots = 5;
	private const float listOffset = 10.0f;
	
	
	private int scoreIndex = -1;
	private bool loading = false;
	private GUIStyle listStyle = null, editListStyle = null, buttonStyle = null;
	
	
	string Name
	{
		get
		{
			return PlayerPrefs.GetString ("Last user", "Player 1");
		}
		set
		{
			PlayerPrefs.SetString ("Last user", value.Substring (0, Mathf.Min (value.Length, maxNameLength)));
		}
	}
	
	
	int Score
	{
		get
		{
			return PlayerPrefs.GetInt ("Last score");
		}
	}
	
	
	void Start ()
	// Enter new score into highscore if applicable
	{
		for (int i = 0; i < maxScoreSlots; i++)
		{
			if (GetScore (i) < Score)
			{
				scoreIndex = i;
				for (int j = maxScoreSlots - 2; j > i; j--)
				{
					SetScore (j + 1, GetScore (j));
					SetScoreName (j + 1, GetScoreName (j));
				}
				break;
			}
		}
	}
	
	
	void VerifyStyles ()
	// If needed, build styles used in this view
	{
		if (listStyle == null)
		{
			listStyle = new GUIStyle (GUI.skin.GetStyle ("Label"));
			listStyle.font = smallFont;
		}
		
		if (editListStyle == null)
		{
			editListStyle = new GUIStyle (GUI.skin.GetStyle ("TextArea"));
			editListStyle.font = smallFont;
		}
		
		if (buttonStyle == null)
		{
			buttonStyle = new GUIStyle (GUI.skin.GetStyle ("Label"));
			buttonStyle.font = largeFont;
		}
	}
	
	
	// Score accessors //
	
	
	void VerifyScoreIndex (int index)
	{
		if (index < 0 || index >= maxScoreSlots)
		{
			throw new System.ArgumentException ("Invalid score index.");
		}
	}
	
	
	int GetScore (int index)
	{
		VerifyScoreIndex (index);
		
		return PlayerPrefs.GetInt ("Score " + index + " score", 0);
	}
	
	
	void SetScore (int index, int value)
	{
		VerifyScoreIndex (index);
		
		PlayerPrefs.SetInt ("Score " + index + " score", value);
	}
	
	
	string GetScoreName (int index)
	{
		VerifyScoreIndex (index);
		
		return PlayerPrefs.GetString ("Score " + index + " name", "Unknown" + (index + 1));
	}
	
	
	void SetScoreName (int index, string name)
	{
		VerifyScoreIndex (index);
		
		PlayerPrefs.SetString ("Score " + index + " name", name);
	}
	
	
	void OnGUI ()
	{
		VerifyStyles ();
		
		GUI.DrawTexture (new Rect (0.0f, 0.0f, Screen.width, Screen.height), scoreBackground);
		
		if (loading)
		// Reloading the main level takes a bit of time, here we're showing that the retry/save touch was registered
		{
			GUIContent loadingContent = new GUIContent ("Loading...");
			Vector2 loadingSize = buttonStyle.CalcSize (loadingContent);
			GUI.Label (new Rect ((Screen.width - loadingSize.x) * 0.5f, (Screen.height - loadingSize.y) * 0.5f, loadingSize.x, loadingSize.y), loadingContent, buttonStyle);
			return;
		}
		
		GUILayout.BeginArea (contentArea);
			GUILayout.BeginHorizontal ();
				GUILayout.Label ("Name", listStyle);
				GUILayout.FlexibleSpace ();
				GUILayout.Label ("Score", listStyle);
			GUILayout.EndHorizontal ();
			
			GUILayout.Space (listOffset);
		
			for (int i = 0; i < maxScoreSlots; i++)
			{
				GUILayout.BeginHorizontal ();
					if (i == scoreIndex)
					{
						Name = GUILayout.TextArea (Name, editListStyle);
						GUILayout.FlexibleSpace ();
						GUILayout.Label (Score.ToString (), listStyle);
					}
					else
					{
						GUILayout.Label (GetScoreName (i), listStyle);
						GUILayout.FlexibleSpace ();
						GUILayout.Label (GetScore (i).ToString (), listStyle);
					}
				GUILayout.EndHorizontal ();
			}
			
			GUILayout.FlexibleSpace ();
			
			GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				
				GUIContent content = new GUIContent (scoreIndex > -1 ? "Save" : "Retry");
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
							loading = true;
							Exit ();
						}
					break;
				}
			GUILayout.EndHorizontal ();
		GUILayout.EndArea ();
	}
	
	
	void Exit ()
	// Save score (if applicable) and load main level
	{
		if (scoreIndex > -1)
		{
			SetScore (scoreIndex, Score);
			SetScoreName (scoreIndex, Name);
		}
		Application.LoadLevel ("LostInSpace");
	}
}