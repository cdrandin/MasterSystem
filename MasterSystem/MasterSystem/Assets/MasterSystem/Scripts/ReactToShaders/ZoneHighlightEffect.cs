using UnityEngine;
using System.Collections;

public class ZoneHighlightEffect : MonoBehaviour 
{
	public string shine_on_zone_with_tag;

	[Range (0.25f, 0.6f)]
	public float max_glow;

	[Range (0.01f, 0.25f)]
	public float min_glow;

	private Renderer _renderer;
	private bool _isPlaying;

	// Use this for initialization
	void Start ()
	{
		_renderer  = null;
		_isPlaying = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate()
	{
		if(shine_on_zone_with_tag != "")
		{
			Vector2 screen_position = InputWrapper.GetInputScreenPosition();
			Ray ray = Camera.main.ScreenPointToRay(new Vector3(screen_position.x, screen_position.y, Camera.main.transform.position.z));
			Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);

			// Check if we hit a valid UI element "zone"
			RaycastHit[] hits = Physics.RaycastAll(ray);
			foreach(RaycastHit hit in hits)
			{
				// Only light up appropriate one
				if(hit.transform.gameObject.layer == LayerMask.NameToLayer("ZoneUI") && hit.transform.tag == shine_on_zone_with_tag)
				{
					_renderer = hit.transform.GetComponent<Renderer>();

					// Get focus renderer
					if(_renderer != null)
					{
						// Valid UI element with proper shader on the material. Perform animation.
						if(_renderer.material.HasProperty("_RimPower"))
						{
							if(!_isPlaying)
							{
								// Play animation
								StartCoroutine(ZoneGlowingAnimation());
							}
						}
						else
						{
							_renderer = null;
						}
					}
					break; // Only care about first one
				}
			
				// Lose focus when cursor is NOT over the zone
				else
				{
					_renderer = null;
				}
			}
		}
	}

	IEnumerator ZoneGlowingAnimation()
	{
		_isPlaying = true;
		float original_rim_power = _renderer.material.GetFloat("_RimPower");
		Renderer temp = _renderer;
		_renderer.material.SetFloat("_RimPower", min_glow);

		while(_renderer != null)
		{
			_renderer.material.SetFloat("_RimPower", Mathf.Clamp(Mathf.PingPong(Time.time, max_glow+min_glow), min_glow, max_glow));
			yield return null;
		}

		temp.material.SetFloat("_RimPower", original_rim_power);

		_isPlaying = false;
		yield return null;
	}
}
