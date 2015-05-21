using UnityEngine;
using System.Collections;

public enum CURRENCY_TYPE
{
	UNDEFINED,
	DEEP_IRON,
	DREAM_SHARD,
	ETHEREAL_DUST
}

public class ItemCost 
{
	private CURRENCY_TYPE _currency_type;
	private CURRENCY_TYPE currency_type
	{
		get { return _currency_type; }
	}

	private int _amount;
	private int amount
	{
		get { return _amount; }
	}

	public ItemCost(CURRENCY_TYPE type, int amount)
	{
		this._currency_type = type;
		this._amount  	   = amount;
	}

	public static ItemCost NONE = new ItemCost(CURRENCY_TYPE.UNDEFINED, 0);
	

	/// <param name="item_cost">Item_cost.</param>
	public static ItemCost operator+(ItemCost lhs, ItemCost rhs)
	{
		if(lhs.currency_type == rhs.currency_type)
		{
			return new ItemCost(lhs.currency_type, lhs.amount + rhs.amount);
		}
		else
		{
			return NONE;
		}
	}

	/// <param name="item_cost">Item_cost.</param>
	public static ItemCost operator-(ItemCost lhs, ItemCost rhs)
	{
		if(lhs.currency_type == rhs.currency_type)
		{
			return new ItemCost(lhs.currency_type, lhs.amount - rhs.amount);
		}
		else
		{
			return NONE;
		}
	}
}
