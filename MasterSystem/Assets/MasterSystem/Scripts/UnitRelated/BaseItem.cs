using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum WEAPON_HANDLE
{
	UNDEFINED     = 0,
	SINGLE_HANDED,
	DOUBLE_HANDED
}

public enum HARMFULNESS
{
	UNDEFINED = 0,
	HELPFUL,
	HARMFUL,
	Utility
}

public enum ABILITY_TYPE
{
	UNKNOWN = 0,
	MELEE,
	RANGED,
	MYSTIC,
}

[System.Serializable]
public class BaseItem : ScriptableObject
{
	// temoporary for quick mod
	public void ModAmount(int amount)
	{
		_amount = amount;
	}

	[SerializeField]
	private string _name;
	public string name
	{ 
		get { return _name; }
	}
	
	[SerializeField]
	private int _cost;
	public int cost
	{
		get { return _cost; }
	}

	private int _charge = 0;
	public int charge
	{
		get { return Mathf.Clamp(_charge, 0, 2); }
	}

	[SerializeField]
	private string _description_text;
	public string text
	{
		get { return _description_text; }
	}

	[SerializeField]
	private WEAPON_HANDLE _hand;
	public WEAPON_HANDLE hand
	{
		get { return _hand; }
	}

	[SerializeField]
	private HARMFULNESS _harmfulness;
	public HARMFULNESS harmfulness
	{
		get { return _harmfulness; }
	}

	public ABILITY_TYPE ability_type;

	private System.Action<UnitEntity, UnitEntity> _action;
	public System.Action<UnitEntity, UnitEntity> action
	{
		get 
		{ 
			if(_action == null)
			{
				if(Applications.type == COMBAT_TYPE.TURNED)
				{
					SetAction(_action_list);
				}
				else
				{
					SetAction_Timer(_action_list);
				}
			}

			return _action; 
		}
	}

	[SerializeField]
	private ACTION_LIST _action_list;
	public ACTION_LIST action_list
	{
		get { return _action_list; }
	}

	private ABILITY_LIST _ability_list;
	public ABILITY_LIST ability_list
	{
		get { return _ability_list; }
	}


	private System.Action<UnitEntity , UnitEntity> _ability;
	public System.Action<UnitEntity , UnitEntity> ability
	{
		get 
		{ 
			if(_ability == null)
			{
				SetAbility(_ability_list);
			}

			return _ability; 
		}
	}

	[SerializeField]
	private int _amount;
	public int amount
	{
		get { return _amount * (charge + 1); }
	}

	private int _internal_cooldown;
	private float _internal_time;
	public int time_left
	{
		get 
		{
			return Mathf.Clamp((int)_internal_time - (int)Time.time, 0, int.MaxValue);
		}
	}

	// Mainly for textual purposes
	public int remaining_cooldown
	{
		get 
		{ 
			if(ItemOffCooldown())
			{
				return 0;
			}
			else
			{
				Debug.Log(_cooldown - _internal_cooldown + 1);
				return _cooldown - _internal_cooldown + 1;
			}
		}
	}

	[SerializeField]
	private int _cooldown;
	public int cooldown
	{
		get { return _cooldown; }
	}

	[SerializeField]
	public string on_use_sound_effect_resource_path;

	private AudioClip _on_use_sound_effect;
	public AudioClip on_use_sound_effect
	{
		get { return _on_use_sound_effect; }
	}

	[SerializeField]
	private Animator _on_use_animation_effect;
	public Animator on_use_animation_effect
	{
		get { return _on_use_animation_effect; }
	}

	[SerializeField]
	private Texture2D _item_image;
	public Texture2D item_image
	{
		get { return _item_image; }
	}

	private int _id;
	public int id
	{
		get { return _id; }
	}
	public void SetID(int id)
	{
		_id = id;
	}

//	public Ability

	public void SetName(string name)
	{
		_name = name;
	}

