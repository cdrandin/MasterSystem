using UnityEngine;
using System.Collections;

[System.Serializable]
public class SoulShard
{
	[SerializeField]
	private BaseItem _item;
	public BaseItem item
	{
		get { return _item; }
	}

	/*
	public SoulShard(string name, int usage_cost, int amount, string description, WEAPON_HANDLE hand, HARMFULNESS harm, ACTION_LIST action, ABILITY_LIST ability)
	{
		_item = new BaseItem(name, usage_cost, amount, description, hand, harm, action, ability);
	}
	*/

	public SoulShard(BaseItem item)
	{
		_item = new BaseItem(item);
	}

	public SoulShard(SoulShard item)
	{
		_item = new BaseItem(item.item);
	}

	override public string ToString()
	{
		return string.Format("Soulshard: => {0}", _item);
	}
}
