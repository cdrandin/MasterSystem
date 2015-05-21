using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ITEM_STATE
{
	UNDEFINED = 0,
	PRIMARY,
	SOULSHARD,
	SECONDARY
}

public class UnitEntity
{
	private BaseEntity _base_entity;
	public BaseEntity base_entity
	{
		get { return _base_entity; }
	}

	private PrimaryItem _primary;
	public PrimaryItem primary
	{ 
		get { return _primary; }
	}
	public PrimaryItem[] primary_slot;

	private SecondaryItem _secondary;
	public SecondaryItem secondary
	{ 
		get { return _secondary; }
	}
	public SecondaryItem[] secondary_slot;

	private SoulShard _soul;
	public SoulShard soul
	{
		get { return _soul; }
	}
	public SoulShard[] soulshard_slot;

	private ITEM_STATE _current_item_state;
	public ITEM_STATE current_item_state
	{
		get { return _current_item_state; }
	}

	private BaseItem _current_item;
	public BaseItem current_item
	{
		get { return _current_item; }
	}

	private List<Buffable> _buffables;
	public List<Buffable> buffables
	{
		get
		{
			if(_buffables == null)
			{
				_buffables = new List<Buffable>();
			}

			return _buffables;
		}
	}

	public bool HasBuffable(ATTRIBUTES attr)
	{
		foreach(Buffable b in buffables)
		{
			if(b.HasBuffable(attr))
			{
				return true;
			}
		}
		return false;
	}

	public void DisplayBuffables()
	{
		foreach(Buffable b in buffables)
		{
			Debug.Log(b);
		}
	}

	public void ApplyBuffable(Buffable b)
	{
		buffables.Add(b);
		b.ApplyEffect(this);
	}

	public void RemoveBuffable(Buffable b)
	{
		b.EndEffect();
		buffables.Remove(b);
	}

	public void RemoveBuffable(ATTRIBUTES attr)
	{
		for(int i=0;i<buffables.Count;++i)
		{
			if(buffables[i].HasBuffable(attr))
			{
				RemoveBuffable(buffables[i]);
				break;
			}
		}
	}

	private ACTION_LIST _current_action;
	public ACTION_LIST current_action
	{
		get { return _current_action; }
	}

	private System.WeakReference _weak_unit_game_object;
	public UnitGameobject unit_game_object
	{
		get 
		{ 
			return _weak_unit_game_object.Target as UnitGameobject;
		}
	}

	// This is for turn based system
	// Keep track of currently charging item
	private BaseItem _charging_item;

//	private Texture2D _unit_image;
//	public Texture2D unit_image
//	{
//		get
//		{
//			return _unit_image;
//		}
//	}

	/// <summary>
	/// Sets the weak reference unit game object.
	/// </summary>
	/// <param name="obj">Object.</param>
	public void SetWeakRefUnitGameObject(UnitGameobject obj)
	{
		_weak_unit_game_object = new System.WeakReference(obj);
	}

	public bool can_perform
	{
		get
		{
			if(this.HasBuffable(ATTRIBUTES.FROZEN))
			{
				return false;
			}

			return true;
		}
	}

	/// <summary>
	/// Determines whether this instance is dead.
	/// </summary>
	/// <returns><c>true</c> if this instance is dead; otherwise, <c>false</c>.</returns>
	public bool IsDead
	{
		get { return _base_entity.hp == 0; }
	}

	/// <summary>
	/// Determines whether this instance is armor up.
	/// </summary>
	/// <returns><c>true</c> if this instance is armor up; otherwise, <c>false</c>.</returns>
	public bool IsArmorUp
	{
		get { return _base_entity.armor > 0; }
	}

