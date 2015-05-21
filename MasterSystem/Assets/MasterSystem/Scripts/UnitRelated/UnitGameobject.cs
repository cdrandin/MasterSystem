using UnityEngine;
using System.Collections;

public class UnitGameobject : MonoBehaviour 
{
	[HideInInspector]
	public UnitEntity _unit_entity;

	public Texture2D large_portrait;
	public Texture2D small_portrait;

	public TextMesh _hp_text;
	public TextMesh _power_text;
	public TextMesh _armor_text;

	public GameObject primary_item_animation;
	public AudioClip primary_sound_effect;
	public TextMesh primary_cooldown_timer_textmesh;
	public TextMesh primary1_cooldown_timer_textmesh;
	public MonochromeEffect _primary_monochrome_effect;

	public GameObject secondary_item_animation;
	public AudioClip secondary_sound_effect;
	public TextMesh secondary_cooldown_timer_textmesh;
	public TextMesh secondary1_cooldown_timer_textmesh;
	public MonochromeEffect _secondary_monochrome_effect;
	
	public GameObject soulshard_animation;
	public AudioClip soulshard_sound_effect;
	public TextMesh soulshard_cooldown_timer_textmesh;
	public MonochromeEffect _soulshard_monochrome_effect;

	private bool _death_animation = false;

	// Use this for initialization
	void Start () 
	{
		if(_unit_entity == null)
		{
			Debug.LogWarning("Missing unit entity object. FAILING");
			return;
		}

		// Set audio clips and animations
		// NOTICE:
		// If missing it could be the following:
		//	- Unit does not have an item equipped
		//  - Sound file does not exist

		//  Below are unlikely scenarios for a working unit (this will only happen if unit's don't exist already
		//  - No reference to _unit_entity
		//  - No reference to primary/secondary/soul
		//  - No reference to item
		try
		{
			_unit_entity.primary.item.SetSoundEffectAudio(_unit_entity.primary.item.on_use_sound_effect);
		}
		catch(System.Exception e)
		{
			Debug.LogWarning(string.Format("{0} Missing sound for primary: {1}", _unit_entity.base_entity.name, e));
		}

		try
		{
			_unit_entity.secondary.item.SetSoundEffectAudio(secondary_sound_effect);
		}
		catch(System.Exception e)
		{
			Debug.LogWarning(string.Format("{0} Missing sound for secondary: {1}", _unit_entity.base_entity.name, e));
		}

		try
		{
			_unit_entity.soul.item.SetSoundEffectAudio(soulshard_sound_effect);
		}
		catch(System.Exception e)
		{
			Debug.LogWarning(string.Format("{0} Missing sound for soulshard: {1}", _unit_entity.base_entity.name, e));
		}

		try
		{
			_unit_entity.primary.item.SetAnimationEffect(primary_item_animation.GetComponent<Animator>());
		}
		catch(System.Exception e)
		{
			Debug.LogWarning(string.Format("{0} Missing animation for primary: {1}", _unit_entity.base_entity.name, e));
		}

		try
		{
			_unit_entity.secondary.item.SetAnimationEffect(secondary_item_animation.GetComponent<Animator>());
		}
		catch(System.Exception e)
		{
			Debug.LogWarning(string.Format("{0} Missing animation for secondary: {1}", _unit_entity.base_entity.name, e));
		}

		try
		{
			_unit_entity.soul.item.SetAnimationEffect(soulshard_animation.GetComponent<Animator>());
		}
		catch(System.Exception e)
		{
			Debug.LogWarning(string.Format("{0} Missing animation for soulshard: {1}", _unit_entity.base_entity.name, e));
		}
	
		// Timed combat start up dependencies
		if(Applications.type== COMBAT_TYPE.TIMED)
		{
			if(primary_cooldown_timer_textmesh == null)
			{
				try
				{
					primary_cooldown_timer_textmesh = this.transform.Find("PrimaryRenderer").gameObject.GetComponentInChildren<TextMesh>();
				}
				catch(System.Exception e)
				{
					Debug.LogWarning(string.Format("Ignore(It only misses once): {0}.", e));
				}
			}

			if(secondary_cooldown_timer_textmesh == null)
			{
				try
				{
					secondary_cooldown_timer_textmesh = this.transform.Find("SecondaryRenderer").gameObject.GetComponentInChildren<TextMesh>();
				}
				catch(System.Exception e)
				{
					Debug.LogWarning(string.Format("Ignore(It only misses once): {0}.", e));
				}
			}

			if(soulshard_cooldown_timer_textmesh == null)
			{
				try
				{
					soulshard_cooldown_timer_textmesh = this.transform.Find("SoulShardRenderer").gameObject.GetComponentInChildren<TextMesh>();
				}
				catch(System.Exception e)
				{
					Debug.LogWarning(string.Format("Ignore(It only misses once): {0}.", e));
				}
			}

			try
			{
				// Empty strings
				primary_cooldown_timer_textmesh.text = "";
				secondary_cooldown_timer_textmesh.text = "";
				soulshard_cooldown_timer_textmesh.text = "";
			}
			catch(System.Exception e)
			{
				Debug.LogWarning(string.Format("Ignore(It only misses once): {0}.", e));
			}

			// Handle monochrome effects
			if(_primary_monochrome_effect == null)
			{
				try
				{
					_primary_monochrome_effect = this.transform.Find("PrimaryRenderer").gameObject.GetComponentInChildren<MonochromeEffect>();
				}
				catch(System.Exception e)
				{
					Debug.LogWarning(string.Format("Ignore(It only misses once): {0}.", e));
				}
			}

			if(_secondary_monochrome_effect == null)
			{
				try
				{
					_secondary_monochrome_effect = this.transform.Find("SecondaryRenderer").gameObject.GetComponentInChildren<MonochromeEffect>();
				}
				catch(System.Exception e)
				{
					Debug.LogWarning(string.Format("Ignore(It only misses once): {0}.", e));
				}
			}

			if(_soulshard_monochrome_effect == null)
			{
				try
				{
					_soulshard_monochrome_effect = this.transform.Find("SoulShardRenderer").gameObject.GetComponentInChildren<MonochromeEffect>();
				}
				catch(System.Exception e)
				{
					Debug.LogWarning(string.Format("Ignore(It only misses once): {0}.", e));
				}
			}
		}
	}
	
