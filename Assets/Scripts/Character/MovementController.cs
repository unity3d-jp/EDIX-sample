using UnityEngine;
using System.Collections;


public delegate void JumpDelegate ();


public class MovementController : MonoBehaviour
{
	public Rigidbody target;
		// The object we're steering
	public float
		// Tweak these to ajust character responsiveness
		speed = 1.0f,
		walkSpeedDownscale = 2.0f,
		turnSpeed = 2.0f,
		jumpSpeed = 10.0f;
	public LayerMask groundLayers = -1;
		// Which layers should be walkable?
		// NOTICE: Make sure that the target collider is not in any of these layers!
	public float groundedCheckOffset = 0.7f;
		// Tweak so check starts from just within target footing
	public JumpDelegate onJump = null;
		// Assign to this delegate to respond to the controller jumping
		
		
	private const float
		// Tweak these to adjust behaviour relative to speed
		inputThreshold = 0.2f,
		groundDrag = 5.0f,
		directionalJumpFactor = 0.7f;
	private const float groundedDistance = 0.5f;
		// Tweak if character lands too soon or gets stuck "in air" often


	private bool grounded;
	
	
	public bool Grounded
	// Make our grounded status available for other components
	{
		get
		{
			return grounded;
		}
	}
	
	
	public static Vector2 LeftStick
	// Abstracted away for testing in playmode
	{
		get
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return XperiaInput.LeftStick;
			}
			else
			{
				return new Vector2 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));
			}
		}
	}
	
	
	public static Vector2 RightStick
	// Abstracted away for testing in playmode
	{
		get
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return XperiaInput.RightStick;
			}
			else
			{
				return new Vector2 (Input.GetAxis ("Mouse X"), Input.GetAxis ("Mouse Y"));
			}
		}
	}
	
	
	public string JumpKey
	// Abstracted away for testing in playmode
	{
		get
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return "joystick button 2";
			}
			else
			{
				return "space";
			}
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
		if (target == null)
		{
			target = GetComponent<Rigidbody> ();
		}
	}
	
		
	void Start ()
	// Verify setup, configure rigidbody
	{
		Setup ();
			// Retry setup if references were cleared post-add
		
		if (target == null)
		{
			Debug.LogError ("No target assigned. Please correct and restart.", this);
			enabled = false;
			return;
		}

		target.freezeRotation = true;
			// We will be controlling the rotation of the target, so we tell the physics system to leave it be
	}
	
	
	void Update ()
	// Handle rotation here to ensure smooth application.
	{
		float input = RightStick.x;
		
		if (Mathf.Abs (input) < inputThreshold)
		{
			return;
		}
		
		target.transform.RotateAround (target.transform.up, input * turnSpeed * Time.deltaTime);
	}
	
	
	void FixedUpdate ()
	// Handle movement here since physics will only be calculated in fixed frames anyway
	{
		grounded = Physics.Raycast (
			target.transform.position + target.transform.up * -groundedCheckOffset,
			target.transform.up * -1,
			groundedDistance,
			groundLayers
		);
			// Shoot a ray downward to see if we're touching the ground
		
		if (grounded)
		{
			target.drag = groundDrag;
				// Apply drag when we're grounded
			
			if (Input.GetKeyDown (JumpKey))
			// Handle jumping
			{
				target.AddForce (
					jumpSpeed * target.transform.up +
						target.velocity.normalized * directionalJumpFactor,
					ForceMode.VelocityChange
				);
					// When jumping, we set the velocity upward with our jump speed
					// plus some application of directional movement
				
				if (onJump != null)
				{
					onJump ();
				}
			}
			else
			// Only allow movement controls if we did not just jump
			{
				Vector3 movement = LeftStick.y * target.transform.forward + LeftStick.x * target.transform.right;
				
				float appliedSpeed = Vector3.Angle (movement, target.transform.forward) > 30.0f ? speed / walkSpeedDownscale : speed;
					// Scale down applied speed if walking primarily sideways or backwards

				if (movement.magnitude > inputThreshold)
				// Only apply movement if we have sufficient input
				{
					target.AddForce (movement.normalized * appliedSpeed, ForceMode.VelocityChange);
				}
				else
				// If we are grounded and don't have significant input, just stop horizontal movement
				{
					target.velocity = new Vector3 (0.0f, target.velocity.y, 0.0f);
					return;
				}
			}
		}
		else
		{
			target.drag = 0.0f;
				// If we're airborne, we should have no drag
		}
	}
}
