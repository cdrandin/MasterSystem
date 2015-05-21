using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PulseImageEffect : MonoBehaviour 
{
	public Image image;
	public Color start_color;
	public Color end_color;
	[Range(0.01f,2f)]
	public float duration;

	// Use this for initialization
	void Start () {
		if(image == null)
		{
			image = this.GetComponent<Image>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		float lerp = Mathf.PingPong(Time.time, duration) / duration;
		image.color = Color.Lerp(start_color, end_color, lerp);
	}
}
