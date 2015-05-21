using UnityEngine;
using System.Collections;

// contains info about:
// - current enemies on this wave
// - reward per wave (currency | exp)
[System.Serializable]
public class EncounterWave
{
	[SerializeField]
	private EnemyEntity[] _enemies;
	public EnemyEntity[] enemies
	{
		get
		{
			return _enemies;
		}
	}

	private EnemyEntity _boss;
	public EnemyEntity boss
	{
		get
		{
			if(_boss == null)
			{
				foreach(EnemyEntity e in _enemies)
				{
					if(e.type == ENEMY_TYPE.BOSS)
						_boss = e;
				}
			}
			
			return _boss;
		}
	}

	[SerializeField]
	private Reward _reward;
	public Reward reward
	{
		get
		{
			return _reward;
		}
	}

	public EncounterWave(params EnemyEntity[] enemies)
	{
		_enemies = enemies;
	}

	public bool ContainBoss()
	{
		if(_boss == null)
		{
			foreach(EnemyEntity e in _enemies)
			{
				if(e.type == ENEMY_TYPE.BOSS)
					_boss = e;
			}
		}

		return (_boss != null);
	}

	/// <summary>
	/// Generates the reward. Given the parameters it will randomize between 1 and the values provided.
	/// </summary>
	/// <returns>The reward.</returns>
	/// <param name="base_exp">Base_exp.</param>
	/// <param name="exp_possible_gain">Exp_possible_gain.</param>
	/// <param name="currency_possible_gain">Currency_possible_gain.</param>
	public void GenerateReward(float base_exp, float exp_possible_gain, int currency_possible_gain)
	{
		_reward = new Reward(Random.Range(1, exp_possible_gain) + base_exp,
			                 Random.Range(1, currency_possible_gain),
			                 Random.Range(1, currency_possible_gain),
			                 Random.Range(1, currency_possible_gain));
	}

	public void SetReward(Reward reward)
	{
		_reward = reward;
	}
}

[System.Serializable]
public struct Reward
{
	public float experience;
	public int deep_iron;
	public int dream_shard;
	public int ethereal_dust;
	
	public Reward(float exp = 0, int di = 0, int ds = 0, int ed = 0)
	{
		experience    = exp;
		deep_iron     = di;
		dream_shard   = ds;
		ethereal_dust = ed;
	}

	/// <summary>
	/// r1 + r2
	/// </summary>
	/// <param name="r1">R1.</param>
	/// <param name="r2">R2.</param>
	public static Reward operator +(Reward r1, Reward r2) 
	{
		return new Reward(r1.experience    + r2.experience,
		                  r1.deep_iron     + r2.deep_iron,
		                  r1.dream_shard   + r2.dream_shard,
		                  r1.ethereal_dust + r2.ethereal_dust);
	}

	/// <summary>
	/// r1 - r2
	/// </summary>
	/// <param name="r1">R1.</param>
	/// <param name="r2">R2.</param>
	public static Reward operator -(Reward r1, Reward r2) 
	{
		return new Reward(r1.experience    - r2.experience,
		                  r1.deep_iron     - r2.deep_iron,
		                  r1.dream_shard   - r2.dream_shard,
		                  r1.ethereal_dust - r2.ethereal_dust);
	}
}