using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum OWNERSHIP
{
	UNDEFINED = -1,
	PLAYER    = 0,
	ENEMY     = 1
}

public enum COMBAT_RETURN_STATUS
{
	UNDEFINED = 0,
	NO_ITEM_EQUIP,
	OUT_OF_RESOURCES,
	COOLDOWN,
	IMPROPER_USE_OF_ITEM,
	NOT_YOUR_TURN,
	TARGET_DEAD,
	NO_TARGET,
	END_TURN,
	NO_AVAILABLE_MOVES,
	UNSUCESSFUL,
	DISABLED,
	SUCCESSFUL
}

public class TurnBasedCombatSystem
{
	private static TurnBasedCombatSystem _instance;
	public static TurnBasedCombatSystem instance
	{
		get 
		{
			if(_instance == null)
			{
				_instance   = new TurnBasedCombatSystem();
			}
			
			return _instance;
		}
	}

	private GUIAnnouncement _announcement;

	private TurnBasedCombatSystem()
	{
		_current_turn          = OWNERSHIP.PLAYER;
		_winner                = OWNERSHIP.UNDEFINED;
		current_combat_status  = COMBAT_RETURN_STATUS.UNDEFINED;
		_turn_number           = 0;

		_all_units             = new List<UnitEntity>[2];
		_all_units[0]          = new List<UnitEntity>();
		_all_units[1]          = new List<UnitEntity>();

		if(_announcement == null)
			_announcement = GameObject.FindObjectOfType<GUIAnnouncement>();
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
	public List<UnitEntity> UnitsFrom(OWNERSHIP turn)
	{
		return _all_units[(int)turn];
	}

	private OWNERSHIP _current_turn;
	/// <summary>
	/// Gets whose current turn.
	/// </summary>
	/// <value>The current turn.</value>
	public OWNERSHIP current_turn
	{
		get { return _current_turn; }
	}
	
	private uint _turn_number;
	/// <summary>
	/// Gets the current turn_number.
	/// </summary>
	/// <value>The turn_number.</value>
	public uint turn_number
	{
		get { return _turn_number; }
	}

	private OWNERSHIP _winner;
	/// <summary>
	/// Gets the winner.
	/// </summary>
	/// <value>The winner.</value>
	public OWNERSHIP winner
	{
		get
		{
			return _winner;
		}
	}

	/// <summary>
	/// Whos the is next.
	/// </summary>
	/// <returns>The is next.</returns>
	public OWNERSHIP whose_next
	{
		get { return (_current_turn == OWNERSHIP.PLAYER) ? OWNERSHIP.ENEMY : OWNERSHIP.PLAYER; }
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
	/// Determines if the team the unit belongs to can make its move only if it is its turn
	/// </summary>
	/// <returns><c>true</c>, if turn was unitsed, <c>false</c> otherwise.</returns>
	/// <param name="unit">Unit.</param>
	public bool UnitsTurn(UnitEntity unit)
	{
		return _current_turn == WhoOwnsMe(unit);
	}

	public string DisplayTurnText()
	{
		return (_current_turn == OWNERSHIP.PLAYER) ? "Your turn" : "Opponent's turn";
	}

	/// <summary>
	/// Start next turn
	/// </summary>
	public void NextTurn()
	{
		if(_current_turn == OWNERSHIP.ENEMY)
		{
			++_turn_number;
		}

		// reset status
		current_combat_status = COMBAT_RETURN_STATUS.UNDEFINED;
		_current_turn         = whose_next;

		// Everyone gets 2 power when it is your turn again
		foreach(UnitEntity unit in UnitsFrom(_current_turn))
		{
			unit.base_entity.power += unit.base_entity.power_recv;

			if(unit.primary != null)
				unit.primary.item.IncrementItemCooldown();

			if(unit.secondary != null)
				unit.secondary.item.IncrementItemCooldown();

			if(unit.soul != null)
				unit.soul.item.IncrementItemCooldown();
		}

		if(_announcement == null)
			_announcement = GameObject.FindObjectOfType<GUIAnnouncement>();

		if(_announcement != null)
			_announcement.PlayAnnouncementWith(DisplayTurnText());

		Debug.Log(string.Format("NextTurn, now {0} turn", _current_turn.ToString()));
	}

	public static void Reset()
	{
		_instance = null;
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
					team.Remove(unit);
					// Play death animation
					// Remove GUI stuff
				}
			}
		}
		
		current_attacking = null;
		_winner = (_all_units[(int)OWNERSHIP.PLAYER].Count == 0) ? OWNERSHIP.ENEMY : (_all_units[(int)OWNERSHIP.ENEMY].Count == 0) ? OWNERSHIP.PLAYER : OWNERSHIP.UNDEFINED;
		current_combat_status = COMBAT_RETURN_STATUS.UNDEFINED;
	}
}