	public void Setup()
	{
		try
		{
			_unit_entity.primary.item.SetSoundEffectAudio(Resources.Load(_unit_entity.primary.item.on_use_sound_effect_resource_path) as AudioClip);
		}
		catch(System.Exception e)
		{
			Debug.LogWarning(string.Format("{0} Missing sound for primary: {1}", _unit_entity.base_entity.name, e));
		}

		try
		{
			_unit_entity.secondary.item.SetSoundEffectAudio(Resources.Load(_unit_entity.secondary.item.on_use_sound_effect_resource_path) as AudioClip);
		}
		catch(System.Exception e)
		{
			Debug.LogWarning(string.Format("{0} Missing sound for secondary: {1}", _unit_entity.base_entity.name, e));
		}
		
		try
		{
			_unit_entity.soul.item.SetSoundEffectAudio(Resources.Load(_unit_entity.soul.item.on_use_sound_effect_resource_path) as AudioClip);
		}
		catch(System.Exception e)
		{
			Debug.LogWarning(string.Format("{0} Missing sound for soulshard: {1}", _unit_entity.base_entity.name, e));
		}
	}

	public void UpdateUnitGameObject()
	{
		UpdatePortrait();
	}

	public void DeathAnimation(float animation_time)
	{
		if(_unit_entity.IsDead && !_death_animation)
		{
			_death_animation = true;
			float current_time = Time.time; 
			float finish_time = current_time + animation_time;
			GameObject death_skull = new GameObject("Death Skull");
			death_skull.transform.position = this.transform.position;
			death_skull.transform.localScale = new Vector3(6.5f, 6.5f, 6.5f);

			SpriteRenderer death_skull_sr = death_skull.AddComponent<SpriteRenderer>();
			death_skull_sr.sprite = Resources.Load<Sprite>("Textures/Death_Skull");
			death_skull_sr.sortingOrder = 1;

			DelayAction.instance.DelayInf(()=>{
				if(death_skull != null)
					death_skull.SetActive(!death_skull.activeSelf);
			}, .25f,
			()=>{ return Time.time > finish_time; });

			DelayAction.instance.DelayInf(()=>{ _unit_entity.unit_game_object.gameObject.GetComponent<MeshRenderer>().material.color =  Color32.Lerp(Color.white, new Color(.3f,.3f,.3f), Time.time - current_time); },
										 .1f,
										  ()=>{ return Time.time > finish_time; });

			DelayAction.instance.Delay(()=>{ _unit_entity.unit_game_object.Hide(); Destroy(death_skull); Camera.main.GetComponent<Game_Timer>().UpdateSwipePortrait(); }, animation_time);
		}
	}

