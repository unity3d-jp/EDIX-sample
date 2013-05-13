using UnityEngine;
using System.Collections;

public class ImpactWarning : MonoBehaviour
{
	new public Renderer renderer;
	
	
	private float fadeSpeed = 1.0f;
	
	
	void Reset ()
	// Run setup on component attach, so it is visually more clear which references are used
	{
		Setup ();
	}
	
	
	void Setup ()
	// If renderer is not set, try using fallbacks
	{
		if (renderer == null)
		{
			renderer = GetComponentInChildren <Renderer> ();
		}
	}
	
	
	void Awake ()
	// Verify setup
	{
		Setup ();
			// Retry setup if references were cleared post-add
		
		if (renderer == null)
		{
			Debug.LogError ("No renderer available. Please correct and restart.", this);
			enabled = false;
			return;
		}
	}
	
	
	void OnEnable ()
	{
		renderer.material.color = new Color (1.0f, 1.0f, 1.0f, 0.0f);
	}
	
	
	void Update ()
	{
		renderer.material.color = new Color (1.0f, 1.0f, 1.0f, renderer.material.color.a + fadeSpeed * Time.deltaTime);
	}
}
