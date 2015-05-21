using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum WEAPON_ORIENTATION
{
	CURRENT,
	NEXT_TIER,
	CRAFT
}
//
// Not a good way of doing it, but it is the simplest
//
[RequireComponent (typeof(Image))]
public class BuilderUIUpdateItemImage : MonoBehaviour 
{
	public WEAPON_ORIENTATION _which_weapon;

	private BuilderMenu _builder_menu;
	private float DELAYED_TIME = 1f/10f;
	private Coroutine coroutine;

	private Image _item_image;

	// Use this for initialization
	void Start () 
	{
		_builder_menu = Camera.main.GetComponent<BuilderMenu>();
		if(_builder_menu == null)
		{
			Debug.LogError("Missing builder menu component in the scene");
		}

		_item_image = this.GetComponent<Image>();

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

	Sprite PortraitToUse()
	{
		Sprite to_use = null;

		switch(_which_weapon)
		{
		case WEAPON_ORIENTATION.CURRENT:
			to_use = _builder_menu.focused_item.current_portrait;
			break;
		case WEAPON_ORIENTATION.CRAFT:
			try
			{
				to_use = _builder_menu.focused_item.next_item_progress.current_portrait;
			}
			catch(System.Exception)
			{
				Debug.LogWarning("Make sure to provide the current item progression object with its next craftable progression item");
				to_use = null;
			}

			break;
		default:
			to_use = null;
			break;
		}

		return to_use;
	}

	void MyUpdate()
	{
		if(_builder_menu != null)
		{
			_item_image.sprite = PortraitToUse();
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
