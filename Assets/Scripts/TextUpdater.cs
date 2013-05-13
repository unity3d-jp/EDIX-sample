using UnityEngine;
using System.Collections;

[RequireComponent(typeof( AudioSource ))]
public class TextUpdater : MonoBehaviour
{
	
	[SerializeField]
	TextMesh english, japaneseb;
	[SerializeField]
	GameObject[] stars;
	[SerializeField]
	TextStruct[] textList;
	[SerializeField]
	AudioClip voice;
	[SerializeField]
	float seekTime;
	
	AudioSource source;
	
	void Start()
	{
		source = GetComponent<AudioSource>();
	}
	
	public void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Mouse0)) {
			StopAllCoroutines ();
			source.Stop();
			StartCoroutine (Read ());
		}
	}
	
	IEnumerator Read ()
	{
		yield return new WaitForSeconds(seekTime);
		
		source.PlayOneShot( voice );
		
		foreach (TextStruct text in textList) {
			english.text = text.english;
			japaneseb.text = text.japanese;
			yield return new WaitForSeconds(text.waitTime);
		
		}
		
		foreach (GameObject obj in stars) {
			obj.animation.Play ();
		}
	}
	
	
	[System.Serializable]
	public class TextStruct
	{
		public string english;
		public string japanese;
		public float waitTime;
	}
	
}
