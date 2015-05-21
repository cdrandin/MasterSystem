using UnityEngine;
using System.Collections;

// Action needs to know to whom the action should be reserved for

public enum ACTION_LIST
{
	PASSIVE = 0,
	BEFORE_ATTACK,
	ON_ATTACK,
	AFTER_ATTACK,
	BEFORE_DEFEND,
	ON_DEFEND,
	AFTER_DEFEND,
	BEFORE_HEAL,
	ON_HEAL,
	AFTER_HEAL
}

public class Action 
{
	static System.Action<UnitEntity, UnitEntity> AttackAction()
	{
		return delegate(UnitEntity self, UnitEntity target) 
		{
			Debug.Log("Attack");
			self.SetCurrentAction(ACTION_LIST.ON_ATTACK);

			int amount = 0;
			amount = self.current_item.amount;
			if(target.HasBuffable(ATTRIBUTES.NEGATE_MYSTIC))
			{
				amount = 0;
				target.RemoveBuffable(ATTRIBUTES.NEGATE_MYSTIC);
			}

			target.base_entity.DamageUnit(amount);

//			switch(state)
//			{
//			case ITEM_STATE.PRIMARY:
//				amount = self.primary.item.amount;
//				target.base_entity.DamageUnit(amount);
//				break;
//			case ITEM_STATE.SECONDARY:
//				amount = self.secondary.item.amount;
//				target.base_entity.DamageUnit(amount);
//				break;
//			case ITEM_STATE.SOULSHARD:
//				amount = self.soul.item.amount;
//				target.base_entity.DamageUnit(amount);
//				break;
//			default:
//				Debug.Log(string.Format("AttackAction called. Undefined state. {0} vs {1}", self.ToString(), target.ToString()));
//				break;
//			}
			Debug.Log(string.Format("Attack: {0} --> {1}  (-{2})", self.base_entity.name, target.base_entity.name, amount));
			Debug.Log(string.Format("Target: {0}", target));
		};
	}

	static System.Action<UnitEntity, UnitEntity> DefendAction()
	{
		return delegate(UnitEntity self, UnitEntity target) 
		{
			Debug.Log("Defend");
			self.SetCurrentAction(ACTION_LIST.ON_DEFEND);
			int amount = 0;
			amount = self.current_item.amount;
			target.base_entity.GainArmor(amount);
//			switch(state)
//			{
//			case ITEM_STATE.SECONDARY:
//				amount = self.secondary.item.amount;
//				target.base_entity.GainArmor(amount);
//				break;
//			default:
//				Debug.Log(string.Format("DefendAction called. Undefined state. {1} vs {2}", self.ToString(), target.ToString()));
//				break;
//			}

			Debug.Log(string.Format("Defend: {0} --> {1}  ({2})", self.base_entity.name, target.base_entity.name, amount));
		};
	}

	static System.Action<UnitEntity, UnitEntity> HealAction()
	{
		return delegate(UnitEntity self, UnitEntity target) 
		{
			Debug.Log("Heal");
			self.SetCurrentAction(ACTION_LIST.ON_HEAL);
			int amount = 0;
			amount = self.current_item.amount;
			target.base_entity.HealUnit(amount);
//
//			switch(state)
//			{
//			case ITEM_STATE.SOULSHARD:
//				amount = self.soul.item.amount;
//				target.base_entity.HealUnit(amount);
//				break;
//			default:
//				Debug.Log(string.Format("HealAction called. Undefined state. {0} vs {1}", self.ToString(), target.ToString()));
//				break;
//			}

			Debug.Log(string.Format("Heal: {0} --> {1}  ({2})", self.base_entity.name, target.base_entity.name, amount));
		};
	}

	static System.Action<UnitEntity, UnitEntity> PassiveAction()
	{
		return delegate(UnitEntity self, UnitEntity target) 
		{
			Debug.Log("Passive");
		};
	}

	public static System.Action<UnitEntity, UnitEntity> GetAction(ACTION_LIST action)
	{
		System.Action<UnitEntity, UnitEntity> a;
		
		switch(action)
		{
		case ACTION_LIST.BEFORE_ATTACK:
		case ACTION_LIST.ON_ATTACK:
		case ACTION_LIST.AFTER_ATTACK:	
			a = AttackAction();
			break;
		case ACTION_LIST.BEFORE_DEFEND:
		case ACTION_LIST.ON_DEFEND:
		case ACTION_LIST.AFTER_DEFEND:
			a = DefendAction();
			break;
		case ACTION_LIST.BEFORE_HEAL:
		case ACTION_LIST.ON_HEAL:
		case ACTION_LIST.AFTER_HEAL:
			a = HealAction();
			break;
		default:
			a = PassiveAction();
			break;
		}
		
		return a;
	}
}
