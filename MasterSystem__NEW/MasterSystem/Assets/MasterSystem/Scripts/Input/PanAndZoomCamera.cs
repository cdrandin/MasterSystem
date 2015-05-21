using UnityEngine;
using System.Collections;

public class PanAndZoomCamera : MonoBehaviour 
{
	public InputType input_type;

	[Range (1.0f, 15.0f)]
	public float mouse_speed_modifier;

	public float scroll_speed;
	public bool lock_horizontal;
	public bool lock_vertical;

	[Range (0.01f, 0.2f)]
	public float pinch_zoom_speed;
	public bool lock_pinch;

	[Range (3.0f, 5.0f)]
	public float zoom_in_max;

	[Range (5.0f, 10.0f)]
	public float zoom_out_max;

	private Camera _camera;
	private Vector3 _camera_position;
	private float _current_pinch_distance;
	private float _previous_pinch_distance;

	// MainGUI object
	private GameObject _focused_object;

	// Bounds to keep track of for the MainGUI
	private float _leftBound;
	private float _rightBound;
	private float _bottomBound;
	private float _topBound;

	// Use this for initialization
	void Start () 
	{
		_camera = this.GetComponent<Camera>();
		_camera.orthographic = true;

		InputWrapper.OverrideInputType(input_type);

		if(_focused_object == null)
		{
			Ray ray = new Ray(_camera_position, _camera.transform.forward);
			RaycastHit[] hits;
			hits = Physics.RaycastAll(ray);
			
			foreach(RaycastHit hit in hits)
			{
				if(hit.transform.gameObject.layer == LayerMask.NameToLayer("MainUI"))
				{
					_focused_object = hit.transform.gameObject;
				}
			}

			if(_focused_object != null)
			{
				ConfigureCameraBounds();
			}
			else
			{
				RaycastHit2D[] hits2D = Physics2D.RaycastAll(_camera_position, _camera.transform.forward);
				foreach(RaycastHit2D hit2D in hits2D)
				{
					if(hit2D.transform.gameObject.layer == LayerMask.NameToLayer("MainUI"))
					{
						_focused_object = hit2D.transform.gameObject;
					}
				}

				if(_focused_object != null)
				{
					ConfigureCameraBounds();
				}
			}
		}
	}

	// Update is called once per frame
	void Update () 
	{
		if(_camera.gameObject.activeSelf)
		{
			// Camera panning
			if(InputWrapper.GetInputIsMoving())
			{
				_camera_position = _camera.transform.position;

				Vector2 delta = InputWrapper.GetInputDeltaPosition();

				if(InputWrapper.isMouse())
				{
					delta *= mouse_speed_modifier;
				}

				if(!lock_horizontal)
				{
					_camera_position.x = Mathf.Clamp(_camera_position.x  - delta.x * Time.deltaTime * scroll_speed, _leftBound, _rightBound);
				}

				if(!lock_vertical)
				{
					_camera_position.y = Mathf.Clamp(_camera_position.y  - delta.y * Time.deltaTime * scroll_speed, _bottomBound, _topBound);
				}

				_camera.transform.position = _camera_position;
			}

			// Camera zoom in/out
			else if(!lock_pinch)
			{
				if(InputWrapper.isMouse())
				{
					if (GetComponent<Camera>().orthographic)
					{
						// ... change the orthographic size based on the change in distance between the touches.
						GetComponent<Camera>().orthographicSize += -1*InputWrapper.GetInputScrollDelta() * mouse_speed_modifier * pinch_zoom_speed;
						
						// Make sure the orthographic size never drops below zero.
						//camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, zoom_in_max, zoom_in_max);
						GetComponent<Camera>().orthographicSize = Mathf.Max(GetComponent<Camera>().orthographicSize, zoom_in_max);
						GetComponent<Camera>().orthographicSize = Mathf.Min(GetComponent<Camera>().orthographicSize, zoom_out_max);
					}
					else
					{
						// Otherwise change the field of view based on the change in distance between the touches.
						//camera.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;
						
						// Clamp the field of view to make sure it's between 0 and 180.
						//camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 0.1f, 179.9f);
					}
				}
				else if(InputWrapper.isTouch())
				{
					if(Input.touchCount == 2)
					{
						float deltaMagnitudeDiff = InputWrapper.GetInputTouchPinchDelta();
						
						// If the camera is orthographic...
						if (GetComponent<Camera>().orthographic)
						{
							// ... change the orthographic size based on the change in distance between the touches.
							GetComponent<Camera>().orthographicSize += deltaMagnitudeDiff * pinch_zoom_speed;
							
							// Make sure the orthographic size never drops below zero.
							//camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, zoom_in_max, zoom_in_max);
							GetComponent<Camera>().orthographicSize = Mathf.Max(GetComponent<Camera>().orthographicSize, zoom_in_max);
							GetComponent<Camera>().orthographicSize = Mathf.Min(GetComponent<Camera>().orthographicSize, zoom_out_max);
						}
						else
						{
							// Otherwise change the field of view based on the change in distance between the touches.
							//camera.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;
							
							// Clamp the field of view to make sure it's between 0 and 180.
							//camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 0.1f, 179.9f);
						}
					}
				}

				ConfigureCameraBounds();
			} // End camera zoom
		}
	}

	void FixedUpdate()
	{
		if(input_type != InputType.UNDEFINED)
			InputWrapper.OverrideInputType(input_type);
		else
			InputWrapper.ConfigureInputType();
		input_type = InputWrapper.GetInputType();
	}

	void ConfigureCameraBounds()
	{
		float vertExtent = Camera.main.GetComponent<Camera>().orthographicSize;  
		float horzExtent = vertExtent * _camera.aspect;
		Bounds gui_bounds;
		if(_focused_object.GetComponent<Collider>() != null)
		{
			gui_bounds = _focused_object.GetComponent<Collider>().bounds;
		}

		if(_focused_object.GetComponent<Collider2D>() != null)
		{
			gui_bounds = _focused_object.GetComponent<Collider2D>().bounds;
		}

		// Half since we start from center, get the corners
		_leftBound   = (float)(horzExtent - gui_bounds.size.x / 2.0f);
		_rightBound  = (float)(gui_bounds.size.x / 2.0f - horzExtent);
		_bottomBound = (float)(vertExtent - gui_bounds.size.y / 2.0f);
		_topBound    = (float)(gui_bounds.size.y  / 2.0f - vertExtent);
	}

	void Reset()
	{
		input_type       = InputType.TOUCH_INPUT;
		scroll_speed 	 = 1.0f;
		lock_horizontal  = false;
		lock_vertical    = false;
		pinch_zoom_speed = 0.05f;
		lock_pinch       = false;
		zoom_in_max      = 2.0f;
		zoom_out_max 	 = 7.5f;
		mouse_speed_modifier = 15.0f;
	}
}
