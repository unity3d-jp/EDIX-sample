using UnityEngine;
using System.Collections;

public class TextUpdater : MonoBehaviour {
	
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
	
	public void Update()
	{
		if( Input.GetKeyDown(KeyCode.Mouse0))
		{
			StopAllCoroutines();
			StartCoroutine(Read());
		}
	}
	
	IEnumerator Read()
	{
		yield return new WaitForSeconds(seekTime);
		
		AudioSource.PlayClipAtPoint(voice, Vector3.zero);
		
		foreach( TextStruct text in textList)
		{
			english.text = text.english;
			japaneseb.text = text.japanese;
			yield return new WaitForSeconds(text.waitTime);
		
		}
		
			foreach(GameObject obj in stars)
			{
				obj.animation.Play();
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
