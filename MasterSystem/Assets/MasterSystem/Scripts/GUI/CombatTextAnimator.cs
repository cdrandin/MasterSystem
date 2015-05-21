using UnityEngine;
using System.Collections;

public class CombatTextAnimator : MonoBehaviour {
	
	private static CombatTextAnimator _instance;
	public static CombatTextAnimator instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = Camera.main.gameObject.AddComponent<CombatTextAnimator>();
				_instance.Setup();
			}
			
			return _instance;
		}
	}

	private GameObject _combat_text_object;
	private TextMesh _combat_text_mesh;

	private void Setup()
	{
		// Some checks
		_combat_text_object = GameObject.FindGameObjectWithTag("CombatText");
		if(_combat_text_object == null)
		{
			Debug.LogWarning("Missing combat text object, damage text won't display");
			return;
		}

		_combat_text_mesh = _combat_text_object.GetComponent<TextMesh>();
		if(_combat_text_mesh == null)
		{
			Debug.LogWarning("Missing test mesh, damage text won't display");
			return;
		}

		_combat_text_mesh.text = "";
		_combat_text_mesh.offsetZ = Camera.main.transform.position.z + 9f;
		_combat_text_mesh.color = Color.white;
	}

	public void SetText(string text, Color color, Vector3 world_position)
	{
		_combat_text_mesh.text = (System.Convert.ToInt32(text) < 1)? "" : text; //text;
		_combat_text_mesh.color = color;
		_combat_text_object.transform.position = new Vector3(world_position.x, world_position.y, world_position.z + 1.0f);

		//StopCoroutine(Transition(0.5f));
		PlayAnimation(0.5f);
	}

	public void PlayText(string text, Color color, Vector3 world_position, float fade_delay=1f)
	{
		GameObject text_obj = PoolingSystem.instance.PS_Instantiate(_combat_text_object) as GameObject;
		text_obj.transform.localScale = new Vector3(1f,1f,1f);
		text_obj.transform.position = new Vector3(world_position.x, world_position.y, world_position.z + 1.0f);
		
		TextMesh text_mesh = text_obj.GetComponent<TextMesh>();
		text_mesh.text = text;
		text_mesh.color = new Color(color.r, color.g, color.b, 1.0f);

		float alpha = 1.0f;
		DelayAction.instance.DelayInf(
		()=>
		{
			alpha -= Time.deltaTime;
			text_mesh.color = new Color(color.r, color.g, color.b, alpha);
		},
		fade_delay,
		()=>
		{
			if(alpha > 0.0f)
			{
				text_mesh.color = new Color(color.r, color.g, color.b, 0);
				PoolingSystem.instance.PS_Destroy(text_obj);
				return true;
			}

			return false;
		});
	}

	public void PlayAnimation(float fade_delay)
	{
		StartCoroutine(Transition(fade_delay));
	}

	IEnumerator Transition(float delay)
	{
		_combat_text_mesh.color = new Color(_combat_text_mesh.color.r, _combat_text_mesh.color.g, _combat_text_mesh.color.b, 1.0f);
		yield return new WaitForSeconds(delay);

		float alpha = 1.0f;
		while(alpha > 0.0f)
		{
			alpha -= Time.deltaTime;
			_combat_text_mesh.color = new Color(_combat_text_mesh.color.r, _combat_text_mesh.color.g, _combat_text_mesh.color.b, alpha);
			yield return null;
		}
		_combat_text_mesh.color = new Color(_combat_text_mesh.color.r, _combat_text_mesh.color.g, _combat_text_mesh.color.b, 0);
	}
}
