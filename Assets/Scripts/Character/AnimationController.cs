using UnityEngine;
using System.Collections;


public enum CharacterState
{
	Normal,
	Jumping,
	Falling,
	Landing,
	Dying
}


[RequireComponent (typeof (MovementController))]
public class AnimationController : MonoBehaviour
{
	public Animation target;
		// The animation component we're controlling
	public Renderer jumpBoostVisuals;
		// The renderer visible while jumping
	public new Rigidbody rigidbody;
		// The rigidbody we draw velocity from
	public float
		walkSpeed = 0.6f,
		runSpeed = 1.0f,
			// Used in connection with the rigidbody velocity to determine which animation to play
		runningLandingFactor = 0.2f,
			// Reduces the duration of the landing animation when the rigidbody has hoizontal movement
		jumpSpeedPeak = 10.0f,
			// Determines at which speed the jump boost visuals will be shown with full alpha
		minJumpBoostAlpha = 0.1f;
			// The minimum alpha of the jump boost visuals
	public CharacterState state = CharacterState.Falling;
		// State of the controller
		
	
	private const string
		kRunForwardState = "runforward",
		kRunBackwardsState = "runbackwards",
		kStrafeLeftState = "strafeleft",
		kStrafeRightState = "straferight",
		kIdleState = "idle",
		kJumpState = "jump",
		kFallState = "fall",
		kLandState = "land",
		kHoverState = "hover",
		kDyingState = "die";
			// The animation state names
	
	
	private MovementController movementController;
		// We grab groundedness from the movement controller
	private bool canLand = true;
	
	
	private Vector3 HorizontalMovement
	{
		get
		{
			return new Vector3 (rigidbody.velocity.x, 0.0f, rigidbody.velocity.z);
		}
	}


	void Reset ()
	// Run setup on component attach, so it is visually more clear which references are used
	{
		Setup ();
	}
	
	
	void Setup ()
	// If rigidbody is not set, try using fallbacks
	{
		if (target == null)
		{
			target = GetComponent<Animation> ();
		}
		
		if (rigidbody == null)
		{
			rigidbody = GetComponent<Rigidbody> ();
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
		
		if (rigidbody == null)
		{
			Debug.LogError ("No rigidbody assigned. Please correct and restart.", this);
			enabled = false;
			return;
		}
		
		if (jumpBoostVisuals == null)
		{
			Debug.LogError ("No jump boost visuals assigned. Please correct and restart.", this);
			enabled = false;
			return;
		}
		
		movementController = GetComponent<MovementController> ();
		movementController.onJump += OnJump;
		
		target[kRunForwardState].layer =
			target[kRunBackwardsState].layer =
			target[kStrafeLeftState].layer =
			target[kStrafeRightState].layer = 1;
		target.SyncLayer (1);
		target[kIdleState].layer = 1;
	}
	
	
	void OnJump ()
	// Start a jump
	{
		canLand = false;
		state = CharacterState.Jumping;
		
		Invoke ("Fall", target[kJumpState].length);
	}
	
	
	void OnLand ()
	// Start landing
	{
		canLand = false;
		state = CharacterState.Landing;
		
		Invoke (
			"Land",
			target[kLandState].length * (HorizontalMovement.magnitude < walkSpeed ? 1.0f : runningLandingFactor)
				// Land quicker if we're moving enough horizontally to start walking after landing
		);
	}
	
	
	void Fall ()
	// End a jump and transition to a falling state (ignore if already grounded)
	{
		if (movementController.Grounded)
		{
			return;
		}
		state = CharacterState.Falling;
	}
	
	
	void Land ()
	// End a landing and transition to normal animation state (ignore if not currently landing)
	{
		if (state != CharacterState.Landing)
		{
			return;
		}
		state = CharacterState.Normal;
	}
	
	
	void FixedUpdate ()
	// Handle changes in groundedness
	{
		if (movementController.Grounded)
		{
			if (state == CharacterState.Falling || (state == CharacterState.Jumping && canLand))
			{
				OnLand ();
			}
		}
		else if (state == CharacterState.Jumping)
		{
			canLand = true;
		}
	}
	
	
	void Update ()
	// Blend animations based on state, speed and direction
	{
		ClearRunLayer ();
		
		jumpBoostVisuals.enabled = Vector3.Angle (rigidbody.velocity, rigidbody.transform.up) <
			Vector3.Angle (rigidbody.velocity, rigidbody.transform.up * -1.0f);
			// Show the jump boost visuals only when going upwards
		
		if (jumpBoostVisuals.enabled)
		// Fade the alpha of the jump boost visuals w. speed
		{
			jumpBoostVisuals.material.color = new Color (
				1.0f,
				1.0f,
				1.0f,
				Vector3.Project (rigidbody.velocity, rigidbody.transform.up).magnitude / jumpSpeedPeak + minJumpBoostAlpha
			);
		}
		
		switch (state)
		{
			case CharacterState.Normal:
				Vector3 movement = HorizontalMovement;
				
				if (movement.magnitude < walkSpeed)
				{
					target.Blend (kIdleState);
				}
				else
				// Moving
				{
					Vector3 movementDirection = movement.normalized;
					
					if (movement.magnitude < runSpeed)
					// Walking
					{
						target[kRunForwardState].speed =
							target[kRunBackwardsState].speed =
							target[kStrafeLeftState].speed =
							target[kStrafeRightState].speed = runSpeed / walkSpeed;
					}
					else
					// Running
					{
						target[kRunForwardState].speed =
							target[kRunBackwardsState].speed =
							target[kStrafeLeftState].speed =
							target[kStrafeRightState].speed = 1.0f;
					}
					
					float
						forward = Vector3.Project (movementDirection, rigidbody.transform.forward).magnitude,
						sideways = Vector3.Project (movementDirection, rigidbody.transform.right).magnitude;
					
					if (
						Vector3.Angle (movementDirection, rigidbody.transform.forward) >
							Vector3.Angle (movementDirection, rigidbody.transform.forward * -1.0f)
					)
					{
						target.Blend (kRunBackwardsState, forward);
					}
					else
					{
						target.Blend (kRunForwardState, forward);
					}
					
					if (
						Vector3.Angle (movementDirection, rigidbody.transform.right) >
							Vector3.Angle (movementDirection, rigidbody.transform.right * -1.0f)
					)
					{
						target.Blend (kStrafeLeftState, sideways);
					}
					else
					{
						target.Blend (kStrafeRightState, sideways);
					}
				}
				
				jumpBoostVisuals.enabled = false;
			break;
			case CharacterState.Jumping:
				target.Blend (kFallState, 0.0f);
				target.Blend (kJumpState, 1.0f, 0.0f);
			break;
			case CharacterState.Falling:
				target.Blend (kFallState);
			break;
			case CharacterState.Landing:
				target.Blend (kFallState, 0.0f);
				target.Blend (kLandState, 1.0f, 0.0f);
				
				jumpBoostVisuals.enabled = true;
				jumpBoostVisuals.material.color = Color.white;
			break;
			case CharacterState.Dying:
				target.CrossFade (kDyingState);
			break;
		}
	}
	
	
	void ClearRunLayer ()
	// Set blend weights on running and idle animations to zero
	{
		target.Blend (kIdleState, 0.0f);
		target.Blend (kRunForwardState, 0.0f);
		target.Blend (kRunBackwardsState, 0.0f);
		target.Blend (kStrafeLeftState, 0.0f);
		target.Blend (kStrafeRightState, 0.0f);
		target.Blend (kDyingState, 0);
	}
}
