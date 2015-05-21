using UnityEngine;
using System.Collections;

[System.Serializable]
public class BaseEntityPD
{
	private const int MAX_INT = 10000;

	[SerializeField]
	private string _name = "DefaultName";
	public string name
	{
		get { return _name; }
		set { _name = value; }
	}
	
	[SerializeField]
	[Range(0, int.MaxValue)]
	private int _max_level = 100;
	
	[SerializeField]
	private int _level = 1;
	public int level
	{
		get { return _level; }
		set { _level = Mathf.Clamp (value, 1, _max_level); }
	}
	
	[SerializeField]
	[Range(1, int.MaxValue)]
	private int _max_health;
	public int max_health
	{
		get { return _max_health; }
	}

	[SerializeField]
	private int _health = 0;
	public int health
	{
		get { return _health; }
		set { _health = Mathf.Clamp(value, 0, _max_health); }
	}

	[SerializeField]
	[Range(0,MAX_INT)]
	private int _max_power;
	public int max_power
	{
		get { return _max_power; }
	}

	[SerializeField]
	[Range(0,MAX_INT)]
	private int _power;
	public int power
	{
		get { return  _power; }
		set { _power = Mathf.Clamp(value, 0, _max_power); }
	}
	
	[SerializeField]
	[Range(0,MAX_INT)]
	private int _power_recv;
	public int power_recv
	{
		get { return _power_recv; }
	}

	[SerializeField]
	private int _damage = 0;
	public int damage
	{
		get { return _damage; }
	}
	
	[SerializeField]
	private int _armor_rating = 0;
	public int armor_rating
	{
		get { return _armor_rating; }
	}
	
	public BaseEntityPD(string name, int level, int max_health, int damage, int armor_rating, int max_power = 0, int power_recovery_rate = 0)
	{
		_name 		  = name;
		_level 		  = level;
		_health 	  = _max_health = max_health;
		_damage 	  = damage;
		_armor_rating = armor_rating;
		_max_power    = (Applications.type == COMBAT_TYPE.TURNED) ? 10 : max_power;
		power         = 1; // start with
		_power_recv   = (Applications.type == COMBAT_TYPE.TURNED) ? 1 : power_recovery_rate;
	}

	public void SetMaxHealth(int amount)
	{
		_max_health = amount;
	}
}
