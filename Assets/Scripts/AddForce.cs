using UnityEngine;
using System.Collections;

public class AddForce : MonoBehaviour
{
	
	[SerializeField]
	float power = 20;
	float p;
	
	void Start ()
	{
		float x = Random.Range (-1f, 1f);
		float y = Random.Range (-1f, 1f);
		float z = Random.Range (-1f, 1f);
		
		gameObject.rigidbody.AddForce (x, y, z);
		
		p = Vector3.Distance (Vector3.zero, rigidbody.velocity);
	}
	
	void FixedUpdate ()
	{
		Vector3 v = Vector3.Normalize (rigidbody.velocity);
		
		rigidbody.velocity = v * power;
		
	}
	
	void OnCollisionEnter (Collision c)
	{
		Debug.Log ("ga");
	}
}
