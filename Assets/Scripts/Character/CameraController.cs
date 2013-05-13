using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	public Camera target;
		// The camera being controlled
	public Collider focus;
		// The transform being focused on by the camera
	public LayerMask obstructionMask;
		// All layers in this mask are trated as obstructions
	public float
		sideAngle = 30.0f,
			// Angle between the forward vector and the left/right look vectors
		distance = 3.0f,
			// Target - focus distance
		elevation = 0.2f,
			// Elevation of the direction vectors
		maxElevation = 0.5f,
			// Max elevation differentiation from default +/-
		upOffset = 0.9f,
			// Offset upwards of the position
		upLookOffset = 0.9f,
			// Upwards offset of the focus position
		rotationSpeed = 3.0f,
			// The speed of the rotation between the vectors
		verticalRotationSpeed = 0.5f;
			// The speed of the up/down look input
	
	
	private float lookLeftFactor = 0.0f, lookRightFactor = 0.0f, currentElevation;
	private bool obstructedLeft = false, obstructedRight = false, obstructedOrigin = false, leftOnRandom;
	
	
	private const float inputThreshold = 0.2f;
	
	
	string LeftKey
	// Abstracted away for testing in playmode
	{
		get
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return "left shift";
			}
			else
			{
				return "left";
			}
		}
	}
	
	
	string RightKey
	// Abstracted away for testing in playmode
	{
		get
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return "right shift";
			}
			else
			{
				return "right";
			}
		}
	}
	
	
	Vector3 ElevationVector
	// Elevation is controlled on the vertical axis of the right stick
	{
		get
		{
			return focus.transform.up * currentElevation;
		}
	}
	
	
	Vector3 OriginVector
	// The vector leading from the focus point to the corresponding camera position behind the focus point
	{
		get
		{
			return (focus.transform.forward * -1.0f + ElevationVector) * distance;
		}
	}
	
	
	Vector3 LeftVector
	// The vector leading from the focus point to the corresponding camera position to the left of the focus point
	{
		get
		{
			return (Vector3.Slerp (focus.transform.forward, focus.transform.right * -1.0f, sideAngle / 90.0f) + ElevationVector) * distance;
		}
	}
	
	
	Vector3 RightVector
	// The vector leading from the focus point to the corresponding camera position to the right of the focus point
	{
		get
		{
			return (Vector3.Slerp (focus.transform.forward, focus.transform.right, sideAngle / 90.0f) + ElevationVector) * distance;
		}
	}
	
	
	float ViewRadius
	// The minimum clear radius between the camera and the target
	{
		get
		{
			float fieldOfViewRadius = (distance / Mathf.Sin (90.0f - target.fieldOfView / 2.0f)) * Mathf.Sin (target.fieldOfView / 2.0f);
				// Half the width of the field of view of the camera at the position of the target
			float doubleCharacterRadius = Mathf.Max (focus.bounds.extents.x, focus.bounds.extents.z) * 2.0f;
			
			return Mathf.Min (doubleCharacterRadius, fieldOfViewRadius);
		}
	}
	

	void Reset ()
	// Run setup on component attach, so it is visually more clear which references are used
	{
		Setup ();
		
		obstructionMask = -1 ^ 1 << LayerMask.NameToLayer ("Player");
			// By default consider everything except the player layer an obstruction
	}
	
	
	void Setup ()
	// If target or focus are not set, try using fallbacks
	{
		if (target == null)
		{
			target = GetComponentInChildren<Camera> ();
		}
		
		if (target == null)
		{
			target = Camera.main;
		}
		
		if (focus == null)
		{
			focus = GetComponentInChildren<Collider> ();
		}
	}
	
	
	void Start ()
	// Verify setup
	{
		Setup ();
			// Retry setup if references were cleared post-add
		
		if (target == null)
		{
			Debug.LogError ("No target assigned. Please correct and restart.", this);
			enabled = false;
			return;
		}
		
		if (focus == null)
		{
			Debug.LogError ("No focus assigned. Please correct and restart.", this);
			enabled = false;
			return;
		}
		
		currentElevation = elevation;
	}
	
	
	void FixedUpdate ()
	// Check for obstructions between the focus point and the three camera positions
	{
		bool wasObstructedOrigin = obstructedOrigin;
		
		Vector3 focusPosition = focus.transform.position + focus.transform.up * upLookOffset;
		Vector3 leftDirection = (focus.transform.position + LeftVector + focus.transform.up * upOffset) - focusPosition;
		Vector3 rightDirection = (focus.transform.position + RightVector + focus.transform.up * upOffset) - focusPosition;
		Vector3 originDirection = (focus.transform.position + OriginVector + focus.transform.up * upOffset) - focusPosition;
		
		RaycastHit hit;
		obstructedLeft = Physics.SphereCast (focusPosition, ViewRadius, leftDirection, out hit, distance, obstructionMask);
		obstructedRight = Physics.SphereCast (focusPosition, ViewRadius, rightDirection, out hit, distance, obstructionMask);
		obstructedOrigin = Physics.SphereCast (focusPosition, ViewRadius, originDirection, out hit, distance, obstructionMask);
		
		if (obstructedOrigin && !wasObstructedOrigin)
		// If the origin vector just got obstructed, decide which direction to pick if a random one is needed
		{
			leftOnRandom = Random.Range (0, 2) == 0;
		}
	}
	
	
	void Update ()
	// Update camera positioning
	{
		float input = MovementController.RightStick.y;
		if (Mathf.Abs (input) > inputThreshold)
		// Right stick vertical axis controls camera vector elevation
		{
			currentElevation = Mathf.Clamp (
				currentElevation - input * verticalRotationSpeed * Time.deltaTime,
				elevation - maxElevation,
				elevation + maxElevation
			);
		}
		
		if (!obstructedLeft && (Input.GetKey (LeftKey) || (obstructedOrigin && (obstructedRight || leftOnRandom))))
		// While left is held, move toward the left position
		{
			ReduceRight ();
			if (lookRightFactor <= 0.0f)
			{
				lookLeftFactor = Mathf.Clamp (lookLeftFactor + Time.deltaTime * rotationSpeed, 0.0f, 1.0f);
			}
		}
		else if (!obstructedRight && (Input.GetKey (RightKey) || (obstructedOrigin && (obstructedLeft || !leftOnRandom))))
		// While left is held, move toward the right position
		{
			ReduceLeft ();
			if (lookLeftFactor <= 0.0f)
			{
				lookRightFactor = Mathf.Clamp (lookRightFactor + Time.deltaTime * rotationSpeed, 0.0f, 1.0f);
			}
		}
		else
		// With no keys held, move back to the original position
		{
			ReduceLeft ();
			ReduceRight ();
		}
		
		if (lookLeftFactor > lookRightFactor)
		// Interpolate relevant vector and set position
		{
			target.transform.position = focus.transform.position + Vector3.Slerp (OriginVector, LeftVector, lookLeftFactor);
		}
		else
		{
			target.transform.position = focus.transform.position + Vector3.Slerp (OriginVector, RightVector, lookRightFactor);
		}
		
		// Apply offsets //
		
		target.transform.position += focus.transform.up * upOffset;
		
		target.transform.LookAt (focus.transform.position + focus.transform.up * upLookOffset);
	}
	
	
	void ReduceLeft ()
	{
		lookLeftFactor = Mathf.Clamp (lookLeftFactor - Time.deltaTime * rotationSpeed, 0.0f, 1.0f);
	}
	
	
	void ReduceRight ()
	{
		lookRightFactor = Mathf.Clamp (lookRightFactor - Time.deltaTime * rotationSpeed, 0.0f, 1.0f);
	}
}
