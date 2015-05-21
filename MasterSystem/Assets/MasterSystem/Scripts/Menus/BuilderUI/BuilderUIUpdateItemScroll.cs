using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuilderUIUpdateItemScroll : MonoBehaviour 
{
	private BuilderMenu _builder_menu;
	private float DELAYED_TIME = 1f/10f;
	private Coroutine coroutine;

	public Image[] _item_scroll;
	private int _mid_point;

	// Use this for initialization
	void Start () 
	{
		_builder_menu = Camera.main.GetComponent<BuilderMenu>();
		if(_builder_menu == null)
		{
			Debug.LogError("Missing builder menu component in the scene");
		}

		if(_item_scroll == null)
		{
			Debug.LogError("Item scroll images are missing");
		}

		if(_item_scroll != null)
		{
			_mid_point = Mathf.FloorToInt((((float)_item_scroll.Length) - 1f)/2f);
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
			if(_item_scroll != null)
			{
				SetItemImage(_item_scroll[_mid_point], _builder_menu.focused_item);

				int focus_index = _builder_menu.backpack_item_progression.FindIndex(v => v == _builder_menu.focused_item);
				int index = focus_index + 1;

				// from focus index and forward
				for(int i = _mid_point + 1; i < _item_scroll.Length; ++i)
				{
					try
					{
						_item_scroll[i].enabled = true;
						SetItemImage(_item_scroll[i], _builder_menu.backpack_item_progression[index]);
					}
					catch
					{
						_item_scroll[i].enabled = false;
					}
					++index;
				}

				index = focus_index - 1;

				// from focus index and backwards
				for(int i = _mid_point - 1; i >= 0; --i)
				{
					try
					{
						_item_scroll[i].enabled = true;
						SetItemImage(_item_scroll[i], _builder_menu.backpack_item_progression[index]);
					}
					catch
					{
						_item_scroll[i].enabled = false;
					}
					--index;
				}
			}
		}
	}

	void SetItemImage(Image image, ItemProgression item)
	{
		image.sprite = item.current_scroll_portrait;
		image.SetNativeSize();
		image.preserveAspect = true;

		// Also add button, on click focus item
		Button button = image.gameObject.GetComponent<Button>();

		if(button == null)
		{
			button = image.gameObject.AddComponent<Button>();
		}
		else
		{
			button.onClick.RemoveAllListeners();
		}

		button.onClick.AddListener(()=>
		{
			_builder_menu.FocusOnItem(item);
		});
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