	public BaseItem(string name, int usage_cost, int amt, int downtime_after_usage, string text, WEAPON_HANDLE hand, HARMFULNESS harm, ACTION_LIST action_list, ABILITY_LIST ability_list)
	{
		_name              = name;
		_cost        	   = usage_cost;
		_internal_cooldown = 0;
		_internal_time     = 0f;
		_cooldown   	   = downtime_after_usage;
		_description_text  = text;
		_hand       	   = hand;
		_harmfulness 	   = harm;
		_amount     	   = amt;
		_charge            = 0;

		_ability_list      = ability_list;
		_action_list       = action_list;

		if(Applications.type == COMBAT_TYPE.TURNED)
		{
			SetAction(_action_list);
		}
		else
		{
			SetAction_Timer(_action_list);
		}
		SetAbility(_ability_list);
	}

	public BaseItem(BaseItem item)
	{
		if(item != null)
		{
			_name              = item.name;
			_cost        	   = item.cost;
			_internal_cooldown = 0;
			_cooldown   	   = item.cooldown;
			_description_text  = item.text;
			_hand       	   = item.hand;
			_harmfulness 	   = item.harmfulness;
			_amount     	   = item.amount;
			_charge            = 0;

			_ability_list      = item.ability_list;
			_action_list       = item.action_list;
			on_use_sound_effect_resource_path = item.on_use_sound_effect_resource_path;
			_on_use_sound_effect = item.on_use_sound_effect;
			_on_use_animation_effect = item._on_use_animation_effect;
			item_fx = item.item_fx;
			_item_image = item.item_image;

			if(Applications.type == COMBAT_TYPE.TURNED)
			{
				SetAction(_action_list);
			}
			else
			{
				SetAction_Timer(_action_list);
			}
			SetAbility(_ability_list);
		}
	}

	public void Init(string name, int usage_cost, int amt, int downtime_after_usage, string text, WEAPON_HANDLE hand, HARMFULNESS harm, ACTION_LIST action_list, ABILITY_LIST ability_list)
	{
		_name              = name;
		_cost        	   = usage_cost;
		_internal_cooldown = 0;
		_cooldown   	   = downtime_after_usage;
		_description_text  = text;
		_hand       	   = hand;
		_harmfulness 	   = harm;
		_amount     	   = amt;
		_charge            = 0;

		_ability_list      = ability_list;
		_action_list       = action_list;
		
		if(Applications.type == COMBAT_TYPE.TURNED)
		{
			SetAction(_action_list);
		}
		else
		{
			SetAction_Timer(_action_list);
		}
		SetAbility(_ability_list);
	}

	public void SetImage(Texture2D img)
	{
		_item_image = img;
	}

	public void AddCharge()
	{
		++_charge;
	}

	public void ResetCharge()
	{
		_charge = 0;
	}

	public void SetAction(ACTION_LIST action)
	{
		_action  = delegate(UnitEntity self, UnitEntity target) 
					{
						// internal cooldown set to 1 as first use
						++_internal_cooldown;
						Action.GetAction(action).Invoke(self, target); 
					};
	}

	public void SetAction_Timer(ACTION_LIST action)
	{
		_action  = delegate(UnitEntity self, UnitEntity target) 
		{
			// internal cooldown set to 1 as first use and later set to 0 to indicate off cooldown
			_internal_time = Time.time + (float)_cooldown;

			++_internal_cooldown;
			Scheduler.instance.ScheduleEventTime(self.unit_game_object.gameObject, string.Format("{0}{1}", this, GetHashCode()), Time.time + (float)_cooldown, ()=>{ _internal_cooldown = 0; Debug.Log(string.Format("Item: {0} going off cooldown", this._name)); }, false);
			Action.GetAction(action).Invoke(self, target); 
		};
	}

	public void SetAbility(ABILITY_LIST ability)
	{
		_ability = Ability.GetAbility(ability);
	}

	public void SetSoundEffectAudio(AudioClip sound)
	{
		_on_use_sound_effect = sound;
	}

	/// <summary>
	/// Plays the sound effect after delay. By default it plays immediately.
	/// </summary>
	/// <param name="delay">Delay.</param>
	public void PlaySoundEffect(float delay = 0.0f)
	{
		if(on_use_sound_effect_resource_path != null && on_use_sound_effect_resource_path != "")
		{
			this.SetSoundEffectAudio(Resources.Load(on_use_sound_effect_resource_path) as AudioClip);
		}

		if(_on_use_sound_effect == null)
		{
			Debug.LogWarning(string.Format("{0} does not contain an audio clip", this._name));
			return;
		}

		if(Camera.main.GetComponent<AudioSource>() == null)
		{
			Debug.LogWarning("Missing an audio course on the camera");
			return;
		}

		// HACK: Using singleton instance that inherits from MonoBehaviour to access the StartCoroutine method
		DelayAction.instance.Delay(delegate() 
		                           {
										Camera.main.GetComponent<AudioSource>().PlayOneShot(_on_use_sound_effect);
								   }, delay);
	}

