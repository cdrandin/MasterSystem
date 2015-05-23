using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//public enum ATTRIBUTE_TYPE
//{
//	STR = 0,
//	DEX = 1,
//	WILL = 2
//}

[RequireComponent (typeof(MasterSystemGUI), typeof(TrainerSystem))]
public class MasterSystem : MonoBehaviour 
{
	public Hashtable attributes;
	private ATTRIBUTE_TYPE _focused_type;
	public Attribute focused_attr
	{
		get	{ return attributes[_focused_type] as Attribute; }
	}

	private int _total_attr_exp_amount = 0;

	public void SetTotalAttrExp(int amount)
	{
		_total_attr_exp_amount = amount;
	}

	private int _current_exp_gain;

	public void Awake()
	{
		attributes = new Hashtable();
		attributes[ATTRIBUTE_TYPE.STR] = new Attribute(ATTRIBUTE_TYPE.STR);
		attributes[ATTRIBUTE_TYPE.DEX] = new Attribute(ATTRIBUTE_TYPE.DEX);
		attributes[ATTRIBUTE_TYPE.WILL] = new Attribute(ATTRIBUTE_TYPE.WILL);
	}

	public void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			foreach (DictionaryEntry entry in attributes)
			{
				Debug.Log(entry.Value);
			}
		}

		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			AddExp(attributes[ATTRIBUTE_TYPE.STR] as Attribute, 111);
		}

		if(Input.GetKeyDown(KeyCode.Alpha2))
		{
			AddExp(attributes[ATTRIBUTE_TYPE.DEX] as Attribute, 222);
		}

		if(Input.GetKeyDown(KeyCode.Alpha3))
		{
			AddExp(attributes[ATTRIBUTE_TYPE.WILL] as Attribute, 333);
		}
	}

//	public void AddExpToStrAttr(int amount)
//	{
//		AddExp(attributes[ATTRIBUTE_TYPE.STR] as Attribute, amount);
//	}
//
//	public void AddExpToDexAttr(int amount)
//	{
//		AddExp(attributes[ATTRIBUTE_TYPE.DEX] as Attribute, amount);
//	}
//
//	public void AddExpToWillAttr(int amount)
//	{
//		AddExp(attributes[ATTRIBUTE_TYPE.WILL] as Attribute, amount);
//	}

	public void SetAttrStrFocus()
	{
		_focused_type = ATTRIBUTE_TYPE.STR;
	}

	public void SetAttrDexFocus()
	{
		_focused_type = ATTRIBUTE_TYPE.DEX;
	}

	public void SetAttrWillFocus()
	{
		_focused_type = ATTRIBUTE_TYPE.WILL;
	}

	public void AddToExpGain(int amount)
	{
		_current_exp_gain += amount;
	}

	public void ResetExpGain()
	{
		_current_exp_gain = 0;
	}

	public void AddExpToFocused()
	{
		AddExp(attributes[_focused_type] as Attribute, _current_exp_gain);
	}

	private void AddExp(Attribute attr, int amount)
	{
		ServerSideAttribute server_attr = new ServerSideAttribute();
		server_attr.attr = attr;
		server_attr.amount = amount;

		string data = XMLUtil.Serialize<ServerSideAttribute> (server_attr);
		Request request = new Request ();
		request.id = "AddExp";
		request.payload = data;
		request.callback = AddExpCallback;
		GameMaster.SendRequest (request);
	}

	public void AddExpCallback(Response response)
	{
		Attribute attr = XMLUtil.Deserialize<Attribute> (response.payload);

		if(attributes.ContainsKey(attr.attr_type))
		{
			attributes[attr.attr_type] = attr;
		}
		else
		{
			Debug.LogError(string.Format("%s invalid attribute name or corrupted", attr.attr_type));
		}
		// Possibly gui here or send singal to update gui
	}
}
