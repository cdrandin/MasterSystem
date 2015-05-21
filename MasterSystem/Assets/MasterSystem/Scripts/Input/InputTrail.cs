using UnityEngine;
using System.Collections;

public class InputTrail : MonoBehaviour {

	private Vector3 mousePosition;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		if(Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved))
		{
			transform.position = new Vector3(mousePosition.x, mousePosition.y, transform.position.z);
		}
	}
}
