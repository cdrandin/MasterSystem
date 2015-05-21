using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusicPlayer : MonoBehaviour
{
	public AudioClip engineStartClip;
	public AudioClip engineLoopClip;

	void Start()
	{
		GetComponent<AudioSource> ().loop = true;
		StartCoroutine(playEngineSound());
	}
	
	IEnumerator playEngineSound()
	{
		GetComponent<AudioSource>().clip = engineStartClip;
		GetComponent<AudioSource>().Play();
		yield return new WaitForSeconds(GetComponent<AudioSource>().clip.length);
		GetComponent<AudioSource>().clip = engineLoopClip;
		GetComponent<AudioSource>().Play();
	}
}