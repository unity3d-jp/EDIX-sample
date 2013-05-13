using UnityEngine;
using System.Collections;

[RequireComponent (typeof (AnimationController))]
[RequireComponent (typeof (MovementController))]
public class Player : MonoBehaviour
{
	public ActionController actionController;
	public Transform spawnpoint;
		// Where to respawn the player on meteorite strike
	public float respawnTime = 4.0f;
		// The time, in seconds, the player should lie dead on the ground before respawning
	
	
	public const int kMaxCargo = 3;
	
	
	private AnimationController animationController;
	private MovementController movementController;
	private int cargo = 0, points = 0;
	private bool respawning = false;
	
	
	public int Cargo
	{
		get
		{
			return cargo;
		}
	}
	
	
	public int Points
	{
		get
		{
			return points;
		}
	}
	
	
	void Reset ()
	{
		Setup ();
	}
	
	
	void Setup ()
	{
		if (actionController == null)
		{
			actionController = GetComponentInChildren<ActionController> ();
		}
	}
	
	
	void Start ()
	{
		Setup ();
		
		if (spawnpoint == null)
		{
			Debug.LogError ("No spawnpoint set. Please correct and restart.", this);
			enabled = false;
			return;
		}
		
		if (actionController == null)
		{
			Debug.LogError ("No action controller set. Please correct and restart.", this);
			enabled = false;
			return;
		}
		
		animationController = GetComponent<AnimationController> ();
		movementController = GetComponent<MovementController> ();
	}
	
	
	public bool AddCargo ()
	{
		if (cargo >= kMaxCargo)
		{
			return false;
		}
		
		cargo++;
		
		return true;
	}
	
	
	public bool UnloadCargo ()
	{
		if (cargo <= 0)
		{
			return false;
		}
		
		cargo--;
		points++;
		
		return true;
	}
	
	
	public void OnMeteoriteStrike ()
	{
		StartCoroutine (Respawn ());
	}
	
	
	IEnumerator Respawn ()
	{
		if (respawning)
		{
			yield break;
		}
		
		respawning = true;
		
		movementController.enabled = false;
		actionController.enabled = false;
		animationController.state = CharacterState.Dying;
		cargo = 0;
		
		yield return new WaitForSeconds (respawnTime);
		
		transform.position = spawnpoint.position;
		transform.rotation = spawnpoint.rotation;
		animationController.state = CharacterState.Falling;
		movementController.enabled = true;
		actionController.enabled = true;
		
		respawning = false;
	}
}
