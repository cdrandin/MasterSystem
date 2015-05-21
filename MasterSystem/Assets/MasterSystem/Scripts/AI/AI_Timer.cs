using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI_Timer
{
	public static System.Func<COMBAT_RETURN_STATUS> Decision(UnitEntity attacker)
	{
		// Check if can make valid move
		if(!attacker.available_action)
		{
			return delegate 
			{
				return COMBAT_RETURN_STATUS.NO_AVAILABLE_MOVES;
			};
		}

		// Determine whom to attack
		//
		UnitEntity target = null;
		try
		{
			List<UnitEntity> team = (Applications.type == COMBAT_TYPE.TIMED) ? TimerBasedCombatSystem.instance.LivingUnitsFrom(OWNERSHIP.PLAYER) :
																			   TurnBasedCombatSystem.instance.UnitsFrom(OWNERSHIP.PLAYER);
			int target_index = 1;// Random.Range(0, team.Count);
			target = team[target_index];
		}
		catch(System.ArgumentOutOfRangeException)
		{
			// No available target
			return delegate()
			{
				return COMBAT_RETURN_STATUS.NO_TARGET;
			};
		}

		return delegate()
		{
			Debug.Log(string.Format("AI: {0} using item on {1}", attacker.base_entity.name, target.base_entity.name));
			return attacker.UseItem(ITEM_STATE.PRIMARY, target, 0);
		};
	}
}