	public void SetAnimationEffect(Animator anim)
	{
		_on_use_animation_effect = anim;
	}

	public GameObject item_fx;
	/// <summary>
	/// Enables the item_fx object assoiated with BaseItem. item_fx assumes it will auto destruct
	/// </summary>
	/// <param name="world_position">World_position.</param>
	public void PlayAnimationFX(Vector3 world_position)
	{
		if(item_fx != null)
		{
			GameObject animation_fx = Instantiate(item_fx, new Vector3(world_position.x, world_position.y, Camera.main.transform.position.z + 10f), Quaternion.identity) as GameObject;
		}
	}

	public void PlayAnimationEffect(Vector3 world_position, bool adjust_position=false)
	{
		GameObject anim_obj = GameObject.FindWithTag("Animation");
		anim_obj = Instantiate(anim_obj);

		if(anim_obj == null)
		{
			Debug.LogWarning("Missing animation game object");
			return;
		}

		Animator anim = anim_obj.GetComponent<Animator>();
		if(anim == null)
		{
			Debug.LogWarning("Missing animator");
			return;
		}

		try
		{
			anim.runtimeAnimatorController = _on_use_animation_effect.runtimeAnimatorController;
			anim_obj.SetActive(true);
			anim.SetTrigger("PLAY");
			DelayAction.instance.Delay(()=>{Destroy(anim_obj);}, anim.runtimeAnimatorController.animationClips[0].length*1.2f); // bad but oh well, pool it later with my PoolSystem
		}
		catch(System.Exception e)
		{
			Debug.Log(string.Format("Ignore(No animation): {0}", e));
			Destroy(anim_obj);
		}

		anim_obj.transform.position = world_position;

		if(adjust_position)
		{
			anim_obj.transform.position = new Vector3(world_position.x, world_position.y, Camera.main.transform.position.z + 1f);
		}
	}

	/// <summary>
	/// Check to see if the item is off its cooldown so we can use it again. 
	/// </summary>
	/// <returns><c>true</c>, if off cooldown was itemed, <c>false</c> otherwise.</returns>
	public bool ItemOffCooldown()
	{
		if(_internal_cooldown == 0)
		{
			return true;
		}
		// Item on cooldown
		else //if(_internal_cooldown > 0)
		{
			// Check if item is done with cooldown
			if(_internal_cooldown > _cooldown)
			{
				_internal_cooldown = 0;
				Debug.Log("Going off cooldown");
				return true;
			} 

			return false;
		}
	}

	/// <summary>
	/// Increments the item cooldown IFF the item has started to begin on cooldown in the first place.
	/// So, unused item won't go on cooldown for no reason.
	/// 
	/// </summary>
	public void IncrementItemCooldown()
	{
		if(_internal_cooldown > 0)
		{
			++_internal_cooldown;
		}
	}

	public void ToCombatText(out string text, out Color color)
	{
		text = (_harmfulness == HARMFULNESS.HARMFUL)?"-":"+";
		text += amount.ToString();

		if(_harmfulness == HARMFULNESS.HARMFUL)
		{
			text = string.Format("-{0}", amount);
			color = Color.red;
		}
		else if(_harmfulness == HARMFULNESS.HELPFUL)
		{
			text = string.Format("+{0}", amount);
			color = Color.green;
		}
		else if(_harmfulness == HARMFULNESS.Utility)
		{
			text = string.Format("+{0}", amount);
			color = Color.white;
		}
		else
		{
			Debug.Log("Shouldn't be here");
			text = "KJAGHJSKDHKJAHSDKHAKDJHAS";
			color = Color.magenta;
		}
	}

	public void Reset()
	{
		_internal_cooldown = 0;
		ResetCharge();
	}

	override public string ToString()
	{
		return string.Format("Item name: {0} Cost: {1} Amount: {2} Description: \"{3}\"", name, cost, amount,text);
	}
}
