using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class Meteorite : MonoBehaviour
{
	public float speed = 2.0f, maxDistance = 100.0f;
	public GameObject[] hitEffectPrefabs;
	public LayerMask warningBase;
	
	
	private const float kWarningDistance = 20.0f;
	
	
	private ParticleEmitter emitter;
	private DemoControl demoControl;
	private ImpactWarning warningInstance;
		// Set when we've detected impending impact and have spawned an impact warning instance
	
	
	void Awake ()
	{
		emitter = GetComponentInChildren<ParticleEmitter> ();
		demoControl = FindObjectOfType (typeof (DemoControl)) as DemoControl;
			// Meteorites get spawned by demo control, so we can assume that it exists
		rigidbody.isKinematic = true;
		rigidbody.useGravity = false;
	}
	
	
	void OnDisable ()
	{
		warningInstance = null;
		emitter.ClearParticles ();
	}
	
	
	void FixedUpdate ()
	{
		RaycastHit hit;
		if (warningInstance == null && Physics.Raycast (transform.position, transform.forward, out hit, kWarningDistance, warningBase))
		// If we haven't detected impact yet and we now do, spawn an impact warning and store its reference
		{
			warningInstance = demoControl.SpawnWarning (hit.point, Quaternion.identity);
		}
	}
	
	
	void Update ()
	// Move forward! (The rigidbody is kinematic, so movement comes from the transform)
	{	
		if (transform.position.magnitude > maxDistance)
		{
			demoControl.DespawnMeteorite (this);
		}
		
		transform.position += transform.forward * speed * Time.deltaTime;
	}
	
	
	void OnTriggerEnter (Collider other)
	// We hit something
	{
		Player player = other.transform.root.GetComponentInChildren<Player> ();
		
		if (player != null)
		// Hit the player if we just struck him
		{
			player.OnMeteoriteStrike ();
		}
		
		foreach (GameObject prefab in hitEffectPrefabs)
		// Spawn all the hit effects
		{
			Instantiate (prefab, transform.position, Quaternion.identity);
		}
		
		// Despawn //
		
		demoControl.DespawnWarning (warningInstance);
		demoControl.DespawnMeteorite (this);
	}
}
