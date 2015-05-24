using System.Collections;
using UnityEngine;

[System.Serializable]
public class Attribute
{
	public int current_exp_amount;
	public int exp_amount;
	public int current_lvl;
	public ATTRIBUTE_TYPE attr_type; // don't like this, but so far easy way to distunigsh stats on both server/client

	public Attribute()
	{
		this.exp_amount = 0;
		this.current_lvl = 1;
	}

	public Attribute(ATTRIBUTE_TYPE type)
	{
		this.exp_amount = 0;
		this.current_lvl = 1;
		this.attr_type = type;
	}

	public override string ToString ()
	{
		return string.Format ("[Attribute]: Type: {0} LvL: {1}  Exp: {2}", this.attr_type, this.current_lvl, this.current_exp_amount);
	}
}

// Update Attribute objects
// This deals with pinging the server the give an updated values of the player's attributes
[System.Serializable]
public class ServerSideUpdateAttribute
{
	public Attribute str;
	public Attribute will;
	public Attribute dex;

	public override string ToString ()
	{
		return string.Format ("[ServerSideUpdateAttribute]: Str: {0},  Dex: {1},  Will: {2}", this.str, this.dex, this.will);
	}
}


[System.Serializable]
public class ServerSideAttribute
{
	public Attribute attr;
	public int exp_amount;
}


[System.Serializable]
public class SubmitCharacterAttribute
{
	public Attribute attr;
	public int exp_amount;
}

