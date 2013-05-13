using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Collider))]
public class Radar : MonoBehaviour
{
	public GameObject target;
	
	
	void Reset ()
	// Run setup on component attach, so it is visually more clear which references are used
	{
		Setup ();
	}
	
	
	void Setup ()
	// If target is not set, try using fallbacks
	{
		if (target == null)
		{
			target = transform.root.gameObject;
		}
	}


	void Start ()
	// Verify setup
	{
		Setup ();
			// Retry setup if references were cleared post-add
		
		if (target == null)
		{
			Debug.LogError ("No target set, please correct and restart.", this);
			enabled = false;
			return;
		}
	}
	

	void OnTriggerEnter (Collider other)
	{
		target.SendMessage ("OnTriggerEnter", other, SendMessageOptions.DontRequireReceiver);
	}
	
	
	void OnTriggerExit (Collider other)
	{
		target.SendMessage ("OnTriggerExit", other, SendMessageOptions.DontRequireReceiver);
	}
}
