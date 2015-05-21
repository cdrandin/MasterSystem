using UnityEngine;
using System.Collections;

[System.Serializable]
public class HeroEntityPD
{
	public BaseEntityPD base_entity_pd;

	private float _max_experience
	{
		get	{ return base_entity_pd.level * 2f + 5.0f; }
	}
	
	public float max_experience
	{
		get	{ return _max_experience; }
	}
	
	[SerializeField]
	private float _experience;
	public float experience
	{
		get { return _experience; }
	}

	public HeroEntityPD(string name, int level, int max_health, int damage, int armor_rating, int max_power = 0, int power_recovery_rate = 0)
	{
		base_entity_pd = new BaseEntityPD(name,level,max_health,damage,armor_rating, max_power, power_recovery_rate);
		_experience    = 0f;
	}

	/// <summary>
	/// Adds the experience.
	/// </summary>
	/// <returns><c>true</c>, if leveled up, <c>false</c> otherwise.</returns>
	/// <param name="value">Value.</param>
	public bool AddExperience(float value)
	{
		_experience += value;
		if(_experience >= _max_experience)
		{
			base_entity_pd.level += (int)(_experience/_max_experience);
			LevelUp();
			float leftover = _experience%_max_experience;
			_experience = leftover;
			
			return true;
		}
		
		return false;
	}

	// Do stuff when player levels, modify stats, etc
	public void LevelUp()
	{
		base_entity_pd.SetMaxHealth(base_entity_pd.max_health + base_entity_pd.level * 2);
		base_entity_pd.health = base_entity_pd.max_health;
	}

	public void Reset()
	{
		_experience           = 0;
		base_entity_pd.level  = 1;
		base_entity_pd.SetMaxHealth(20);
		base_entity_pd.health = base_entity_pd.max_health;
	}
}

public class PlayerHeroSingleton
{
	private string _key;

	private HeroEntityPD _main_hero;
	public HeroEntityPD main_hero
	{
		get { return _main_hero;	}
	}

	private static PlayerHeroSingleton _instance;
	public static PlayerHeroSingleton instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = new PlayerHeroSingleton();
				_instance._key = "PlayerHeroSingleton".GetHashCode().ToString();

				// Existing
				if(PlayerPrefs.HasKey(_instance._key))
				{
					Load();
				}

				// New
				else
				{
					Reset();
					Save();
				}
			}
			
			//PlayerPrefs.DeleteAll();
			
			return _instance;
		}
	}

	private PlayerHeroSingleton()
	{}

	public static void Copy(HeroEntityPD hero)
	{
		// worried about this
		_instance._main_hero = hero;
	}

	public static void Reset()
	{
		if(Applications.type == COMBAT_TYPE.TURNED)
		{
			_instance._main_hero = new HeroEntityPD("Thayne", 1, 20, 2, 11, 10, 1);
		}
		else
		{
			_instance._main_hero = new HeroEntityPD("Thayne", 1, 20, 2, 11);
		}
	}

	public static void Save()
	{
		SimpleSerializer.Save(_instance._key, _instance._main_hero);
	}
	
	public static void Load()
	{
		if(!PlayerPrefs.HasKey(_instance._key))
		{
			Save();
		}
		
		_instance._main_hero = SimpleSerializer.Load<HeroEntityPD>(_instance._key);
	}

	public static void Delete()
	{
		PlayerPrefs.DeleteKey(_instance._key);
		_instance._main_hero = null;
		_instance            = null;
	}
}
