using UnityEngine;
using System.Collections;

[System.Serializable]
public class Currency
{
	[SerializeField]
	private int _deep_iron_amount;
	public int deep_iron_amount
	{
		get { return _deep_iron_amount; }
	}
	
	[SerializeField]
	private int _dream_shard_amount;
	public int dream_shard_amount
	{
		get { return _dream_shard_amount; }
	}
	
	[SerializeField]
	private int _ethereal_dust_amount;
	public int ethereal_dust_amount
	{
		get { return _ethereal_dust_amount; }
	}

	public Currency()
	{
		_deep_iron_amount = _dream_shard_amount = _ethereal_dust_amount = 0;
	}

	public void AddTo(CURRENCY_TYPE type, uint amount)
	{
		if(amount > int.MaxValue)
		{
			amount = int.MaxValue;
		}
		
		switch(type)
		{
		case CURRENCY_TYPE.DEEP_IRON:
			try   
			{
				checked
				{
					_deep_iron_amount += (int) amount;
				}
			}
			catch (System.OverflowException)
			{
				_deep_iron_amount = int.MaxValue;
			}
			break;
		case CURRENCY_TYPE.DREAM_SHARD:
			try   
			{
				checked
				{
					_dream_shard_amount += (int) amount;
				}
			}
			catch (System.OverflowException)
			{
				_dream_shard_amount = int.MaxValue;
			}
			break;
		case CURRENCY_TYPE.ETHEREAL_DUST:
			try   
			{
				checked
				{
					_ethereal_dust_amount += (int) amount;
				}
			}
			catch (System.OverflowException)
			{
				_ethereal_dust_amount = int.MaxValue;
			}
			break;
		}
	}
	
	public void SubTo(CURRENCY_TYPE type, uint amount)
	{
		switch(type)
		{
		case CURRENCY_TYPE.DEEP_IRON:
			_deep_iron_amount -= (int) amount;
			_deep_iron_amount = (_deep_iron_amount < 0) ? 0 : _deep_iron_amount;
			break;
		case CURRENCY_TYPE.DREAM_SHARD:
			_dream_shard_amount -= (int) amount;
			_dream_shard_amount = (_dream_shard_amount < 0) ? 0 : _dream_shard_amount;
			break;
		case CURRENCY_TYPE.ETHEREAL_DUST:
			_ethereal_dust_amount -= (int) amount;
			_ethereal_dust_amount = (_ethereal_dust_amount < 0) ? 0 : _ethereal_dust_amount;
			break;
		}
	}
	
	public void Reset()
	{
		_deep_iron_amount     = 0;
		_dream_shard_amount   = 0;
		_ethereal_dust_amount = 0;
	}

	public override string ToString ()
	{
		return string.Format ("[Currency: deep_iron_amount={0}, dream_shard_amount={1}, ethereal_dust_amount={2}]", deep_iron_amount, dream_shard_amount, ethereal_dust_amount);
	}
}

public class CurrencySingleton
{
	private string _key;

	private Currency _currency;
	public Currency currency
	{
		get { return _currency;	}
	}

	private static CurrencySingleton _instance;
	public static CurrencySingleton instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = new CurrencySingleton();
				_instance._key = "CurrencySingleton".GetHashCode().ToString();
	
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

	private CurrencySingleton()
	{}

	public static void Reset()
	{
		_instance._currency = new Currency();
	}

	public static void Save()
	{
		SimpleSerializer.Save(_instance._key, _instance.currency);
	}

	public static void Load()
	{
		if(!PlayerPrefs.HasKey(_instance._key))
		{
			SimpleSerializer.Save(_instance._key, _instance.currency);
		}
		
		_instance._currency = SimpleSerializer.Load<Currency>(_instance._key);
	}
}
