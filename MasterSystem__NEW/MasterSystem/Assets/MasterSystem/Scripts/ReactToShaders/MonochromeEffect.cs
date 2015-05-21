using UnityEngine;
using System.Collections;

public class MonochromeEffect : MonoBehaviour 
{
	private Renderer _renderer;
	private bool _playing;

	void Awake ()
	{
		_renderer = this.GetComponent<Renderer>();
	}

	// Use this for initialization
	void Start () 
	{
		if(_renderer != null)
		{
			if(!_renderer.material.HasProperty("_DimIntensity"))
			{
				Debug.LogWarning(string.Format("{0} doesn't have monochrome shader.", this.gameObject.name));
				_renderer = null;
			}
			else
			{
				_renderer.material.SetFloat("_DimIntensity", 1.0f);
			}
		}
		else
		{
			Debug.LogWarning("Failed to find renderer");
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
//		if(Input.GetKeyDown(KeyCode.Space))
//		{
//			PlayCooldownEffect (3.0f, 5.0f);
//		}
	}

	public void Dim(int total_cd, int remaining_cd)
	{
		if(_renderer.material.HasProperty("_DimIntensity"))
		{
			_renderer.material.SetFloat("_DimIntensity", (3.5f / total_cd) * (remaining_cd + 1));
		}
	}

	public void UnDim()
	{
		if(_renderer.material.HasProperty("_DimIntensity"))
		{
			_renderer.material.SetFloat("_DimIntensity", 1f);
		}
	}

	public void ModIntensity(float amount)
	{
		if(_renderer.material.HasProperty("_DimIntensity"))
		{
			_renderer.material.SetFloat("_DimIntensity", Mathf.Clamp(amount, 0f, 20f));
		}
	}

	public void PlayCooldownEffect(float time, float dim_out_value = 2.2f)
	{
		if(_playing || _renderer == null)
			return;

		StartCoroutine(CooldownEffect(time, dim_out_value, 0.5f));
	}

	public IEnumerator CooldownEffect(float time, float dim_out_value, float shine_value)
	{
		_playing = true;

		// Start by instantly fading to darkness
		float initial = _renderer.material.GetFloat("_DimIntensity");
		_renderer.material.SetFloat("_DimIntensity", dim_out_value);

		float start_time = Time.time;
		float v          = 0.0f;
		float t          = 0.0f;

		float glow_effect_leftover_time = 0.75f;

		// Overtime brighten up
		while((Time.time - start_time) <= time - glow_effect_leftover_time)
		{
			v = Mathf.Lerp(dim_out_value, initial, t);
			t+= Time.deltaTime/time; // equal distribution of the value over a duration "time"

			_renderer.material.SetFloat("_DimIntensity", v);
			yield return null;
		}

		start_time = Time.time;
		v 		   = 0.0f;
		t		   = 0.0f;

		// Then start with glow effect
		while((Time.time - start_time) <= glow_effect_leftover_time/2.0f)
		{
			v = Mathf.Lerp(initial, shine_value, t);
			t+= Time.deltaTime/time; // equal distribution of the value over a duration "time"
			
			_renderer.material.SetFloat("_DimIntensity", v);
			yield return null;
		}

		yield return new WaitForSeconds(0.1f);

		start_time = Time.time;
		v 		   = 0.0f;
		t		   = 0.0f;

		// Fade down
		while((Time.time - start_time) <= glow_effect_leftover_time/2.0f)
		{
			v = Mathf.Lerp(shine_value, initial, t);
			t+= Time.deltaTime/time; // equal distribution of the value over a duration "time"
			
			_renderer.material.SetFloat("_DimIntensity", v);
			yield return null;
		}

		_renderer.material.SetFloat("_DimIntensity", initial);

		_playing = false;
		yield return null;
	}
}
