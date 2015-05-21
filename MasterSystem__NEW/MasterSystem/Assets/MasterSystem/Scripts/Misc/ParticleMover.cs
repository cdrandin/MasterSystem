using UnityEngine;
using System.Collections;

public class ParticleMover : MonoBehaviour 
{
	private Transform _transform;
	[Range (0.01f, 1.0f)]
	public float speed;
	public Transform start;
	public Transform end;

	// Use this for initialization
	void Start () 
	{
		_transform = this.transform;
		_transform.position = start.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		_transform.position = Vector3.Lerp(start.position, end.position, Mathf.PingPong(Time.time * speed, 1.0f));
	}
}
