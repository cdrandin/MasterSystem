using UnityEngine;
using System.Collections;

public class MagicMissleAnimation : MonoBehaviour, IPlayStop
{
	public GameObject missle;

	[Range(.1f, 50f)]
	public float speed;

	[Range(1, 5)]
	public int number_of_missiles = 3;

	[Range(.1f, 5f)]
	public float delay_per_missle;

//	[Range(.1f, 20f)]
//	public float elapse_time;

	public Transform[] end_positions;

	private bool _begin;
	private int _done = 0;
	private System.Action[] _impact_event;

	// Use this for initialization
	void Start () 
	{
		_begin = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.Space))
		{
			Play();
		}
	}

	public void Setup(int number_of_missiles, Transform[] end_positions, System.Action[] on_impact_event)
	{
		if(_begin) return;

		this.number_of_missiles = number_of_missiles;
		this.end_positions      = end_positions;
		this._impact_event 	    = on_impact_event;
	}

	public void Play()
	{
		if(!_begin && end_positions != null)
		{
			DebugUtils.Assert(number_of_missiles == end_positions.Length, "Requires same number of positions as the number of missles");

			missle = Resources.Load<GameObject>("Effects/MagicMissle");
			missle.SetActive(false);

			if(missle)
			{
				for(int i=0;i<number_of_missiles;++i)
				{
					int copy_i = i;
					DelayAction.instance.Delay(
					()=>
					{
						UseMissle(copy_i);
					}, delay_per_missle * (copy_i + 1));
				}
			}
			_begin = true;
		}
	}

	void UseMissle(int i)
	{
		GameObject go = Instantiate(missle) as GameObject;
		go.transform.position = this.transform.position;

		float start_time = 0f;

		DelayAction.instance.DelayInf(
		()=>
		{
			if(!go.activeSelf)
			{
				go.SetActive(true);
				start_time = Time.time;
			}
			go.transform.position = Vector3.MoveTowards(go.transform.position, end_positions[i].position, Time.deltaTime * speed);
		},
		.0f,
		()=>
		{
			if(go.transform.position == end_positions[i].position)
			{
				Destroy(go);
				if(_impact_event != null)
				{
					_impact_event[i].Invoke();
					Instantiate(Resources.Load("CustomFX/MagicMissleImpact+Text"));
				}

				_done = i;

				// done with anim
				if(_done+1 == number_of_missiles)
				{
					_begin = false;
					Destroy(this.gameObject);
				}

				return true;
			}
			return false;
		});
	}

	public void Stop()
	{
		
	}
}
