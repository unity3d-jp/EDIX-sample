using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent (typeof (Player))]
public class ActionController : MonoBehaviour
{
	new public Camera camera;
		// The camera reference is used to determine the field of view
	public float
		objectUpdateFramerate = 10.0f,
			// This framerate dictates how often the current object should be re-evaluated
		// The object split settings dictates when a better candidate should replace the current object
		objectSplitDistance = 2.0f,
		objectSplitAngle = 10.0f,
		closeProximity = 2.0f;
			// Objects within close proximity are considered available - regardless of angle to view

	
	public Player player;
		// The player instance to work with
	private List<ActiveObject> nearbyObjects = new List<ActiveObject> ();
	private ActiveObject currentObject = null;
	
	
	public ActiveObject CurrentObject
	{
		get
		{
			return currentObject;
		}
	}
	
	
	string InteractKey
	{
		get
		{
			return (Application.platform == RuntimePlatform.Android) ? "joystick button 0" : "backspace";
		}
	}
	
	
	void Reset ()
	// Run setup on component attach, so it is visually more clear which references are used
	{
		Setup ();
	}
	
	
	void Setup ()
	// If target is not set, try using fallbacks
	{
		if (camera == null)
		{
			camera = Camera.main;
		}
		
		if (player == null)
		{
			player = transform.root.GetComponentInChildren<Player> ();
		}
	}
	
	
	IEnumerator Start ()
	{
		Setup ();
			// Retry setup if references were cleared post-add
		
		if (camera == null)
		{
			Debug.LogError ("No camera assigned. Please correct and restart.", this);
			enabled = false;
			yield break;
		}
		
		if (player == null)
		{
			player = GetComponent<Player> ();
		}
		
		while (Application.isPlaying)
		// Update the currently focues object from the list
		{
			Vector3 vectorToCurrent = currentObject == null ? Vector3.zero : currentObject.transform.position - transform.position;
			
			if (
				currentObject != null && (
					!nearbyObjects.Contains (currentObject) ||
					Vector3.Angle (vectorToCurrent, transform.forward) > camera.fieldOfView * 0.5f
				)
			)
			// If the current object is out of focus, clear it
			{
				currentObject = null;
			}
			
			foreach (ActiveObject activeObject in nearbyObjects)
			{
				Vector3 vectorToObject = activeObject.transform.position - transform.position;
				
				if (
					Vector3.Angle (vectorToObject, transform.forward) > camera.fieldOfView * 0.5f &&
					!(vectorToObject.magnitude < closeProximity)
				)
				// Out of focus, ignore
				{
					continue;
				}
				
				if (currentObject == null)
				// No current object? We'll set this as current for now.
				{
					currentObject = activeObject;
					vectorToCurrent = vectorToObject;
					continue;
				}
				
				float distanceDifference = Mathf.Abs (vectorToCurrent.magnitude - vectorToObject.magnitude);
				float angularDifference = Mathf.Abs (Vector3.Angle (vectorToCurrent, transform.forward) - Vector3.Angle (vectorToObject, transform.forward));
				if (
					vectorToCurrent.magnitude > vectorToObject.magnitude &&
						distanceDifference > objectSplitDistance &&
					Vector3.Angle (vectorToCurrent, transform.forward) > Vector3.Angle (vectorToObject, transform.forward) &&
						angularDifference > objectSplitAngle
				)
				// If the object is closer distance and angle wise - and by enough, make that the current object
				{
					currentObject = activeObject;
					vectorToCurrent = vectorToObject;
				}
			}
			
			yield return new WaitForSeconds (1.0f / objectUpdateFramerate);
				// Maintain specified framerate
		}
	}
	
	
	public void Interact ()
	// Interact with the current object if set
	{
		if (currentObject == null)
		{
			return;
		}
		
		currentObject.Activate (player);
	}
	
	
	void Update ()
	// Handle user input
	{
		if (Input.GetKeyDown (InteractKey))
		{
			Interact ();
		}
	}
	
	
	void OnTriggerEnter (Collider other)
	// Add active objects to the list of nearby objects
	{
		ActiveObject activeObject = other.transform.root.GetComponentInChildren<ActiveObject> ();
		
		if (activeObject != null)
		{
			nearbyObjects.Add (activeObject);
		}
	}
	
	
	void OnTriggerExit (Collider other)
	// Remove active objects from the list of nearby objects
	{
		ActiveObject activeObject = other.transform.root.GetComponentInChildren<ActiveObject> ();
		
		if (activeObject != null)
		{
			nearbyObjects.Remove (activeObject);
		}
	}
}