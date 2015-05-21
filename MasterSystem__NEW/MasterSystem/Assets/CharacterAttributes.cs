using System.Collections;
using UnityEngine;

[System.Serializable]
public class Attribute
{
	public int amount;
	public int current_lvl;
	public ATTRIBUTE_TYPE attr_type; // don't like this, but so far easy way to distunigsh stats on both server/client

	public Attribute()
	{
		this.amount = 0;
		this.current_lvl = 1;
	}

	public Attribute(ATTRIBUTE_TYPE type)
	{
		this.amount = 0;
		this.current_lvl = 1;
		this.attr_type = type;
	}

	public override string ToString ()
	{
		return string.Format ("[Attribute]({0}) amount: {1},  current_lvl: {2}", this.attr_type, this.amount, this.current_lvl);
	}
}

[System.Serializable]
public class ServerSideAttribute
{
	public Attribute attr;
	public int amount;
}
