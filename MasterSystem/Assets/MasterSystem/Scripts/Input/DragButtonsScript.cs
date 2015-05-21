using UnityEngine;
using System.Collections;

public class DragButtonsScript : MonoBehaviour {


	private Vector3 originalPosition;
	private bool selected;
	private static bool globalSelected;
	private Vector2 mousePosition;
	private TrailRenderer m_trailRenderer;
	private float newZ;
	//private string[] tagList = {'Primary', 'Secondary', 'Soulshard'};
	private AudioClip _on_up;
	private AudioClip _on_down;

	void Awake()
	{
		_on_up   = Resources.Load("snap_up_button") as AudioClip;
		_on_down = Resources.Load("snap_down_button") as AudioClip;
	}

	// Use this for initialization
	void Start () {
		selected = false;
		globalSelected = false;
		originalPosition = transform.parent.position;
		newZ = transform.parent.position.z - 5 ;

		m_trailRenderer = transform.parent.gameObject.GetComponent<TrailRenderer>();
		m_trailRenderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	

		if(!globalSelected && (Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)))
		{
			if(CheckIfHit( new Vector2(Input.mousePosition.x, Input.mousePosition.y)) ){
				Camera.main.GetComponent<AudioSource>().PlayOneShot(_on_up);
				selected = true;
				globalSelected = true;
				m_trailRenderer.enabled = true;
				//print ( gameObject.name + " selected");
			}
			else{
				selected = false;
			}
		}
	
		if((Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) ) && this.selected){;

			selected = false;
			globalSelected = false;
			m_trailRenderer.enabled = false;
			transform.parent.position = originalPosition;
			Camera.main.GetComponent<AudioSource>().PlayOneShot(_on_down);
			
		}
	
		if(this.selected){
			mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			transform.parent.position = new Vector3( mousePosition.x, mousePosition.y, newZ);
		}
	}

	private bool CheckIfHit(Vector2 mousePosition){

		Ray ray = Camera.main.ScreenPointToRay(mousePosition);
		RaycastHit hit;
		
		if (Physics.Raycast(ray, out hit, 100f ))
		{
			if(hit.transform.tag == transform.tag)
				return true;
		}

		return false;

	}
}
