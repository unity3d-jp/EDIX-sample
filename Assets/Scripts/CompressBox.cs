using UnityEngine;
using System.Collections;

public class CompressBox : MonoBehaviour {
	
	[Range(1000, 6000)]
	[SerializeField]
	int power = 100;
	
	
	// Update is called once per frame
	void Update () {
		rigidbody.AddForce(transform.up * -power);
	}
	
	void OnGUI()
	{
		Rect rect = new Rect(0, 0, 600, 30);
	}
}
