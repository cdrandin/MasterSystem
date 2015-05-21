using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public enum BUILDER_MENU_STATES
{
	DEFAULT,
	FORGE,
	LEVEL,
	CRAFT,
	DISENCHANT,
	DONATE,
	EFFECT,
	CONFIRMATION
}

public class BuilderMenu : MonoBehaviour 
{
	public GameObject default_builder_ui;

	public Button go_back_button;

	public SpriteRenderer main_builder_background;
	public Sprite default_builder_background;
	public Sprite in_progress_builder_background;

	public Button forge_button;
	public Button disenchant_button;
	public Button donate_button;

	public Button.ButtonClickedEvent forge_on_enter;
	public Button.ButtonClickedEvent disenchant_on_enter;
	public Button.ButtonClickedEvent donate_on_enter;

	public Button forge_exit_button;
	public Button disenchant_exit_button;
	public Button donate_exit_button;

	public Button forge_crafting_button;

	// Item progression stuff
	public Text exp_gain_text_level;

	public GameObject backpack;

	private GameObject menu_effect;
	private ItemProgression _focused_item;
	public ItemProgression focused_item
	{
		get { return _focused_item; }
	}

	private List<ItemProgression> _backpack_item_progression;
	public List<ItemProgression>  backpack_item_progression
	{
		get { return _backpack_item_progression; }
	}

	void Awake()
	{
		_backpack_item_progression = new List<ItemProgression>();
		_backpack_item_progression.Add(backpack.transform.GetChild(0).GetComponent<ItemProgression>());
		_backpack_item_progression.Add(backpack.transform.GetChild(1).GetComponent<ItemProgression>());

		FocusOnItem(_backpack_item_progression[0]);
	}

	// Use this for initialization
	void Start () 
	{
		main_builder_background.sprite = default_builder_background;

		if(forge_button != null)
		{
			// On enter button event
			forge_button.onClick.AddListener(()=>
			                                 {
				StartTransition(BUILDER_MENU_STATES.FORGE);
				forge_on_enter.Invoke();
			});
		}

		if(disenchant_button != null)
		{
			disenchant_button.onClick.AddListener(()=>
			                                      {
				StartTransition(BUILDER_MENU_STATES.DISENCHANT);
				disenchant_on_enter.Invoke();
			});
		}

		if(donate_button != null)
		{
			donate_button.onClick.AddListener(()=>
			                                  {
				StartTransition(BUILDER_MENU_STATES.DONATE);
				donate_on_enter.Invoke();
			});
		}

		if(forge_exit_button != null)
		{
			// On exit button event
			forge_exit_button.onClick.AddListener(()=>
			                                      {
				StartTransition(BUILDER_MENU_STATES.DEFAULT);
				//forge_on_exit.Invoke();
			});
		}

		if(disenchant_exit_button != null)
		{
			disenchant_exit_button.onClick.AddListener(()=>
			                                           {
				StartTransition(BUILDER_MENU_STATES.DEFAULT);
				//disenchant_on_exit.Invoke();
			});
		}

		if(donate_exit_button != null)
		{
			donate_exit_button.onClick.AddListener(()=>
			                                       {
				StartTransition(BUILDER_MENU_STATES.DEFAULT);
				//	donate_on_exit.Invoke();
			});
		}

//		for(int i =0; i < _backpack_item_progression.Count; ++i)
//		{
//			item_scroll_images_forge[i].sprite = _backpack_item_progression[i].current_scroll_portrait;
//			item_scroll_images_forge[i].preserveAspect = true;
//			item_scroll_images_forge[i].SetNativeSize();
//
//			item_scroll_images_disenchant[i].sprite = _backpack_item_progression[i].current_scroll_portrait;
//			item_scroll_images_disenchant[i].preserveAspect = true;
//			item_scroll_images_disenchant[i].SetNativeSize();
//		}
	}
	
	// Update is called once per frame
	void Update () {
	}