	public void Hide()
	{
		this.gameObject.GetComponent<MeshRenderer>().enabled = false;
		this._hp_text.GetComponent<MeshRenderer>().enabled = false;
	}

	public void UpdatePortrait()
	{
		if(this.gameObject.layer != LayerMask.NameToLayer("Enemy"))
		{
			if(TimerBasedCombatSystem.instance.selected_unit == this._unit_entity)
			{
				this.gameObject.GetComponent<MeshRenderer>().material.mainTexture = this.large_portrait;
			}
			else
			{
				this.gameObject.GetComponent<MeshRenderer>().material.mainTexture = this.small_portrait;
			}
		}
	}

	public void UpdateHP()
	{
		// HP text object available
		if(_hp_text != null)
		{
			_hp_text.text = _unit_entity.base_entity.hp.ToString();
		}
	}

	public void UpdateGUI()
	{
		// HP text object available
		if(_hp_text != null)
		{
//			_hp_text.text = _unit_entity.base_entity.hp.ToString();
		}

		// Power text object available
		if(_power_text != null)
		{
			_power_text.text = _unit_entity.base_entity.power.ToString();
		}

		// Armor text object available
		if(_armor_text != null)
		{
			int armor = _unit_entity.base_entity.armor;

			// Enable object
			if(armor == 0)
				_armor_text.transform.parent.gameObject.SetActive(false);
			else
				_armor_text.transform.parent.gameObject.SetActive(true);
				
			_armor_text.text = armor.ToString();
		}
	}
	
	public void StartItemCooldownTurn(ITEM_STATE item_state)
	{
		TextMesh current = null;
		BaseItem item = null;
		MonochromeEffect effect = null;

		// Determine which textmesh to use
		switch(item_state)
		{
		case ITEM_STATE.PRIMARY:
			current = primary_cooldown_timer_textmesh;
			item    = _unit_entity.primary.item;
			if(_primary_monochrome_effect != null)
			{
				effect = _primary_monochrome_effect;
			}
			break;
		case ITEM_STATE.SECONDARY:
			current = secondary_cooldown_timer_textmesh;
			item    = _unit_entity.secondary.item;
			if(_secondary_monochrome_effect != null)
			{
				effect = _secondary_monochrome_effect;
			}
			break;
		case ITEM_STATE.SOULSHARD:
			current = soulshard_cooldown_timer_textmesh;
			item    = _unit_entity.soul.item;
			if(_soulshard_monochrome_effect != null)
			{
				effect = _soulshard_monochrome_effect;
			}
			break;
		}

		// Start display
		if(current != null)
		{
			// Make sure to not over write the cooldown timer if called multiple times while still running
			if(current.text == "" && effect != null)
			{
				current.text = item.remaining_cooldown.ToString();

				effect.Dim(item.cooldown, item.remaining_cooldown);

				DelayAction.instance.DelayInf(()=> // count down every delay
				{
					current.text = item.remaining_cooldown.ToString();
					effect.Dim(item.cooldown, item.remaining_cooldown);
				}, 
				.5f, // do this every 1s
				()=> // when to stop countdown
				{
					if(current.text == "0")
					{
						current.text = "";
						effect.UnDim();
						return true;
					}
					
					return false;
				});
			}
			else
			{
				Debug.Log("This shouldn't be getting called already. Cooldown timer still in progress!");
			}
		}
	}

