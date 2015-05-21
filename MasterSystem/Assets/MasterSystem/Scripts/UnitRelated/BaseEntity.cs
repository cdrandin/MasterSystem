using UnityEngine;
using System.Collections;

public class BaseEntity
{
	private const int MAX_INT = 10000;

	private string _name;
	public string name
	{
		get { return _name; }
	}

	[Range(0,MAX_INT)]
	private int _max_hp;
	public int max_hp
	{
		get { return _max_hp; }
	}

	[Range(0,MAX_INT)]
	private int _hp;
	public int hp
	{
		get { return  _hp; }
		set { _hp = Mathf.Clamp(value, 0, _max_hp); }
	}
		
	//[SerializeField]
	[Range(0,MAX_INT)]
	private int _max_power;
	public int max_power
	{
		get { return _max_power; }
	}

	//[SerializeField]
	[Range(0,MAX_INT)]
	private int _power;
	public int power
	{
		get { return  _power; }
		set { _power = Mathf.Clamp(value, 0, _max_power); }
	}

	//[SerializeField]
	[Range(0,MAX_INT)]
	private int _power_recv;
	public int power_recv
	{
		get { return _power_recv; }
	}

	[Range(0,MAX_INT)]
	private int _armor;
	public int armor
	{
		get { return Mathf.Clamp(_armor, 0, MAX_INT); }
	}

	public BaseEntity(int max_hp, int max_power, int power_recovery, string name = "DefaultName")
	{
		_max_hp     = _hp    = max_hp;
		_max_power  = max_power;
		_power      = 0;
		_power_recv = power_recovery;
		_armor      = 0;
		_name       = name;
	}

	public BaseEntity(BaseEntitySO be)
	{
		_max_hp     = _hp    = be._max_hp;
		_max_power  = 0;
		_power_recv = 1;
		_armor      = 0;
		_name       = be.name;
	}

	public BaseEntity(BaseEntityPD be)
	{
		_max_hp     = be.max_health;
		_hp         = be.health;
		_max_power  = be.max_power;
		_power      = be.power;
		_power_recv = be.power_recv;
		_armor      = 0;
		_name       = be.name;
	}

	/// <summary>
	/// Damages the unit. Modifies armor value if any then decrements to health.
	/// </summary>
	/// <param name="amount">Amount.</param>
	public void DamageUnit(int amount)
	{
		Debug.Log(string.Format("Damage: {0}", amount));

		int leftover = _armor - amount;
		if(leftover < 0)
		{
			_armor = 0;
			hp += leftover;
		}
		else
		{
			_armor = leftover;
		}
	}

	/// <summary>
	/// Heals the unit.
	/// </summary>
	/// <param name="amount">Amount.</param>
	public void HealUnit(int amount)
	{
		hp += amount;
	}

	/// <summary>
	/// Gains the armor.
	/// </summary>
	/// <param name="amount">Amount.</param>
	public void GainArmor(int amount)
	{
		_armor += amount;
	}

	public void Reset()
	{
		_hp = _max_hp;
		_armor = 0;
	}

	override public string ToString()
	{
		return string.Format("Base entity => Name: {0}  HP: {1}/{2}  Armor:{3}  Power: {4}/{5}  Power recv: {6}", _name, hp, _max_hp, _armor, _power, _max_power, _power_recv);
	}
}