	// item progression related functions
	void VisualUpdate()
	{
		if(_focused_item != null)
		{
			if(exp_gain_text_level != null)
			{
				exp_gain_text_level.text = string.Format("+ {0}/{1}", _focused_item.projected_exp.ToString("N0"), _focused_item.MAX_EXP_PER_LEVEL.ToString("N0"));
			}
//
//			for(int i =0; i < _backpack_item_progression.Count; ++i)
//			{
//				item_scroll_images_forge[i].sprite = _backpack_item_progression[i].current_scroll_portrait;
//				item_scroll_images_forge[i].preserveAspect = true;
//				item_scroll_images_forge[i].SetNativeSize();
//				
//				item_scroll_images_disenchant[i].sprite = _backpack_item_progression[i].current_scroll_portrait;
//				item_scroll_images_disenchant[i].preserveAspect = true;
//				item_scroll_images_disenchant[i].SetNativeSize();
//			}

			if(_focused_item.transformable)
			{
				if(_focused_item.ready_to_transform)
				{
					forge_crafting_button.interactable = true;
					forge_crafting_button.transform.GetChild(0).GetComponent<Text>().text = "Craft";
				}
				else
				{
					forge_crafting_button.interactable = false;
					forge_crafting_button.transform.GetChild(0).GetComponent<Text>().text = "Not ready";
				}
			}
			else
			{
				forge_crafting_button.interactable = false;
				forge_crafting_button.transform.GetChild(0).GetComponent<Text>().text = "Does not transform";
			}
		}
	}

	public void FocusOnItem(ItemProgression ip)
	{
		_focused_item = ip;
		VisualUpdate();
	}

	public void NextWeapon(int c)
	{
		int i = _backpack_item_progression.IndexOf(_focused_item);
		i = Mathf.Clamp(i + c, 0, _backpack_item_progression.Count - 1);
		FocusOnItem(_backpack_item_progression[i]);
	}

	public void AddExpToFocusItem(int amount)
	{
		_focused_item.AddExp(amount);
		VisualUpdate();
	}

	public void SaveChangesToFocusItem()
	{
		// Before the save. Lets check if we leveled
		if(_focused_item.leveled)
		{
			StartTransition_EFFECT();
		}

		_focused_item.SaveChanges();
		VisualUpdate();
	}

	public void UndoChangesToFocusItem()
	{
		_focused_item.UndoChanges();
		VisualUpdate();
	}

	public void SetMenuEffect(GameObject obj)
	{
		if(menu_effect != null)
			menu_effect.SetActive(false);

		menu_effect = obj;
		menu_effect.SetActive(false);
	}

	public void CraftToNextItem()
	{
		if(_focused_item.ready_to_transform)
		{
			ItemProgression prev = _focused_item;
			int focus_index = _backpack_item_progression.FindIndex(v => v == prev);

			_backpack_item_progression[focus_index] = prev.next_item_progress;
			_backpack_item_progression[focus_index].prev_item_progress = prev;
			FocusOnItem(_backpack_item_progression[focus_index]);
		}
	}
	// End Item progression methods



	public void StartTransition_Default()
	{
		StartTransition(BUILDER_MENU_STATES.DEFAULT);
	}

	public void StartTransition_FORGE()
	{
		StartTransition(BUILDER_MENU_STATES.FORGE);
	}
	public void StartTransition_LEVEL()
	{
		StartTransition(BUILDER_MENU_STATES.LEVEL);
	}
	public void StartTransition_CRAFT()
	{
		StartTransition(BUILDER_MENU_STATES.CRAFT);
	}
	public void StartTransition_DISENCHANT()
	{
		StartTransition(BUILDER_MENU_STATES.DISENCHANT);
	}
	public void StartTransition_DONATE()
	{
		StartTransition(BUILDER_MENU_STATES.DONATE);
	}
	public void StartTransition_EFFECT()
	{
		StartTransition(BUILDER_MENU_STATES.EFFECT);
	}
	public void StartTransition_CONFIRMATION()
	{
		StartTransition(BUILDER_MENU_STATES.CONFIRMATION);
	}

	void StartTransition(BUILDER_MENU_STATES state)
	{
		if(state != BUILDER_MENU_STATES.DEFAULT)
		{
			SetInProgress();

			if(state == BUILDER_MENU_STATES.EFFECT)
			{
				menu_effect.SetActive(true);
				DelayAction.instance.Delay(()=> 
				{ 
					Button button = menu_effect.GetComponent<Button>();
					if(button != null)
					{
						button.onClick.Invoke();
					}

					menu_effect.SetActive(false); 
				}, 1.5f);
			}
		}
		else
		{
			SetDefault();
		}
	}

	void SetDefault()
	{
		default_builder_ui.SetActive(true);
		main_builder_background.sprite = default_builder_background;

		if(menu_effect != null)
			menu_effect.SetActive(false);
	}

	void SetInProgress()
	{
		if(default_builder_ui != null)
			default_builder_ui.SetActive(false);

		main_builder_background.sprite = in_progress_builder_background;
	}
}