	// Start couroutine countdown onto the textmesh associated with the appropriate item
	public void StartItemCooldownTimer(ITEM_STATE item_state, int action_index )
	{
		TextMesh current;
		BaseItem item;

		// Determine which textmesh to use
		switch(item_state)
		{
		case ITEM_STATE.PRIMARY:
			if(_primary_monochrome_effect != null)
			{
				_primary_monochrome_effect.PlayCooldownEffect(_unit_entity.secondary_slot[0].item.cooldown);
			}
			current = primary_cooldown_timer_textmesh;

			if(action_index == 0)
			{
				current = primary_cooldown_timer_textmesh;
			}
			else if(action_index == 1)
			{
				current = primary1_cooldown_timer_textmesh;
			}
			item    = _unit_entity.primary_slot[action_index].item;
			break;
		case ITEM_STATE.SECONDARY:
			if(_secondary_monochrome_effect != null)
			{
				_secondary_monochrome_effect.PlayCooldownEffect(_unit_entity.secondary_slot[0].item.cooldown);
			}
			current = secondary_cooldown_timer_textmesh;
			if(action_index == 0)
			{
				current = secondary_cooldown_timer_textmesh;
			}
			else if(action_index == 1)
			{
				current = secondary1_cooldown_timer_textmesh;
			}
			item    = _unit_entity.secondary_slot[action_index].item;
			break;
		case ITEM_STATE.SOULSHARD:
			if(_soulshard_monochrome_effect != null)
			{
				_soulshard_monochrome_effect.PlayCooldownEffect(_unit_entity.soulshard_slot[0].item.cooldown);
			}
			current = soulshard_cooldown_timer_textmesh;
			item    = _unit_entity.soulshard_slot[action_index].item;
			break;
		default:
			current = null;
			item = null;
			break;
		}

		// Start display
		if(current != null)
		{
			// Make sure to not over write the cooldown timer if called multiple times while still running
			if(current.text == "")
			{
				current.text = item.cooldown.ToString();
				DelayAction.instance.DelayInf(()=> // count down every delay
				{
						current.gameObject.SetActive(true);
						current.text = (int.Parse(current.text) - 1.0f).ToString();
				}
				, 1.0f, // do this every 1s
				()=> // when to stop countdown
				{
					if(current.text == "0")
					{
						current.text = "";
						return true;
					}

					return false;
				});
			}
			else
			{
				Debug.Log("This shouldn't be getting called already. Cooldown timer still in progress!");
			}
		}
	}

	public void AddItemChargePrimary()
	{
		Debug.Log("Primary charge");
		AddItemCharge(ITEM_STATE.PRIMARY);
	}

	public void AddItemChargeSecondary()
	{
		Debug.Log("Secondary charge");
		AddItemCharge(ITEM_STATE.SECONDARY);
	}

	public void AddItemChargeSoulshard()
	{
		Debug.Log("Soulshard charge");
		AddItemCharge(ITEM_STATE.SOULSHARD);
	}

	public void AddItemCharge(ITEM_STATE state)
	{
		// also draws
		_unit_entity.ChargeItem(state);
	}
}
