using UnityEngine;
using System.Collections;

public class XperiaInput : MonoBehaviour
{
	static AndroidJavaObject currentConfig = null;
	static AndroidJavaObject CurrentConfig
	{
		get
		{
			if (currentConfig == null)
			{
				using (AndroidJavaClass player = new AndroidJavaClass ("com.unity3d.player.UnityPlayer"))
				{
					AndroidJavaObject activity = player.GetStatic<AndroidJavaObject> ("currentActivity");
					currentConfig = activity.Call<AndroidJavaObject> ("getResources").Call<AndroidJavaObject> ("getConfiguration");
				}
			}
			
			return currentConfig;
		}
	}
	
	
	public static bool KeypadAvailable
	{
		get
		{
			const int NAVIGATIONHIDDEN_UNDEFINED = 0;
			const int NAVIGATIONHIDDEN_NO = 1;
			//const int NAVIGATIONHIDDEN_YES = 2;
				// Based on http://developer.android.com/reference/android/content/res/Configuration.html

			int nav = CurrentConfig.Get<int> ("navigationHidden");
			
			return nav == NAVIGATIONHIDDEN_NO || nav == NAVIGATIONHIDDEN_UNDEFINED;
		}
	}
	
	
	public static int KeypadHiddenStatus
	{
		get
		{
			return CurrentConfig.Get<int> ("navigationHidden");
		}
	}
	
	
	public static Vector2 LeftStick
	{
		get
		{
			return GetStick (0.0f, 0.0f);
		}
	}
	
	
	public static Vector2 RightStick
	{
		get
		{
			return GetStick (AndroidInput.secondaryTouchWidth - AndroidInput.secondaryTouchHeight, 0.0f);
		}
	}
	
	
	static Vector2 GetStick (float zeroX, float zeroY)
	// Result has a magnitude of max 1. X -1 is left, +1 is right. Y -1 is down, +1 is up.
	{
		if (AndroidInput.secondaryTouchEnabled && KeypadAvailable)
		{
			for (int i = 0; i < AndroidInput.touchCountSecondary; i++)
			{
				Vector2 touchPosition = AndroidInput.GetSecondaryTouch (i).position;
			
				if (touchPosition.x < zeroX || touchPosition.x > zeroX + AndroidInput.secondaryTouchHeight)
				{
					continue;
				}
			
				return new Vector2 (
					((touchPosition.x - zeroX) / AndroidInput.secondaryTouchHeight) * 2.0f - 1.0f,
						// Height is the radius, so we use that for X as well
					(((touchPosition.y - zeroY) / AndroidInput.secondaryTouchHeight) * 2.0f - 1.0f) * -1.0f
				);
			}
		}
		
		return Vector2.zero;
	}
}
