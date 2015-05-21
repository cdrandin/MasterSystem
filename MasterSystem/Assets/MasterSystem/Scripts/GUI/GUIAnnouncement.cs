using UnityEngine;
using System.Collections;

public class GUIAnnouncement : MonoBehaviour
{
	public int size_rate;
	public TextMesh _turn_announce;
	private bool _playing;
	private int _cur_size;

	void Awake()
	{
		Stop ();
	}

	// Update is called once per frame
	void Update ()
	{
		//*
		if(_playing)
		{
			_cur_size += size_rate;

			// own ping pong
			if(_cur_size >= 60)
			{
				size_rate *= -1;
			}
			else if(_cur_size < 1)
			{
				size_rate *= -1;
				Stop();
			}

			_turn_announce.fontSize = _cur_size;
		}
		//*/
	}

	public void PlayAnnouncementWith(string word)
	{
		Stop ();
		_playing            = true;
		_turn_announce.text = word;
		_turn_announce.color = Color.white;
	}

	void Stop()
	{
		_turn_announce.text     = "";
		_turn_announce.fontSize = _cur_size = 0;
		_playing                = false;
	}

	public void PlayAnnouncementWithTime(string word, float time, Color color)
	{
		Stop ();
		_turn_announce.text  = word;
		_turn_announce.color = color;

		StartCoroutine(Animate(time));
	}

	IEnumerator Animate(float delay)
	{
		bool animate = true;

		while(animate)
		{
			_cur_size += size_rate;
			
			// own ping pong
			if(_cur_size >= 60)
			{
				size_rate *= -1;
				animate = false;
			}
			
			_turn_announce.fontSize = _cur_size;
			yield return null;
		}

		yield return new WaitForSeconds(5.0f);
		Stop ();
	}
}
