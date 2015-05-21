using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI
{
	public static System.Func<COMBAT_RETURN_STATUS> Decision(UnitEntity attacker)
	{
		// Determine whom to attack
		//
		UnitEntity target = null;

		try
		{
			List<UnitEntity> team = TurnBasedCombatSystem.instance.UnitsFrom(OWNERSHIP.PLAYER);
			int target_index = Random.Range(0, team.Count);
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


		// Determine which attack to use
		//
		int random;
		ITEM_STATE state;
		while(true)
		{			
			random = Random.Range(0, 100);

			// 50% of the time use primary
			if(random <= 100) // always uses primary
			{
				state = ITEM_STATE.PRIMARY;
				break;
			}
			// 30% of the time use secondary
			else if(random <= 80)
			{
				state = ITEM_STATE.SECONDARY;
				break;
			}
			// 20% of the time use soulshard
			else
			{
				state = ITEM_STATE.SOULSHARD;
				break;
			}
		}

//		if(!attacker.available_action)
//		{
//			return delegate()
//			{
//				TurnBasedCombatSystem.instance.NextTurn();
//				return COMBAT_RETURN_STATUS.END_TURN;
//			};
//		}

		return delegate()
		{
			Debug.Log(string.Format("{0} using item", attacker.base_entity.name));

			return attacker.UseItem(state, target);
		};
	}
}
