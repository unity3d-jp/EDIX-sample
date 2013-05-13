using UnityEngine;
using System.Collections;

public class Spaceship : MonoBehaviour
{
	private Player activePlayer = null;
	
	
	IEnumerator Start ()
	{
		while (Application.isPlaying)
		{
			if (activePlayer == null)
			// Hold until we have an active player
			{
				yield return null;
				continue;
			}
			
			yield return new WaitForSeconds (0.2f);
				// Waiting time between cargo unload
			
			if (activePlayer == null)
			{
				continue;
			}
			
			if (activePlayer.UnloadCargo ())
			{
				// NOTE: Could play audio feedback here
			}
		}
	}
	
	
	void OnTriggerEnter (Collider other)
	{
		Player player = other.transform.root.GetComponentInChildren<Player> ();
		
		if (player == null)
		{
			return;
		}
		
		activePlayer = player;
	}
	
	
	void OnTriggerExit (Collider other)
	{
		Player player = other.transform.root.GetComponentInChildren<Player> ();
		
		if (player == null)
		{
			return;
		}
		
		if (activePlayer == player)
		{
			activePlayer = null;
		}
	}
}
