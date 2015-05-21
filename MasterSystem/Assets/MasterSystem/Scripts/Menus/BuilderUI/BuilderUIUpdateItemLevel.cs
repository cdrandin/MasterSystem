using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuilderUIUpdateItemLevel : MonoBehaviour 
{
	private BuilderMenu _builder_menu;
	private float DELAYED_TIME = 1f/10f;
	private Coroutine coroutine;


	public Text level_text_formatted;
	public Image level_experience_bar;

	// Use this for initialization
	void Start () {
		_builder_menu = Camera.main.GetComponent<BuilderMenu>();
		if(_builder_menu == null)
		{
			Debug.LogError("Missing builder menu component in the scene");
		}

		if(level_experience_bar == null)
		{
			Debug.LogError("Missing level experience bar image for builder UI");
		}

		if(level_text_formatted == null)
		{
			Debug.LogError("Missing level text formatted text for builder UI");
		}
		
		MyUpdate ();
	}
	
	void OnEnable()
	{
		MyUpdate ();
		
		if(coroutine == null)
		{
			coroutine = StartCoroutine(DelayedUpdate());
		}
	}
	
	void OnDisable()
	{
		if(coroutine != null)
		{
			StopCoroutine(coroutine);
			coroutine = null;
		}
	}
	void MyUpdate()
	{
		if(_builder_menu != null)
		{
			if(level_experience_bar != null)
			{
				level_experience_bar.fillAmount = _builder_menu.focused_item.normalized_current_exp;
			}

			if(level_text_formatted != null)
			{
				level_text_formatted.text = string.Format("{0}/{1}", _builder_menu.focused_item.projected_lvl, _builder_menu.focused_item.item_max_craft_level);
			}
		}
	}
	
	IEnumerator DelayedUpdate()
	{
		while(true)
		{
			MyUpdate();
			yield return new WaitForSeconds(DELAYED_TIME);
		}
	}
}
