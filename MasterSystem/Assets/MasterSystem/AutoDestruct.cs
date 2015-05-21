using UnityEngine;
using System.Collections;

public class AutoDestruct : MonoBehaviour 
{
	[Range (0f, float.MaxValue)]
	public float live_timer; // went current object should be destroyed
//	public float? live_timer; // went current object should be destroyed
	private bool _init;

	// Use this for initialization
	void Start () {
//		live_timer = null;
//		live_timer = 1f;
		_init = false;
		StartCoroutine(DelayUpdate());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void MyUpdate()
	{
//		if(live_timer != null && !_init)
		if(!_init)
		{
			_init = true;
			DelayAction.instance.Delay(()=>
			{
				Destroy(this.gameObject);
			}, (float)(live_timer));
		}
	}

	IEnumerator DelayUpdate()
	{
		while(true)
		{
			MyUpdate();
			yield return new WaitForSeconds(1f/20f);
		}
	}
}
