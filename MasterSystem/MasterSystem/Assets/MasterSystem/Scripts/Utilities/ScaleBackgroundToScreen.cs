using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ScaleBackgroundToScreen : MonoBehaviour 
{
	public SpriteRenderer scale_background_sprite;
	private Vector2 _prev;
	public bool debug;

	// Use this for initialization
	void Start () 
	{
		Screen.autorotateToLandscapeLeft = false;
		Screen.autorotateToLandscapeRight = false;
		Screen.autorotateToPortrait = true;
		Screen.autorotateToPortraitUpsideDown = true;
		Screen.orientation = ScreenOrientation.AutoRotation;

		#if UNITY_ANDROID || UNITY_IPHONE
		ResizeSpriteToScreen();
		#endif
	}

	void Update()
	{
		#if UNITY_EDITOR
		if(debug)
		{
			if(scale_background_sprite == null)
			{
				Debug.Log("Missing SpriteRenderer. If you do not want this then you probably shouldn't be attaching this");
				return;
			}
			
			if(_prev != new Vector2(Screen.width, Screen.height))
			{
				ResizeSpriteToScreen();
				Debug.Log(string.Format("Resizing: {0}", scale_background_sprite.name));
			}
		}
		else
		{
			scale_background_sprite.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		}
		#endif
	}

	void ResizeSpriteToScreen()
	{
		if (scale_background_sprite == null) return;
		
		scale_background_sprite.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		
		float width = scale_background_sprite.sprite.bounds.size.x;
		float height = scale_background_sprite.sprite.bounds.size.y;
		
		float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
		float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
		
		scale_background_sprite.transform.localScale = new Vector3(worldScreenWidth / width, worldScreenHeight / height, 0.0f);
		//scale_background_sprite.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, scale_background_sprite.transform.position.z);
		scale_background_sprite.transform.position = new Vector3(0.0f,this.GetComponent<Camera>().transform.position.y,0.0f); // 1 just works for everything
		_prev = new Vector2(Screen.width, Screen.height);
	}
}