	/// <summary>
	/// Gets a value indicating whether this <see cref="UnitEntity"/> has any more power to use on a currently wielded item.
	/// </summary>
	/// <value><c>true</c> if available_action; otherwise, <c>false</c>.</value>
	public bool available_action
	{
		get
		{
			bool able = false;
			BaseItem item = null;

			if(!this.can_perform) return false;

			if(primary_slot != null)
			{
				foreach(PrimaryItem pi in primary_slot)
				{
					if(pi.item.ItemOffCooldown())
					{
						able = true;
						item = pi.item;
					}
				}
			}

			if(secondary_slot != null)
			{
				foreach(SecondaryItem si in secondary_slot)
				{
					if(si.item.ItemOffCooldown())
					{
						able = true;
						item = si.item;
					}
				}
			}

			if(soulshard_slot != null)
			{
				foreach(SoulShard ss in soulshard_slot)
				{
					if(ss.item.ItemOffCooldown())
					{
						able = true;
						item = ss.item;
					}
				}
			}

			if(able && item != null)
			{
				if((TimerBasedCombatSystem.instance.group_power - item.cost) >= 0)
				{
					able = true;
				}
			}

			return able;
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UnitEntity"/> class.
	/// </summary>
	/// <param name="be">Be.</param>
	/// <param name="pi">Pi.</param>
	/// <param name="si">Si.</param>
	/// <param name="ss">Ss.</param>
	public UnitEntity(BaseEntity be, PrimaryItem pi, SecondaryItem si, SoulShard ss)
	{
		_base_entity = be;

		if(pi != null && pi.item != null)
		{
			EquipToPrimary(pi.item);
		}
		else
		{
			_primary = null;
		}

		if(si != null && si.item != null)
		{
			EquipToSecondary(si.item);
		}
		else
		{
			_secondary = null;
		}

		if(ss != null && ss.item != null)
		{
			EquipToSoulshard(ss.item);
		}
		else
		{
			_soul = null;
		}
			
		_current_item_state  = ITEM_STATE.UNDEFINED; // No item in specific use on init
	}

	public UnitEntity(UnitEntitySO ue)
	{
		BaseEntitySO be    = ue.base_entity_so;
		PrimaryItem pi   = ue.primary;
		SecondaryItem si = ue.secondary;
		SoulShard ss     = ue.soul;

		_base_entity = new BaseEntity(be);
		
		if(pi != null && pi.item != null)
		{
			EquipToPrimary(pi.item);
		}
		else
		{
			_primary = null;
		}
		
		if(si != null && si.item != null)
		{
			EquipToSecondary(si.item);
		}
		else
		{
			_secondary = null;
		}
		
		if(ss != null && ss.item != null)
		{
			EquipToSoulshard(ss.item);
		}
		else
		{
			_soul = null;
		}
		
		_current_item_state  = ITEM_STATE.UNDEFINED; // No item in specific use on init
	}

	/// <summary>
	/// Equips item to primary area.
	/// </summary>
	/// <returns><c>true</c>, if to primary was equiped, <c>false</c> otherwise.</returns>
	/// <param name="item">Item.</param>
	public bool EquipToPrimary(BaseItem item)
	{
		if(ValidateEquip(ITEM_STATE.PRIMARY, item))
		{
			_primary = new PrimaryItem(item);
			return true;
		}

		return true;
	}

	/// <summary>
	/// Equips item to secondary area.
	/// </summary>
	/// <returns><c>true</c>, if to secondary was equiped, <c>false</c> otherwise.</returns>
	/// <param name="item">Item.</param>
	public bool EquipToSecondary(BaseItem item)
	{
		if(ValidateEquip(ITEM_STATE.SECONDARY, item))
		{
			_secondary = new SecondaryItem(item);
			return true;
		}

		return false;
	}

	/// <summary>
	/// Equips item to soulshard area.
	/// </summary>
	/// <returns><c>true</c>, if to soulshard was equiped, <c>false</c> otherwise.</returns>
	/// <param name="item">Item.</param>
	public bool EquipToSoulshard(BaseItem item)
	{
		if(ValidateEquip(ITEM_STATE.SOULSHARD, item))
		{
			_soul = new SoulShard(item);
			return true;
		}
		
		return false;
	}

	public void UnequipPrimary()
	{
		_primary = null;
	}

	public void UnequipSecondary()
	{
		_secondary = null;
	}

	public void UnequipSoulshard()
	{
		_soul = null;
	}

	/// <summary>
	/// Retrieve the item that on unit with respect to the ITEM_STATE
	/// </summary>
	/// <returns>The on.</returns>
	/// <param name="state">State.</param>
	public BaseItem ItemOn(ITEM_STATE state)
	{
		BaseItem item = null;

		switch(state)
		{
		case ITEM_STATE.PRIMARY:
			item = _primary.item;
			break;
		case ITEM_STATE.SECONDARY:
			item = _primary.item;
			break;
		case ITEM_STATE.SOULSHARD:
			item = _primary.item;
			break;
		default:
			break;
		}

		return item;
	}

	/// <summary>
	/// Propers the use of item.
	/// Usage terms: unit is alive to attack, item is equipped, unit's turn to attack, attack only your enemies and help only your allies
	/// </summary>
	/// <returns><c>true</c>, if use of item was propered, <c>false</c> otherwise.</returns>
	/// <param name="target">Target.</param>
	/// <param name="item">Item.</param>
	private COMBAT_RETURN_STATUS ProperUseOfItem(UnitEntity target, BaseItem item)
	{
		// Not equipped
		if(item == null)
		{
			Debug.LogWarning("No item equipped");
			return COMBAT_RETURN_STATUS.IMPROPER_USE_OF_ITEM;
		}
		
		// Don't use item when dead
		if(this.IsDead)
		{
			Debug.LogWarning(string.Format("{0} is dead", this.base_entity.name));
			return COMBAT_RETURN_STATUS.IMPROPER_USE_OF_ITEM;
		}

		// for some reason can't attack crippling or disable
		if(!this.can_perform)
		{
			return COMBAT_RETURN_STATUS.DISABLED;
		}

		// HACK THIS IS BULLSHIT
		if(Applications.type == COMBAT_TYPE.TURNED)
		{
			// Not your turn to perform action
			if(TurnBasedCombatSystem.instance.current_turn != TurnBasedCombatSystem.instance.WhoOwnsMe(this))
			{
				Debug.LogWarning(string.Format("{0} - Not your turn to attack. It is {1} turn.",
				                               System.Enum.GetName(typeof(OWNERSHIP), TurnBasedCombatSystem.instance.whose_next),
				                               System.Enum.GetName(typeof(OWNERSHIP), TurnBasedCombatSystem.instance.current_turn)));
				return COMBAT_RETURN_STATUS.NOT_YOUR_TURN;
			}
		}
		// HACK THIS IS BULLSHIT
		else
		{
			// There is no turn stuff
		}


		// Item is on cooldown
		// TODO(Chris): Need fixing for TimerBasedCombat
		if(!item.ItemOffCooldown())
		{
			Debug.LogWarning(string.Format("COOLDOWN: {0}", item));
			return COMBAT_RETURN_STATUS.COOLDOWN;
		}

		// HACK THIS IS BULLSHIT
		if(Applications.type == COMBAT_TYPE.TURNED)
		{
			// Item is harmful
			if(item.harmfulness == HARMFULNESS.HARMFUL)
			{
				// Can't attack your allies
				foreach(UnitEntity unit in TurnBasedCombatSystem.instance.UnitsFrom(TurnBasedCombatSystem.instance.current_turn))
				{
					if(unit == target)
					{
						Debug.LogWarning(string.Format("Can't attack ally with: {0}", item.name));
						return COMBAT_RETURN_STATUS.IMPROPER_USE_OF_ITEM;
					}
				}
			}
			// Helpful item
			else
			{
				// Can't help your enemies
				foreach(UnitEntity unit in TurnBasedCombatSystem.instance.UnitsFrom(TurnBasedCombatSystem.instance.whose_next))
				{
					if(unit == target)
					{
						Debug.LogWarning(string.Format("Can't help enemies with: {0}", item.name));
						return COMBAT_RETURN_STATUS.IMPROPER_USE_OF_ITEM;
					}
				}
			}
		}
		// HACK THIS IS BULLSHIT
		else
		{
			// Item is harmful
			if(item.harmfulness == HARMFULNESS.HARMFUL)
			{
				// Can't attack your allies
				if(TimerBasedCombatSystem.instance.LivingUnitsFrom(TimerBasedCombatSystem.instance.WhoOwnsMe(this)).Contains(target))
				{
					Debug.LogWarning(string.Format("Can't attack ally({0}) with: {1}", target.base_entity.name, item.name));
					return COMBAT_RETURN_STATUS.IMPROPER_USE_OF_ITEM;
				}
//				foreach(UnitEntity unit in TimerBasedCombatSystem.instance.UnitsFrom(TimerBasedCombatSystem.instance.WhoOwnsMe(this)))
//				{
//					if(unit == target)
//					{
//						Debug.LogWarning(string.Format("Can't attack ally with: {0}", item.name));
//						return COMBAT_RETURN_STATUS.IMPROPER_USE_OF_ITEM;
//					}
//				}
			}
			// Helpful item
			else
			{
				// Can't help your enemies
				if(!TimerBasedCombatSystem.instance.LivingUnitsFrom(TimerBasedCombatSystem.instance.WhoOwnsMe(this)).Contains(target))
				{
					Debug.LogWarning(string.Format("Can't help enemy({0}) with: {1}", target.base_entity.name, item.name));
					return COMBAT_RETURN_STATUS.IMPROPER_USE_OF_ITEM;
				}

//				// Can't help your enemies
//				foreach(UnitEntity unit in TimerBasedCombatSystem.instance.UnitsFrom(TimerBasedCombatSystem.instance.WhoOwnsMe(this)))
//				{
//					Debug.Log(unit);
//					Debug.Log(target);
//
//					if(unit != target)
//					{
//						Debug.LogWarning(string.Format("Can't help enemies with: {0}", item.name));
//						return COMBAT_RETURN_STATUS.IMPROPER_USE_OF_ITEM;
//					}
//				}
			}
		}

		// Successful usage of item
		return COMBAT_RETURN_STATUS.SUCCESSFUL;
	}

	public void ChargeItem(ITEM_STATE state, int action_index = -1)
	{
		if(TurnBasedCombatSystem.instance.current_turn != OWNERSHIP.PLAYER) return;

		if(Applications.type == COMBAT_TYPE.TURNED)
		{
			BaseItem prev = _charging_item;

			switch(state)
			{
			case ITEM_STATE.PRIMARY:
				if(action_index != -1)
				{
					_charging_item = this.primary_slot[action_index].item;
					break;
				}
				_charging_item = this.primary.item;
				break;
			case ITEM_STATE.SECONDARY:
				if(action_index != -1)
				{
					_charging_item = this.secondary_slot[action_index].item;
					break;
				}
				_charging_item = this.secondary.item;
				break;
			case ITEM_STATE.SOULSHARD:
				if(action_index != -1)
				{
					_charging_item = this.soulshard_slot[action_index].item;
					break;
				}
				_charging_item = this.soul.item;
				break;
			default:
				return;
			}

			// Conditions must be met in order to charge
			if(!_charging_item.ItemOffCooldown()) return;
			if(this.base_entity.power - (_charging_item.cost + _charging_item.charge + 1) < 0) return;

			// Continue charge on same item
			if(prev == null || _charging_item.Equals(prev))
			{
				// Cap at 3
				if(_charging_item.charge >= 2) return;
				
				_charging_item.AddCharge(); // up the power
				this.base_entity.power -= 1; // charge amount
				
				Debug.Log("Charging");
			}

			// Charging different item, refund power and start charging the other item
			else
			{
				// Not really attacking, refund power
				if(TurnBasedCombatSystem.instance.current_attacking == null)
				{
					this.base_entity.power += 1 * _charging_item.charge + 1;
				}

				prev.ResetCharge();
				 Camera.main.GetComponent<ItemChargeScript>().ResetCharges();
				prev = null;
				_charging_item.AddCharge(); // up the power
				this.base_entity.power -= 1; // charge amount
				
				Debug.Log("change Charging");
			}
		 	
			Camera.main.GetComponent<ItemChargeScript>().AddCharge(state);
		}
	}

	// Amount of power used
	// Use on TURNED BASED
	public void ReleaseCharge()
	{
		if(Applications.type == COMBAT_TYPE.TURNED)
		{
			if(_charging_item != null)
			{
				// Not really attacking, refund power
				if(TurnBasedCombatSystem.instance.current_attacking == null)
				{
					this.base_entity.power += 1 * _charging_item.charge;
				}
					
				_charging_item.ResetCharge();
				Debug.Log("Releasing charge");

				 Camera.main.GetComponent<ItemChargeScript>().ResetCharges();
			}
		}
	}

	/// <summary>
	/// Uses the item given a specific ITEM_STATE.
	/// </summary>
	/// <returns>The COMBAT_RETURN_STATUS.</returns>
	/// <param name="state">State.</param>
	/// <param name="target">Target.</param>
	public COMBAT_RETURN_STATUS UseItem(ITEM_STATE state, UnitEntity target, int action_index = -1)
	{
		if(target.IsDead)
		{
			return COMBAT_RETURN_STATUS.TARGET_DEAD;
		}

		BaseItem item = null;

		switch(state)
		{
		case ITEM_STATE.PRIMARY:
			_current_item_state = ITEM_STATE.PRIMARY;
			if(action_index != -1)
			{
				item = this.primary_slot[action_index].item;
				_current_item = item;
				break;
			}
			item = this.primary.item;
				_current_item = item;
			break;
		case ITEM_STATE.SECONDARY:
			_current_item_state = ITEM_STATE.SECONDARY;
			if(action_index != -1)
			{
				item = this.secondary_slot[action_index].item;
				_current_item = item;
				break;
			}
			item = this.secondary.item;
			_current_item = item;
			break;
		case ITEM_STATE.SOULSHARD:
			_current_item_state = ITEM_STATE.SOULSHARD;
			if(action_index != -1)
			{
				item = this.soulshard_slot[action_index].item;
				_current_item = item;
				break;
			}
			item = this.soul.item;
			_current_item = item;
			break;
		default:
			item = null;
			break;
		}

		// Prepare attack if item equipped
		if(item != null)
		{
			COMBAT_RETURN_STATUS status = ProperUseOfItem(target, item);

			// Cannot attack when someone else is attacking, just won't do it for now.
			// TODO: Possibly in the future have the attack queued
			if(Applications.type == COMBAT_TYPE.TURNED)
			{
				if(TurnBasedCombatSystem.instance.current_attacking != null)
				{
					return COMBAT_RETURN_STATUS.NOT_YOUR_TURN;
				}
			}
			else
			{
				if(TimerBasedCombatSystem.instance.current_attacking != null)
				{
					return COMBAT_RETURN_STATUS.NOT_YOUR_TURN;
				}
			}

			// If not proper use of item, deny request
			if(status != COMBAT_RETURN_STATUS.SUCCESSFUL)
			{
				return status;
			}

			// Using item
//			if((_base_entity.power - item.cost) >= 0)
			if((TimerBasedCombatSystem.instance.group_power - item.cost) >= 0)
			{
				if(Applications.type == COMBAT_TYPE.TURNED)
				{
					TurnBasedCombatSystem.instance.current_attacking = this;
				}
				else
				{
					TimerBasedCombatSystem.instance.current_attacking = this;
				}
				
				// Insert some sort of effects
				item.PlaySoundEffect();
				item.PlayAnimationEffect(target.unit_game_object.transform.position, true);
				item.PlayAnimationFX(target.unit_game_object.transform.position);
				// Combat text
				string text;
				Color color;
				item.ToCombatText(out text, out color);

				// Target hit animation
				if(item.harmfulness == HARMFULNESS.HARMFUL)
				{
					try
					{
						UnitAnimation anim = target.unit_game_object.GetComponent<UnitAnimation>();
						anim.StaggerAnimation();
					}
					catch(System.Exception e)
					{
						Debug.Log(string.Format("Ignore: {0}", e));
					}
				}
				
				CombatTextAnimator.instance.SetText(text, color, target.unit_game_object.transform.position);

				// On TURNED BASED extinguish cost afterwards, possible choice player may charge ability
//				if(Applications.type == COMBAT_TYPE.TIMED)
//					_base_entity.power -= item.cost;
					TimerBasedCombatSystem.instance.ModPower(-item.cost);


				item.action(this, target);
				item.ability(this, target);

				if(Applications.type == COMBAT_TYPE.TURNED)
					ReleaseCharge(); // Release afterwards, so the stats are applied on usage
				
				// will start countdown on text mesh with respective item
				if(Applications.type == COMBAT_TYPE.TIMED)
					//; // Script is taking care of cooldown stuff only for player however.
					unit_game_object.StartItemCooldownTimer(state, action_index);
				else
					unit_game_object.StartItemCooldownTurn(state);

				//TurnBasedCombatSystem.instance.UpdateCombatState();
				Debug.Log(string.Format("{0} Using <{1}> item: <{2}> on <{3}>", this.base_entity.name, _current_item_state, item.name, target.base_entity.name));

				return COMBAT_RETURN_STATUS.SUCCESSFUL;
			}
			else
			{
				return COMBAT_RETURN_STATUS.OUT_OF_RESOURCES;
			}
		}
		else
		{
			return COMBAT_RETURN_STATUS.NO_ITEM_EQUIP;
		}
	}

	/// <summary>
	/// Uses the primary item.
	/// </summary>
	/// <returns>COMBAT_RETURN_STATUS.</returns>
	/// <param name="target">Target.</param>
	public COMBAT_RETURN_STATUS UsePrimaryItem(UnitEntity target)
	{
		return UseItem(ITEM_STATE.PRIMARY, target);
	}

	/// <summary>
	/// Uses the secondary item.
	/// </summary>
	/// <returns>COMBAT_RETURN_STATUS.</returns>
	/// <param name="target">Target.</param>
	public COMBAT_RETURN_STATUS UseSecondaryItem(UnitEntity target)
	{
		return UseItem(ITEM_STATE.SECONDARY, target);
	}

	/// <summary>
	/// Uses the soul shard.
	/// </summary>
	/// <returns>COMBAT_RETURN_STATUS.</returns>
	/// <param name="target">Target.</param>
	public COMBAT_RETURN_STATUS UseSoulShard(UnitEntity target)
	{
		return UseItem(ITEM_STATE.SOULSHARD, target);
	}

	/// <summary>
	/// Sets the current action. For now assume of an ON[ACITION_LIST] type action
	/// i.e. ACTION_LIST.ON_ATTACK
	/// </summary>
	/// <param name="action">Action.</param>
	public void SetCurrentAction(ACTION_LIST action)
	{
		_current_action = action;
	}

	/// <summary>
	/// Clears the current action.
	/// </summary>
	public void ClearCurrentAction()
	{
		_current_action = ACTION_LIST.PASSIVE;
	}

	/// <summary>
	/// Validates the item to determine if it is possible to equip said item with the rules of your hand strength
	/// </summary>
	/// <returns><c>true</c>, if item equip to slot was valid, <c>false</c> otherwise.</returns>
	/// <param name="item">Item.</param>
	private bool ValidateEquip(ITEM_STATE state, BaseItem item)
	{
		if(item == null)
			return false;

		// check if can equip
		// Just add it. No checks
		if(state == ITEM_STATE.PRIMARY)
		{
			return true;
		}
		else if(state == ITEM_STATE.SECONDARY)
		{
			if(item.hand == WEAPON_HANDLE.SINGLE_HANDED)
			{
				return true;
			}
		}
		else if(state == ITEM_STATE.SOULSHARD)
		{
			return true;
		}

		Debug.LogWarning(string.Format("Unable to equip item: {0}", item));
		return false;
	}

	public void Reset()
	{
		ReleaseCharge();

		if(_base_entity != null)
			_base_entity.Reset();

		if(_primary != null)
			_primary.item.Reset();

		if(_secondary != null)
			_secondary.item.Reset();

		if(_soul != null)
			_soul.item.Reset();
	}

	/// <summary>
	/// ToString this instance.
	/// </summary>
	override public string ToString()
	{
		return _base_entity.name;
//		return string.Format("UnitEntity:\n{0}\n{1}\n{2}\n{3}", 
//		                     (base_entity != null)?base_entity.ToString():"Missing base entity", 
//		                     (primary != null)? primary.ToString():"Primary unequipped",
//		                     (_secondary != null)?secondary.ToString():"Secondary unequipped", 
//		                     (_soul != null)?soul.ToString():"Soulshard unequipped");
	}
}
