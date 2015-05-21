using UnityEngine;
using System.Collections;

public static class EncounterManagement
{
	private static int _index_wave;


	private static Encounter _current_encounter;
	public static void SetCurrentEncounter(Encounter encounter)
	{
		_current_encounter = encounter;
		SetCurrentEncounterWave(_current_encounter.waves);
	}

	public static void SetCurrentEncounterAsCompleted()
	{
		if(_at_end)
		{
			_current_encounter.SetToComplete();
		}
	}

	private static EncounterWave[] _current_wave;
	private static void SetCurrentEncounterWave(EncounterWave[] w)
	{
		Reset();
		_current_wave = w;
	}

	/// <summary>
	/// Gets the current_wave for the encounter
	/// </summary>
	/// <value>The current_wave.</value>
	public static EncounterWave current_wave
	{
		get
		{
			return _current_wave[_index_wave];
		}
	}

	private static bool _at_end;
	public static bool at_end
	{
		get
		{
			return _at_end;
		}
	}

	/// <summary>
	/// Advances the wave on the encounter. Returns true if no more waves, else false.
	/// </summary>
	/// <returns><c>true</c>, if there are no more waves, thus meaning completing the encounter, <c>false</c> otherwise.</returns>
	public static void NextWave()
	{
		_at_end = (++_index_wave >= _current_wave.Length) ? true : false;
	}

	public static Reward total_reward
	{
		get
		{
			Reward total = new Reward();
			foreach(EncounterWave wave in _current_wave)
			{
				total += wave.reward;
			}

			return total;
		}
	}

	public static void Reset()
	{
		_current_wave = null;
		_index_wave   = 0;
		_at_end    = false;
	}
}
