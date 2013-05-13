using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (SetupGUI))]
[RequireComponent (typeof (GameGUI))]
[RequireComponent (typeof (MenuGUI))]
[RequireComponent (typeof (CreditsGUI))]
[RequireComponent (typeof (AudioSource))]
public class DemoControl : MonoBehaviour
{
	public Meteorite meteoritePrefab;
		// Prefab used for meteorites
	public ImpactWarning meteoriteWarningPrefab;
		// Prefab used for meteorite impact warnings
	public Vector2 showerArea = new Vector2 (10.0f, 10.0f);
		// The size of the area in which meteorites can spawn
	public float minimumDelay = 0.1f, maximumDelay = 1.0f;
		// Dicate delay between meteorite spawns
	public int
		poolSize = 30,
			// Number of items in object pools
		timerSeconds = 120;
			// Start game time in seconds
	
	
	private Player player;
	private GameGUI gameGUI;
	private MenuGUI menuGUI;
	private CreditsGUI creditsGUI;
	private SetupGUI setupGUI;
	private Queue<Meteorite> meteoritePool;
	private Queue<ImpactWarning> warningPool;
	private float timeSpent = 0.0f;
	
	
	public int TimeLeft
	{
		get
		{
			return timerSeconds - (int)timeSpent;
		}
	}
	
	
	void Awake ()
	// When just starting up, pause immediately
	{
		Time.timeScale = 0.0f;
	}
	
	
	IEnumerator Start ()
	{
		if (meteoritePrefab == null)
		{
			Debug.LogError ("No meteorite prefab set. Please correct and restart.", this);
			enabled = false;
			yield break;
		}
		
		player = FindObjectOfType (typeof (Player)) as Player;
		gameGUI = GetComponent<GameGUI> ();
		menuGUI = GetComponent<MenuGUI> ();
		creditsGUI = GetComponent<CreditsGUI> ();
		setupGUI = GetComponent<SetupGUI> ();
		
		// Instantiate meteorite and warning object pools - one instance of each per frame //
		
		meteoritePool = new Queue<Meteorite> ();
		warningPool = new Queue<ImpactWarning> ();
		for (int i = 0; i < poolSize && Application.isPlaying; i++)
		{
			yield return null;
			DespawnMeteorite (Instantiate (meteoritePrefab, transform.position, transform.rotation) as Meteorite);
			DespawnWarning (Instantiate (meteoriteWarningPrefab, transform.position, transform.rotation) as ImpactWarning);
		}
		
		while (Application.isPlaying)
		// Meteorite spawn loop
		{
			yield return new WaitForSeconds (Random.Range (minimumDelay, maximumDelay));
			
			SpawnMeteorite (
				transform.position +
					transform.forward * Random.Range (showerArea.y * -0.5f, showerArea.y * 0.5f) +
					transform.right * Random.Range (showerArea.x * -0.5f, showerArea.x * 0.5f),
				transform.rotation * Quaternion.AngleAxis (90.0f, transform.right)
			);
		}
	}
	
	
	public Meteorite SpawnMeteorite (Vector3 position, Quaternion rotation)
	// Grab a meteorite from the object pool and activate it with the given position and rotation
	{
		if (meteoritePool.Count < 1)
		{
			return null;
		}
		
		Meteorite meteorite = meteoritePool.Dequeue ();
		meteorite.transform.position = position;
		meteorite.transform.rotation = rotation;
		meteorite.gameObject.SetActiveRecursively (true);
		
		return meteorite;
	}
	
	
	public void DespawnMeteorite (Meteorite meteorite)
	// Disable the given meteorite and move it back into the object pool
	{
		meteorite.gameObject.SetActiveRecursively (false);
		meteoritePool.Enqueue (meteorite);
	}
	
	
	public ImpactWarning SpawnWarning (Vector3 position, Quaternion rotation)
	// Grab an impact warning from the object pool and activate it with the given position and rotation
	{
		if (warningPool.Count < 1)
		{
			return null;
		}
		
		ImpactWarning warning = warningPool.Dequeue ();
		warning.transform.position = position;
		warning.transform.rotation = rotation;
		warning.gameObject.SetActiveRecursively (true);
		
		return warning;
	}
	
	
	public void DespawnWarning (ImpactWarning warning)
	// Disable the given impact warning and move it back into the object pool
	{
		if (warning == null)
		{
			return;
		}
		
		warning.gameObject.SetActiveRecursively (false);
		warningPool.Enqueue (warning);
	}

	
	void Update ()
	{
		if (setupGUI.enabled)
		{
			if (setupGUI.Verified)
			{
				setupGUI.enabled = false;
				menuGUI.enabled = true;
			}
			else
			{
				menuGUI.enabled = false;
				return;
			}
		}
		
		// Handle pause and unpause/menu-exit keys //
		
		if (Time.timeScale == 0.0f)
		{
			if (Input.GetKeyDown ("joystick button 3"))
			{
				Time.timeScale = 1.0f;
			}
		}
		else
		{
			timeSpent += Time.deltaTime;
			
			if (Input.GetKeyDown (KeyCode.Return) || Input.GetKeyDown (KeyCode.Menu) || (Time.timeScale > 0.0f && !XperiaInput.KeypadAvailable))
			{
				Time.timeScale = 0.0f;
			}
		}
		
		if (Input.GetKeyDown (KeyCode.Escape))
		// Hard quit on the back button
		{
			Application.Quit ();
		}
		
		audio.enabled = PlayerPrefs.GetInt ("Play audio", 1) != 0;
			// Enable/disable audio dependent on player prefs setting toggled elsewhere
		
		if (TimeLeft <= 0)
		// Set score and load highscore on timer end
		{
			PlayerPrefs.SetInt ("Last score", player.Points);
			Application.LoadLevel ("Endgame");
		}
		
		// Flip game and menu/credits GUI based on game pause state //
		
		gameGUI.enabled = Time.timeScale > 0.0f;
		creditsGUI.enabled = menuGUI.enabled = !gameGUI.enabled;
	}
	
	
	void OnDrawGizmos ()
	// Visualize meteorite spawn field
	{
		Gizmos.matrix = Matrix4x4.TRS (transform.position, transform.localRotation, transform.localScale);
		Gizmos.DrawWireCube (Vector3.zero, new Vector3 (showerArea.x, 0.0f, showerArea.y));
	}
}
