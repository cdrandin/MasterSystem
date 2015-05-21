using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class HandleEvents : MonoBehaviour {



	public static object begin;
	public static object end;

	private const float TIME_HOLDING_THRESHOLD = 0.25f; //seconds

	private float m_timeHolding;

	public Hashtable m_hashTable;
	public ItemAbilitityDescription iad;

	// Use this for initialization
	void Start () {

		m_hashTable   = null;
		
		StartCoroutine("CreateHashTable");

		begin = null;
		end = null;

		m_timeHolding = 0.0f;

	}

	// Update is called once per frame


	void Update () {
		// Check if touched
		if(begin == null){
			
			begin = HandleInputs.CheckIfPressed();
		}
		// Check to find end result
		else{
			// Check update end object
			end = HandleInputs.CheckIfReleased(); // returns an object as a string of the object's tag
		
			// End is released, released beginning
			if(end == null){
				
				begin = null;
			}

			iad.UpdateDescriptionObject(false);

			// Don't like, but what is being used right now
			if(end.ToString() == "Background")
			{
				if(Applications.type == COMBAT_TYPE.TURNED)
				{
					Game.player.ReleaseCharge();
				}
				CleanInputs();
			}

			// 
			if(end != null && !(end is bool)){
				try
				{
					System.Action<UnitEntity> action = (System.Action<UnitEntity>)m_hashTable[(string)begin];
					if(action != null && m_hashTable[end] is UnitEntity )
					{
						try
						{
							action.Invoke((UnitEntity)m_hashTable[(string)end]);
						}
						catch(System.Exception e)
						{
							Debug.LogException(e, this);
						}
					}
				}
				catch(System.Exception e)
				{
					Debug.LogException(e, this);
				}
				
				begin = null;
				end = null;
				m_timeHolding = 0;
			}

			CheckIfHolding(begin, end);
		}

}

public void CheckForInput(){
	
	
	
	while(true){


	}

}

	public void CheckIfHolding(object begin, object end) {

		if(!HandleInputs.CheckIfSwipping() &&
		   m_hashTable.ContainsKey( HandleInputs.CheckIfHolding() ) && 
		   m_hashTable[HandleInputs.CheckIfHolding()] == m_hashTable[(string)begin]) {
		

			m_timeHolding += Time.deltaTime;
			if( m_timeHolding > TIME_HOLDING_THRESHOLD){
				/*
				 * This is where we turn on the display and update the information
				 * 
				 */
				switch(begin.ToString())
				{
				case "Primary0":
					iad.UpdateDescriptionObject(true, TimerBasedCombatSystem.instance.selected_unit.primary_slot[0].item);
					break;
				case "Primary1":
					iad.UpdateDescriptionObject(true, TimerBasedCombatSystem.instance.selected_unit.primary_slot[1].item);
					break;
				case "Secondary0":
					iad.UpdateDescriptionObject(true, TimerBasedCombatSystem.instance.selected_unit.secondary_slot[0].item);
					break;
				case "Secondary1":
					iad.UpdateDescriptionObject(true, TimerBasedCombatSystem.instance.selected_unit.secondary_slot[1].item);
					break;
				case "SoulShard0":
					iad.UpdateDescriptionObject(true, TimerBasedCombatSystem.instance.selected_unit.soulshard_slot[0].item);
					break;
//				case "SoulShard1":
//					break;
				}
			}
			
		}
		else{
			m_timeHolding = 0;
		}

	}

	public void CleanInputs()
	{
		begin = null;
		end = null;
	}

	private IEnumerator CreateHashTable(){

		m_hashTable = new Hashtable();

		// HACK THIS IS BULLSHIT
		if(Applications.type == COMBAT_TYPE.TURNED)
		{
			while( !Game.m_init){

				yield return null;
			}
				
			#region TURNED BASED 
			if(Game.player != null){

				m_hashTable["MainPlayer"] = Game.player;

				System.Action<UnitEntity>item_use = delegate(UnitEntity obj) 
				{
					TurnBasedCombatSystem.instance.current_combat_status = Game.player.UsePrimaryItem(obj);
					Debug.Log(string.Format("Player combat return status: {0}", TurnBasedCombatSystem.instance.current_combat_status));
//					TurnBasedCombatSystem.instance.UpdateCombatState();
				};

				m_hashTable["Primary"] = item_use;

				item_use = delegate(UnitEntity obj) 
				{
					TurnBasedCombatSystem.instance.current_combat_status = Game.player.UseSecondaryItem(obj);
					Debug.Log(string.Format("Player combat return status: {0}", TurnBasedCombatSystem.instance.current_combat_status));
//					TurnBasedCombatSystem.instance.UpdateCombatState();
				};

				m_hashTable["Secondary"] = item_use;


				item_use = delegate(UnitEntity obj) 
				{
					TurnBasedCombatSystem.instance.current_combat_status = Game.player.UseSoulShard(obj);
					Debug.Log(string.Format("Player combat return status: {0}", TurnBasedCombatSystem.instance.current_combat_status));
//					TurnBasedCombatSystem.instance.UpdateCombatState();
				};
				m_hashTable["Soulshard"] = item_use;
			}

			if(Game.all_enemies != null)
			{
				foreach(UnitEntity ue in Game.all_enemies)
				{
					// Using tags as key, where to keep track of what is hitting
					m_hashTable[ue.unit_game_object.gameObject.tag] = ue as UnitEntity;
				}
			}
//			if(Game.boss != null){
//
//				m_hashTable["Boss"] = Game.boss;
//			}
		}
		#endregion

		// HACK THIS IS BULLSHIT
		// TIMER
		else
		{
			while( !Game_Timer.m_init){
				
				yield return null;
			}
			
			if(Game_Timer.player != null){
				
				m_hashTable["Hitbox"] = Game_Timer.player;
				m_hashTable["AllyLeft"] = Game_Timer.ally_unit_left;
				m_hashTable["AllyRight"] = Game_Timer.ally_unit_right;
				
				// PRIMARY
				System.Action<UnitEntity>item_use = delegate(UnitEntity obj) 
				{
					TimerBasedCombatSystem.instance.current_combat_status = TimerBasedCombatSystem.instance.selected_unit.UseItem(ITEM_STATE.PRIMARY, obj,0);
//					TimerBasedCombatSystem.instance.current_combat_status = Game_Timer.player.UseItem(ITEM_STATE.PRIMARY, obj,0);
					Debug.Log(string.Format("Player combat return status: {0}", TimerBasedCombatSystem.instance.current_combat_status));
					TimerBasedCombatSystem.instance.UpdateCombatState();
				};
				
				m_hashTable["Primary0"] = item_use;

				item_use = delegate(UnitEntity obj) 
				{
					TimerBasedCombatSystem.instance.current_combat_status = TimerBasedCombatSystem.instance.selected_unit.UseItem(ITEM_STATE.PRIMARY, obj,1);
//					TimerBasedCombatSystem.instance.current_combat_status = Game_Timer.player.UseItem(ITEM_STATE.PRIMARY, obj,1);
					Debug.Log(string.Format("Player combat return status: {0}", TimerBasedCombatSystem.instance.current_combat_status));
					TimerBasedCombatSystem.instance.UpdateCombatState();
				};
				
				m_hashTable["Primary1"] = item_use;


				// SECONDARY
				item_use = delegate(UnitEntity obj) 
				{
					TimerBasedCombatSystem.instance.current_combat_status = TimerBasedCombatSystem.instance.selected_unit.UseItem(ITEM_STATE.SECONDARY, obj,0);
//					TimerBasedCombatSystem.instance.current_combat_status = Game_Timer.player.UseItem(ITEM_STATE.SECONDARY, obj,0);
					Debug.Log(string.Format("Player combat return status: {0}", TimerBasedCombatSystem.instance.current_combat_status));
					TimerBasedCombatSystem.instance.UpdateCombatState();
				};
				
				m_hashTable["Secondary0"] = item_use;

				item_use = delegate(UnitEntity obj) 
				{
					TimerBasedCombatSystem.instance.current_combat_status = TimerBasedCombatSystem.instance.selected_unit.UseItem(ITEM_STATE.SECONDARY, obj, 1);
//					TimerBasedCombatSystem.instance.current_combat_status = Game_Timer.player.UseItem(ITEM_STATE.SECONDARY, obj, 1);
					Debug.Log(string.Format("Player combat return status: {0}", TimerBasedCombatSystem.instance.current_combat_status));
					TimerBasedCombatSystem.instance.UpdateCombatState();
				};
				
				m_hashTable["Secondary1"] = item_use;
				

				// SOULSHARD
				item_use = delegate(UnitEntity obj) 
				{
					TimerBasedCombatSystem.instance.current_combat_status = TimerBasedCombatSystem.instance.selected_unit.UseItem(ITEM_STATE.SOULSHARD, obj, 0);
//					TimerBasedCombatSystem.instance.current_combat_status = Game_Timer.player.UseItem(ITEM_STATE.SOULSHARD, obj, 0);
					Debug.Log(string.Format("Player combat return status: {0}", TimerBasedCombatSystem.instance.current_combat_status));
					TimerBasedCombatSystem.instance.UpdateCombatState();
				};
				m_hashTable["Soulshard0"] = item_use;

				item_use = delegate(UnitEntity obj) 
				{
					TimerBasedCombatSystem.instance.current_combat_status = TimerBasedCombatSystem.instance.selected_unit.UseItem(ITEM_STATE.SOULSHARD, obj, 1);
//					TimerBasedCombatSystem.instance.current_combat_status = Game_Timer.player.UseItem(ITEM_STATE.SOULSHARD, obj, 1);
					Debug.Log(string.Format("Player combat return status: {0}", TimerBasedCombatSystem.instance.current_combat_status));
					TimerBasedCombatSystem.instance.UpdateCombatState();
				};
				m_hashTable["Soulshard1"] = item_use;
			}
			
//			if(Game_Timer.boss != null){
//				
//				m_hashTable["Boss"] = Game_Timer.boss;
//			}

			if(Game_Timer.all_enemies != null)
			{
				foreach(UnitEntity ue in Game_Timer.all_enemies)
				{
					// Using tags as key, where to keep track of what is hitting
					m_hashTable[ue.unit_game_object.gameObject.tag] = ue as UnitEntity;
				}
			}
		}



		yield return null;
	}

}
