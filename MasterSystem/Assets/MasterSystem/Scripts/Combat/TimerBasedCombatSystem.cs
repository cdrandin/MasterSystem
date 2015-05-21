using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TimerBasedCombatSystem
{
	private static TimerBasedCombatSystem _instance;
	public static TimerBasedCombatSystem instance
	{
		get 
		{
			if(_instance == null)
			{
				_instance   = new TimerBasedCombatSystem();
			}
			
			return _instance;
		}
	}

	public static void Reset()
	{
		_instance = null;
	}

	private GUIAnnouncement _announcement;

	private System.Action _combat_timer;

	private TimerBasedCombatSystem()
	{
		_winner                = OWNERSHIP.UNDEFINED;
		current_combat_status  = COMBAT_RETURN_STATUS.UNDEFINED;
		_time_progression      = 0;
		_revolve_index		   = 0;
		_power				   = 0;

		_all_units             = new List<UnitEntity>[2];
		_all_units[0]          = new List<UnitEntity>();
		_all_units[1]          = new List<UnitEntity>();
		
		if(_announcement == null)
			_announcement = GameObject.FindObjectOfType<GUIAnnouncement>();

		_combat_timer = ()=>{++_time_progression;};
		DelayAction.instance.DelayInf(_combat_timer, 1.0f, delegate{ return false; });
	}
	
		/// <summary>
	/// The current_combat_status.
	/// </summary>
	public COMBAT_RETURN_STATUS current_combat_status;
	public UnitEntity current_attacking;

	private List<UnitEntity>[] _all_units;
	/// <summary>
	/// Get list of units corresponding to who owns it.
	/// </summary>
	/// <returns>The from.</returns>
	/// <param name="turn">Turn.</param>
	public List<UnitEntity> AllUnitsFrom(OWNERSHIP owner)
	{
		return _all_units[(int)owner];
	}

	public List<UnitEntity> LivingUnitsFrom(OWNERSHIP owner)
	{
		return _all_units[(int)owner].Where(unit => !unit.IsDead).ToList();
	}

	public void AddUnitTo(UnitEntity ue, OWNERSHIP owner)
	{
		_all_units[(int)owner].Add(ue);
	}

	private int _power;
	private int _max_power;

	public int group_power
	{
		get { return _power; }
	}

	public int max_group_power
	{
		get { return _max_power; }
	}

	public void ModPower(int amount)
	{
		_power = Mathf.Clamp(_power + amount, 0, max_group_power);
	}

	public void SetMaxPower(int amount)
	{
		_max_power = Mathf.Clamp(amount, 0, int.MaxValue);
	}

	public float group_power_normalized
	{
		get { return (float)_power/(float)_max_power; }
	}


	public void DisplayUnitsList()
	{
		Debug.Log(OWNERSHIP.PLAYER);
		foreach(UnitEntity unit in AllUnitsFrom(OWNERSHIP.PLAYER))
		{
			Debug.Log(string.Format("\t {0}", unit));
		}

		Debug.Log(OWNERSHIP.ENEMY);
		foreach(UnitEntity unit in AllUnitsFrom(OWNERSHIP.ENEMY))
		{
			Debug.Log(string.Format("\t {0}", unit));
		}
	}

	public void DisplayTeamOf(UnitEntity unit)
	{
		OWNERSHIP o = WhoOwnsMe(unit);
		Debug.Log(o);
		foreach(UnitEntity u in AllUnitsFrom(o))
		{
			Debug.Log(string.Format("\t {0}", u));
		}
	}

	private int _revolve_index;
	public int revolve_index
	{
		get { return _revolve_index; }
	}

	public UnitEntity left_unit
	{
		get 
		{ 
			try
			{
				UnitEntity unit = _instance.AllUnitsFrom(OWNERSHIP.PLAYER)[0];
				return unit;
				return !unit.IsDead ? unit : null;
			}
			catch(System.ArgumentOutOfRangeException)
			{	
				return null;
			}


			int index = _revolve_index;
			--index;
			if(index < 0)
			{
				index += 3;
			}

			try
			{
				UnitEntity unit = _instance.LivingUnitsFrom(OWNERSHIP.PLAYER)[index]; 
				if(index == _revolve_index) return null; // no dups
				return !unit.IsDead ? unit : null;
			}
			catch(System.ArgumentOutOfRangeException)
			{
				return null;
			}
		}
	}

	public UnitEntity right_unit
	{
		get 
		{ 
			try
			{
				UnitEntity unit = _instance.AllUnitsFrom(OWNERSHIP.PLAYER)[2];
				return unit;
				return !unit.IsDead ? unit : null;
			}
			catch(System.ArgumentOutOfRangeException)
			{	
				return null;
			}

			int index = (_revolve_index + 1)%3;

			try
			{
				UnitEntity unit = _instance.LivingUnitsFrom(OWNERSHIP.PLAYER)[index]; 
				if(index == _revolve_index) return null; // no dups
				return !unit.IsDead ? unit : null;
			}
			catch(System.ArgumentOutOfRangeException)
			{
				return null;
			}
		}
	}

	/// <summary>
	/// Gets the selected_unit. Used for player mainly
	/// </summary>
	/// <value>The selected_unit.</value>
	public UnitEntity selected_unit
	{
		get 
		{ 
			try
			{
				UnitEntity unit = _instance.AllUnitsFrom(OWNERSHIP.PLAYER)[1];
				return unit;
				return !unit.IsDead ? unit : null;
			}
			catch(System.ArgumentOutOfRangeException)
			{	
				return null;
			}

			return _instance.LivingUnitsFrom(OWNERSHIP.PLAYER)[_revolve_index]; 
		}
	}

	public void SwapRight()
	{
		UnitEntity center = _instance.AllUnitsFrom(OWNERSHIP.PLAYER)[1];
		UnitEntity right = _instance.AllUnitsFrom(OWNERSHIP.PLAYER)[2];
		Utility.Swap(ref center, ref right);

		_instance.AllUnitsFrom(OWNERSHIP.PLAYER)[1] = center;
		_instance.AllUnitsFrom(OWNERSHIP.PLAYER)[2] = right;
	}

	public void SwapLeft()
	{
		UnitEntity left = _instance.AllUnitsFrom(OWNERSHIP.PLAYER)[0];
		UnitEntity center = _instance.AllUnitsFrom(OWNERSHIP.PLAYER)[1];

		Utility.Swap(ref left, ref center);
		
		_instance.AllUnitsFrom(OWNERSHIP.PLAYER)[0] = left;
		_instance.AllUnitsFrom(OWNERSHIP.PLAYER)[1] = center;
	}

	public void PortraitSpinRight()
	{
		_revolve_index = (_revolve_index + 1)%_instance.LivingUnitsFrom(OWNERSHIP.PLAYER).Count; // _instance.UnitsFrom(OWNERSHIP.PLAYER).Count;
	}	

	public void PortraitSpinLeft()
	{
		--_revolve_index;
		if(_revolve_index < 0)
		{
			_revolve_index += _instance.LivingUnitsFrom(OWNERSHIP.PLAYER).Count;
		}
	}

//	public int group_power
//	{
//		get 
//		{
//			int total = 0;
//			foreach(UnitEntity ue in _instance.UnitsFrom(OWNERSHIP.PLAYER))
//			{
//				total += ue.base_entity.power;
//			}
//
//			return total;
//		}
//	}
//	public int group_max_power
//	{
//		get 
//		{
//			int total = 0;
//			foreach(UnitEntity ue in _instance.UnitsFrom(OWNERSHIP.PLAYER))
//			{
//				total += ue.base_entity.max_power;
//			}
//			
//			return total;
//		}
//	}
//	public float group_power_normalized
//	{
//		get { return (float)group_power/(float)group_max_power; }
//	}


	private OWNERSHIP _winner;
	/// <summary>
	/// Gets the winner.
	/// </summary>
	/// <value>The winner.</value>
	public OWNERSHIP winner
	{
		get { return _winner; }
	}

	private uint _time_progression;
	/// <summary>
	/// Gets the time_progression of the current combat scenario.
	/// </summary>
	/// <value>The time_progression.</value>
	public uint time_progression
	{
		get { return _time_progression; }
	}

	/// <summary>
	/// Whos the owns me.
	/// </summary>
	/// <returns>The owns me.</returns>
	/// <param name="unit">Unit.</param>
	public OWNERSHIP WhoOwnsMe(UnitEntity unit)
	{
		if(_all_units[(int)OWNERSHIP.PLAYER].Contains(unit))
		{
			return OWNERSHIP.PLAYER;
		}
		else
		{
			return OWNERSHIP.ENEMY;
		}
	}

	/// <summary>
	/// Updates the state of the combat including unit container per OWNERSHIP.
	/// Also sets current combat state to undefined
	/// </summary>
	public void UpdateCombatState()
	{
		// Remove dead units
		foreach(List<UnitEntity> team in _all_units)
		{
			for(int i=0; i<team.Count; ++i)
			{
				UnitEntity unit = team[i];
				if(unit.IsDead)
				{
					unit.unit_game_object.UpdateHP();
					unit.unit_game_object.DeathAnimation(1.5f);
					// Play death animation
					// Remove GUI stuff
				}
			}
		}

		current_attacking = null;
		_winner = (LivingUnitsFrom(OWNERSHIP.PLAYER).Count == 0) ? OWNERSHIP.ENEMY : (LivingUnitsFrom(OWNERSHIP.ENEMY).Count == 0) ? OWNERSHIP.PLAYER : OWNERSHIP.UNDEFINED;
		current_combat_status = COMBAT_RETURN_STATUS.UNDEFINED; // ?
	}
}
